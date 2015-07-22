using System;
using System.Collections.Generic;
using System.Linq;
using StorageLogic;
using StorageLogic.Exception;
using StorageLogic.Model;

namespace StoragePersistence
{
    public class StorageMemoryRepository : IStorageRepository
    {
        // хранилище в статических объектах класса
        private static readonly List<Room> _rooms = new List<Room>();
        private static readonly List<RoomState> _roomStates = new List<RoomState>();
        private static object @lock = new object();

        public Room CreateRoom(string name, DateTime creationDate)
        {
            var existRoom = _rooms.FirstOrDefault(c => c.Name == name);
            if (existRoom == null)
            {
                lock (@lock)
                {
                    existRoom = _rooms.FirstOrDefault(c => c.Name == name);
                    if (existRoom == null)
                    {
                        var newRoom = new Room
                        {
                            Name = name,
                            CreationDate = creationDate,
                            FurnitureList = new List<string>()
                        };
                        _rooms.Add(newRoom);
                        return newRoom;
                    }
                }
            }
            throw new ItemAlreadyExistsException("Room with name {0} already exists", name);
        }

        public Room GetRoomByName(string name)
        {
            return _rooms.FirstOrDefault(c => c.Name == name);
        }

        public List<Room> GetRoomsWithStateOnDate(DateTime? date)
        {
            //return _roomStates
            //    .OrderByDescending(c => c.StateDate)
            //    .GroupBy(c => c.Room)
            //    .Where(group => group.First())
            // todo придумать нормальный linq запрос с группировкой
            // такое получение списка комнат на дату не проканает для StorageDBRepository - нужно писать нормальный мапирующийся в SQL запрос

            var roomsInState = new List<Room>();
            var actualRooms = _rooms.Where(c => c.CreationDate <= date && (c.RemoveDate == null || c.RemoveDate >= date));
            foreach (var room in actualRooms)
            {
                var roomState = GetLatestRoomState(room.Name, date);
                if (roomState != null)
                {
                    roomsInState.Add(roomState.Room);
                }
            }
            return roomsInState;
        }

        public RoomState AddRoomState(Room room, DateTime stateDate)
        {
            lock (@lock)
            {
                var lastRoomState = GetLatestRoomState(room.Name, stateDate);
                if (lastRoomState == null || !room.Equals(lastRoomState.Room))
                {
                    var newRoomState = new RoomState(room, stateDate);
                    _roomStates.Add(newRoomState);
                    return newRoomState;
                }
            }
            return null;
        }

        public RoomState GetLatestRoomState(string roomName, DateTime? queryDate)
        {
            return _roomStates
                .OrderByDescending(c => c.StateDate)
                .FirstOrDefault(c => c.Room.Name == roomName && (queryDate == null || c.StateDate <= queryDate));
        }

        public List<RoomState> GetRoomStates(string roomName)
        {
            return _roomStates
                .Where(c => c.Room.Name == roomName)
                .ToList();
        }

        public List<RoomState> GetAllRoomStates()
        {
            return _roomStates;
        }

        public void RemoveRoom(string name, DateTime removeDate)
        {
            var room = _rooms.FirstOrDefault(c => c.Name == name);
            if (room != null)
            {
                _rooms.Remove(room);
            }
        }
    }
}
