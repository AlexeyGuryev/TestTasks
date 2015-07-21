using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageLogic.Exception;
using StorageLogic.Model;

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
    public class RemoveRoomTest : StorageBaseTest
    {
        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void RemoveRoomChecksThatTransferRoomExistsOnRemoveDate()
        {
            var roomToRemove = GetRoomWithFurniture(DateTime.Now);
            Service.RemoveRoom(roomToRemove.Name, "The transfer room, which not exists", DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void RemovingRoomMustExistsOnRemoveDate()
        {
            var roomToRemove = GetRoomWithFurniture(DateTime.Now);
            Service.RemoveRoom("The transfer room, which not exists", roomToRemove.Name, DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof (DateConsistenceException))]
        public void RemoveRoomChecksThatDateIsLaterThanLastRoomStateDate()
        {
            var yesterdayDate = DateTime.Now.AddDays(-1);

            var roomToRemove = Service.EnsureRoom("RemoveRoomChecksThatDateIsLaterThanLastRoomStateDate_Room", yesterdayDate);
            Service.CreateFurniture("Table", roomToRemove.Name, DateTime.Now);

            var transferRoom = GetRoomWithFurniture(yesterdayDate, "Desk", "RemoveRoomChecksThatDateIsLaterThanLastRoomStateDate_Bath");

            Service.RemoveRoom(roomToRemove.Name, transferRoom.Name, yesterdayDate);
        }

        [TestMethod]
        [ExpectedException(typeof (ItemNotFoundException))]
        public void RemoveRoomCheckCreationDateIsLaterThanRemoveDate()
        {
            var yesterdayDate = DateTime.Now.AddDays(-1);

            var roomToRemove = GetTestRoom(DateTime.Now);
            var transferRoom = GetRoomWithFurniture(DateTime.Now);

            Service.RemoveRoom(roomToRemove.Name, transferRoom.Name, yesterdayDate);
        }

        [TestMethod]
        public void RemoveRoomCauseTransferAllFurniture()
        {
            var furnitureType1 = "Desk";
            var furnitureType2 = "Chair";

            var roomToRemove = GetRoomWithFurniture(DateTime.Now, furnitureType1);
            Service.CreateFurniture(furnitureType2, roomToRemove.Name, DateTime.Now);

            var transferRoom = Service.CreateRoom("The unique room, i hope it's really unique", DateTime.Now);

            var roomToRemoveFurniture1Count = roomToRemove.Furnitures[furnitureType1];
            var roomToRemoveFurniture2Count = roomToRemove.Furnitures[furnitureType2];
            Service.RemoveRoom(roomToRemove.Name, transferRoom.Name, DateTime.Now);

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
            
            Service.RemoveRoom(roomToRemove.Name, transferRoom.Name, removeDate);

            var roomToRemoveState = Service.GetRoomHistory(roomToRemove.Name);
            var transferRoomState = Service.GetRoomHistory(transferRoom.Name);

            Assert.IsTrue(roomToRemoveState.Any(rr => rr.StateDate == removeDate 
                &&
                transferRoomState.Any(tr => tr.StateDate == removeDate)));
        }

        private Room GetTestRoom(DateTime date)
        {
            return Service.EnsureRoom("Living room", date);
        }

        private Room GetRoomWithFurniture(DateTime date, string furnitureType = "Desk", string roomName = "Bath")
        {
            var room = Service.EnsureRoom(roomName, date);
            Service.CreateFurniture(furnitureType, room.Name, date);
            return room;
        }
    }
}
