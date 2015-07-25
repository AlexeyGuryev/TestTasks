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
        private Room RoomFrom { get; set; }
        private Room RoomTo { get; set; }

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void InitRooms()
        {
            RoomFrom = Service.EnsureRoom(TestContext.TestName + "_From", Now);
            RoomTo = Service.EnsureRoom(TestContext.TestName + "To", Now);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void FurnitureMoveChecksSourceRoomExistsOnFurnitureCreationDate()
        {
            Service.MoveFurniture("desk", RoomFrom.Name, "The room, which i really did not create",
                Tomorrow);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemNotFoundException))]
        public void FurnitureMoveChecksDestinationRoomExistingOnFurnitureCreationDate()
        {
            Service.MoveFurniture("desk", "The room, which i really did not create", RoomTo.Name,
                Tomorrow);
        }

        [TestMethod]
        [ExpectedException(typeof(DateConsistenceException))]
        public void FurnitureMoveChecksMoveDateIsLaterThanLastRoomStateDate()
        {
            Service.CreateFurniture("Desk", RoomFrom.Name, Tomorrow);

            Service.MoveFurniture("Desk", RoomFrom.Name, RoomTo.Name, Now);
        }

        [TestMethod]
        public void FurnitureMoveCauseUpdateRoomsStates()
        {
            var furnitureType = "desk";
            
            Service.CreateFurniture(furnitureType, RoomFrom.Name, Tomorrow);

            Service.MoveFurniture(furnitureType, RoomFrom.Name, RoomTo.Name, AfterTomorrow);

            var roomToStateHistory = Service.GetRoomHistory(RoomTo.Name) ?? Enumerable.Empty<RoomState>();
            var roomFromStateHistory = Service.GetRoomHistory(RoomFrom.Name) ?? Enumerable.Empty<RoomState>();

            Assert.IsTrue(roomToStateHistory.Any(c => c.StateDate == AfterTomorrow) &&
                roomFromStateHistory.Any(c => c.StateDate == AfterTomorrow));
        }

        [TestMethod]
        public void FurnitureMoveCauseUpdateRoomsFurnitureLists()
        {
            var furnitureType = "desk";

            Service.CreateFurniture(furnitureType, RoomFrom.Name, Tomorrow);

            var roomFromFurnitureCountPrev = RoomFrom.Furnitures[furnitureType];
            var roomToFurnitureCountPrev = 0;

            Service.MoveFurniture(furnitureType, RoomFrom.Name, RoomTo.Name, AfterTomorrow);

            var roomToFurnitureCountNow = RoomTo.Furnitures.ContainsKey(furnitureType)
                ? RoomTo.Furnitures[furnitureType]
                : 0;

            Assert.IsTrue(
                roomToFurnitureCountNow == roomToFurnitureCountPrev + roomFromFurnitureCountPrev
                &&
                !RoomFrom.Furnitures.ContainsKey(furnitureType)
                );
        }
    }
}
