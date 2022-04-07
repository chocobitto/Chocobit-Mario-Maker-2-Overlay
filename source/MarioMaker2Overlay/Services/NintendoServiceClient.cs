using MarioMaker2Overlay.Models;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MarioMaker2Overlay
{
	public class NintendoServiceClient
	{
		private readonly HttpClient _httpClient;
		private CancellationTokenSource _cancellationToken = new CancellationTokenSource();

		public NintendoServiceClient(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<MarioMakerLevelData> GetLevelInfo(string levelCode)
		{
			_cancellationToken = new CancellationTokenSource();

			HttpResponseMessage response = await _httpClient.GetAsync($"https://tgrcode.com/mm2/level_info/{levelCode}", _cancellationToken.Token);

			response.EnsureSuccessStatusCode();

			MarioMakerLevelData result = 
				await System.Text.Json.JsonSerializer.DeserializeAsync<MarioMakerLevelData>(await response.Content.ReadAsStreamAsync())
				?? new();

			return result;
		}

        internal void CancelOutstandingRequest()
        {
			_cancellationToken.Cancel();
		}
    }
}
