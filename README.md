# Recruiting Platform
## Running the project locally
### To run this project locally, follow the instructions below:
1. Clone the project repository and navigate to the root directory of the project named "RecruitingPlatform" in your solution explorer.
2. Create a file named .env in the root directory of the project and copy the following content into it: 

```
CONNECTION_STRING="Server=(localdb)\\MSSQLLocalDB;Database=RecruitingPlatformDb;Trusted_Connection=True;TrustServerCertificate=True;"
ADMIN_EMAIL="admintest@gmail.com"
ADMIN_PASSWORD="SuperSecretPassword123!"
```

> [!NOTE]
> You can also paste you own connection string and admin credentials if you wish.

3. Run the following command to create the database, apply the necessary tables, build, and start the application:

```
dotnet ef database update --project RecruitingPlatform
dotnet run --project RecruitingPlatform
```

4. Copy one of the generated URLs (typically https://localhost:7275 or http://localhost:5000) from the console and open it in your web browser.