using RWF.GameModes;
using Simple_Gamemodes.Gamemodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple_Gamemodes.GamemodeHandlers
{
    public class TitanFall_Handler : RWFGameModeHandler<GM_TitanFall>
    {
        internal const string GameModeName = "TitanFall";
        internal const string GameModeID = "TitanFall";
        public TitanFall_Handler() : base(
            name: GameModeName,
            gameModeId: GameModeID,
            allowTeams: false,
            pointsToWinRound: 4,
            roundsToWinGame: 5,
            // null values mean RWF's instance values
            playersRequiredToStartGame: null,
            maxPlayers: null,
            maxTeams: null,
            maxClients: null,
            description: $"Each round a random player is picked to be the TITAN, who gets a damage buff and a damage resentence buff. all other players try to kill the TITAN. The TITAN gets 4 points per win. Point role over enabled. NOTE: this gamemode is intended to be played with 5 or more players.")
        {

        }
    }
}
