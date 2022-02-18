using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarioMaker2Overlay
{
    [Serializable]
    class LevelData
    {
        public string Code { get; set; }
        public int PlayerDeaths { get; set; }
        public int TotalGlobalClears { get; set; }
        public int TotalGlobalAttpemts { get; set; }
    }
}
