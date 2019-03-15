This app is used for time tracking.

DEV ENVIRONMENT

1. Create SQL user with write access and edit the connection string in appsettings.json

2. Enter to project dir and create database:

    dotnet ef database update

3. Enter to project dir and set the admin user name:

    dotnet user-secrets set "SeedUserEmail" "admin@admin.com"
    
or to some anothe e-mail value.

4. Set admin password:

    dotnet user-secrets set "SeedUserPW" "password"
    
You can use another password value.

PRODUCTION ENVIRONMENT

1. Publish TimeTracking with command:

  dotnet publish -c Release

2. Modfy appsettings.json file by appending the following strings:

  "SeedUserEmail":"admin@admin.com",
  "SeedUserPW":"password"

  to the end of file.

3. Specify required application port in Port property.

4. Create user in Postgres and grant permissions to an empty database and update connection string in appsettings.json.
The database will be updated with schema and data when you run application first time.

5. Go to bin/Release/netcoreapp2.1/publish directory and upload the publiched application to server into /var/www diretory throw sftp with commands:

  sftp admin@192.168.88.27
  password:
  >put -r .

6. Register Timetracking as Linux services:

    sudo nano /lib/systemd/system/timetracking.service

    Add the following text to created file:

[Unit]
Description=timetracking service
After=network.target

[Service]
ExecStart=/usr/bin/dotnet /var/www/TimeTracking.dll
WorkingDirectory=/var/www
Restart=on-failure

[Install]
WantedBy=multi-user.target
EOF

  You can Commands to control new service:
# Reload SystemD and enable the service, so it will restart on reboots

sudo systemctl daemon-reload 
sudo systemctl enable daemonsrv

# Start service
sudo systemctl start daemonsrv

# View service status
systemctl status daemonsrv
    
USER ROLES
The users of this app are have two roles:
1. Administrator role;
  Can create edit and delete tasks, sprints and view and edit of any time track value of all users.
2. User role
  Can create tasks, view and edit of his own time tracks.
  
USERS REGISTRATION
You can also register users in the "Register" link of top menu

TEST DATA
TimeTracker stop create the test data on starting, if you just add at least one TimeTrack to admin user.