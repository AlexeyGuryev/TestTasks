﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StorageLogic.Exception;
using StorageLogic.Model;

namespace StorageLogic.Test
{
    /// <summary>
    /// # create-furniture: room существует на date и все его состояния раньше date
    /// # create-furniture: если мебели нет - создается 1 шт, если есть: +1 шт
    /// # create-furniture: новое состояние room - обновился список мебели
    /// </summary>
    [TestClass]
    public class CreateFurnitureTest : StorageBaseTest
    {
        [TestMethod]
        [ExpectedException(typeof (ItemNotFoundException))]
        public void FurnitureCreationChecksThatRoomExistsOnDate()
        {
            var room = Service.EnsureRoom("FurnitureCreationChecksThatRoomExistsOnDate_room", DateTime.Now);

            var yesterdayDate = DateTime.Now.AddDays(-1);

            Service.CreateFurniture("desk", room.Name, yesterdayDate);
        }

        [TestMethod]
        [ExpectedException(typeof (DateConsistenceException))]
        public void FurnitureCreationDateIsLaterThanLastRoomStateDate()
        {
            var room = Service.EnsureRoom("FurnitureCreationDateIsLaterThanLastRoomStateDate_room", DateTime.Now);

            var tomorrowDate = DateTime.Now.AddDays(1);
            Service.CreateFurniture("desk", room.Name, tomorrowDate);

            Service.CreateFurniture("chair", room.Name, DateTime.Now);
        }

        [TestMethod]
        public void FurnitureCreatesIfNotExists()
        {
            var furnitureType = "The unique desk";
            var room = Service.EnsureRoom("FurnitureCreatesIfNotExists_room", DateTime.Now);

            Service.CreateFurniture(furnitureType, room.Name, DateTime.Now);

            Assert.IsTrue(room.Furnitures.ContainsKey(furnitureType));
        }

        [TestMethod]
        public void FurnitureCreationCauseIncrementCountThenItExistsInRoom()
        {
            var furnitureType = "Desk";
            var room = Service.EnsureRoom("FurnitureCreationCauseIncrementCountThenItExistsInRoom_room", DateTime.Now);

            if (!room.Furnitures.ContainsKey(furnitureType))
            {
                Service.CreateFurniture(furnitureType, room.Name, DateTime.Now);
            }
            var furnitureCountPrev = room.Furnitures[furnitureType];

            Service.CreateFurniture(furnitureType, room.Name, DateTime.Now);

            var furnitureCountNow = room.Furnitures[furnitureType];
            Assert.AreEqual(furnitureCountPrev + 1, furnitureCountNow);
        }

        [TestMethod]
        public void FurnitureCreationUpdatesRoomState()
        {
            var room = Service.EnsureRoom("FurnitureCreationUpdatesRoomState_room", DateTime.Now);

            var furnitureCreationDate = DateTime.Now;
            Service.CreateFurniture("desk", room.Name, furnitureCreationDate);
            Service.CreateFurniture("desk", room.Name, furnitureCreationDate);
            Service.CreateFurniture("desk", room.Name, furnitureCreationDate);

            var roomStateHistory = Service.GetRoomHistory(room.Name) ?? Enumerable.Empty<RoomState>();

            Assert.IsTrue(roomStateHistory.Any(c => c.StateDate == furnitureCreationDate));
        }
    }
}