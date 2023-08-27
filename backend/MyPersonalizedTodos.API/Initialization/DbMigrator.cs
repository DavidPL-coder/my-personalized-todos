using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MyPersonalizedTodos.API.Database;
using MyPersonalizedTodos.API.Extensions;

namespace MyPersonalizedTodos.API.Initialization;

// TODO: Refactor it.
// TODO: Make the class as non-static. It will be maybe a good idea.
public static class DbMigrator
{
    private static ILogger _logger;

    public static bool TryToMigrate(WebApplication app, IServiceScope serviceScope)
    {
        var appDatabase = serviceScope.Get<AppDbContext>().Database;
        _logger = app.Logger;
        var connectionTimeLimit = int.Parse(app.Configuration["MPT_DB_CONNECTION_LIMIT"]);
        var waitingInterval = int.Parse(app.Configuration["MPT_DB_CONNECTION_WAITING_TIME"]);

        var waitingTime = 0;
        while (waitingTime < connectionTimeLimit)
        {
            var dbConnectionStatus = DbConnectionChecker.GetConnectionStatus(appDatabase);
            if (dbConnectionStatus == AppDbConnectionStatus.Succesfull)
                return ApplyPendingMigrations(appDatabase);

            if (dbConnectionStatus == AppDbConnectionStatus.DbNotExist)
                return CreateAppDbWithTables(appDatabase);

            WaitForNextConnectionAttempt(dbConnectionStatus, waitingTime, waitingInterval);
            waitingTime += waitingInterval;
        }

        return false;
    }

    private static bool ApplyPendingMigrations(DatabaseFacade appDb)
    {
        appDb.Migrate();
        _logger.LogInformation("# The database has the new version of project.");
        return true;
    }

    private static bool CreateAppDbWithTables(DatabaseFacade appDb)
    {
        appDb.Migrate();
        _logger.LogWarning("# The database wasn't exist, so the app has created it as new with all required tables.");
        return true;
    }

    private static void WaitForNextConnectionAttempt(AppDbConnectionStatus dbConnectionStatus, int waitingTime, int waitingInterval)
    {
        var thingToConnect = dbConnectionStatus == AppDbConnectionStatus.NoDbServerConnection ? "DB SERVER" : "DATABASE";
        _logger.LogWarning("Waiting for {thingToConnect} connection. {waitingTime}ms have passed.", thingToConnect, waitingTime);
        Thread.Sleep(waitingInterval);
    }
}