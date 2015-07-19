using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageLogic.Exception;
using StorageLogic.Service;
using StorageLogic.Test.Stub;
using System.Linq;
using StorageLogic.Model;

namespace StorageLogic.Test
{
    /// <summary>
    /// # create-furniture: room существует на date и все его состояния раньше date
    /// # create-furniture: если мебели нет - создается 1 шт, если есть: +1 шт
    /// # create-furniture: новое состояние room - обновился список мебели
    /// </summary>
    [TestClass]
    public class CreateFurnitureTest
    {
        private readonly StorageService _service = new StorageService(new StorageRepositoryStub());

        [TestMethod]
        [ExpectedException(typeof (ItemNotFoundException))]
        public void RoomMustExistsOnFurnitureCreationDate()
        {
            _service.CreateFurniture("desk", "The room, which i really did not create",
                DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof (DateConsistenceException))]
        public void FurnitureCreationDateIsLaterThanLastRoomStateDate()
        {
            var room = GetTestRoomNow();

            var yesterdayDate = DateTime.Now.AddDays(-1);

            _service.CreateFurniture("desk", room.Name, yesterdayDate);
        }

        [TestMethod]
        public void FurnitureCreatesIfNotExists()
        {
            var furnitureName = "The unique desk";
            var room = GetTestRoomNow();

            var furnitureExistsPrev = room.Furnitures.ContainsKey(furnitureName);

            _service.CreateFurniture(furnitureName, room.Name, DateTime.Now);

            var furnitureExistsNow = room.Furnitures.ContainsKey(furnitureName);

            Assert.AreEqual(furnitureExistsPrev, furnitureExistsNow);
        }

        [TestMethod]
        public void FurnitureCountIncrementsThenItExistsInRoom()
        {
            var furnitureName = "Desk";
            var room = GetTestRoomNow();

            if (room.Furnitures.ContainsKey(furnitureName))
            {
                _service.CreateFurniture(furnitureName, room.Name, DateTime.Now);
            }
            var furnitureCountPrev = room.Furnitures[furnitureName];

            _service.CreateFurniture(furnitureName, room.Name, DateTime.Now);

            var furnitureCountNow = room.Furnitures[furnitureName];
            Assert.AreEqual(furnitureCountPrev + 1, furnitureCountNow);
        }

        [TestMethod]
        public void FurnitureCreationUpdatesRoomState()
        {
            var room = GetTestRoomNow();

            var furnitureCreationDate = DateTime.Now;
            _service.CreateFurniture("desk", room.Name, furnitureCreationDate);

            var roomStateHistory = _service.GetRoomHistory(room.Name) ?? Enumerable.Empty<RoomState>();

            Assert.IsTrue(roomStateHistory.Any(c => c.StateDate == furnitureCreationDate));
        }

        private Room GetTestRoomNow()
        {
            return _service.EnsureRoom("Living room", DateTime.Now);
        }
    }
}
