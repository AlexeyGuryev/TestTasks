using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using StorageLogic.Model;

namespace StoragePersistence.Entities
{
    public class RoomStateEntity
    {
        [Key]
        public int Id { get; set; }

        public int? RoomId { get; set; }

        [Required]
        public virtual RoomEntity Room { get; set; }

        [Required]
        public DateTime StateDate { get; set; }

        public virtual ICollection<FurnitureEntity> FurnitureList { get; set; }

        public RoomStateEntity()
        {
            FurnitureList = new HashSet<FurnitureEntity>();
        }

        public RoomStateEntity(RoomEntity room, DateTime stateDate, Dictionary<string, int> furnitures)
        {
            this.Room = room;
            this.StateDate = stateDate;
            this.FurnitureList = this.FurnitureList ?? new HashSet<FurnitureEntity>();

            foreach (var furnitureType in furnitures)
            {
                var newFurnitureItem = new FurnitureEntity
                {
                    Type = furnitureType.Key,
                    Count = furnitureType.Value
                };
                FurnitureList.Add(newFurnitureItem);
            }
        }


        public RoomState ToModel()
        {
            var roomModel = new Room
            {
                Name = Room.Name,
                CreationDate = Room.CreationDate,
                RemoveDate = Room.RemoveDate,
                Furnitures = GetFurnitureDict()
            };

            return new RoomState 
            {
                Room = roomModel,
                StateDate = StateDate
            };
        }

        internal Dictionary<string, int> GetFurnitureDict()
        {
            return FurnitureList != null
                ? FurnitureList
                    .GroupBy(t => t.Type)
                    .ToDictionary(c => c.Key, c => c.Sum(s => s.Count))

                : new Dictionary<string, int>();
        }
    }
}
