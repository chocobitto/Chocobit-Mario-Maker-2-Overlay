using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MarioMaker2Overlay.Persistence
{
    public class PlayerRepository
    {
        public List<Player> GetPlayers()
        {
            using (MarioMaker2OverlayContext context = new())
            {
                List<Player> players = context.Player.ToList();

                return players;
            }
        }
    }
}
