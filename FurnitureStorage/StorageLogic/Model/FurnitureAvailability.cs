using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StorageLogic.Model
{
    /// <summary>
    /// Реализуя требование к задаче:
    /// "Конкретный предмет мебели не может находиться в двух комнатах одновременно или ни в какой из комнат,
    /// т.е., когда мебель создается, она сразу должна быть помещена в комнату"
    /// НЕ СОЗДАЕМ спец.тип для мебели - т.к. он не может существовать сам по себе, отдельно от комнаты
    /// </summary>
    public class FurnitureAvailability
    {
        public string FurnitureType { get; set; }
        
        public Room Room { get; set; }

        public DateTime InRoomStartDate { get; set; }

        public DateTime? InRoomEndDate { get; set; }
    }
}
