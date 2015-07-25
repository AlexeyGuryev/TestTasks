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
        private Room RoomToRemove { get; set; }
        private Room TransferRoom { get; set; }

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void InitRooms()
        {
            RoomToRemove = Service.EnsureRoom(TestContext.TestName + "_From", Now);
            TransferRoom = Service.EnsureRoom(TestContext.TestName + "To", Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void RemoveRoomChecksThatTransferRoomExistsOnRemoveDate()
        {
            Service.RemoveRoom(RoomToRemove.Name, "The transfer room, which not exists", Tomorrow);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void RemovingRoomMustExistsOnRemoveDate()
        {
            Service.CreateFurniture("Desk", RoomToRemove.Name, Tomorrow);

            Service.RemoveRoom("The transfer room, which not exists", RoomToRemove.Name, AfterTomorrow);
        }

        [TestMethod]
        [ExpectedException(typeof (DateConsistenceException))]
        public void RemoveRoomChecksThatDateIsLaterThanLastRoomStateDate()
        {
            Service.CreateFurniture("Desk", RoomToRemove.Name, Tomorrow);

            Service.RemoveRoom(RoomToRemove.Name, TransferRoom.Name, Tomorrow);
        }

        [TestMethod]
        [ExpectedException(typeof (ItemNotFoundException))]
        public void RemoveRoomCheckCreationDateIsLaterThanRemoveDate()
        {
            Service.CreateFurniture("Desk", RoomToRemove.Name, Tomorrow);

            Service.RemoveRoom(RoomToRemove.Name, TransferRoom.Name, Yesterday);
        }

        [TestMethod]
        public void RemoveRoomCauseTransferAllFurniture()
        {
            var furnitureType1 = "Desk";
            var furnitureType2 = "Chair";

            Service.CreateFurniture(furnitureType1, RoomToRemove.Name, Tomorrow);
            Service.CreateFurniture(furnitureType2, RoomToRemove.Name, AfterTomorrow);

            var roomToRemoveFurniture1Count = RoomToRemove.Furnitures[furnitureType1];
            var roomToRemoveFurniture2Count = RoomToRemove.Furnitures[furnitureType2];
            Service.RemoveRoom(RoomToRemove.Name, TransferRoom.Name, AfterTomorrow.AddDays(1));

            Assert.IsTrue(
                TransferRoom.Furnitures.ContainsKey(furnitureType1)
                && TransferRoom.Furnitures.ContainsKey(furnitureType2)
                && TransferRoom.Furnitures[furnitureType1] == roomToRemoveFurniture1Count
                && TransferRoom.Furnitures[furnitureType2] == roomToRemoveFurniture2Count
            );
        }

        [TestMethod]
        public void RemoveWithTransferChangeRoomStates()
        {
            var furnitureType = "Desk";
            var removeDate = AfterTomorrow;

            Service.CreateFurniture(furnitureType, RoomToRemove.Name, Tomorrow);
            
            Service.RemoveRoom(RoomToRemove.Name, TransferRoom.Name, removeDate);

            var roomToRemoveState = Service.GetRoomHistory(RoomToRemove.Name);
            var transferRoomState = Service.GetRoomHistory(TransferRoom.Name);

            Assert.IsTrue(roomToRemoveState.Any(rr => rr.StateDate == removeDate 
                &&
                transferRoomState.Any(tr => tr.StateDate == removeDate)));
        }
    }
}
