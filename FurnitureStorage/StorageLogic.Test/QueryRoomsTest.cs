using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageLogic.Service;
using StoragePersistence;

namespace StorageLogic.Test
{
    /// <summary>
    /// # query: вывод списка комнат с мебелью внутри каждой
    /// </summary>
    [TestClass]
    public class QueryRoomsTest
    {
        private static DateTime _now;
        private static DateTime _tomorrow;
        private static DateTime _yesterday;
        private static string _testRoom = "QueryRoomsTest_Room";
        private static string _anotherTestRoom = "QueryRoomsTest_AnotherRoom";

        private static StorageService Service { get; set; }

        [ClassInitialize]
        public static void QueryRoomsInit(TestContext testContext)
        {
            _now = DateTime.Now.Date;
            _tomorrow = _now.AddDays(1);
            _yesterday = _now.AddDays(-1);

            Service = new StorageService(new StorageMemoryRepositoryStub());

            var room = Service.EnsureRoom(_testRoom, _yesterday);
            Service.CreateFurniture("Desk", room.Name, _now);
            Service.CreateFurniture("Chair", room.Name, _tomorrow);

            var anotherRoom = Service.EnsureRoom(_anotherTestRoom, _yesterday);
            Service.CreateFurniture("Desk", anotherRoom.Name, _now);
            Service.CreateFurniture("Chair", anotherRoom.Name, _tomorrow);
        }

        [TestMethod]
        public void QueryRoomsContainsNoRoomBeforeCreateRoom()
        {
            var dayBeforeYesterday = _yesterday.AddDays(-1);
            Assert.IsTrue(Service.QueryRooms(dayBeforeYesterday).Count == 0);
        }

        [TestMethod]
        public void QueryRoomsContainsRoomWithoutFurnitureAfterCreateRoom()
        {
            var rooms = Service.QueryRooms(_yesterday);
            var room = rooms.First(c => c.Name == _testRoom);
            Assert.IsTrue(room.Furnitures.Count == 0);
        }

        [TestMethod]
        public void QueryRoomsContainsRoomWithFurnitureAfterCreateFurniture()
        {
            var rooms = Service.QueryRooms(_tomorrow);
            var room = rooms.First(c => c.Name == _testRoom);
            Assert.IsTrue(room.Furnitures.Count == 2);
        }
    }
}
