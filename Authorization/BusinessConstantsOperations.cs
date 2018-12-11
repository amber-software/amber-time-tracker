using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TimeTracking.Authorization
{
    public static class BusinessConstantsOperations
    {        
        public static OperationAuthorizationRequirement EditBusinessConstants = 
          new OperationAuthorizationRequirement {Name=Constants.EditBusinessConstants};
    }
}