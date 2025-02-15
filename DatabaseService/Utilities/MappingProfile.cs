using AutoMapper;
using DBService.Models;

namespace DBService.Utilities;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<User, UserMessage>()
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

		CreateMap<CallHistoryRecord, CallHistory>()
			.ForMember(
				dest => dest.StartTime,
				opt => opt.MapFrom(src => DateTime.Parse(src.StartTime))
			)
			.ForMember(
				dest => dest.EndTime,
				opt => opt.MapFrom(src => DateTime.Parse(src.EndTime))
			);

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
			.ForMember(dest => dest.EventID, opt => opt.Ignore());
		CreateMap<NotificationRecord, Notification>()
			.ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => DateTime.Parse(src.SentAt)))
			.ForMember(
				dest => dest.EventID,
				opt =>
				{
					opt.Condition(src => src.EventID > 0);
					opt.MapFrom(src => src.EventID);
				}
			)
			.ForMember(
				dest => dest.TaskID,
				opt =>
				{
					opt.Condition(src => src.TaskID > 0);
					opt.MapFrom(src => src.TaskID);
				}
			)
			.ForMember(dest => dest.NotificationID, opt => opt.Ignore());

		CreateMap<Notification, NotificationRecord>()
			.ForMember(dest => dest.SentAt, opt => opt.MapFrom(src => src.SentAt.ToString("o")))
			.ForMember(
				dest => dest.EventID,
				opt =>
				{
					opt.Condition(src => src.EventID > 0);
					opt.MapFrom(src => src.EventID);
				}
			)
			.ForMember(
				dest => dest.TaskID,
				opt =>
				{
					opt.Condition(src => src.TaskID > 0);
					opt.MapFrom(src => src.TaskID);
				}
			);

		CreateMap<EventDetails, EventDetailRecord>()
			.ForMember(
				dest => dest.StartTime,
				opt => opt.MapFrom(src => src.StartTime.ToString("o"))
			)
			.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("o")))
			.ForMember(
				dest => dest.CreatedAt,
				opt => opt.MapFrom(src => src.CreatedAt.ToString("o"))
			);

		CreateMap<TaskDetailRecord, TaskDetails>()
			.ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => DateTime.Parse(src.DueDate)))
			.ForMember(
				dest => dest.CreatedAt,
				opt => opt.MapFrom(src => DateTime.Parse(src.CreatedAt))
			)
			.ForMember(dest => dest.TaskID, opt => opt.Ignore());

		CreateMap<TaskDetails, TaskDetailRecord>()
			.ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate.ToString("o")))
			.ForMember(
				dest => dest.CreatedAt,
				opt => opt.MapFrom(src => src.CreatedAt.ToString("o"))
			);
	}
}
