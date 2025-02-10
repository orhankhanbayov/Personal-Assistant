using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PersonalAssistant.Models;
using PersonalAssistant.Services;

namespace PersonalAssistant.Utilities;

public class ChatToolsFunctions
{
	private readonly AppDbContext _dbContext;

	public ChatToolsFunctions(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<bool> AddToCalendarAsync(FunctionCallerArgs data)
	{
		AddToCalendarArgs? functionArgs = JsonSerializer.Deserialize<AddToCalendarArgs>(
			data.FunctionArgs.ToString()
		);
		if (functionArgs == null)
		{
			return false;
		}
		EventDetails? newEvent = new EventDetails
		{
			UserID = data.user.UserID,
			Title = functionArgs.name,
			Description = functionArgs.notes,
			StartTime = functionArgs.datetime,
			EndTime = functionArgs.datetime.AddHours(1),
			Location = functionArgs.location,
			IsRecurring = null,
			RecurrencePattern = null,
			CreatedAt = DateTime.Now,
		};
		await _dbContext.Events.AddAsync(newEvent);
		await _dbContext.SaveChangesAsync();

		if (functionArgs.notification != null)
		{
			Notification? newNotification = new Notification
			{
				UserID = data.user.UserID,
				EventID = newEvent.EventID,
				Message = functionArgs.notification.Message,
				SentAt = functionArgs.notification.SentAt,
			};
			await _dbContext.Notifications.AddAsync(newNotification);
		}

		int saveResult = await _dbContext.SaveChangesAsync();
		return saveResult > 0;
	}

	// ability to find events based on all event detaild
	public async Task<List<EventDetails>> ReadFromCalendarTodayAsync(FunctionCallerArgs data)
	{
		var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserID == data.user.UserID);
		if (user == null)
		{
			return new List<EventDetails>();
		}

		var today = DateTime.Today;
		var events = await _dbContext
			.Events.Where(e => e.UserID == user.UserID && e.StartTime.Date == today)
			.ToListAsync();

		return events;
	}

	public async Task<bool> RemoveFromCalendarAsync(FunctionCallerArgs data)
	{
		RemoveFromCalendarArgs? functionArgs = JsonSerializer.Deserialize<RemoveFromCalendarArgs>(
			data.FunctionArgs.ToString()
		);
		if (functionArgs == null)
		{
			return false;
		}

		var eventToRemove = await _dbContext.Events.FirstOrDefaultAsync(e =>
			e.EventID.ToString() == functionArgs.eventId && e.UserID == data.user.UserID
		);
		if (eventToRemove == null)
		{
			return false;
		}

		_dbContext.Events.Remove(eventToRemove);
		int saveResult = await _dbContext.SaveChangesAsync();
		string message = saveResult > 0 ? "Success" : "Failed";
		return saveResult > 0;
	}

	public async Task<EventDetails> GetEventDetailsAsync(FunctionCallerArgs data)
	{
		GetEventDetailsArgs? functionArgs = JsonSerializer.Deserialize<GetEventDetailsArgs>(
			data.FunctionArgs.ToString()
		);

		EventDetails? eventDetails = await _dbContext.Events.FirstOrDefaultAsync(e =>
			e.EventID.ToString() == functionArgs.eventId && e.UserID == data.user.UserID
		);
		// handle event not found
		return eventDetails;
	}

	public async Task<bool> UpdateCalendarEventAsync(FunctionCallerArgs data)
	{
		if (data == null || data.FunctionArgs == null || data.user == null)
		{
			return false;
		}
		UpdateCalendarEventArgs? functionArgs = JsonSerializer.Deserialize<UpdateCalendarEventArgs>(
			data.FunctionArgs.ToString()
		);
		if (functionArgs == null)
		{
			return false;
		}

		EventDetails? eventToUpdate = await _dbContext.Events.FirstOrDefaultAsync(e =>
			e.EventID.ToString() == functionArgs.eventId && e.UserID == data.user.UserID
		);
		if (eventToUpdate == null)
		{
			return false;
		}

		eventToUpdate.Title = functionArgs.name;
		eventToUpdate.StartTime = functionArgs.datetime;
		eventToUpdate.EndTime = functionArgs.datetime.AddHours(1);
		eventToUpdate.Location = functionArgs.location;
		eventToUpdate.Description = functionArgs.notes;

		_dbContext.Events.Update(eventToUpdate);
		int saveResult = await _dbContext.SaveChangesAsync();
		return saveResult > 0;
	}

	public async Task<bool> CreateTaskAsync(FunctionCallerArgs data)
	{
		CreateTaskArgs? functionArgs = JsonSerializer.Deserialize<CreateTaskArgs>(
			data.FunctionArgs.ToString()
		);
		if (functionArgs == null)
		{
			return false;
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
		await _dbContext.Tasks.AddAsync(newTask);
		await _dbContext.SaveChangesAsync();

		if (functionArgs.notification != null)
		{
			Notification? newNotification = new Notification
			{
				UserID = data.user.UserID,
				TaskID = newTask.TaskID,
				Message = functionArgs.notification.Message,
				SentAt = functionArgs.notification.SentAt,
			};
			await _dbContext.Notifications.AddAsync(newNotification);
		}

		int saveResult = await _dbContext.SaveChangesAsync();
		return saveResult > 0;
	}
}
