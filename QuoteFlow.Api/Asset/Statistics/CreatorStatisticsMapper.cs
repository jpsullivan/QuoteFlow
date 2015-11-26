using QuoteFlow.Api.Asset.Search.Constants;
using QuoteFlow.Api.Services;

namespace QuoteFlow.Api.Asset.Statistics
{
    public class CreatorStatisticsMapper : UserStatisticsMapper
    {
        public CreatorStatisticsMapper(IUserService userService) 
            : base(SystemSearchConstants.ForCreator(), userService)
        {
        }
    }
}