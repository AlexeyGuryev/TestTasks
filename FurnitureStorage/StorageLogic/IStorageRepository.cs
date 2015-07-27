using System;
using System.Collections.Generic;
using System.Data;
using StorageLogic.Model;

namespace StorageLogic
{
    public interface IStorageRepository : IDisposable
    {
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        Room CreateRoom(string name, DateTime creationDate);

        void UpdateRoom(Room room, DateTime stateDate);

        Room GetRoomByName(string name);

        List<Room> GetRoomsWithStateOnDate(DateTime? date);

        List<RoomState> GetRoomStates(string roomName);

        List<RoomState> GetAllRoomStates();

        void CommitTransaction();

        void RollbackTransaction();
    }
}
