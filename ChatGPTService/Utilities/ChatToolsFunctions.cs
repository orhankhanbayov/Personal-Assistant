using System.Text.Json;
using AutoMapper;
using ChatGPTService.Models;
using ChatGPTService.Services;
using Grpc.Net.Client;

namespace ChatGPTService.Utilities;

public class ChatToolsFunctions
{
	private readonly IMapper _mapper;
	private readonly Users.UsersClient _usersClient;
	private readonly Notifications.NotificationsClient _notificationsClient;
	private readonly Tasks.TasksClient _tasksClient;
	private readonly Events.EventsClient _eventsClient;

	public ChatToolsFunctions(
		IMapper mapper,
		Users.UsersClient usersClient,
		Notifications.NotificationsClient notificationsClient,
		Tasks.TasksClient tasksClient,
		Events.EventsClient eventsClient
	)
	{
		_mapper = mapper;
		_usersClient = usersClient;
		_notificationsClient = notificationsClient;
		_tasksClient = tasksClient;
		_eventsClient = eventsClient;
	}

	public async Task<FunctionCallerReturn> AddToCalendarAsync(FunctionCallerArgs data)
	{
		AddToCalendarArgs? functionArgs = JsonSerializer.Deserialize<AddToCalendarArgs>(
			data.FunctionArgs.ToString()
		);
		if (functionArgs == null)
		{
			return new FunctionCallerReturn { Success = false, Message = "Invalid arguments" };
		}
		EventDetails? newEvent = new EventDetails
		{
			UserID = data.user.UserID,
			Title = functionArgs.name,
			Description = functionArgs.notes,
			StartTime = functionArgs.datetime,
			EndTime = functionArgs.datetime.AddHours(1),
			Location = functionArgs.location,
			IsRecurring = functionArgs.IsRecurring,
			RecurrencePattern = functionArgs.RecurrencePattern,
			CreatedAt = DateTime.Now,
		};

		var eventRequest = new AddEventRequest
		{
			EventDetailsEntity = _mapper.Map<EventDetailRecord>(newEvent),
		};
		var result = await _eventsClient.AddEventAsync(eventRequest);

		if (functionArgs.notification != null)
		{
			Notification? newNotification = new Notification
			{
				UserID = data.user.UserID,
				EventID = result.EventID,
				Message = functionArgs.notification.Message,
				SentAt = functionArgs.notification.SentAt,
			};

			var notificationRequest = new AddNotificationsRequest
			{
				NotificationEntity = _mapper.Map<NotificationRecord>(newNotification),
			};
			await _notificationsClient.AddNotificationAsync(notificationRequest);
		}

		return new FunctionCallerReturn { Success = true, Message = "Successfly added event" };
	}

	public async Task<FunctionCallerReturn> ReadFromCalendarTodayAsync(FunctionCallerArgs data)
	{
		var result = await _eventsClient.ReadTodayCalendarAsync(
			new ReadTodayCalendarRequest { UserID = data.user.UserID }
		);

		return new FunctionCallerReturn { Success = result.Success > 1, Message = result.Message };
	}

	public async Task<FunctionCallerReturn> RemoveFromCalendarAsync(FunctionCallerArgs data)
	{
		GetEventDetailsArgs? functionArgs = JsonSerializer.Deserialize<GetEventDetailsArgs>(
			data.FunctionArgs.ToString()
		);
		if (functionArgs == null)
		{
			return new FunctionCallerReturn { Success = false, Message = "Invalid arguments" };
		}

		var result = await _eventsClient.RemoveFromCalendarAsync(
			new RemoveFromCalendarRequest
			{
				SearchColumn = functionArgs.SearchColumn,
				SearchTerm = functionArgs.SearchTerm,
				UserID = data.user.UserID,
			}
		);
		string message = result.Success == 0 ? "Event not found" : "Event removed successfully";
		return new FunctionCallerReturn { Success = result.Success > 1, Message = message };
	}

	public async Task<FunctionCallerReturn> GetEventDetailsAsync(FunctionCallerArgs data)
	{
		GetEventDetailsArgs? functionArgs = JsonSerializer.Deserialize<GetEventDetailsArgs>(
			data.FunctionArgs.ToString()
		);
		if (functionArgs == null)
		{
			return new FunctionCallerReturn { Success = false, Message = "Invalid arguments" };
		}

		var result = await _eventsClient.GetEventDetailsAsync(
			new GetEventDetailsRequest
			{
				SearchColumn = functionArgs.SearchColumn,
				SearchTerm = functionArgs.SearchTerm,
				UserID = data.user.UserID,
			}
		);

		return new FunctionCallerReturn { Success = result.Success, Message = result.Message };
	}

	public async Task<FunctionCallerReturn> UpdateCalendarEventAsync(FunctionCallerArgs data)
	{
		if (data == null || data.FunctionArgs == null || data.user == null)
		{
			return new FunctionCallerReturn { Success = false, Message = "Invalid arguments" };
		}
		UpdateCalendarEventArgs? functionArgs = JsonSerializer.Deserialize<UpdateCalendarEventArgs>(
			data.FunctionArgs.ToString()
		);
		if (functionArgs == null)
		{
			return new FunctionCallerReturn { Success = false, Message = "Invalid arguments" };
		}

		var result = await _eventsClient.UpdateEventAsync(
			new UpdateCalendarRequest
			{
				SearchColumn = functionArgs.SearchColumn,
				SearchTerm = functionArgs.SearchTerm,
				UserID = data.user.UserID,
			}
		);
		string message = result.Success == 0 ? "Event not found" : "Event updated successfully";
		return new FunctionCallerReturn { Success = result.Success > 1, Message = message };
	}

	public async Task<FunctionCallerReturn> CreateTaskAsync(FunctionCallerArgs data)
	{
		CreateTaskArgs? functionArgs = JsonSerializer.Deserialize<CreateTaskArgs>(
			data.FunctionArgs.ToString()
		);
		if (functionArgs == null)
		{
			return new FunctionCallerReturn { Success = false, Message = "Invalid arguments" };
		}

		TaskDetails? newTask = new TaskDetails
		{
			UserID = data.user.UserID,
			Title = functionArgs.title,
			Description = functionArgs.description,
			Priority = functionArgs.priority,
			CreatedAt = DateTime.Now,
			DueDate = functionArgs.dueDate,
		};

		await _tasksClient.AddTaskAsync(
			new AddTaskRequest { TaskDetailsEntity = _mapper.Map<TaskDetailRecord>(newTask) }
		);

		if (functionArgs.notification != null)
		{
			Notification? newNotification = new Notification
			{
				UserID = data.user.UserID,
				TaskID = newTask.TaskID,
				Message = functionArgs.notification.Message,
				SentAt = functionArgs.notification.SentAt,
			};
			var result = _notificationsClient.AddNotificationAsync(
				new AddNotificationsRequest
				{
					NotificationEntity = _mapper.Map<NotificationRecord>(newNotification),
				}
			);
		}

		return new FunctionCallerReturn { Success = true, Message = "Successfly added event" };
	}
}
