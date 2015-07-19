using System;
using System.Collections.Generic;
using System.Linq;
using StorageLogic;
using StorageLogic.Model;

namespace StoragePersistence
{
    // todo create generic class for repo - with _context.SaveChanges (and the second one - with SaveChangesAsync)
    // except manual saving context - special method for it
    public class StorageDBRepository : IStorageRepository
    {
        private readonly StorageContext _context = null;

        public StorageDBRepository(StorageContext storageContext)
        {
            _context = storageContext;
        }

        public List<Room> Rooms
        {
            get { return _context.Rooms.ToList(); }
        }

        public List<RoomState> RoomStates
        {
            get { return _context.RoomStates.ToList(); }
        }

        public Room CreateRoom(string name, DateTime creationDate)
        {
            var room = _context.Rooms.Add(new Room
            {
                Name = name,
                CreationDate = creationDate
            });
            _context.SaveChanges();
            return room;
        }

        public RoomState AddRoomState(Room room, DateTime stateDate)
        {
            var newRoomState = _context.RoomStates.Add(new RoomState(room, stateDate));
            _context.SaveChanges();
            return newRoomState;
        }

        public void RemoveRoom(string name, DateTime removeDate)
        {
            var room = _context.Rooms.FirstOrDefault(c => c.Name == name);
            if (room != null)
            {
                _context.Rooms.Remove(room);
                _context.SaveChanges();
            }
        }

        public RoomState GetLatestRoomState(string roomName, DateTime? queryDate)
        {
            return _context.RoomStates
                .OrderByDescending(c => c.StateDate)
                .FirstOrDefault(c => c.Room.Name == roomName && (queryDate == null || c.StateDate <= queryDate));
        }
    }
}
