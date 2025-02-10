using OpenAI.Chat;
using PersonalAssistant.Models;
using PersonalAssistant.Utilities;

namespace PersonalAssistant.Services;

public class ChatGPTService : IChatGPTService
{
	private readonly ChatClient _chatClient;

	private readonly ILogger<ChatGPTService> _logger;

	public ChatGPTService(ChatClient chatClient, ILogger<ChatGPTService> logger)
	{
		_chatClient = chatClient;
		_logger = logger;
	}

	public async Task<ChatCompletionData> GetChatCompletion(string userMessage)
	{
		var messages = new List<ChatMessage> { new UserChatMessage(userMessage) };

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
		};

		ChatCompletion response = await _chatClient.CompleteChatAsync(messages, options);

		var message = response.Content.FirstOrDefault()?.Text;
		var toolCalls = response.ToolCalls;
		if (message == null && toolCalls == null)
		{
			_logger.LogError("ChatGPTService.GetChatCompletion: message is null");
			return new ChatCompletionData
			{
				Message = "Sorry, I am unable to process your request at the moment.",
			};
		}

		return new ChatCompletionData { Message = message, ToolCalls = toolCalls };
	}
}
