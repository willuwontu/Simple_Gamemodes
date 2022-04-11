using RWF.GameModes;
using Simple_Gamemodes.Gamemodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple_Gamemodes.GamemodeHandlers
{
    public class Timed_Deathmatch_Handler : RWFGameModeHandler<GM_Timed_Deathmatch>
    {
        internal const string GameModeName = "Timed Deathmatch";
        internal const string GameModeID = "Timed_Deathmatch";
        public Timed_Deathmatch_Handler() : base(
            name: GameModeName,
            gameModeId: GameModeID,
            allowTeams: false,
            pointsToWinRound: 1,
            roundsToWinGame: 3,
            // null values mean RWF's instance values
            playersRequiredToStartGame: null,
            maxPlayers: null,
            maxTeams: null,
            maxClients: null,
            description: $"Timed free for all. Respawns enabled. The Player with the most kills each round wins.",
            videoURL: "https://github.com/Tess-y/Simple_Gamemodes/raw/master/Videos/Timed_Deathmatch.MP4")
        {

        }
    }
    
    public class Team_Timed_Deathmatch_Handler : RWFGameModeHandler<GM_Timed_Deathmatch>
    {
        internal const string GameModeName = "Team Timed Deathmatch";
        internal const string GameModeID = "Team_Timed_Deathmatch";
        public Team_Timed_Deathmatch_Handler() : base(
            name: GameModeName,
            gameModeId: GameModeID,
            allowTeams: true,
            pointsToWinRound: 1,
            roundsToWinGame: 5,
            // null values mean RWF's instance values
            playersRequiredToStartGame: null,
            maxPlayers: null,
            maxTeams: null,
            maxClients: null,
            description: $"Timed free for all. Respawns enabled. The Team with the most kills each round wins.",
            videoURL: "https://github.com/Tess-y/Simple_Gamemodes/raw/master/Videos/Timed_Deathmatch.MP4")
        {

        }
    }
}
