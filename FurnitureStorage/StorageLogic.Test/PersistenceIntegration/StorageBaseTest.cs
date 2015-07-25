using System;
using StorageLogic.Service;
using StoragePersistence;

namespace StorageLogic.Test
{
    public class StorageBaseTest
    {
        protected DateTime Now = DateTime.Now.Date;
        protected DateTime Tomorrow = DateTime.Now.Date.AddDays(1);
        protected DateTime AfterTomorrow = DateTime.Now.Date.AddDays(2);
        protected DateTime Yesterday = DateTime.Now.Date.AddDays(-1);

        protected StorageService Service { get; set; } 

        public StorageBaseTest()
        {
            Service = new StorageService(new StorageMemoryRepositoryStub());
        }
    }
}
