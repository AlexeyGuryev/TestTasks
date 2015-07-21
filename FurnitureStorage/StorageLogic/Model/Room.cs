using System;
using System.Collections.Generic;
using System.Linq;

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

        public List<string> FurnitureList { get; set; }

        /// <summary>
        /// HACK: изначально логика и тесты были написаны под dictionary
        /// для экономии времени использовал get-проперти вместо переделки логики
        /// </summary>
        public Dictionary<string, int> Furnitures
        {
            get
            {
                var list = FurnitureList ?? Enumerable.Empty<string>();
                return list
                    .GroupBy(c => c)
                    .ToDictionary(key => key.Key, value => value.Count());
            }
        }

        internal void AddFurniture(string furnitureType, int count = 1)
        {
            FurnitureList.AddRange(
                Enumerable.Repeat(furnitureType, count));
        }

        internal void RemoveFurniture(string furnitureType, int count = 1)
        {
            if (count < FurnitureList.Count())
            {
                var listItemsByTypeCopy = FurnitureList.Where(c => c == furnitureType).ToList();
                foreach (var listItem in listItemsByTypeCopy)
                {
                    if (listItem == furnitureType)
                    {
                        FurnitureList.Remove(furnitureType);
                    }
                }
            }
            else
            {
                FurnitureList.RemoveAll(c => c == furnitureType);
            }
        }
    }
}
