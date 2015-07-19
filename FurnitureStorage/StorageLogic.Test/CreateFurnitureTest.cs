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
        public void FurnitureCreationChecksThatRoomExistsOnDate()
        {
            var room = GetTestRoomNow();

            var yesterdayDate = DateTime.Now.AddDays(-1);

            _service.CreateFurniture("desk", room.Name, yesterdayDate);
        }

        [TestMethod]
        [ExpectedException(typeof (DateConsistenceException))]
        public void FurnitureCreationDateIsLaterThanLastRoomStateDate()
        {
            var room = GetTestRoomNow();

            var tomorrowDate = DateTime.Now.AddDays(1);
            _service.CreateFurniture("desk", room.Name, tomorrowDate);

            _service.CreateFurniture("chair", room.Name, DateTime.Now);
        }

        [TestMethod]
        public void FurnitureCreatesIfNotExists()
        {
            var furnitureType = "The unique desk";
            var room = GetTestRoomNow();

            _service.CreateFurniture(furnitureType, room.Name, DateTime.Now);

            Assert.IsTrue(room.Furnitures.ContainsKey(furnitureType));
        }

        [TestMethod]
        public void FurnitureCreationCauseIncrementCountThenItExistsInRoom()
        {
            var furnitureType = "Desk";
            var room = GetTestRoomNow();

            if (!room.Furnitures.ContainsKey(furnitureType))
            {
                _service.CreateFurniture(furnitureType, room.Name, DateTime.Now);
            }
            var furnitureCountPrev = room.Furnitures[furnitureType];

            _service.CreateFurniture(furnitureType, room.Name, DateTime.Now);

            var furnitureCountNow = room.Furnitures[furnitureType];
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
