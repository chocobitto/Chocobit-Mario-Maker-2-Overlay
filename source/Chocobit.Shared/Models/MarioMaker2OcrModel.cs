using System.Text.Json.Serialization;

namespace MarioMaker2Overlay.Models
{
    public class MarioMaker2OcrModel
    {
		[JsonPropertyName("level")]
        public LevelDetails Level { get; set; }

		[JsonPropertyName("data")]
        public string Data { get; set; }

		[JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class LevelDetails
    {
		[JsonPropertyName("author")]
        public string Author { get; set; }

		[JsonPropertyName("code")]
        public string Code { get; set; }

		[JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
