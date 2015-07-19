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
                _repository.AddRoomState(newRoom, creationDate);
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
                CreateRoom(roomName, creationDate);
            }
            return room;
        }

        public void RemoveRoom(string roomName, string transferRoomName, DateTime removeDate) { }

        public void CreateFurniture(string furnitureType, string roomName, DateTime createFurnitureDate) { }

        public void MoveFurniture(string furnitureType, string roomNameFrom, string roomNameTo, DateTime moveFurnitureDate) { }

        public List<Room> QueryRooms(DateTime queryDate) { return null; }



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
