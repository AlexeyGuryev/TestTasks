using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageLogic.Model;

namespace StoragePersistence
{
    public class StorageContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomState> RoomStates { get; set; }

        public StorageContext() { }
    }
}
