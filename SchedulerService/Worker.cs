using AutoMapper;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using SchedulerService;
using SchedulerService.Models;
using SchedulerService.Utilities;

namespace SchedulerService;

public class Worker : BackgroundService
{
	private readonly ILogger<Worker> _logger;
	private readonly Users.UsersClient _usersClient;
	private readonly Notifications.NotificationsClient _notificationsClient;
	private readonly Tasks.TasksClient _tasksClient;
	private readonly Events.EventsClient _eventsClient;

	private readonly TwilioPhone.TwilioPhoneClient _twilioClient;

	private readonly IMapper _mapper;

	public Worker(
		ILogger<Worker> logger,
		Users.UsersClient usersClient,
		Notifications.NotificationsClient notificationsClient,
		Tasks.TasksClient tasksClient,
		Events.EventsClient eventsClient,
		IMapper mapper,
		TwilioPhone.TwilioPhoneClient twilioClient
	)
	{
		_logger = logger;
		_usersClient = usersClient;
		_notificationsClient = notificationsClient;
		_tasksClient = tasksClient;
		_eventsClient = eventsClient;
		_mapper = mapper;
		_twilioClient = twilioClient;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			await NotifyUpComingEvents();
			await Task.Delay(1000, stoppingToken);
		}
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
					() =>
						_twilioClient.Notify(
							new NotifyRequest
							{
								Message = message,
								ToPhoneNumber = ToPhoneNumber,
								FromPhoneNumber = FromPhoneNumber,
							},
							new Grpc.Core.CallOptions()
						),
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
					() =>
						_twilioClient.Notify(
							new NotifyRequest
							{
								Message = message,
								ToPhoneNumber = ToPhoneNumber,
								FromPhoneNumber = FromPhoneNumber,
							},
							new Grpc.Core.CallOptions()
						),
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
