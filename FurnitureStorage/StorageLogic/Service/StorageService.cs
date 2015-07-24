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
            var newRoom = _repository.CreateRoom(roomName, creationDate);
            _repository.AddRoomState(newRoom, creationDate);
            return newRoom;
        }

        public Room EnsureRoom(string roomName, DateTime creationDate)
        {
            var room = _repository.GetRoomByName(roomName);
            return room ?? CreateRoom(roomName, creationDate);
        }

        public void RemoveRoom(string roomName, string transferRoomName, DateTime removeDate) 
        {
            var roomToRemove = GetRoom(roomName, removeDate);
            var transferRoom = GetRoom(transferRoomName, removeDate);

            CheckIfRoomStateIsLatest(roomToRemove, removeDate);
            CheckIfRoomStateIsLatest(transferRoom, removeDate);

            MoveFurnitureBetweenRooms(roomToRemove, transferRoom);
            roomToRemove.RemoveDate = removeDate;

            _repository.AddRoomState(roomToRemove, removeDate);
            _repository.AddRoomState(transferRoom, removeDate);
        }

        public void CreateFurniture(string furnitureType, string roomName, DateTime createFurnitureDate)
        {
            var room = GetRoom(roomName, createFurnitureDate);
            CheckIfRoomStateIsLatest(room, createFurnitureDate);

            room.AddFurniture(furnitureType);
            _repository.AddRoomState(room, createFurnitureDate);
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

                _repository.AddRoomState(roomFrom, moveFurnitureDate);
                _repository.AddRoomState(roomTo, moveFurnitureDate);
            }
            else
            {
                throw new ItemNotFoundException("The furniture of type {0} is not exists in room {1}",
                    furnitureType, roomFrom);
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

        #region util methods

        private Room GetRoom(string roomName, DateTime? date = null)
        {
            var room = _repository.GetRoomByName(roomName);
            if (room != null && (room.CreationDate <= date || date == null))
            {
                return room;
            }
            else
            {
                var dateMessage = date != null ? string.Format(" on date {0:dd.MM.yyyy}", date) : string.Empty;
                throw new ItemNotFoundException("Room with name {0} not exists{1}", roomName, dateMessage);
            }
        }

        private void CheckIfRoomStateIsLatest(Room room, DateTime date)
        {
            var latestState = _repository.GetLatestRoomState(room.Name, null);
            if (latestState.StateDate > date)
            {
                throw new DateConsistenceException("There are later changes for room {0}", room.Name);
            }
        }

        private void MoveFurnitureBetweenRooms(Room roomFrom, Room roomTo, string furnitureTypeToMove = null)
        {
            var furnitures = roomFrom.Furnitures.Keys.ToList();
            foreach (var furniture in furnitures)
            {
                if (string.IsNullOrEmpty(furnitureTypeToMove) || furnitureTypeToMove == furniture)
                {
                    var countValue = roomFrom.Furnitures[furniture];
                    roomTo.AddFurniture(furniture, countValue);
                    roomFrom.RemoveFurniture(furniture, countValue);
                }
            }
        }

        #endregion
    }
}
