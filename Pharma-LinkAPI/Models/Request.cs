﻿using Pharma_LinkAPI.Services.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Pharma_LinkAPI.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string? Pharmacy_License { get; set; }
        [Required]
        public string? DR_Name { get; set; }
        [Required]
        public string? Pharmacy_Name { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        [EmailAddress]
        [IsEmailInUse]
        public string? Email { get; set; }

        [Required]
        public string? Street { get; set; }
        [Required]
        public string? State { get; set; }
        [Required]
        public string? City { get; set; }

        [Required]
        public string? License_File { get; set; }
        //[Required]
        //public string? ImageUrl { get; set; }
        [Required]
        [StringLength(50)]
        [IsUserNameInUse]
        public string? UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool IsEmailConfirmed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
