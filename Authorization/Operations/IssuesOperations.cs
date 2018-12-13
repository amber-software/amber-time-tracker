using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TimeTracking.Authorization
{
    public static class IssuesOperations
    {
        public static OperationAuthorizationRequirement DeleteTasks = 
          new OperationAuthorizationRequirement {Name=Constants.DeleteTasks};
    }
}  