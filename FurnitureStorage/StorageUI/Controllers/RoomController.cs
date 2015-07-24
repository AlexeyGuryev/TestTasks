using System;
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
            if (ModelState.IsValid)
            {
                _service.CreateRoom(model.RoomName, model.Date ?? DateTime.Now);
                return Ok;
            }
            else
            {
                return Error;
            }
        }

        [HttpPost]
        [LogicExceptionFilter]
        public JsonResult RemoveRoom(RemoveRoomViewModel model)
        {
            if (ModelState.IsValid)
            {
                _service.RemoveRoom(model.RoomName, model.Transfer, model.Date ?? DateTime.Now);
                return Ok;
            }
            else
            {
                return Error;
            }
        }

        [HttpPost]
        [LogicExceptionFilter]
        public JsonResult CreateFurniture(CreateFurnitureViewModel model)
        {
            if (ModelState.IsValid)
            {
                _service.CreateFurniture(model.Type, model.RoomName, model.Date ?? DateTime.Now);
                return Ok;
            }
            else
            {
                return Error;
            }
        }

        [HttpPost]
        [LogicExceptionFilter]
        public JsonResult MoveFurniture(MoveFurnitureViewModel model)
        {
            if (ModelState.IsValid)
            {
                _service.MoveFurniture(model.Type, model.RoomNameFrom, model.RoomNameTo, model.Date ?? DateTime.Now);
                return Ok;
            }
            else
            {
                return Error;
            }
        }

        #region помощники ошибок

        private JsonResult Ok { get { return Json(new ResultViewModel { IsOk = true }); } }

        private JsonResult Error
        {
            get
            {
                var errorList = ModelState
                    .Select(v => v.Value)
                    .SelectMany(e => e.Errors)
                    .Select(m => m.ErrorMessage)
                    .ToList();

                return Json(new ResultViewModel
                {
                    IsOk = false,
                    Errors = errorList
                });
            }
        }

        #endregion
    }
}