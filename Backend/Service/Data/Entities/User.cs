using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Services;

namespace Backend.Data.Entities;

[Table("Users")] 
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int UserId { get; set; }

    [Required]
    [StringLength(50)] 
    public required string Username { get; set; }

    [Required]
    [StringLength(255)]
    public required string PasswordHash { get; set; }

    public List<Card>? WarCards { get; set; }

    public void SetWarDeck(Card card, int i)
    {
        if (WarCards == null || i < 0 || i >= WarCards.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(i), "Index out of range.");
        }

        WarCards[i] = card;
    }

    public Card GetWarCard(int i)
    {
        if (WarCards == null || i < 0 || i >= WarCards.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(i), "Index out of range.");
        }

        return WarCards[i];
    }
}
