using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageLogic.Exception;
using StorageLogic.Model;
using StorageLogic.Service;
using StorageLogic.Test.Stub;

namespace StorageLogic.Test
{
    /// <summary>
    /// # remove-room: transferRoom существует на date и все его состояния раньше date
    /// # remove-room: room существует на date и все его состояния раньше date
    /// # remove-room: date >= room.creationDate
    /// # remove-room: вся мебель мигригрует
    /// # remove-room: новое состояние room - без мебели и с removeDate not null
    /// # remove-room: новое состояние transferRoom - с новой мебелью
    /// </summary>
    [TestClass]
    public class RemoveRoomTest
    {
        private readonly StorageService _service = new StorageService(new StorageRepositoryStub());

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void TransferRoomMustExistsOnRemoveDate()
        {
            var roomToRemove = GetRoomWithFurniture(DateTime.Now);
            _service.RemoveRoom(roomToRemove.Name, "The transfer room, which not exists", DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void RoomToRemoveMustExistsOnRemoveDate()
        {
            var roomToRemove = GetRoomWithFurniture(DateTime.Now);
            _service.RemoveRoom("The transfer room, which not exists", roomToRemove.Name, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof (DateConsistenceException))]
        public void RemoveDateIsLaterThanLastRoomStateDate()
        {
            var yesterdayDate = DateTime.Now.AddDays(-1);

            var roomToRemove = GetTestRoom(yesterdayDate);
            _service.CreateFurniture("Table", roomToRemove.Name, DateTime.Now);

            var transferRoom = GetRoomWithFurniture(DateTime.Now);

            _service.RemoveRoom(roomToRemove.Name, transferRoom.Name, yesterdayDate);
        }

        [TestMethod]
        [ExpectedException(typeof (DateConsistenceException))]
        public void RemoveRoomCheckCreationDateIsLaterThanRemoveDate()
        {
            var yesterdayDate = DateTime.Now.AddDays(-1);

            var roomToRemove = GetTestRoom(DateTime.Now);
            var transferRoom = GetRoomWithFurniture(DateTime.Now);

            _service.RemoveRoom(roomToRemove.Name, transferRoom.Name, yesterdayDate);
        }

        [TestMethod]
        [ExpectedException(typeof(DateConsistenceException))]
        public void RemoveRoomCauseTransferAllFurniture()
        {
            var yesterdayDate = DateTime.Now.AddDays(-1);

            var furnitureName = "Desk";

            var roomToRemove = GetRoomWithFurniture(DateTime.Now, furnitureName);
            var transferRoom = GetTestRoom(DateTime.Now);

            var transferRoomFurnitureCount = transferRoom.Furnitures.ContainsKey(furnitureName)
                ? transferRoom.Furnitures[furnitureName]
                : 0;

            var roomToRemoveFurnitureCount = roomToRemove.Furnitures[furnitureName];            
            _service.RemoveRoom(roomToRemove.Name, transferRoom.Name, yesterdayDate);

            Assert.IsTrue(transferRoom.Furnitures.ContainsKey(furnitureName)
                && transferRoom.Furnitures[furnitureName] == transferRoomFurnitureCount + roomToRemoveFurnitureCount);
        }

        [TestMethod]
        public void RemoveWithTransferChangeRoomStates()
        {
            var furnitureName = "Desk";
            var removeDate = DateTime.Now;

            var roomToRemove = GetRoomWithFurniture(removeDate, furnitureName);
            var transferRoom = GetTestRoom(removeDate);
            
            _service.RemoveRoom(roomToRemove.Name, transferRoom.Name, removeDate);

            var roomToRemoveState = _service.GetRoomHistory(roomToRemove.Name);
            var transferRoomState = _service.GetRoomHistory(transferRoom.Name);

            Assert.IsTrue(roomToRemoveState.Any(rr => rr.StateDate == removeDate 
                &&
                transferRoomState.Any(tr => tr.StateDate == removeDate)));
        }

        private Room GetTestRoom(DateTime date)
        {
            return _service.EnsureRoom("Living room", date);
        }

        private Room GetRoomWithFurniture(DateTime date, string furnitureName = "Bath")
        {
            var room = _service.EnsureRoom(furnitureName, date);
            _service.CreateFurniture("Desk", room.Name, date);
            return room;
        }
    }
}
