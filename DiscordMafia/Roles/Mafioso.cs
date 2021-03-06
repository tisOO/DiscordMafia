﻿namespace DiscordMafia.Roles
{
    public class Mafioso : BaseRole
    {
        public override string Name
        {
            get
            {
                return "Маф";
            }
        }

        public override string[] NameCases
        {
            get
            {
                return new string[] {
                    "маф",
                    "мафа",
                    "мафу",
                    "мафа",
                    "мафом",
                    "мафе",
                };
            }
        }

        public override Team Team
        {
            get
            {
                return Team.Mafia;
            }
        }

        public override void NightInfo(Game game, InGamePlayerInfo currentPlayer)
        {
            base.NightInfo(game, currentPlayer);
            game.GetAlivePlayersMesssage(true, true, currentPlayer, "/kill");
        }

        public override bool IsReady(GameState currentState)
        {
            switch (currentState)
            {
                case GameState.Night:
                    if (Player.VoteFor == null)
                    {
                        return false;
                    }
                    break;
            }
            return base.IsReady(currentState);
        }
    }
}
