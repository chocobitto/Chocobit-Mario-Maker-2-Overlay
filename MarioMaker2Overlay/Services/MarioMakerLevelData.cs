using System.Text.Json.Serialization;

namespace MarioMaker2Overlay.Services
{
	public class MarioMakerLevelData
	{
		[JsonPropertyName("clear_rate")]
		public string ClearRate { get; set; }
	}
}
