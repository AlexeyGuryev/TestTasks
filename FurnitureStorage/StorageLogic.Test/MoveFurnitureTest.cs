using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageLogic.Exception;
using StorageLogic.Service;
using StorageLogic.Test.Stub;
using StorageLogic.Model;

namespace StorageLogic.Test
{
    /// <summary>
    /// # move-furniture: roomFrom существует на date и все его состояния раньше date
    /// # move-furniture: roomTo существует на date и все его состояния раньше date
    /// # move-furniture: мебель типа type мигрирует
    /// # move-furniture: новое состояние roomFrom - без мебели типа type
    /// # move-furniture: новое состояние roomTo - добавлена мебель типа type
    /// </summary>
    [TestClass]
    public class MoveFurnitureTest
    {
        private readonly StorageService _service = new StorageService(new StorageRepositoryStub());

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void SourceRoomMustExistsOnFurnitureCreationDate()
        {
            var existRoom = GetTestRoomNow();
            _service.MoveFurniture("desk", existRoom.Name, "The room, which i really did not create",
                DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void DestinationRoomMustExistsOnFurnitureCreationDate()
        {
            var existRoom = GetTestRoomNow();
            _service.MoveFurniture("desk", "The room, which i really did not create", existRoom.Name,
                DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(DateConsistenceException))]
        public void FurnitureMoveDateIsLaterThanLastDestionationRoomStateDate()
        {
            var yesterdayDate = DateTime.Now.AddDays(-1);

            var roomFrom = _service.EnsureRoom("Bath", yesterdayDate);
            var roomTo = _service.EnsureRoom("Living room", yesterdayDate);

            _service.CreateFurniture("Desk", roomTo.Name, DateTime.Now);

            _service.MoveFurniture("desk", roomFrom.Name, roomTo.Name, yesterdayDate);
        }

        [TestMethod]
        [ExpectedException(typeof(DateConsistenceException))]
        public void FurnitureMoveDateIsLaterThanLastSourceRoomStateDate()
        {
            var yesterdayDate = DateTime.Now.AddDays(-1);

            var roomFrom = _service.EnsureRoom("Bath", yesterdayDate);
            var roomTo = _service.EnsureRoom("Living room", yesterdayDate);

            _service.CreateFurniture("Desk", roomFrom.Name, DateTime.Now);

            _service.MoveFurniture("desk", roomFrom.Name, roomTo.Name, yesterdayDate);
        }

        [TestMethod]
        public void FurnitureMoveCauseUpdateRoomsStates()
        {
            var furnitureType = "desk";
            var roomFrom = _service.EnsureRoom("Bath", DateTime.Now);
            var roomTo = _service.EnsureRoom("Living room", DateTime.Now);
            
            var tomorrowDate = DateTime.Now.AddDays(1);

            _service.CreateFurniture(furnitureType, roomFrom.Name, DateTime.Now);

            _service.MoveFurniture(furnitureType, roomFrom.Name, roomTo.Name, tomorrowDate);

            var roomToStateHistory = _service.GetRoomHistory(roomTo.Name) ?? Enumerable.Empty<RoomState>();
            var roomFromStateHistory = _service.GetRoomHistory(roomFrom.Name) ?? Enumerable.Empty<RoomState>();

            Assert.IsTrue(roomToStateHistory.Any(c => c.StateDate == tomorrowDate) &&
                roomFromStateHistory.Any(c => c.StateDate == tomorrowDate));
        }

        [TestMethod]
        public void FurnitureMoveCauseUpdateRoomsFurnitureLists()
        {
            var furnitureType = "desk";
            var roomFrom = _service.EnsureRoom("Bath", DateTime.Now);
            var roomTo = _service.EnsureRoom("Living room", DateTime.Now);

            _service.CreateFurniture(furnitureType, roomFrom.Name, DateTime.Now);

            var roomFromFurnitureCountPrev = roomFrom.Furnitures[furnitureType];

            var roomToFurnitureCountPrev = roomTo.Furnitures.ContainsKey(furnitureType)
                ? roomFrom.Furnitures[furnitureType]
                : 0;

            _service.MoveFurniture(furnitureType, roomFrom.Name, roomTo.Name, DateTime.Now);

            var roomToFurnitureCountNow = roomTo.Furnitures.ContainsKey(furnitureType)
                ? roomFrom.Furnitures[furnitureType]
                : 0;

            Assert.IsTrue(
                roomToFurnitureCountNow == roomToFurnitureCountPrev + roomFromFurnitureCountPrev
                &&
                !roomFrom.Furnitures.ContainsKey(furnitureType)
                );
        }

        private Room GetTestRoomNow()
        {
            return _service.EnsureRoom("Living room", DateTime.Now);
        }
    }
}
