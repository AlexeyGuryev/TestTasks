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

        public void AddFurniture(string furnitureType)
        {
            if (Furnitures.ContainsKey(furnitureType))
            {
                Furnitures[furnitureType]++;
            }
            else
            {
                Furnitures.Add(furnitureType, 1);
            }
        }

        public void RemoveFurniture(string furnitureType)
        {
            if (Furnitures.ContainsKey(furnitureType))
            {
                Furnitures[furnitureType]--;
                if (Furnitures[furnitureType] == 0)
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
