using System;
using System.ComponentModel.DataAnnotations;

namespace StoragePersistence.Entities
{
    public class FurnitureEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }

        public int Count { get; set; }
    }
}
