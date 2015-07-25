using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageLogic.Exception;
using StorageLogic.Model;

namespace StorageLogic.Test
{
    /// <summary>
    /// # create-furniture: room существует на date и все его состояния раньше date
    /// # create-furniture: если мебели нет - создается 1 шт, если есть: +1 шт
    /// # create-furniture: новое состояние room - обновился список мебели
    /// </summary>
    [TestClass]
    public class CreateFurnitureTest : StorageBaseTest
    {
        private Room Room { get; set; }

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void InitRoom()
        {
            Room = Service.EnsureRoom(TestContext.TestName + "_Room", Now);
        }

        [TestMethod]
        [ExpectedException(typeof (ItemNotFoundException))]
        public void FurnitureCreationChecksThatRoomExistsOnDate()
        {
            Service.CreateFurniture("desk", Room.Name, Yesterday);
        }

        [TestMethod]
        [ExpectedException(typeof (DateConsistenceException))]
        public void FurnitureCreationDateIsLaterThanLastRoomStateDate()
        {
            Service.CreateFurniture("desk", Room.Name, Tomorrow);

            Service.CreateFurniture("chair", Room.Name, Now);
        }

        [TestMethod]
        public void FurnitureCreatesIfNotExists()
        {
            var furnitureType = "The unique desk";

            Service.CreateFurniture(furnitureType, Room.Name, Tomorrow);

            Assert.IsTrue(Room.Furnitures.ContainsKey(furnitureType));
        }

        [TestMethod]
        public void FurnitureCreationCauseIncrementCountThenItExistsInRoom()
        {
            var furnitureType = "Desk";

            Service.CreateFurniture(furnitureType, Room.Name, Tomorrow);

            var furnitureCountPrev = Room.Furnitures[furnitureType];

            Service.CreateFurniture(furnitureType, Room.Name, AfterTomorrow);

            var furnitureCountNew = Room.Furnitures[furnitureType];

            Assert.AreEqual(furnitureCountPrev + 1, furnitureCountNew);
        }

        [TestMethod]
        public void FurnitureCreationUpdatesRoomState()
        {
            var furnitureCreationDate = Tomorrow;
            Service.CreateFurniture("desk", Room.Name, furnitureCreationDate);

            var roomStateHistory = Service.GetRoomHistory(Room.Name) ?? Enumerable.Empty<RoomState>();

            Assert.IsTrue(roomStateHistory.Any(c => c.StateDate == furnitureCreationDate));
        }
    }
}
