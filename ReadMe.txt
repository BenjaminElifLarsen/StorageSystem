
Database:
Regarding the usages of a MSSQL database, you will have the possiblity of either using Window login Authentication, SQL Server Authentication (SA) or no SQL database.
When selecting to use a database you will be prompted to enter the servername and database. For SQL SA you will also need to enter password and username.
After this you will be asked if you want to initialise a database creation or not. For the initialisation the program will for a short period need access to the Master database.  
	The initialsation will create the database, its table with columns and add some few wares. 

If you got Docker, you can run the following command to create a MSSQL container: 
	docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Password123." -p 1435:1433 --name storageDB -d mcr.microsoft.com/mssql/server:2019-latest
The username will be 'SA' and the password will be 'Password123.' and the servername will be 'localHost,1435'. 
Note, for the first time the container is started it might take between 30 and 60 seconds before it is ready to be connected to. 

