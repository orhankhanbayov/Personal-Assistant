using AutoMapper;
using DBService;
using DBService.Models;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace DBService.Services;

public class NotificationService : Notifications.NotificationsBase
{
	private readonly AppDbContext _dbContext;
	private readonly IMapper _mapper;

	public NotificationService(AppDbContext dbContext, IMapper mapper)
	{
		_dbContext = dbContext;
		_mapper = mapper;
	}

	public override async Task<GetNotificationListReply> GetNextNotifications(
		GetNotificationListRequest request,
		ServerCallContext context
	)
	{
		DateTime now = DateTime.Now;
		DateTime oneHourLater = now.AddHours(1);

		List<Notification> NextNotifications = await _dbContext
			.Notifications.Where(e =>
				e.SentAt >= now && e.SentAt <= oneHourLater && e.IsRead == false
			)
			.ToListAsync();

		return new GetNotificationListReply
		{
			NotifiationList = { NextNotifications.Select(_mapper.Map<NotificationRecord>) },
		};
	}

	public override async Task<AddNotificationReply> AddNotification(
		AddNotificationsRequest request,
		ServerCallContext context
	)
	{
		var notificationReq = _mapper.Map<Notification>(request.NotificationEntity);

		await _dbContext.Notifications.AddAsync(notificationReq);
		var saveResult = await _dbContext.SaveChangesAsync();

		return new AddNotificationReply { Success = saveResult };
	}

	public override async Task<MarkNotificationReadReply> MarkNotificationRead(
		MarkNotificationReadRequest request,
		ServerCallContext context
	)
	{
		var notification = await _dbContext.Notifications.FindAsync(request.NotificationID);
		if (notification == null)
		{
			return new MarkNotificationReadReply { Success = 0 };
		}

		notification.IsRead = true;
		_dbContext.Notifications.Update(notification);
		var saveResult = await _dbContext.SaveChangesAsync();

		return new MarkNotificationReadReply { Success = saveResult };
	}
}
