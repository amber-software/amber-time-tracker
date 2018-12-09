using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace TimeTracking.Authorization
{
    public static class ContactOperations
    {        
        public static OperationAuthorizationRequirement EditBusinessConstants = 
          new OperationAuthorizationRequirement {Name=Constants.EditBusinessConstants};
    }

    public class Constants
    {
        public const string ProductionMultiplier = "ProductionMultiplier";
        
        public static readonly string EditBusinessConstants = "EditBusinessConstants";       
         

        public static readonly string AdministratorsRole = "Administrators";
    }
}