﻿using System;
using System.Linq;
using System.Web.Mvc;
using StorageLogic;
using StorageLogic.Service;
using StorageUI.Filters;
using System.Collections.Generic;
using StorageUI.Models;

namespace StorageUI.Controllers
{
    public class RoomController : Controller
    {
        private readonly StorageService _service = null;

        public RoomController(IStorageRepository repository)
        {
            _service = new StorageService(repository);
        }

        public ActionResult Index()
        {
            return View();
        }
        
        public JsonResult RoomsByDate(DateTime? date)
        {
            var rooms = _service.QueryRooms(date);
            return Json(rooms, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RoomHistory(bool shortFormat) {
            var history = _service.GetHistory();

            var rows = new List<string>();
            foreach (var item in history)
            {
                rows.Add(shortFormat 
                    ? string.Format("{0:dd.MM.yyyy}", item.StateDate)
                    : string.Format("{0:dd.MM.yyyy}: {1}({2:dd.MM.yyyy} - {3:dd.MM.yyyy}), furnitures: {4}",
                        item.StateDate, item.Room.Name, item.Room.CreationDate, item.Room.RemoveDate,
                        item.Room.Furnitures.Values.Sum())
                );
            }
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogicExceptionFilter]
        public JsonResult CreateRoom(CreateRoomViewModel model) //string roomName, DateTime? date)
        {
            if (ModelState.IsValid) {
                var room = _service.CreateRoom(model.RoomName, model.Date ?? DateTime.Now);
                return Json(room);
            }
            else
            {
                // todo перебор ModelState по всем ошибкам полей
                // и возврат return не Ok;
                // moment не возвращает пустое :( и невалидное
            }
            return Ok;
        }

        [HttpPost]
        [LogicExceptionFilter]
        public JsonResult RemoveRoom(string roomName, string transfer, DateTime? date)
        {
            _service.RemoveRoom(roomName, transfer, date ?? DateTime.Now);
            return Ok;
        }

        [HttpPost]
        [LogicExceptionFilter]
        public JsonResult CreateFurniture(string type, string roomName, DateTime? date)
        {
            _service.CreateFurniture(type, roomName, date ?? DateTime.Now);
            return Ok;
        }

        [HttpPost]
        [LogicExceptionFilter]
        public JsonResult MoveFurniture(string type, string roomNameFrom, string roomNameTo, DateTime? date)
        {
            _service.MoveFurniture(type, roomNameFrom, roomNameTo, date ?? DateTime.Now);
            return Ok;
        }

        private JsonResult Ok { get { return Json(new { IsOk = true }); } }
    }
}