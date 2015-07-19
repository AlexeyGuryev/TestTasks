using System;
using System.Collections.Generic;
using System.Linq;
using StorageLogic.Model;

namespace StorageLogic.Test.Stub
{
    public class StorageRepositoryStub : IStorageRepository
    {
        public List<Room> Rooms { get; private set; }
        public List<RoomState> RoomStates { get; private set; }

        public StorageRepositoryStub()
        {            
            Rooms = new List<Room>();
            RoomStates = new List<RoomState>();
        }

        public Room CreateRoom(string name, DateTime creationDate)
        {
            var newRoom = new Room
            {
                Name = name,
                CreationDate = creationDate,
                Furnitures = new Dictionary<string, int>()
            };
            Rooms.Add(newRoom);
            return newRoom;
        }

        public RoomState AddRoomState(Room room, DateTime stateDate)
        {
            var newRoomState = new RoomState(room, stateDate);
            RoomStates.Add(newRoomState);
            return newRoomState;
        }

        public RoomState GetLatestRoomState(string roomName, DateTime queryDate)
        {
            return RoomStates
                .OrderByDescending(c => c.StateDate)
                .FirstOrDefault(c => c.Room.Name == roomName && c.StateDate <= queryDate);
        }

        public void RemoveRoom(string name, DateTime removeDate)
        {
            var room = Rooms.FirstOrDefault(c => c.Name == name);
            if (room != null)
            {
                Rooms.Remove(room);
            }
        }
    }
}
