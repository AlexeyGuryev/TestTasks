using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using StorageLogic.Model;

namespace StoragePersistence.Entities
{
    public class RoomEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        public DateTime? RemoveDate { get; set; }

        public virtual ICollection<RoomStateEntity> RoomStates { get; set; }

        public RoomEntity()
        {
            RoomStates = new HashSet<RoomStateEntity>();
        }

        internal Room ToModel(DateTime? stateDate = null)
        {
            var latestState = GetStateOnDate(stateDate);
            var furnitureDict = latestState != null
                ? latestState.GetFurnitureDict()
                : new Dictionary<string, int>();

            return new Room
            {
                Name = this.Name,
                CreationDate = this.CreationDate,
                RemoveDate = this.RemoveDate,
                Furnitures = furnitureDict
            };
        }

        internal void UpdateFromModel(Room room, DateTime stateDate)
        {
            // фиксируем изменения только даты удаления комнаты
            RemoveDate = room.RemoveDate;

            RoomStateEntity newState = new RoomStateEntity(this, stateDate, room.Furnitures);
            RoomStates.Add(newState);
        }

        internal RoomStateEntity GetStateOnDate(DateTime? queryDate = null)
        {
            return RoomStates
                .OrderByDescending(c => c.StateDate)
                .FirstOrDefault(c => queryDate == null || c.StateDate <= queryDate);
        }
    }
}
