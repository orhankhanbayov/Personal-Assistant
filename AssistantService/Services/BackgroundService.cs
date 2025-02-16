using AssistantService.Models;
using AutoMapper;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;

namespace AssistantService.Services;

public class NotifyBackgroundService : IBackgroundService
{
	private readonly ITwilioService _twilioService;
	private readonly Users.UsersClient _usersClient;
	private readonly Notifications.NotificationsClient _notificationsClient;
	private readonly Tasks.TasksClient _tasksClient;
	private readonly Events.EventsClient _eventsClient;

	private readonly IMapper _mapper;

	public NotifyBackgroundService(
		ITwilioService twilioService,
		Users.UsersClient usersClient,
		Notifications.NotificationsClient notificationsClient,
		Tasks.TasksClient tasksClient,
		Events.EventsClient eventsClient,
		IMapper mapper
	)
	{
		_twilioService = twilioService;
		_usersClient = usersClient;
		_notificationsClient = notificationsClient;
		_tasksClient = tasksClient;
		_eventsClient = eventsClient;
		_mapper = mapper;
	}

	public void processOutgoingRequests(IRecurringJobManager recurringJobManager)
	{
		recurringJobManager.AddOrUpdate(
			"NotifyUpComingEvents",
			() => NotifyUpComingEvents(),
			"*/30 * * * * *"
		);
	}

	public async Task NotifyUpComingEvents()
	{
		var list = await _notificationsClient.GetNextNotificationsAsync(
			new GetNotificationListRequest()
		);
		List<Notification> NextNotifications = _mapper.Map<List<Notification>>(
			list.NotifiationList
		);

		if (NextNotifications.Count == 0)
		{
			return;
		}

		foreach (Notification nextNotification in NextNotifications)
		{
			var getUser = await _usersClient.GetUserByUserIDAsync(
				new GetUserByUserIDRequest { UserID = nextNotification.UserID ?? 0 }
			);

			User user = _mapper.Map<User>(getUser.UserEntity);
			if (user == null)
			{
				continue;
			}
			string ToPhoneNumber = user?.PhoneNumber ?? "";
			string FromPhoneNumber = "+441156476174";
			if (nextNotification.TaskID != null)
			{
				var getTask = await _tasksClient.GetTaskByTaskIDAsync(
					new GetTaskByTaskIDRequest { TaskID = nextNotification.TaskID ?? 0 }
				);
				TaskDetails task = _mapper.Map<TaskDetails>(getTask.TaskDetailsEntity);
				if (task == null)
				{
					continue;
				}

				string message =
					$"Hello {user?.FirstName}. You have a task titled '{task?.Title}' due by {task?.DueDate}. Description: {task?.Description}.";

				Hangfire.BackgroundJob.Schedule(
					() => _twilioService.Notify(message, ToPhoneNumber, FromPhoneNumber),
					nextNotification.SentAt
				);
				await _notificationsClient.MarkNotificationReadAsync(
					new MarkNotificationReadRequest
					{
						NotificationID = nextNotification.NotificationID ?? 0,
					}
				);
			}
			else if (nextNotification.EventID != null)
			{
				var getEvent = await _eventsClient.GetEventByIDAsync(
					new GetEventByIDRequest { EventID = nextNotification.EventID ?? 0 }
				);
				EventDetails Event = _mapper.Map<EventDetails>(getEvent.EventDetailsEntity);
				if (Event == null)
				{
					continue;
				}

				string message =
					$"Hello {user?.FirstName}. You have an upcoming event titled '{Event?.Title}' scheduled from {Event?.StartTime} to {Event?.EndTime} at {Event?.Location}. Description: {Event?.Description}.";

				Hangfire.BackgroundJob.Schedule(
					() => _twilioService.Notify(message, ToPhoneNumber, FromPhoneNumber),
					nextNotification.SentAt
				);
				await _notificationsClient.MarkNotificationReadAsync(
					new MarkNotificationReadRequest
					{
						NotificationID = nextNotification.NotificationID ?? 0,
					}
				);
			}
		}
	}
}
