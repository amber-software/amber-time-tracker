using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TimeTracking.Authorization
{
    public static class ReportOperations
    {
        public static OperationAuthorizationRequirement ViewReport = 
          new OperationAuthorizationRequirement {Name=Constants.ViewReport};      
    }
}  