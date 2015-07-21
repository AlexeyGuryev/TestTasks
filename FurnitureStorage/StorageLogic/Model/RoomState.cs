using System;
using System.Linq;

namespace StorageLogic.Model
{
    /// <summary>
    /// Состояние комнаты на определенную дату
    /// </summary>
    public class RoomState
    {
        public Room Room { get; set; }

        public DateTime StateDate { get; set; }

        /// <summary>
        /// Конструктор с клонированием объекта комнаты
        /// </summary>
        /// <param name="room"></param>
        /// <param name="state"></param>
        public RoomState(Room room, DateTime state)
        {
            Room = new Room
            {
                Name = room.Name,
                CreationDate = room.CreationDate,
                RemoveDate = room.RemoveDate,
                FurnitureList = room.FurnitureList.ToList()
            };
            StateDate = state;
        }
    }
}
