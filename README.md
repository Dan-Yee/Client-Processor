# Client Processor
Repository for ICSI 499, Capstone Project in Computer Science at UAlbany, Fall 2022

## Project Description:
[Boost Skin and Body Studio](https://boostskinbody.com/) is a cosmetology company that specializes in permanent makeupand skin operations. The operations performed by the company are more complicated and permanent than other cosmetology companies. Therefore, the clients are required to fill out legal consent forms before being operated on. Once the client fills out the paperwork, they can move on to the next step which is taking a photo. An after photo will also be taken to demonstrate the results of the procedure. Then the procedure will be performed. Then another photo will be taken so that the client can see the results of the operation. It would also be great marketing to get permission from some of the clients to show off the results of the procedures. Each client can have multiple operations, so the information about the procedures, the forms, and the files have to be managed properly. It needs to be easy to find information and maintain a system for organizing the files.

## Building and Running the Project:
> **Setting up the project**:

1) Open the solution file (Client-Processor.sln) using Visual Studio (not Visual Studio Code). The version should not matter.
2) Once the solution file has successfully loaded, ensure that the required Nuget Packages are installed. The required Nuget packages are saved in each projects project file (ClientApp.csproj and Server.csproj). The IDE should warn you that there are packages you need to install.
3) Build the project from the "Build" tab in Visual Studio. The project should successfully build after a few minutes.

--------------------------------------------------
> **Setting up the Database**:

If running using AWS: 
1) Ensure a .env file is saved as ".env" at the top-level of the "Server" directory. This file contains the credentials for connecting to the PostgreSQL database hosted on AWS RDS.
2) Connect to the AWS RDS database using pgadmin and execute all the .sql files included. You may need to execute them in a specific order due to foreign key constraints.
3) Edit the .env file with the database credentials and make sure the file is saved as ".env" in the top-level of the "Server" directory.

If running using localhost PostgreSQL Server:
1) Create a new database and execute all the .sql files that are included. You may need to execute them in a specific order due to foreign key constraints.
2) Once created, edit the .env file with the localhost database credentials and make sure the file is saved as ".env" in the top-level of the "Server" directory.

--------------------------------------------------
> **Running the project**:

Once the database is setup and the project is built (compiled), you need to set the project as a start up project.
1) Right click the "Solution" in the tab and select "Set as startup project". Set both the "Server" and "ClientApp" to "Run" and ensure that the Server is started before the ClientApp by placing the Server start up on top of the ClientApp in the list.
2) "Start" the project by selecting the "Run" button in Visual Studio. 
3) Once the app launches, you may login to the application using the default Admin credentials (Username: admin, Password: adminPassword). If you receive an error about incorrect login credentials, please double check that you entered the information correctly and that the AdminAccount.sql file was executed successfully. You can check this by selecting all from the Employee table in pgadmin.
