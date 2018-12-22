using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TimeTracking.Authorization
{
    public static class TimeTracksOperations
    {
        public static OperationAuthorizationRequirement ViewStatistics = 
          new OperationAuthorizationRequirement {Name=Constants.ViewStatistics};

        public static OperationAuthorizationRequirement EditTimeTracksOfOthers = 
          new OperationAuthorizationRequirement {Name=Constants.EditTimeTracksOfOthers};
    }
}  