using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarioMaker2Overlay.Persistence
{
    [Table(nameof(Player))]
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlayerId { get; set; }

        [StringLength(11)]
        public string? PlayerName { get; set; }
    }
}
