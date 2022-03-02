using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioMaker2Overlay.Persistence
{
    class LevelDataRepository
    {
        public void Insert(LevelData levelData)
        {
            using (MarioMaker2OverlayContext context = new())
            {
                context.LevelData.Add(levelData);
            }
        }

        public LevelData GetByLevelCode(string levelCode)
        {
            LevelData? result = null;

            using (MarioMaker2OverlayContext context = new())
            {
                result = context.LevelData
                    .Where(a => a.Code == levelCode)
                    .FirstOrDefault();
            }

            return result;
        }

        public void Update(LevelData levelData)
        {
            using (MarioMaker2OverlayContext context = new())
            {
                LevelData? current = context.LevelData
                    .Where(a => a.Code == levelData.Code)
                    .FirstOrDefault();

                //copy data from passed levelData object
                //onto "current"

                context.SaveChanges();
            }
        }

    }
}
