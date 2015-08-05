using System.Data.Entity.SqlServer;

namespace StorageUI
{
    internal static class MissingDllHack
    {
        private static SqlProviderServices instance = SqlProviderServices.Instance;
    }
}