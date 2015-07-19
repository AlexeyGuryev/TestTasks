using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageLogic.Exception;
using StorageLogic.Model;
using StorageLogic.Service;
using StorageLogic.Test.Stub;

namespace StorageLogic.Test
{
    /// <summary>
    /// # create-room: комната room уникальна независимо от date
    /// # create-room: новое состояние - новая комната без мебели
    /// </summary>
    [TestClass]
    public class CreateRoomTest
    {
        private readonly StorageService _service = new StorageService(new StorageRepositoryStub());

        [TestMethod]
        public void CreateRoomIncreasesAllRoomsCount()
        {
            var previousRoomCount =
                (_service.QueryRooms() ?? Enumerable.Empty<Room>()).Count();

            _service.EnsureRoom("Yet another room", DateTime.Now);

            var newRoomCount =
                (_service.QueryRooms() ?? Enumerable.Empty<Room>()).Count();

            Assert.AreEqual(previousRoomCount, newRoomCount + 1);
        }

        [TestMethod]
        [ExpectedException(typeof (ItemAlreadyExistsException))]
        public void CreateNonUniqueRoomThrowsException()
        {
            var roomName = "Living room";

            _service.EnsureRoom(roomName, DateTime.Now);
            _service.CreateRoom(roomName, DateTime.Now);
        }

        [TestMethod]
        public void CreateRoomCreatesNewRoomState()
        {
            var roomName = "CreateRoomCreatesNewRoomState";
            var date = DateTime.Now;

            _service.EnsureRoom(roomName, date);

            var roomStateHistory = _service.GetRoomHistory(roomName) ?? Enumerable.Empty<RoomState>();

            Assert.IsTrue(roomStateHistory.Any(c => c.StateDate == date));
        }
    }
}
