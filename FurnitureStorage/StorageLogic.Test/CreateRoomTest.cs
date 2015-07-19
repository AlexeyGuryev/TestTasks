using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageLogic.Exception;
using StorageLogic.Model;

namespace StorageLogic.Test
{
    /// <summary>
    /// # create-room: комната room уникальна независимо от date
    /// # create-room: новое состояние - новая комната без мебели
    /// </summary>
    [TestClass]
    public class CreateRoomTest : StorageBaseTest
    {
        [TestMethod]
        public void CreateRoomIncreasesAllRoomsCount()
        {
            var previousRoomCount =
                (Service.Rooms ?? Enumerable.Empty<Room>()).Count();

            Service.EnsureRoom("Yet another room", DateTime.Now);

            var newRoomCount =
                (Service.Rooms ?? Enumerable.Empty<Room>()).Count();

            Assert.AreEqual(previousRoomCount + 1, newRoomCount);
        }

        [TestMethod]
        [ExpectedException(typeof (ItemAlreadyExistsException))]
        public void CreateNonUniqueRoomThrowsException()
        {
            var roomName = "Living room";

            Service.EnsureRoom(roomName, DateTime.Now);
            Service.CreateRoom(roomName, DateTime.Now);
        }

        [TestMethod]
        public void CreateRoomCreatesNewRoomState()
        {
            var roomName = "CreateRoomCreatesNewRoomState";
            var date = DateTime.Now;

            Service.EnsureRoom(roomName, date);

            var roomStateHistory = Service.GetRoomHistory(roomName) ?? Enumerable.Empty<RoomState>();

            Assert.IsTrue(roomStateHistory.Any(c => c.StateDate == date));
        }
    }
}
