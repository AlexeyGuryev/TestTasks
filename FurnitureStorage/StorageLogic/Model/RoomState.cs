using System;

namespace StorageLogic.Model
{
    /// <summary>
    /// Состояние комнаты на определенную дату
    /// </summary>
    public class RoomState
    {
        public Room Room { get; set; }

        public DateTime StateDate { get; set; }
    }
}
