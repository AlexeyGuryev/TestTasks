using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageLogic.Model
{
    public class Room
    {
        public string Name { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? RemoveDate { get; set; }

        public List<FurnitureAvailability> FurnitureInRoomFacts { get; set; }
    }
}
