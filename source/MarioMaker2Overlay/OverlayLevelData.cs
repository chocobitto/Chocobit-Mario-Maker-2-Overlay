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
        public string ClearCheckTime { get; set; }
		public long TimeElapsed { get; set; }
	}
}
