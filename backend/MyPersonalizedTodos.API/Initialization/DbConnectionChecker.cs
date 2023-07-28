using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.Common;

namespace MyPersonalizedTodos.API.Initialization;

public enum AppDbConnectionStatus
{
    NoDbServerConnection,
    DbNotExist,
    NotAvailable,
    Succesfull
}

public static class DbConnectionChecker
{
    public static AppDbConnectionStatus GetConnectionStatus(DatabaseFacade appDb)
    {
        using var masterDbContext = GetMasterDbContext();
        var masterDb = masterDbContext.Database;

        if (!masterDb.CanConnect())
            return AppDbConnectionStatus.NoDbServerConnection;

        if (!IsAppDatabaseExist(masterDb))
            return AppDbConnectionStatus.DbNotExist;

        return appDb.CanConnect() ? AppDbConnectionStatus.Succesfull : AppDbConnectionStatus.NotAvailable;
    }

    private static DbContext GetMasterDbContext()
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlServer(Environment.GetEnvironmentVariable("MPT_CONNECTION_STRING_FOR_CONNECTION_TEST"))
            .Options;

        return new DbContext(options);
    }

    private static bool IsAppDatabaseExist(DatabaseFacade masterDb)
    {
        masterDb.OpenConnection();
        using var checkAppDbExistingCommand = masterDb.GetDbConnection().CreateCommand();
        ConfigureCheckDbExistingCommand(checkAppDbExistingCommand);

        using var reader = checkAppDbExistingCommand.ExecuteReader();
        var isAppDatabaseExist = reader.HasRows;

        masterDb.CloseConnection();
        return isAppDatabaseExist;
    }

    private static void ConfigureCheckDbExistingCommand(DbCommand command)
    {
        command.CommandText = "SELECT name FROM sys.databases WHERE name = @appDb;";
        var appDbNameParameter = new SqlParameter("@appDb", Environment.GetEnvironmentVariable("MPT_DATABASE_NAME"));
        command.Parameters.Add(appDbNameParameter);
    }
}
