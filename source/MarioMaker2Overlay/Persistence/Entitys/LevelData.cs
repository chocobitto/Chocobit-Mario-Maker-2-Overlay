using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MarioMaker2Overlay.Persistence
{
    [Table(nameof(LevelData))]
    [Index(nameof(Code), IsUnique = true)]
    class LevelData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LevelDataId { get; set; }

        [StringLength(11)]
        public string? Code { get; set; }

        public int PlayerDeaths { get; set; }

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
    }
}
