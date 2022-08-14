using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarioMaker2Overlay.Persistence;

namespace Chocobit.Shared.Logic
{
    public class AwesomenessIndexLogic
    {
        private LevelDataRepository _levelDataRepository = new();
     
        public async Task<double> GetAwesomenessIndex(int playerId)
        {
            Task<List<(string, double, int)>> averageClearRatesTask = _levelDataRepository.GetAverageClearRates(playerId);
            Task<double> clearRateAverageSuperiorityIndexTask = _levelDataRepository.GetClearRateSuperiorityIndex(playerId);

            await Task.WhenAll(averageClearRatesTask, clearRateAverageSuperiorityIndexTask);

            double awesomenessIndex = 0;

            foreach((string Difficulty, double Rate, int LevelsPlayed) rate in await averageClearRatesTask)
            {
                if (rate.Difficulty.Equals("super expert", StringComparison.OrdinalIgnoreCase))
                {
                    awesomenessIndex += (rate.Rate / 5) * (rate.LevelsPlayed / 100);
                }
                else if (rate.Difficulty.Equals("expert", StringComparison.OrdinalIgnoreCase))
                {
                    awesomenessIndex += (rate.Rate / 10) * (rate.LevelsPlayed / 100);
                }
                else if (rate.Difficulty.Equals("normal", StringComparison.OrdinalIgnoreCase))
                {
                    awesomenessIndex += (rate.Rate / 25) * (rate.LevelsPlayed / 100);
                }
                else if (rate.Difficulty.Equals("easy", StringComparison.OrdinalIgnoreCase))
                {
                    awesomenessIndex += (rate.Rate / 50) * (rate.LevelsPlayed / 100);
                }
            }

            double clearRateAverageSuperiorityIndex = await clearRateAverageSuperiorityIndexTask;

            awesomenessIndex += clearRateAverageSuperiorityIndex;

            return awesomenessIndex;
        }
    }
}
