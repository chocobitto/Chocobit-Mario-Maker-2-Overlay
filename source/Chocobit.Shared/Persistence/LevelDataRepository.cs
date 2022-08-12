using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MarioMaker2Overlay.Persistence
{
    public class LevelDataRepository
    {
        public void Insert(LevelData levelData)
        {
            using (MarioMaker2OverlayContext context = new())
            {
                context.LevelData.Add(levelData);

                context.SaveChanges();
            }
        }

        public async Task<LevelData?> GetByLevelCode(string levelCode)
        {
            LevelData? result = null;

            using (MarioMaker2OverlayContext context = new())
            {
                result = await context.LevelData
                    .Where(a => a.Code == levelCode)
                    .FirstOrDefaultAsync();
            }

            return result;
        }

        public void Upsert(LevelData levelData)
        {
            using (MarioMaker2OverlayContext context = new())
            {
                LevelData? current = context.LevelData
                    .Where(a => a.Code == levelData.Code)
                    .FirstOrDefault();

                if (current?.LevelDataId == null)
                {
                    context.LevelData.Add(levelData);
                }
                else if (current != null)
                {
                    current.PlayerDeaths = levelData.PlayerDeaths;
                    current.TimeElapsed = levelData.TimeElapsed;
                    current.TotalGlobalAttempts = levelData.TotalGlobalAttempts;
                    current.TotalGlobalClears = levelData.TotalGlobalClears;
                }

                context.SaveChanges();
            }
        }

        public void Update(LevelData levelData)
        {
            using (MarioMaker2OverlayContext context = new())
            {
                LevelData? current = context.LevelData
                    .Where(a => a.Code == levelData.Code)
                    .FirstOrDefault();

                if (current!= null)
                {
                    //copy data from passed levelData object
                    //onto "current"
                    current.PlayerDeaths = levelData.PlayerDeaths;
                }

                context.SaveChanges();
            }
        }

        public void MarkLevelCleared(string code, TimeSpan? clearTime)
        {
            using (MarioMaker2OverlayContext context = new())
            {
                LevelData? current = context.LevelData
                    .Where(a => a.Code == code)
                    .FirstOrDefault();

                if (current != null)
                {
                    current.DateTimeCleared = DateTime.Now;

                    if (clearTime != null)
                    {
                        current.ClearTime = clearTime.Value.Ticks;
                    }
                }

                context.SaveChanges();
            }
        }

        public void MarkFirstClear(string code)
        {
            using (MarioMaker2OverlayContext context = new())
            {
                LevelData? current = context.LevelData
                    .Where(a => a.Code == code)
                    .FirstOrDefault();

                if (current != null)
                {
                    current.FirstClear = true;
                }

                context.SaveChanges();
            }
        }

        public void MarkWorldRecord(string code)
        {
            using (MarioMaker2OverlayContext context = new())
            {
                LevelData? current = context.LevelData
                    .Where(a => a.Code == code)
                    .FirstOrDefault();

                if (current != null)
                {
                    current.WorldRecord = true;
                }

                context.SaveChanges();
            }
        }
    }
}
