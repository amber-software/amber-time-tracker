This app is used for time tracking.


DEPLOYMENT
1. Create SQL user with write access and edit the connection string in appsettings.json

2. Enter to project dir and create database:
    dotnet ef database update

3. Enter to project dir and set the admin user name:
    dotnet user-secrets set "SeedUserEmail" "admin@admin.com"
    
or to some anothe e-mail value.

4. Set admin password:
    dotnet user-secrets set "SeedUserPW" "password"
    
or to some anothe password value.

5. Run the app execution by the command:
    dotnet run
    
USER ROLES
The users of this app are have two roles:
1. Administrator role;
  Can create edit and delete tasks, sprints and view and edit of any time track value of all users.
2. User role
  Can create tasks, view and edit of his own time tracks.
  
USERS REGISTRATION
You can also register users in the "Register" link of top menu