using StorageLogic.Service;
using StoragePersistence;

namespace StorageLogic.Test
{
    public class StorageBaseTest
    {
        protected StorageService Service { get; set; } 

        public StorageBaseTest()
        {
            Service = new StorageService(new StorageMemoryRepositoryStub());
        }
    }
}
