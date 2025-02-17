using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using AutoMapper;
using ChatGPTService;
using ChatGPTService.Models;
using ChatGPTService.Utilities;
using Grpc.Core;
using OpenAI.Chat;

namespace ChatGPTService.Services;

public class ChatService : Chat.ChatBase
{
	private readonly ChatClient _chatClient;

	private readonly ILogger<ChatService> _logger;

	private readonly IMapper _mapper;

	private readonly ChatToolsFunctions _chatToolsFunctions;

	private readonly Users.UsersClient _usersClient;

	public ChatService(
		ChatClient chatClient,
		ILogger<ChatService> logger,
		IMapper mapper,
		ChatToolsFunctions chatToolsFunctions,
		Users.UsersClient usersClient
	)
	{
		_chatClient = chatClient;
		_logger = logger;
		_mapper = mapper;
		_chatToolsFunctions = chatToolsFunctions;
		_usersClient = usersClient;
	}

	public override async Task<GetChatCompletionReply> GetChatCompletion(
		GetChatCompletionRequest request,
		ServerCallContext context
	)
	{
		var getUser = await _usersClient.GetUserByUserIDAsync(
			new GetUserByUserIDRequest { UserID = request.UserID }
		);
		User? user = _mapper.Map<User>(getUser.UserEntity);

		List<ChatMessage> messages = request
			.UserMessage.Select(e => new UserChatMessage(e) as ChatMessage)
			.ToList();

		ChatCompletionOptions options = new()
		{
			Tools =
			{
				ChatTools.ReadFromCalendarTodayTool,
				ChatTools.AddToCalendarTool,
				ChatTools.RemoveFromCalendarTool,
				ChatTools.UpdateCalendarEventTool,
				ChatTools.GetEventDetailsTool,
			},
			ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
				jsonSchemaFormatName: "assistant_response_format",
				jsonSchema: BinaryData.FromBytes(
					"""
			{
				"type": "object",
				"properties": {
				"steps": {
					"type": "array",
					"items": {
					"type": "object",
					"properties": {
						"explanation": { "type": "string" },
						"output": { "type": "string" }
					},
					"required": ["explanation", "output"],
					"additionalProperties": false
					}
				},
				"final_answer": { "type": "string" }
				},
				"required": ["steps", "final_answer"],
				"additionalProperties": false
			}
			"""u8.ToArray()
				),
				jsonSchemaIsStrict: true
			),
		};

		ChatCompletion response = await _chatClient.CompleteChatAsync(messages, options);
		// create seperate chat options without tool calls being passed in just for completions
		var message = response.Content.FirstOrDefault()?.Text;
		var toolCalls = response.ToolCalls;
		if (message != null && response.ToolCalls == null)
		{
			return new GetChatCompletionReply { Message = message };
		}

		if (toolCalls != null)
		{
			IReadOnlyList<ChatToolCall>? tools = response.ToolCalls;
			List<string> messagesFromTools = await FunctionCaller(tools, user);
			messagesFromTools.Insert(
				0,
				"Here is a list of the function calls made and the responses from each call. Generate an appropriate response based on the returns from the function calls, if an event is not found return a message to say the event is not found. Always return outputs in the structured data format always including a final answer even if it's just confirming what you have done or action that has been taken."
			);
			messages.AddRange(messagesFromTools.Select(e => new UserChatMessage(e) as ChatMessage));
			ChatCompletion updatedResponse = await _chatClient.CompleteChatAsync(messages, options);
			// updatedResponse.Content[0].Text could be null so how to handle this
			if (updatedResponse.Content[0]?.Text == null)
			{
				return new GetChatCompletionReply
				{
					Message = "Error: No content in updated response.",
				};
			}
			using JsonDocument structuredJson = JsonDocument.Parse(updatedResponse.Content[0].Text);
			JsonElement root = structuredJson.RootElement;
			if (
				root.TryGetProperty("final_answer", out JsonElement finalAnswerElement)
				&& finalAnswerElement.ValueKind == JsonValueKind.String
			)
			{
				message = finalAnswerElement.GetString();
			}
			else
			{
				message = "Error: final_answer not found or invalid.";
			}

			if (message == null)
			{
				message = "Error processing tool calls.";
			}
		}

		return new GetChatCompletionReply { Message = message };
	}

	public async Task<List<string>> FunctionCaller(IReadOnlyList<ChatToolCall> tools, User user)
	{
		List<string> messages = new List<string>();

		foreach (ChatToolCall tool in tools)
		{
			string funcName = tool.FunctionName;
			var args = tool.FunctionArguments;

			FunctionCallerArgs? functionCallerArgs = new FunctionCallerArgs
			{
				FunctionName = funcName,
				FunctionArgs = args,
				user = user,
			};

			MethodInfo? methodInfo = _chatToolsFunctions
				.GetType()
				.GetMethod(funcName, BindingFlags.Public | BindingFlags.Instance);

			if (methodInfo == null || functionCallerArgs == null)
			{
				_logger.LogError("Invalid function call");
				messages.Add("Invalid function call: " + funcName);
				continue;
			}

			var invokeResult = methodInfo.Invoke(
				_chatToolsFunctions,
				new object[] { functionCallerArgs }
			);

			if (invokeResult is Task<FunctionCallerReturn> taskResult)
			{
				FunctionCallerReturn result = await taskResult;
				if (!result.Success)
				{
					_logger.LogInformation("Error calling function: " + funcName);
					messages.Add("Error calling function: " + funcName);
				}
				else
				{
					_logger.LogInformation("successful function response here: " + funcName);
					messages.Add("function name: " + funcName + result.Message);
				}
			}
		}
		return messages;
	}
}
