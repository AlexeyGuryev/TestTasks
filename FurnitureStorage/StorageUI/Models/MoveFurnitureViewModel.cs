using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StorageUI.Models
{
    public class MoveFurnitureViewModel
    {
        [Required(ErrorMessage = "Room name cannot be empty")]
        public string RoomNameFrom { get; set; }

        [Required(ErrorMessage = "Room name cannot be empty")]
        public string RoomNameTo { get; set; }

        [Required(ErrorMessage = "Furniture type cannot be empty")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Move date cannot be empty")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
    }
}