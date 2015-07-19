using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageLogic.Exception;
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
    public class MoveFurnitureTest : StorageBaseTest
    {
        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void FurnitureMoveChecksSourceRoomExistsOnFurnitureCreationDate()
        {
            var existRoom = GetTestRoomNow();
            Service.MoveFurniture("desk", existRoom.Name, "The room, which i really did not create",
                DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void FurnitureMoveChecksDestinationRoomExistingOnFurnitureCreationDate()
        {
            var existRoom = GetTestRoomNow();
            Service.MoveFurniture("desk", "The room, which i really did not create", existRoom.Name,
                DateTime.Now);
        }

        [TestMethod]
        [ExpectedException(typeof(DateConsistenceException))]
        public void FurnitureMoveDateIsLaterThanLastDestionationRoomStateDate()
        {
            var yesterdayDate = DateTime.Now.AddDays(-1);

            var roomFrom = Service.EnsureRoom("Bath", yesterdayDate);
            var roomTo = Service.EnsureRoom("Living room", yesterdayDate);

            Service.CreateFurniture("Desk", roomTo.Name, DateTime.Now);

            Service.MoveFurniture("desk", roomFrom.Name, roomTo.Name, yesterdayDate);
        }

        [TestMethod]
        [ExpectedException(typeof(DateConsistenceException))]
        public void FurnitureMoveDateIsLaterThanLastSourceRoomStateDate()
        {
            var yesterdayDate = DateTime.Now.AddDays(-1);

            var roomFrom = Service.EnsureRoom("Bath", yesterdayDate);
            var roomTo = Service.EnsureRoom("Living room", yesterdayDate);

            Service.CreateFurniture("Desk", roomFrom.Name, DateTime.Now);

            Service.MoveFurniture("desk", roomFrom.Name, roomTo.Name, yesterdayDate);
        }

        [TestMethod]
        public void FurnitureMoveCauseUpdateRoomsStates()
        {
            var furnitureType = "desk";
            var roomFrom = Service.EnsureRoom("Bath", DateTime.Now);
            var roomTo = Service.EnsureRoom("Living room", DateTime.Now);
            
            var tomorrowDate = DateTime.Now.AddDays(1);

            Service.CreateFurniture(furnitureType, roomFrom.Name, DateTime.Now);

            Service.MoveFurniture(furnitureType, roomFrom.Name, roomTo.Name, tomorrowDate);

            var roomToStateHistory = Service.GetRoomHistory(roomTo.Name) ?? Enumerable.Empty<RoomState>();
            var roomFromStateHistory = Service.GetRoomHistory(roomFrom.Name) ?? Enumerable.Empty<RoomState>();

            Assert.IsTrue(roomToStateHistory.Any(c => c.StateDate == tomorrowDate) &&
                roomFromStateHistory.Any(c => c.StateDate == tomorrowDate));
        }

        [TestMethod]
        public void FurnitureMoveCauseUpdateRoomsFurnitureLists()
        {
            var furnitureType = "desk";
            var roomFrom = Service.EnsureRoom("Bath", DateTime.Now);
            var roomTo = Service.EnsureRoom("Living room", DateTime.Now);

            Service.CreateFurniture(furnitureType, roomFrom.Name, DateTime.Now);

            var roomFromFurnitureCountPrev = roomFrom.Furnitures[furnitureType];

            var roomToFurnitureCountPrev = roomTo.Furnitures.ContainsKey(furnitureType)
                ? roomFrom.Furnitures[furnitureType]
                : 0;

            Service.MoveFurniture(furnitureType, roomFrom.Name, roomTo.Name, DateTime.Now);

            var roomToFurnitureCountNow = roomTo.Furnitures.ContainsKey(furnitureType)
                ? roomTo.Furnitures[furnitureType]
                : 0;

            Assert.IsTrue(
                roomToFurnitureCountNow == roomToFurnitureCountPrev + roomFromFurnitureCountPrev
                &&
                !roomFrom.Furnitures.ContainsKey(furnitureType)
                );
        }

        private Room GetTestRoomNow()
        {
            return Service.EnsureRoom("Living room", DateTime.Now);
        }
    }
}
