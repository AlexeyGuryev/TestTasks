using System.Data.Entity;
using StoragePersistence.Entities;

namespace StoragePersistence
{
    public class StorageEFContext : DbContext
    {
        public StorageEFContext() : base("StorageContext") { }

        public DbSet<RoomEntity> Rooms { get; set; }
    }
}
