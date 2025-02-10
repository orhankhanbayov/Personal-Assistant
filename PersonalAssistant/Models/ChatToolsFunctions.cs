namespace PersonalAssistant.Models;

public class AddToCalendarArgs
{
	public required DateTime datetime { get; set; }
	public required string name { get; set; }
	public string? location { get; set; }
	public string? notes { get; set; }
	public string? alerts { get; set; }
	public Notification? notification { get; set; }
}

public class CreateTaskArgs
{
	public required string title { get; set; }
	public required string description { get; set; }
	public required DateTime dueDate { get; set; }
	public string? priority { get; set; }
	public string? alerts { get; set; }
	public Notification? notification { get; set; }
}

public class FunctionCallerArgs
{
	public required string FunctionName { get; set; }
	public required BinaryData FunctionArgs { get; set; }
	public required User user { get; set; }
}

public class RemoveFromCalendarArgs
{
	public required string eventId { get; set; }
}

public class GetEventDetailsArgs
{
	public required string eventId { get; set; }
}

public class UpdateCalendarEventArgs
{
	public required string eventId { get; set; }
	public required DateTime datetime { get; set; }
	public required string name { get; set; }
	public string? location { get; set; }
	public string? notes { get; set; }
	public string? alerts { get; set; }
}
