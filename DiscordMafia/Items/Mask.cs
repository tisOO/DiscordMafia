﻿using System;
using System.Collections.Generic;

namespace DiscordMafia.Items
{
    public class Mask : BaseItem
    {
        public override string Name
        {
            get
            {
                return "Маскировочный комплект";
            }
        }
        public override string Description
        {
            get
            {
                return "Покупая комплект, игрок ночью избегает встречи с персонажами. Т.е. маф не убьет, комиссар не проверит, путана не заблокирует. Можно купить только 1 раз за игру.";
            }
        }

        public override string[] NameCases
        {
            get
            {
                return new string[] {
                    "маскировочный комплект",
                    "маскировочного комплекта",
                    "маскировочному комплекту",
                    "маскировочный комплект",
                    "маскировочным комплектом",
                    "маскировочном комплекте",
                };
            }
        }

        public override int Cost
        {
            get
            {
                return 20;
            }
        }

        public override void Use(InGamePlayerInfo currentPlayer, List<InGamePlayerInfo> playersList)
        {
            foreach (var player in playersList)
            {
                if (player != currentPlayer)
                {
                    player.CancelActivity(currentPlayer);
                }
            }
            IsActive = false;
        }
    }
}
