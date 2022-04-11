using RWF.GameModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnboundLib.GameModes;

namespace Simple_Gamemodes.Gamemodes
{
    public class GM_Rollover_Deathmatch : RWFGameMode
    {
        public override void RoundOver(int[] winningTeamIDs)
        {

            int points_to_win = (int)GameModeManager.CurrentHandler.Settings["pointsToWinRound"];

            for (int i = 0; i < teamPoints.Count; i++)
            {
                if (teamPoints[i] >= points_to_win) teamPoints[i] -= points_to_win;
            }
            previousRoundWinners = winningTeamIDs.ToArray();
            StartCoroutine(RoundTransition(winningTeamIDs));
        }
    }
}
