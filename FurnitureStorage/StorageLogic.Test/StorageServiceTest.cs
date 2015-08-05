using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StorageLogic.Exception;
using StorageLogic.Model;
using StorageLogic.Service;

namespace StorageLogic.Test
{
    [TestClass]
    public class StorageServiceTest
    {
        private IStorageRepository repository = null;
        private StorageService service = null;
        private DateTime now = DateTime.Now.Date;
        private Mock<IStorageRepository> mock = null;

        private Room bathRoomWithoutFurniture = null;
        private Room livingRoomWithFurniture = null;

        [TestInitialize]
        public void Init()
        {
            mock = new Mock<IStorageRepository>();

            bathRoomWithoutFurniture = new Room()
            {
                CreationDate = now,
                Name = "Bathroom",
                Furnitures = new Dictionary<string, int>()
            };

            mock.Setup(c => c.CreateRoom(bathRoomWithoutFurniture.Name, now)).Returns(bathRoomWithoutFurniture);
            mock.Setup(c => c.GetRoomByName(bathRoomWithoutFurniture.Name)).Returns(bathRoomWithoutFurniture);

            livingRoomWithFurniture = new Room
            {
                Name = "LivingRoom",
                CreationDate = now,
                Furnitures = (new List<Tuple<string, int>>
                {
                    new Tuple<string, int>("Table", 1),
                    new Tuple<string, int>("Chair", 2)
                }).ToDictionary(k => k.Item1, v => v.Item2)
            };

            mock.Setup(c => c.GetRoomByName("LivingRoom")).Returns(livingRoomWithFurniture);

            repository = mock.Object;
            service = new StorageService(repository);
        }

        [TestMethod]
        [ExpectedException(typeof(ItemAlreadyExistsException))]
        public void U_CreateRoomChecksExists()
        {
            service.CreateRoom(bathRoomWithoutFurniture.Name, now);
        }

        [TestMethod]
        public void U_CreateRoomCallsRepository()
        {
            mock.Setup(c => c.CreateRoom(It.IsAny<string>(), It.IsAny<DateTime>()));
            service.CreateRoom("Bedroom", now);
            mock.Verify();
        }

        [TestMethod]
        public void U_EnsureRoomCallsRepositoryGetRoomAndCreate()
        {
            mock.Setup(c => c.CreateRoom(It.IsAny<string>(), It.IsAny<DateTime>()));
            service.EnsureRoom("U_EnsureRoomCallsRepositoryGetRoomAndCreate", now);
            mock.Verify(c => c.GetRoomByName(It.IsAny<string>()), Times.Between(2, 2, Range.Inclusive));
        }

        [TestMethod]
        public void U_RemoveRoomCallsUpdateTransferRoom()
        {
            service.RemoveRoom("LivingRoom", "Bathroom", now);
            mock.Verify(c => c.UpdateRoom(It.IsAny<Room>(), It.IsAny<DateTime>()), Times.AtLeastOnce);
        }

        [TestMethod]
        public void U_RemoveRoomMoveAllFurnitures()
        {
            service.RemoveRoom(livingRoomWithFurniture.Name,
                bathRoomWithoutFurniture.Name, now);

            Assert.IsTrue(livingRoomWithFurniture.Furnitures.Count == 0 && bathRoomWithoutFurniture.Furnitures.Count > 0);
        }

        [TestMethod]
        public void U_CreateFurnitureCallsUpdateRoom()
        {
            service.CreateFurniture("Table", bathRoomWithoutFurniture.Name, now);
            mock.Verify(c => c.UpdateRoom(It.IsAny<Room>(), It.IsAny<DateTime>()), Times.AtLeastOnce);
        }

        [TestMethod]
        public void U_CreateFurnitureUpdatesFurnitureCount()
        {
            var prevTablesCount = livingRoomWithFurniture.Furnitures["Table"];
            service.CreateFurniture("Table", livingRoomWithFurniture.Name, now);

            var newTablesCount = livingRoomWithFurniture.Furnitures["Table"];

            Assert.AreEqual(prevTablesCount + 1, newTablesCount);
        }

        [TestMethod]
        public void U_MoveFurnitureUpdatesMoveFromRoom()
        {
            service.MoveFurniture("Table", livingRoomWithFurniture.Name, bathRoomWithoutFurniture.Name, now);
            mock.Verify(c => c.UpdateRoom(livingRoomWithFurniture, It.IsAny<DateTime>()), Times.AtLeastOnce);
        }
    }
}
