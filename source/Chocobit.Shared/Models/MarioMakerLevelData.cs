﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MarioMaker2Overlay.Models
{
	public class MarioMakerLevelData
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }

		[JsonPropertyName("difficulty_name")]
		public string DifficultyName { get; set; }

		[JsonPropertyName("tags_name")]
		public List<string> TagsName { get; set; }

		[JsonPropertyName("world_record_pretty")]
		public string WorldRecord { get; set; }

		[JsonPropertyName("upload_time_pretty")]
        public string ClearCheckTime { get; set; }

        [JsonPropertyName("clears")]
		public int Clears { get; set; }
		
		[JsonPropertyName("attempts")]
		public int Attempts { get; set; }

		[JsonPropertyName("clear_rate")]
		public string ClearRate { get; set; }

		[JsonPropertyName("likes")]
		public int Likes { get; set; }

		[JsonPropertyName("boos")]
		public int Boos { get; set; }

        [JsonPropertyName("theme_name")]
		public string ThemeName { get; set; }
		
		[JsonPropertyName("game_style_name")]
		public string GameStyleName { get; set; }
    }
}
