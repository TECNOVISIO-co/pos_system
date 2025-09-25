using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace Pos.Infrastructure.Access;

public class OleDbAccessConnectionFactory : IAccessConnectionFactory
{
    private readonly string _connectionString;

    public OleDbAccessConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Access")
            ?? throw new InvalidOperationException("Access connection string is not configured");
    }

    public DbConnection CreateConnection()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("ACE OLEDB provider is only supported on Windows hosts.");
        }

        var type = Type.GetType("System.Data.OleDb.OleDbConnection, System.Data.OleDb");
        if (type is null)
        {
            throw new InvalidOperationException("System.Data.OleDb is not available. Install the ACE OLEDB provider.");
        }

        var connection = Activator.CreateInstance(type, _connectionString) as DbConnection;
        if (connection is null)
        {
            throw new InvalidOperationException("Unable to create OleDbConnection instance.");
        }

        return connection;
    }
}
