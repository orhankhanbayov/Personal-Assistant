using OpenAI.Chat;

namespace PersonalAssistant.Models;

public interface IChatGPTService
{
	Task<ChatCompletionData> GetChatCompletion(string userMessage);
}

public class ChatCompletionData
{
	public string? Message { get; set; } = string.Empty;
	public IReadOnlyList<ChatToolCall>? ToolCalls { get; set; } = new List<ChatToolCall>();
}
