using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using StorageLogic;
using StorageLogic.Exception;
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

        // todo тут можно реализовать Serializable-транзакцию
        //private static object @lock = new object();

        //private List<Room> TranRooms { get; set; }
        //private List<RoomState> TranRoomStates { get; set; }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            //Monitor.Enter(@lock);
            //TranRooms = new List<Room>();
            //TranRoomStates = new List<RoomState>();
        }

        public void CommitTransaction()
        {
            //TranRooms = null;
            //TranRoomStates = null;
            //Monitor.Exit(@lock);
        }

        public void RollbackTransaction()
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
                Furnitures = new Dictionary<string, int>()
            };
            _rooms.Add(newRoom);
            AddRoomState(newRoom, creationDate);
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

        private RoomState AddRoomState(Room room, DateTime newStateDate)
        {
            var lastRoomState = GetLatestRoomState(room.Name);
            if (lastRoomState != null)
            {
                if (lastRoomState.StateDate >= newStateDate)
                {
                    throw new DateConsistenceException(
                        "Room {0} already has state on {1:dd.MM.yyyy} or later date",
                        room.Name, newStateDate);
                }
                if (room.Equals(lastRoomState.Room))
                {
                    return null;
                }
            }
            var newRoomState = new RoomState(room, newStateDate);
            _roomStates.Add(newRoomState);
            return newRoomState;
        }

        private RoomState GetLatestRoomState(string roomName, DateTime? queryDate = null)
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

        public void UpdateRoom(Room room, DateTime stateDate)
        {
            AddRoomState(room, stateDate);
        }

        public void Dispose()
        {

        }
    }
}
