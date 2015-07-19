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
        public void RemoveRoomChecksThatTransferRoomExistsOnRemoveDate()
        {
            var roomToRemove = GetRoomWithFurniture(DateTime.Now);
            _service.RemoveRoom(roomToRemove.Name, "The transfer room, which not exists", DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void RemovingRoomMustExistsOnRemoveDate()
        {
            var roomToRemove = GetRoomWithFurniture(DateTime.Now);
            _service.RemoveRoom("The transfer room, which not exists", roomToRemove.Name, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof (DateConsistenceException))]
        public void RemoveRoomChecksThatDateIsLaterThanLastRoomStateDate()
        {
            var yesterdayDate = DateTime.Now.AddDays(-1);

            var roomToRemove = GetTestRoom(yesterdayDate);
            _service.CreateFurniture("Table", roomToRemove.Name, DateTime.Now);

            var transferRoom = GetRoomWithFurniture(yesterdayDate);

            _service.RemoveRoom(roomToRemove.Name, transferRoom.Name, yesterdayDate);
        }

        [TestMethod]
        [ExpectedException(typeof (ItemNotFoundException))]
        public void RemoveRoomCheckCreationDateIsLaterThanRemoveDate()
        {
            var yesterdayDate = DateTime.Now.AddDays(-1);

            var roomToRemove = GetTestRoom(DateTime.Now);
            var transferRoom = GetRoomWithFurniture(DateTime.Now);

            _service.RemoveRoom(roomToRemove.Name, transferRoom.Name, yesterdayDate);
        }

        [TestMethod]
        public void RemoveRoomCauseTransferAllFurniture()
        {
            var furnitureType1 = "Desk";
            var furnitureType2 = "Chair";

            var roomToRemove = GetRoomWithFurniture(DateTime.Now, furnitureType1);
            _service.CreateFurniture(furnitureType2, roomToRemove.Name, DateTime.Now);

            var transferRoom = _service.CreateRoom("The unique room, i hope it's really unique", DateTime.Now);

            var roomToRemoveFurniture1Count = roomToRemove.Furnitures[furnitureType1];
            var roomToRemoveFurniture2Count = roomToRemove.Furnitures[furnitureType2];
            _service.RemoveRoom(roomToRemove.Name, transferRoom.Name, DateTime.Now);

            Assert.IsTrue(
                transferRoom.Furnitures.ContainsKey(furnitureType1)
                && transferRoom.Furnitures.ContainsKey(furnitureType2)
                && transferRoom.Furnitures[furnitureType1] == roomToRemoveFurniture1Count
                && transferRoom.Furnitures[furnitureType2] == roomToRemoveFurniture2Count
            );
        }

        [TestMethod]
        public void RemoveWithTransferChangeRoomStates()
        {
            var furnitureType = "Desk";
            var removeDate = DateTime.Now;

            var roomToRemove = GetRoomWithFurniture(removeDate, furnitureType);
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

        private Room GetRoomWithFurniture(DateTime date, string furnitureType = "Bath")
        {
            var room = _service.EnsureRoom(furnitureType, date);
            _service.CreateFurniture("Desk", room.Name, date);
            return room;
        }
    }
}
