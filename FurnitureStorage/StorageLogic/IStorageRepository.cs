using System;
using System.Collections.Generic;
using StorageLogic.Model;

namespace StorageLogic
{
    public interface IStorageRepository
    {
        List<Room> Rooms { get; }

        List<RoomState> RoomStates { get; }

        Room CreateRoom(string name, DateTime creationDate);

        RoomState AddRoomState(Room room, DateTime stateDate);

        void RemoveRoom(string name, DateTime removeDate);
    }
}
