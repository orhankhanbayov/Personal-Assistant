namespace PersonalAssistant.Models
{
	public interface IBackgroundService
	{
		Task NotifyUpComingEvents();

		void processOutgoingRequests(IRecurringJobManager recurringJobManager);
	}
}
