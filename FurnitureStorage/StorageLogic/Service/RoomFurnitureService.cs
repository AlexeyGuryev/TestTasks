using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StorageLogic.Exception;
using StorageLogic.Model;

namespace StorageLogic.Service
{
    public class RoomFurnitureService
    {
        public static void MoveFurnitureBetweenRooms(Room roomFrom, Room roomTo, DateTime removeDate, string furnitureType)
        {
            //var furnitures = roomToRemove.Furnitures.Keys.ToList();
            //foreach (var furniture in furnitures)
            //{
            //    var countValue = roomToRemove.Furnitures[furniture];
            //    roomTo.AddFurniture(furniture, countValue);
            //    roomFrom.RemoveFurniture(furniture, countValue);
            //}
            //roomFrom.RemoveDate = removeDate;

            //AddRoomStateIfChanged(roomToRemove, removeDate);
            //AddRoomStateIfChanged(transferRoom, removeDate);
        }

        public static void AddFurnitureToRoom(string furnitureType, Room roomName, DateTime createFurnitureDate)
        {
            //var room = GetRoom(roomName, createFurnitureDate);
            //CheckIfRoomStateIsLatest(room, createFurnitureDate);

            //room.AddFurniture(furnitureType);
            //AddRoomStateIfChanged(room, createFurnitureDate);
        }

        //private void CheckIfRoomStateIsLatest(Room room, DateTime date)
        //{
        //    var latestState = _repository.GetLatestRoomState(room.Name, DateTime.MaxValue);
        //    if (latestState.StateDate > date)
        //    {
        //        throw new DateConsistenceException("There is later changes for room {0}", room.Name);
        //    }
        //}

        public void MoveFurniture(string furnitureType, string roomNameFrom, string roomNameTo,
            DateTime moveFurnitureDate)
        {
            //var roomFrom = GetRoom(roomNameFrom, moveFurnitureDate);
            //var roomTo = GetRoom(roomNameTo, moveFurnitureDate);

            //CheckIfRoomStateIsLatest(roomFrom, moveFurnitureDate);
            //CheckIfRoomStateIsLatest(roomTo, moveFurnitureDate);

            //if (roomFrom.Furnitures.ContainsKey(furnitureType)
            //    && roomFrom.Furnitures[furnitureType] > 0)
            //{
            //    roomTo.AddFurniture(furnitureType);
            //    roomFrom.RemoveFurniture(furnitureType);

            //    AddRoomStateIfChanged(roomFrom, moveFurnitureDate);
            //    AddRoomStateIfChanged(roomTo, moveFurnitureDate);
            //}
            //else
            //{
            //    throw new ItemNotFoundException("The furniture of type {0} is not exists in room {1}",
            //        furnitureType, roomFrom);
            //}
        }
    }
}
