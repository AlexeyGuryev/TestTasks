using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StorageUI.Models
{
    public class RemoveRoomViewModel
    {
        [Required(ErrorMessage = "Room name cannot be empty")]
        public string RoomName { get; set; }

        [Required(ErrorMessage = "Transfer room cannot be empty")]
        public string Transfer { get; set; }

        [Required(ErrorMessage = "Remove date cannot be empty")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
    }
}