using System;
using System.Web.Mvc;
using StorageLogic;
using StorageLogic.Service;
using StorageUI.Filters;

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
        
        public JsonResult RoomsByDate(DateTime date)
        {
            var rooms = _service.QueryRooms(date);
            return Json(rooms, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [LogicExceptionFilter]
        public JsonResult CreateRoom(string roomName, DateTime? date)
        {
            var room = _service.CreateRoom(roomName, date ?? DateTime.Now);
            return Json(room);
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