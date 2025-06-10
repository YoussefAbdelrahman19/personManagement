using System;
using System.ComponentModel.DataAnnotations;

namespace PersonManagement.Domain.Entities
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [Range(1, 100)]
        public int Age { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}