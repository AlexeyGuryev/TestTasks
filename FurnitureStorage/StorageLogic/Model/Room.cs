using System;
using System.Collections.Generic;
using StorageLogic.Exception;

namespace StorageLogic.Model
{
    /// <summary>
    /// Модель комнаты со списком мебели
    /// </summary>
    public class Room
    {
        public string Name { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? RemoveDate { get; set; }

        public Dictionary<string, int> Furnitures { get; set; }

        public Room()
        {
            Furnitures = new Dictionary<string, int>();
        }

        public void AddFurniture(string furnitureType, int count = 1)
        {
            Furnitures = Furnitures ?? new Dictionary<string, int>();

            if (Furnitures.ContainsKey(furnitureType))
            {
                Furnitures[furnitureType] += count;
            }
            else
            {
                Furnitures.Add(furnitureType, count);
            }
        }

        public void RemoveFurniture(string furnitureType, int count = 1)
        {
            Furnitures = Furnitures ?? new Dictionary<string, int>();           

            if (Furnitures.ContainsKey(furnitureType))
            {
                Furnitures[furnitureType] -= count;
                if (Furnitures[furnitureType] <= 0)
                {
                    Furnitures.Remove(furnitureType);
                }
            }
            else
            {
                throw new ItemNotFoundException("Furniture with type {0} was not found in room {1}",
                    furnitureType, this.Name);
            }
        }
    }
}
