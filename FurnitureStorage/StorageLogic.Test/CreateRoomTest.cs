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
        [ExpectedException(typeof (ItemAlreadyExistsException))]
        public void CreateNonUniqueRoomThrowsException()
        {
            var roomName = "Living room";

            Service.EnsureRoom(roomName, DateTime.Now);
            Service.CreateRoom(roomName, DateTime.Now);
        }
    }
}
