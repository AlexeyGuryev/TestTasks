using System;
using System.Web.Mvc;
using StorageLogic.Service;
using StoragePersistence;

namespace StorageUI.Controllers
{
    public class RoomController : Controller
    {
        private StorageService _service = new StorageService(new StorageMemoryRepository());

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult RoomsByDate(DateTime? date)
        {
            var rooms = _service.QueryRooms(date);
            return Json(rooms, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}