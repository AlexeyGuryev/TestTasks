﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StorageUI.Models
{
    public class CreateRoomViewModel
    {
        [Required(ErrorMessage = "Room name cannot be empty")]
        public string RoomName { get; set; }

        [Required(ErrorMessage = "Creation date cannot be empty")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }
    }
}