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
    public class StorageService : IDisposable
    {
        private readonly IStorageRepository _repository = null;

        public StorageService(IStorageRepository repository)
        {
            _repository = repository;
        }

        public Room CreateRoom(string roomName, DateTime creationDate)
        {
            _repository.BeginTransaction();
            try
            {
                var room = GetRoom(roomName, null);
                if (room == null)
                {
                    room = _repository.CreateRoom(roomName, creationDate);
                    _repository.CommitTransaction();
                    return room;
                }
                else
                {
                    throw new ItemAlreadyExistsException("Room with name {0} already exists", roomName);
                }
            }
            catch (System.Exception)
            {
                _repository.RollbackTransaction();
                throw;
            }
        }

        public Room EnsureRoom(string roomName, DateTime creationDate)
        {
            return GetRoom(roomName) ?? CreateRoom(roomName, creationDate);
        }

        public void RemoveRoom(string roomName, string transferRoomName, DateTime removeDate)
        {
            _repository.BeginTransaction();
            try
            {
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

                _repository.UpdateRoom(roomToRemove, removeDate);
                _repository.UpdateRoom(transferRoom, removeDate);

                _repository.CommitTransaction();
            }
            catch (System.Exception)
            {
                _repository.RollbackTransaction();
                throw;
            }
        }

        public void CreateFurniture(string furnitureType, string roomName, DateTime createFurnitureDate)
        {
            _repository.BeginTransaction();
            try
            {
                var room = GetExistsRoom(roomName, createFurnitureDate);
                room.AddFurniture(furnitureType);

                _repository.UpdateRoom(room, createFurnitureDate);

                _repository.CommitTransaction();
            }
            catch (System.Exception)
            {
                _repository.RollbackTransaction();
                throw;
            }
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

                    _repository.UpdateRoom(roomTo, moveFurnitureDate);
                    _repository.UpdateRoom(roomFrom, moveFurnitureDate);
                    _repository.CommitTransaction();
                }
                else
                {
                    throw new ItemNotFoundException("The furniture of type {0} is not exists in room {1}",
                        furnitureType, roomFrom);
                }
            }
            catch (System.Exception)
            {
                _repository.RollbackTransaction();
                throw;
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

        public void Dispose()
        {
            if (_repository != null)
            {
                _repository.Dispose();
            }
        }
    }
}
