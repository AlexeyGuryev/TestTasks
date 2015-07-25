using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using StorageLogic;
using StorageLogic.Model;

namespace StoragePersistence
{
    /// <summary>
    /// Заглушка репозитория (!!!без транзакционности!!!)
    /// </summary>
    public class StorageMemoryRepositoryStub : IStorageRepository
    {
        // хранилище в статических объектах класса
        private static readonly List<Room> _rooms = new List<Room>();
        private static readonly List<RoomState> _roomStates = new List<RoomState>();

        // todo реализовать Serializable-транзакцию
        //private static object @lock = new object();

        //private List<Room> TranRooms { get; set; }
        //private List<RoomState> TranRoomStates { get; set; }

        public void BeginTransaction()
        {
            //Monitor.Enter(@lock);
            //TranRooms = new List<Room>();
            //TranRoomStates = new List<RoomState>();
        }

        public void EndTransaction()
        {
            //TranRooms = null;
            //TranRoomStates = null;
            //Monitor.Exit(@lock);
        }

        public Room CreateRoom(string name, DateTime creationDate)
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

        public Room GetRoomByName(string name)
        {
            return _rooms.FirstOrDefault(c => c.Name == name);
        }

        public List<Room> GetRoomsWithStateOnDate(DateTime? date)
        {
            var roomsInState = new List<Room>();
            var actualRooms = _rooms
                .Where(c => c.CreationDate <= date && (c.RemoveDate == null || c.RemoveDate >= date));

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
            var newRoomState = new RoomState(room, stateDate);
            _roomStates.Add(newRoomState);
            return newRoomState;
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

        public void UpdateRoom(Room room)
        {
            // в памяти уже все обновилось по ссылке к этому моменту
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
