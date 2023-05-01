using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MyPersonalizedTodos.API.Database;

namespace MyPersonalizedTodos.API.Initialization;

// TODO: Take some constants from config file.
public static class DbMigrator
{
    private static ILogger _logger;

    public static bool TryToMigrate(WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();
        var appDatabase = serviceScope.ServiceProvider.GetService<AppDbContext>().Database;
        _logger = app.Logger;

        var waitingTime = 0;
        while (waitingTime < 30_000)
        {
            var dbConnectionStatus = DbConnectionChecker.GetConnectionStatus(appDatabase);
            if (dbConnectionStatus == AppDbConnectionStatus.Succesfull)
                return ApplyPendingMigrations(appDatabase);

            if (dbConnectionStatus == AppDbConnectionStatus.DbNotExist)
                return CreateAppDbWithTables(appDatabase);

            WaitForNextConnectionAttempt(dbConnectionStatus, waitingTime);
            waitingTime += 400;
        }

        return false;
    }

    private static bool ApplyPendingMigrations(DatabaseFacade appDb)
    {
        appDb.Migrate();
        return true;
    }

    private static bool CreateAppDbWithTables(DatabaseFacade appDb)
    {
        appDb.Migrate();
        _logger.LogWarning("The database wasn't exist, so the app has created it as new with all required tables.");
        return true;
    }

    private static void WaitForNextConnectionAttempt(AppDbConnectionStatus dbConnectionStatus, int waitingTime)
    {
        var thingToConnect = dbConnectionStatus == AppDbConnectionStatus.NoDbServerConnection ? "DB SERVER" : "DATABASE";
        _logger.LogWarning($"Waiting for {thingToConnect} connection. {waitingTime}ms have passed.");
        Thread.Sleep(400);
    }
}