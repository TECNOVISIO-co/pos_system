using System.Data.Common;

namespace Pos.Infrastructure.Access;

public interface IAccessConnectionFactory
{
    DbConnection CreateConnection();
}
