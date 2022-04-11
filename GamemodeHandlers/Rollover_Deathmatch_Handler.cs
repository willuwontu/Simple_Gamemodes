using RWF.GameModes;
using Simple_Gamemodes.Gamemodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple_Gamemodes.GamemodeHandlers
{
    public class Rollover_Deathmatch_Handler : RWFGameModeHandler<GM_Rollover_Deathmatch>
    {
        internal const string GameModeName = "Rollover Deathmatch";
        internal const string GameModeID = "Rollover_Deathmatch";
        public Rollover_Deathmatch_Handler() : base(
            name: GameModeName,
            gameModeId: GameModeID,
            allowTeams: false,
            pointsToWinRound: 2,
            roundsToWinGame: 3,
            // null values mean RWF's instance values
            playersRequiredToStartGame: null,
            maxPlayers: null,
            maxTeams: null,
            maxClients: null,
            description: $"Free For All Deathmatch. Last player standing wins. Point role over enabled.    Blame RS_Mind he asked for this.",
            videoURL: "https://github.com/olavim/RoundsWithFriends/raw/main/Media/TeamDeathmatch.mp4")
        {

        }
    }
    
    public class Team_Rollover_Deathmatch_Handler : RWFGameModeHandler<GM_Rollover_Deathmatch>
    {
        internal const string GameModeName = "Team Rollover Deathmatch";
        internal const string GameModeID = "Team_Rollover_Deathmatch";
        public Team_Rollover_Deathmatch_Handler() : base(
            name: GameModeName,
            gameModeId: GameModeID,
            allowTeams: true,
            pointsToWinRound: 2,
            roundsToWinGame: 5,
            // null values mean RWF's instance values
            playersRequiredToStartGame: null,
            maxPlayers: null,
            maxTeams: null,
            maxClients: null,
            description: $"Team Deathmatch. Last team standing wins. Point role over enabled.    Blame RS_Mind he asked for this.",
            videoURL: "https://github.com/olavim/RoundsWithFriends/raw/main/Media/TeamDeathmatch.mp4")
        {

        }
    }
}
