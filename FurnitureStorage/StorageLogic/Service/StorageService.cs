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

        public StorageService(IStorageRepository repository)
        {
            _repository = repository;
        }

        public Room CreateRoom(string roomName, DateTime creationDate)
        {
            _repository.BeginTransaction();

            var room = GetRoom(roomName, null);
            try
            {                
                if (room == null)
                {
                    room = _repository.CreateRoom(roomName, creationDate);
                    AddRoomStateIfChanged(room, creationDate);
                }
                else
                {
                    throw new ItemAlreadyExistsException("Room with name {0} already exists", roomName);
                }
            }
            finally
            {
                _repository.EndTransaction();
            }
            return room;
        }

        public Room EnsureRoom(string roomName, DateTime creationDate)
        {
            var room = _repository.GetRoomByName(roomName);
            return room ?? CreateRoom(roomName, creationDate);
        }

        public void RemoveRoom(string roomName, string transferRoomName, DateTime removeDate) 
        {
            _repository.BeginTransaction();

            var roomToRemove = GetExistsRoom(roomName, removeDate);
            var transferRoom = GetExistsRoom(transferRoomName, removeDate);

            var furnitures = roomToRemove.Furnitures.Keys.ToList();
            foreach (var furniture in furnitures)
            {
                var countValue = roomToRemove.Furnitures[furniture];
                transferRoom.AddFurniture(furniture, countValue);
                roomToRemove.RemoveFurniture(furniture, countValue);
            }
            roomToRemove.RemoveDate = removeDate;

            UpdateRoomAndAddNewState(roomToRemove, removeDate);
            UpdateRoomAndAddNewState(transferRoom, removeDate);

            _repository.EndTransaction();
        }

        public void CreateFurniture(string furnitureType, string roomName, DateTime createFurnitureDate)
        {
            _repository.BeginTransaction();

            var room = GetExistsRoom(roomName, createFurnitureDate);
            room.AddFurniture(furnitureType);

            UpdateRoomAndAddNewState(room, createFurnitureDate);

            _repository.EndTransaction();
        }

        public void MoveFurniture(string furnitureType, string roomNameFrom, string roomNameTo,
            DateTime moveFurnitureDate)
        {
            _repository.BeginTransaction();

            try
            {
                var roomFrom = GetExistsRoom(roomNameFrom, moveFurnitureDate);
                var roomTo = GetExistsRoom(roomNameTo, moveFurnitureDate);

                if (roomFrom.Furnitures.ContainsKey(furnitureType)
                    && roomFrom.Furnitures[furnitureType] > 0)
                {
                    roomTo.AddFurniture(furnitureType);
                    roomFrom.RemoveFurniture(furnitureType);

                    UpdateRoomAndAddNewState(roomTo, moveFurnitureDate);
                    UpdateRoomAndAddNewState(roomFrom, moveFurnitureDate);
                }
                else
                {
                    throw new ItemNotFoundException("The furniture of type {0} is not exists in room {1}",
                        furnitureType, roomFrom);
                }
            }
            finally
            {
                _repository.EndTransaction();
            }
        }

        public List<Room> QueryRooms(DateTime? queryDate)
        {
            return _repository.GetRoomsWithStateOnDate(queryDate ?? DateTime.MaxValue);
        }

        public List<RoomState> GetHistory()
        {
            return _repository.GetAllRoomStates();
        }

        public List<RoomState> GetRoomHistory(string roomName)
        {
            return _repository.GetRoomStates(roomName);
        }

        private void AddRoomStateIfChanged(Room room, DateTime newStateDate)
        {
            var lastRoomState = _repository.GetLatestRoomState(room.Name, newStateDate);
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
                    return;
                }
            }
            _repository.AddRoomState(room, newStateDate);
        }

        private void UpdateRoomAndAddNewState(Room room, DateTime updateDate)
        {
            _repository.UpdateRoom(room);
            AddRoomStateIfChanged(room, updateDate);
        }

        private Room GetExistsRoom(string roomName, DateTime? date = null)
        {
            var room = GetRoom(roomName, date);
            if (room == null)
            {
                var dateMessage = date != null ? string.Format(" on date {0:dd.MM.yyyy}", date) : string.Empty;
                throw new ItemNotFoundException("Room with name {0} not exists{1}", roomName, dateMessage);
            }
            return room;
        }

        private Room GetRoom(string roomName, DateTime? date = null)
        {
            var room = _repository.GetRoomByName(roomName);
            if (room != null && (room.CreationDate <= date || date == null))
            {
                return room;
            }
            return null;
        }
    }
}
