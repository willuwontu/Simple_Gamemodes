using RWF.GameModes;
using Simple_Gamemodes.Gamemodes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple_Gamemodes.GamemodeHandlers
{
    public class Reverse_Rounds_Handler : RWFGameModeHandler<GM_Reverse_Rounds>
    {
        internal const string GameModeName = "Reverse Rounds";
        internal const string GameModeID = "Reverse_Rounds";
        public Reverse_Rounds_Handler() : base(
            name: GameModeName,
            gameModeId: GameModeID,
            allowTeams: false,
            pointsToWinRound: 2,
            roundsToWinGame: 5,
            // null values mean RWF's instance values
            playersRequiredToStartGame: null,
            maxPlayers: null,
            maxTeams: null,
            maxClients: null,
            description: $"Free for all. Pick all of your cards at the sart. Lose one each time you win a round. Run out of cards and win a round to win the game.")
        {
            
        }
    }
    
    public class Team_Reverse_Rounds_Handler : RWFGameModeHandler<GM_Reverse_Rounds>
    {
        internal const string GameModeName = "Team Reverse Rounds";
        internal const string GameModeID = "Team_Reverse_Rounds";
        public Team_Reverse_Rounds_Handler() : base(
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
            description: $"Pick all of your cards at the sart. Each member of your time loses one each time you win a round. Have a teammeber run out and win a round to win the game.")
        {
            
        }
    }
}
