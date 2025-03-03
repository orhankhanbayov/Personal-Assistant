using AutoMapper;
using DBService;
using DBService.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace DBService.Services;

public class EventService : Events.EventsBase
{
	private readonly AppDbContext _dbContext;
	private readonly IMapper _mapper;

	public EventService(AppDbContext dbContext, IMapper mapper)
	{
		_dbContext = dbContext;
		_mapper = mapper;
	}

	public override async Task<GetEventByIDReply> GetEventByID(
		GetEventByIDRequest request,
		ServerCallContext context
	)
	{
		var Event = await _dbContext
			.Events.Where(e => e.EventID == request.EventID)
			.FirstOrDefaultAsync();

		if (Event == null)
			throw new RpcException(new Status(StatusCode.NotFound, "Event not found"));

		return new GetEventByIDReply { EventDetailsEntity = _mapper.Map<EventDetailRecord>(Event) };
	}

	public override async Task<AddEventReply> AddEvent(
		AddEventRequest request,
		ServerCallContext context
	)
	{
		var eventDetails = _mapper.Map<EventDetails>(request.EventDetailsEntity);
		await _dbContext.Events.AddAsync(eventDetails);
		var saveResult = await _dbContext.SaveChangesAsync();

		return new AddEventReply { Success = saveResult, EventID = eventDetails.EventID ?? 0 };
	}

	public override async Task<ReadTodayCalendarReply> ReadTodayCalendar(
		ReadTodayCalendarRequest request,
		ServerCallContext context
	)
	{
		User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserID == request.UserID);
		if (user == null)
			throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
		var today = DateTime.Today;

		List<EventDetails>? events = await _dbContext
			.Events.Where(e => e.UserID == user.UserID && e.StartTime.Date == today)
			.ToListAsync();
		if (events == null)
		{
			return new ReadTodayCalendarReply { Success = 0, Message = "" };
		}

		return new ReadTodayCalendarReply
		{
			Success = 1,
			Message = string.Join(
				"---",
				events.Select(e =>
					$"Title: {e.Title}, Location: {e.Location}, Description: {e.Description}, StartTime: {e.StartTime}, EndTime: {e.EndTime}"
				)
			),
		};
	}

	public override async Task<RemoveFromCalendarReply> RemoveFromCalendar(
		RemoveFromCalendarRequest request,
		ServerCallContext context
	)
	{
		var eventDetails = await _dbContext.Events.FirstOrDefaultAsync(e =>
			EF.Property<string>(e, request.SearchColumn) == request.SearchTerm
			&& e.UserID == request.UserID
		);

		if (eventDetails == null)
		{
			return new RemoveFromCalendarReply { Success = 0 };
		}

		var relatedNotifications = _dbContext.Notifications.Where(n =>
			n.EventID == eventDetails.EventID
		);
		_dbContext.Notifications.RemoveRange(relatedNotifications);

		_dbContext.Events.Remove(eventDetails);
		var result = await _dbContext.SaveChangesAsync();
		return new RemoveFromCalendarReply { Success = result };
	}

	public override async Task<GetEventDetailsReply> GetEventDetails(
		GetEventDetailsRequest request,
		ServerCallContext context
	)
	{
		var eventDetails = await _dbContext.Events.FirstOrDefaultAsync(e =>
			EF.Property<string>(e, request.SearchColumn) == request.SearchTerm
			&& e.UserID == request.UserID
		);

		if (eventDetails == null)
		{
			return new GetEventDetailsReply { Success = false, Message = "Event not found" };
		}

		return new GetEventDetailsReply
		{
			Success = true,
			Message = string.Join(
				"---",
				$"Title: {eventDetails.Title}, Location: {eventDetails.Location}, Description: {eventDetails.Description}, StartTime: {eventDetails.StartTime}, EndTime: {eventDetails.EndTime}"
			),
		};
	}

	public override async Task<UpdateEventReply> UpdateEvent(
		UpdateCalendarRequest request,
		ServerCallContext context
	)
	{
		var eventToUpdate = await _dbContext.Events.FirstOrDefaultAsync(e =>
			EF.Property<string>(e, request.SearchColumn) == request.SearchTerm
			&& e.UserID == request.UserID
		);
		if (eventToUpdate == null)
		{
			return new UpdateEventReply { Success = 0 };
		}

		eventToUpdate.Title = string.IsNullOrEmpty(request.Title)
			? eventToUpdate.Title
			: request.Title;
		eventToUpdate.StartTime =
			request.StartTime != null ? DateTime.Parse(request.StartTime) : eventToUpdate.StartTime;
		eventToUpdate.EndTime =
			request.EndTime != null ? DateTime.Parse(request.EndTime) : eventToUpdate.EndTime;
		eventToUpdate.Location = string.IsNullOrEmpty(request.Location)
			? eventToUpdate.Location
			: request.Location;
		eventToUpdate.Description = string.IsNullOrEmpty(request.Description)
			? eventToUpdate.Description
			: request.Description;

		_dbContext.Events.Update(eventToUpdate);
		int saveResult = await _dbContext.SaveChangesAsync();

		return new UpdateEventReply { Success = saveResult };
	}
}
