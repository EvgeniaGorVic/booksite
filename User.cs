using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

