using System;
using System.Collections.Generic;
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

        public Room CreateRoom(string roomName, DateTime creationDate) { return null; }

        public Room EnsureRoom(string roomName, DateTime creationDate) { return null; }

        public void RemoveRoom(string roomName, string transferRoomName, DateTime removeDate) { }

        public void CreateFurniture(string furnitureType, string roomName, DateTime createFurnitureDate) { }

        public void MoveFurniture(string furnitureType, string roomNameFrom, string roomNameTo, DateTime moveFurnitureDate) { }

        public List<Room> QueryRooms(DateTime queryDate) { return null; }

        public List<Room> QueryRooms() { return null; }

        public List<RoomState> GetHistory() { return null; }

        public List<RoomState> GetRoomHistory(string roomName) { return null; }
    }
}
