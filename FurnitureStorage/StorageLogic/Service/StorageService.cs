using System;
using System.Collections.Generic;
using System.Linq;
using StorageLogic.Exception;
using StorageLogic.Model;

namespace StorageLogic.Service
{
    /// <summary>
    /// Доменная логика работы с комнатами и мебелью
    /// </summary>
    public class StorageService
    {
        private readonly IStorageRepository _repository = null;

        public List<Room> Rooms
        {
            get { return _repository.Rooms; }
        }

        public StorageService(IStorageRepository repository)
        {
            _repository = repository;
        }

        public Room CreateRoom(string roomName, DateTime creationDate)
        {
            var existRoom = Rooms.FirstOrDefault(c => c.Name == roomName);
            if (existRoom == null)
            {
                var newRoom = _repository.CreateRoom(roomName, creationDate);
                AddRoomStateIfChanged(newRoom, creationDate);
                return newRoom;
            }
            else
            {
                throw new ItemAlreadyExistsException("Room with name {0} already exists", roomName);
            }
        }

        public Room EnsureRoom(string roomName, DateTime creationDate)
        {
            var room = Rooms.FirstOrDefault(c => c.Name == roomName);
            if (room == null)
            {
                return CreateRoom(roomName, creationDate);
            }
            else
            {
                return room;
            }
        }

        public void RemoveRoom(string roomName, string transferRoomName, DateTime removeDate) 
        {
            var roomToRemove = GetRoom(roomName, removeDate);
            var transferRoom = GetRoom(transferRoomName, removeDate);

            CheckIfRoomStateIsLatest(roomToRemove, removeDate);
            CheckIfRoomStateIsLatest(transferRoom, removeDate);

            var furnitures = roomToRemove.Furnitures.Keys.ToList();
            foreach (var furniture in furnitures)
            {
                var countValue = roomToRemove.Furnitures[furniture];
                transferRoom.AddFurniture(furniture, countValue);
                roomToRemove.RemoveFurniture(furniture, countValue);
            }
            roomToRemove.RemoveDate = removeDate;

            AddRoomStateIfChanged(roomToRemove, removeDate);
            AddRoomStateIfChanged(transferRoom, removeDate);
        }

        // todo implement room state comparing
        private void AddRoomStateIfChanged(Room room, DateTime newStateDate)
        {
            //var lastRoomState = _repository.GetLatestRoomState(room.Name, newStateDate);

            //if (lastRoomState != null)
            //{
                //if (room.CheckEqualRoom())
                _repository.AddRoomState(room, newStateDate);
            //}
        }

        public void CreateFurniture(string furnitureType, string roomName, DateTime createFurnitureDate)
        {
            var room = GetRoom(roomName, createFurnitureDate);
            CheckIfRoomStateIsLatest(room, createFurnitureDate);

            room.AddFurniture(furnitureType);
            AddRoomStateIfChanged(room, createFurnitureDate);
        }

        private Room GetRoom(string roomName, DateTime? date = null)
        {
            var room = Rooms.FirstOrDefault(c => c.Name == roomName && (c.CreationDate <= date || date == null));
            if (room != null)
            {
                return room;
            }
            else
            {
                var dateMessage = date != null ? " on date " + date : string.Empty;
                throw new ItemNotFoundException("Room with name {0} not exists{1}", roomName, dateMessage);
            }
        }

        private void CheckIfRoomStateIsLatest(Room room, DateTime date)
        {
            var latestState = _repository.GetLatestRoomState(room.Name, DateTime.MaxValue);
            if (latestState.StateDate > date)
            {
                throw new DateConsistenceException("There is later changes for room {0}", room.Name);
            }
        }

        public void MoveFurniture(string furnitureType, string roomNameFrom, string roomNameTo,
            DateTime moveFurnitureDate)
        {
            var roomFrom = GetRoom(roomNameFrom, moveFurnitureDate);
            var roomTo = GetRoom(roomNameTo, moveFurnitureDate);

            CheckIfRoomStateIsLatest(roomFrom, moveFurnitureDate);
            CheckIfRoomStateIsLatest(roomTo, moveFurnitureDate);

            if (roomFrom.Furnitures.ContainsKey(furnitureType)
                && roomFrom.Furnitures[furnitureType] > 0)
            {
                roomTo.AddFurniture(furnitureType);
                roomFrom.RemoveFurniture(furnitureType);

                AddRoomStateIfChanged(roomFrom, moveFurnitureDate);
                AddRoomStateIfChanged(roomTo, moveFurnitureDate);
            }
            else
            {
                throw new ItemNotFoundException("The furniture of type {0} is not exists in room {1}",
                    furnitureType, roomFrom);
            }            
        }

        public List<Room> QueryRooms(DateTime queryDate)
        {
            var roomsInState = new List<Room>();
            foreach (var room in Rooms)
            {
                var roomState = _repository.GetLatestRoomState(room.Name, queryDate);
                if (roomState != null)
                {
                    roomsInState.Add(roomState.Room);
                }
            }
            return roomsInState;
        }

        public List<RoomState> GetHistory()
        {
            return _repository.RoomStates;
        }

        public List<RoomState> GetRoomHistory(string roomName)
        {
            return _repository.RoomStates
                .Where(c => c.Room.Name == roomName)
                .ToList();
        }
    }
}
