using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using StorageLogic;
using StorageLogic.Exception;
using StorageLogic.Model;
using StoragePersistence.Entities;

namespace StoragePersistence
{
    public class StorageEFRepository : IStorageRepository
    {
        private StorageEFContext context;
        private DbContextTransaction transaction;

        public StorageEFRepository()
        {
            context = new StorageEFContext();
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            transaction = context.Database.BeginTransaction(isolationLevel);
        }

        public Room CreateRoom(string name, DateTime creationDate)
        {
            var newRoom = new RoomEntity
            {
                Name = name,
                CreationDate = creationDate
            };
            context.Rooms.Add(newRoom);
            context.SaveChanges();
            return newRoom.ToModel();
        }

        public void UpdateRoom(Room room, DateTime stateDate)
        {
            var roomEntity = context.Rooms.FirstOrDefault(c => c.Name == room.Name);
            CheckLatestRoomState(roomEntity, stateDate);

            if (roomEntity != null)
            {
                roomEntity.UpdateFromModel(room, stateDate);
                context.SaveChanges();
            }
        }

        private void CheckLatestRoomState(RoomEntity roomEntity, DateTime stateDate)
        {
            var lastRoomState = GetLatestRoomState(roomEntity, stateDate);
            if (lastRoomState != null)
            {
                if (lastRoomState.StateDate >= stateDate)
                {
                    throw new DateConsistenceException(
                        "Room {0} already has state on {1:dd.MM.yyyy} or later date",
                        roomEntity.Name, stateDate);
                }
            }
        }

        public Room GetRoomByName(string name)
        {
            var roomEntity = GetRoomEntityByName(name);
            return roomEntity == null ? null : roomEntity.ToModel();
        }

        private RoomEntity GetRoomEntityByName(string name)
        {
            return context.Rooms.FirstOrDefault(c => c.Name == name);
        }

        public List<Room> GetRoomsWithStateOnDate(DateTime? date)
        {
            var actualRooms = context.Rooms
                .Where(c => c.CreationDate <= date && (c.RemoveDate == null || c.RemoveDate > date))
                .ToList();

            return actualRooms
                .Select(c => c.ToModel(date))
                .ToList();
        }

        private RoomState GetLatestRoomState(string roomName, DateTime? queryDate)
        {
            var roomEntity = GetRoomEntityByName(roomName);
            return GetLatestRoomState(roomEntity, queryDate);
        }

        private RoomState GetLatestRoomState(RoomEntity roomEntity, DateTime? queryDate)
        {
            var roomStateEntity = roomEntity.GetStateOnDate(queryDate);
            return roomStateEntity == null ? null : roomStateEntity.ToModel();
        }

        public List<RoomState> GetRoomStates(string roomName)
        {
            var roomEntity = GetRoomEntityByName(roomName);
            var roomStatesEntities = roomEntity.RoomStates.ToList();
            return roomStatesEntities
                .Select(c => c.ToModel())
                .ToList();
        }

        public List<RoomState> GetAllRoomStates()
        {
            var roomStatesEntities = context.Rooms.SelectMany(room => room.RoomStates).ToList();
            return roomStatesEntities
                .Select(c => c.ToModel())
                .ToList();
        }

        public void CommitTransaction()
        {
            if (transaction != null)
            {
                transaction.Commit();
            }
        }

        public void RollbackTransaction()
        {
            if (transaction != null)
            {
                transaction.Rollback();
            }
        }

        public void Dispose()
        {
            if (transaction != null)
            {
                transaction.Dispose();
            }
            if (context != null)
            {
                context.Dispose();
            }
        }
    }
}
