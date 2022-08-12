using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MarioMaker2Overlay.Persistence
{
    [Table(nameof(LevelData))]
    [Index(nameof(Code), IsUnique = true)]
    public class LevelData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LevelDataId { get; set; }

        public int PlayerId { get; set; }

        [StringLength(11)]
        public string? Code { get; set; }

        public int PlayerDeaths { get; set; }

        public long ClearTime { get; set; }

        public bool WorldRecord { get; set; }

        public bool FirstClear { get; set; }

        public int TotalGlobalClears { get; set; }

        public int TotalGlobalAttempts { get; set; }

        public long TimeElapsed { get; set; }

        public string? Theme { get; set; }

        public string? GameStyle { get; set; }

        public string? Difficulty { get; set; }

        public string? Tags { get; set; }

        public DateTime? DateTimeUploaded { get; set; }

        public int? ClearConditionMagnitude { get; set; }

        public string? ClearCondition { get; set; }

        public DateTime DateTimeStarted { get; set; }

        public DateTime DateTimeCleared { get; set; }
    }
}
