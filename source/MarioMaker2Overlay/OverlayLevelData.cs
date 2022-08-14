using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioMaker2Overlay
{
    class OverlayLevelData
    {
        public int LevelDataId { get; set; }
        public string Code { get; set; }
        public int PlayerDeaths { get; set; }
        public int Clears { get; set; }
        public int Attempts { get; set; }
		public string Name { get; set; }
		public string DifficultyName { get; set; }
		public List<string> TagsName { get; set; }
		public string WorldRecord { get; set; }
		public string ClearRate { get; set; }
		public int Likes { get; set; }
		public int Boos { get; set; }
        public string ClearCheckTime { get; set; }

		public long TimeElapsed { get; set; }

        public long ClearTime { get; set; }

        public bool FirstClear { get; set; }

        public int TotalGlobalClears { get; set; }

        public int TotalGlobalAttempts { get; set; }

        public string? Theme { get; set; }

        public string? GameStyle { get; set; }

        public DateTime? DateTimeUploaded { get; set; }

        public int? ClearConditionMagnitude { get; set; }

        public string? ClearCondition { get; set; }

        public DateTime DateTimeStarted { get; set; }

        public DateTime? DateTimeCleared { get; set; }
		public decimal LikeRatio
		{
			get
            {
                decimal result = Likes;

                if (Boos > 0)
                {
                    result = decimal.Round((decimal)Likes / (decimal)Boos, 2);
                }

                return result;
            }
        }
    }
}
