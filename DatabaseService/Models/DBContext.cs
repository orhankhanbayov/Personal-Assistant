namespace DBService.Models;

public class User
{
	public int UserID { get; set; }
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public required string Email { get; set; }
	public required string PhoneNumber { get; set; }

	public string? PasswordHash { get; set; }
	public required DateTime CreatedAt { get; set; }
	public DateTime? LastLoginAt { get; set; }

	public ICollection<EventDetails>? Events { get; set; }
	public ICollection<TaskDetails>? Tasks { get; set; }
	public ICollection<Notification>? Notifications { get; set; }
	public ICollection<CallHistory>? CallHistories { get; set; }
}

public class EventDetails
{
	public int? EventID { get; set; }
	public required int UserID { get; set; }
	public required string Title { get; set; }
	public required string? Description { get; set; }
	public required DateTime StartTime { get; set; }
	public required DateTime EndTime { get; set; }
	public string? Location { get; set; }
	public bool? IsRecurring { get; set; }
	public string? RecurrencePattern { get; set; }
	public required DateTime CreatedAt { get; set; }

	public User? User { get; set; }
	public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

public class TaskDetails
{
	public int? TaskID { get; set; }
	public required int UserID { get; set; }
	public required string Title { get; set; }
	public required string Description { get; set; }
	public required DateTime DueDate { get; set; }
	public bool? IsCompleted { get; set; }
	public string? Priority { get; set; }
	public required DateTime CreatedAt { get; set; }

	public User? User { get; set; }
	public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}

public class Notification
{
	public int? NotificationID { get; set; }
	public int? UserID { get; set; }
	public int? EventID { get; set; }
	public int? TaskID { get; set; }
	public required string Message { get; set; }
	public required DateTime SentAt { get; set; }
	public bool? IsRead { get; set; }

	public User? User { get; set; }
	public EventDetails? EventDetail { get; set; }
	public TaskDetails? TaskDetail { get; set; }
}

public class CallHistory
{
	public int? CallHistoryId { get; set; }
	public required string CallSid { get; set; }
	public required string From { get; set; }
	public required string To { get; set; }
	public required DateTime StartTime { get; set; }
	public DateTime EndTime { get; set; }
	public required string CallStatus { get; set; }
	public required int UserID { get; set; }
	public List<string> ConversationHistory { get; set; } = new List<string>();

	public User? User { get; set; }
}
