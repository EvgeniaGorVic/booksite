using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace booksite.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(20)]
        public string Phone { get; set; } = string.Empty; // ДОБАВЛЕНО

        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;

        public bool IsAdmin { get; set; } = false;
        public bool IsActive { get; set; } = true;

        public string AvatarUrl { get; set; } = "/images/default-avatar.png";

        [StringLength(500)]
        public string Bio { get; set; } = string.Empty;
    }
}