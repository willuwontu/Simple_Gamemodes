using RWF.GameModes;
using Simple_Gamemodes.Gamemodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple_Gamemodes.GamemodeHandlers
{
    public class Stock_Battle_Handler : RWFGameModeHandler<GM_Stock_Battle>
    {
        internal const string GameModeName = "Stock Battle";
        internal const string GameModeID = "Stock_Battle";
        public Stock_Battle_Handler() : base(
            name: GameModeName,
            gameModeId: GameModeID,
            allowTeams: false,
            pointsToWinRound: 1,
            roundsToWinGame: 5,
            // null values mean RWF's instance values
            playersRequiredToStartGame: null,
            maxPlayers: null,
            maxTeams: null,
            maxClients: null,
            description: $"Free for all with limited respawns enabled",
            videoURL: "https://github.com/Tess-y/Simple_Gamemodes/raw/master/Videos/Stock_Battle.MP4")
        {

        }
    }
    
    public class Team_Stock_Battle_Handler : RWFGameModeHandler<GM_Stock_Battle>
    {
        internal const string GameModeName = "Team Stock Battle";
        internal const string GameModeID = "Team_Stock_Battle";
        public Team_Stock_Battle_Handler() : base(
            name: GameModeName,
            gameModeId: GameModeID,
            allowTeams: true,
            pointsToWinRound: 1,
            roundsToWinGame: 3,
            // null values mean RWF's instance values
            playersRequiredToStartGame: null,
            maxPlayers: null,
            maxTeams: null,
            maxClients: null,
            description: $"Team battle with limited respawns enabled",
            videoURL: "https://github.com/Tess-y/Simple_Gamemodes/raw/master/Videos/Stock_Battle.MP4")
        {

        }
    }
}
