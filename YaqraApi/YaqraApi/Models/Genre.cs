﻿using System.ComponentModel.DataAnnotations;

namespace YaqraApi.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }
    }
}