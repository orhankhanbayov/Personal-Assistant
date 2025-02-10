using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PersonalAssistant.Models;

namespace PersonalAssistant.Services;

public class NotifyBackgroundService : IBackgroundService
{
	private readonly IAssistantService _assistantService;
	private readonly ITwilioService _twilioService;
	private readonly AppDbContext _dbContext;
	private readonly ICacheService _cacheService;

	public NotifyBackgroundService(
		IAssistantService assistantService,
		ITwilioService twilioService,
		AppDbContext dbContext,
		ICacheService cacheService
	)
	{
		_assistantService = assistantService;
		_twilioService = twilioService;
		_dbContext = dbContext;
		_cacheService = cacheService;
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
		DateTime now = DateTime.Now;
		DateTime oneHourLater = now.AddHours(1);

		List<Notification> NextNotifications = await _dbContext
			.Notifications.Where(e => e.SentAt >= now && e.SentAt <= oneHourLater)
			.ToListAsync();
		if (NextNotifications.Count == 0)
		{
			return;
		}

		foreach (Notification nextNotification in NextNotifications)
		{
			var user = await _dbContext
				.Users.Where(u => u.UserID == nextNotification.UserID)
				.FirstOrDefaultAsync();
			if (user == null)
			{
				continue;
			}
			string ToPhoneNumber = user?.PhoneNumber ?? "";
			string FromPhoneNumber = "+441156476174";
			if (nextNotification.TaskID != null)
			{
				var task = await _dbContext
					.Tasks.Where(t => t.TaskID == nextNotification.TaskID)
					.FirstOrDefaultAsync();
				if (task == null)
				{
					continue;
				}

				string message =
					$"Hello {user.FirstName}. You have a task titled '{task.Title}' due by {task.DueDate}. Description: {task.Description}.";

				Hangfire.BackgroundJob.Schedule(
					() => _twilioService.Notify(message, ToPhoneNumber, FromPhoneNumber),
					nextNotification.SentAt
				);
			}
			else if (nextNotification.EventID != null)
			{
				var Event = await _dbContext
					.Events.Where(e => e.EventID == nextNotification.EventID)
					.FirstOrDefaultAsync();
				if (Event == null)
				{
					continue;
				}

				string message =
					$"Hello {user.FirstName}. You have an upcoming event titled '{Event.Title}' scheduled from {Event.StartTime} to {Event.EndTime} at {Event.Location}. Description: {Event.Description}.";

				Hangfire.BackgroundJob.Schedule(
					() => _twilioService.Notify(message, ToPhoneNumber, FromPhoneNumber),
					nextNotification.SentAt
				);
			}
		}
	}
}
