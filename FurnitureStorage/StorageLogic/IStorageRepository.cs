using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageLogic.Model;

namespace StorageLogic
{
    public interface IStorageRepository
    {
        List<Room> Rooms { get; }

        Room CreateRoom(string name, DateTime creationDate);

        void RemoveRoom(string name, DateTime removeDate);
    }
}
