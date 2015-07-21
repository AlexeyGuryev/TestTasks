using System;
using System.Collections.Generic;
using StorageLogic.Model;

namespace StorageLogic
{
    public interface IStorageRepository
    {
        Room CreateRoom(string name, DateTime creationDate);

        Room GetRoomByName(string name);

        List<Room> GetRoomsWithStateOnDate(DateTime? date);

        RoomState AddRoomState(Room room, DateTime stateDate);

        RoomState GetLatestRoomState(string roomName, DateTime? queryDate);

        List<RoomState> GetRoomStates(string roomName);

        List<RoomState> GetAllRoomStates();

        void RemoveRoom(string name, DateTime removeDate);

        //void SaveState();

        //void SaveStateAsync();
    }
}
