using AssistantService.Models;
using AutoMapper;

namespace AssistantService.Utilities;

public class MapperService : Profile
{
	public MapperService()
	{
		CreateMap<UserMessage, User>()
			.ForMember(
				dest => dest.CreatedAt,
				opt => opt.MapFrom(src => DateTime.Parse(src.CreatedAt))
			)
			.ForMember(
				dest => dest.LastLoginAt,
				opt =>
					opt.MapFrom(src =>
						string.IsNullOrWhiteSpace(src.LastLoginAt)
							? (DateTime?)null
							: DateTime.Parse(src.LastLoginAt)
					)
			);
		CreateMap<User, UserRecord>()
			.ForMember(
				dest => dest.CreatedAt,
				opt => opt.MapFrom(src => src.CreatedAt.ToString("o"))
			)
			.ForMember(
				dest => dest.LastLoginAt,
				opt =>
					opt.MapFrom(src =>
						src.LastLoginAt.HasValue ? src.LastLoginAt.Value.ToString("o") : null
					)
			);

		CreateMap<CallHistory, CallHistoryRecord>()
			.ForMember(
				dest => dest.StartTime,
				opt => opt.MapFrom(src => src.StartTime.ToString("o"))
			)
			.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("o")));

		CreateMap<CallHistoryType, CallHistory>()
			.ForMember(
				dest => dest.StartTime,
				opt => opt.MapFrom(src => DateTime.Parse(src.StartTime))
			)
			.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => DateTime.Parse(src.EndTime)))
			.ForMember(dest => dest.CallHistoryId, opt => opt.Ignore());

		CreateMap<EventDetails, EventDetailRecord>()
			.ForMember(
				dest => dest.StartTime,
				opt => opt.MapFrom(src => src.StartTime.ToString("o"))
			)
			.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("o")))
			.ForMember(
				dest => dest.CreatedAt,
				opt => opt.MapFrom(src => src.CreatedAt.ToString("o"))
			)
			.ForMember(
				dest => dest.RecurrencePattern,
				opt => opt.MapFrom(src => src.RecurrencePattern ?? "")
			)
			.ForMember(dest => dest.EventID, opt => opt.Ignore());
		CreateMap<EventDetailRecord, EventDetails>()
			.ForMember(
				dest => dest.StartTime,
				opt => opt.MapFrom(src => DateTime.Parse(src.StartTime))
			)
			.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => DateTime.Parse(src.EndTime)))
			.ForMember(
				dest => dest.CreatedAt,
				opt => opt.MapFrom(src => DateTime.Parse(src.CreatedAt))
			)
			.ForMember(
				dest => dest.RecurrencePattern,
				opt =>
					opt.MapFrom(src =>
						string.IsNullOrWhiteSpace(src.RecurrencePattern)
							? null
							: src.RecurrencePattern
					)
			);

		CreateMap<Notification, NotificationRecord>()
			.ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.SentAt.ToString("o")))
			.ForMember(
				dest => dest.TaskID,
				opt =>
				{
					opt.Condition(src => src.TaskID > 0);
					opt.MapFrom(src => src.TaskID);
				}
			)
			.ForMember(
				dest => dest.EventID,
				opt =>
				{
					opt.Condition(src => src.EventID > 0);
					opt.MapFrom(src => src.EventID);
				}
			);
		CreateMap<NotificationRecord, Notification>()
			.ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => DateTime.Parse(src.SentAt)))
			.ForMember(
				dest => dest.TaskID,
				opt =>
				{
					opt.Condition(src => src.TaskID > 0);
					opt.MapFrom(src => src.TaskID);
				}
			)
			.ForMember(
				dest => dest.EventID,
				opt =>
				{
					opt.Condition(src => src.EventID > 0);
					opt.MapFrom(src => src.EventID);
				}
			);
		CreateMap<TaskDetailRecord, TaskDetails>()
			.ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => DateTime.Parse(src.DueDate)))
			.ForMember(
				dest => dest.CreatedAt,
				opt => opt.MapFrom(src => DateTime.Parse(src.CreatedAt))
			);
	}
}
