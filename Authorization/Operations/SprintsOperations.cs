using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TimeTracking.Authorization
{
    public static class SprintsOperations
    {
        public static OperationAuthorizationRequirement EditSprints = 
          new OperationAuthorizationRequirement {Name=Constants.EditSprints};
    }
}  