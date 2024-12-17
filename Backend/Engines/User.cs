using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackendAPI.Models;
using System.Dynamic;
using Engines;

namespace BackendAPI.Models
{
    [Table("Users")] 
    public class User : IUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int UserId { get; set; }

        [Required]
        [StringLength(50)] 
        public required string Username { get; set; }

        [Required]
        [StringLength(255)] // Matches CHARACTER_MAXIMUM_LENGTH of PasswordHash column
        public required string PasswordHash { get; set; }
      
        public List<Card>? WarCards { get; set; }
        
        public void SetWarDeck(Card card, int i)
        {
            if (i < 0 || i >= WarCards.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(i), "Index out of range.");
            }
            WarCards[i] = card;
        }

        public Card GetWarCard(int i)
        {
            if (i < 0 || i >= WarCards.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(i), "Index out of range.");
            }
            return WarCards[i];
        }
    }
}