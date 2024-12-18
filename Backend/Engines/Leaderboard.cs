using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Models
{
    [Table("Leaderboards")]
    public class Leaderboard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeaderboardId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int GameId { get; set; }

        [Required]
        public int Wins { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
