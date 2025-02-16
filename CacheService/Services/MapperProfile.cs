namespace CacheService.Services;

using AutoMapper;
using CacheService;
using CacheService.Models;

public class MapperProfile : Profile
{
	public MapperProfile()
	{
		CreateMap<CreateConversationHistoryRequest, CallHistory>()
			.ForMember(
				dest => dest.StartTime,
				opt => opt.MapFrom(src => DateTime.Parse(src.StartTime))
			)
			.ForMember(dest => dest.CallHistoryId, opt => opt.Ignore())
			.ForMember(dest => dest.EndTime, opt => opt.Ignore())
			.ForMember(dest => dest.CallHistoryId, opt => opt.Ignore())
			.ForMember(
				dest => dest.ConversationHistory,
				opt => opt.MapFrom(src => new List<string> { src.InitialMessage })
			)
			.ForMember(dest => dest.User, opt => opt.Ignore());
		CreateMap<CallHistory, CallHistoryType>()
			.ForMember(
				dest => dest.StartTime,
				opt => opt.MapFrom(src => src.StartTime.ToString("o"))
			)
			.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("o")))
			.ForMember(dest => dest.CallHistoryId, opt => opt.Ignore());
	}
}
