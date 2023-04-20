<h1 style="color:darkorange"> Selenium Crawler </h1> 


# To add migration
- ```dotnet ef migrations add --project ./Dashboard/  Initial_Migration```

# To update database
- ```dotnet ef database update --project ./Dashboard/```

# To remove last migration
To remove last migration; (dont forget to update database before this (will run down() for local): 
- dotnet ef migrations remove --project ./Dashboard/ -f

# Run
1. initialize db
2. run the app
3. login; email: `demo@demo.com`  password: `demo`

# Description 

- SOLID or other common principles are not implemented very well, such as injection etc. because of a limited time, Can be better with refactoring for sure. Target is to show selenium basics.
- Webb app to be updated 
- hangfire to be prepared 
- screen-shot to be added to read me refactor readme (getting started, run, purpose of app etc.)