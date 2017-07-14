﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMafia.Items
{
    public abstract class BaseItem
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string[] NameCases { get; }
        public abstract int Cost { get; }
        public virtual bool IsActive { get; protected set; }

        public BaseItem()
        {
            IsActive = true;
        }

        private static BaseItem[] _availableItems;
        public static BaseItem[] AvailableItems
        {
            get
            {
                if (_availableItems == null)
                {
                    _availableItems = new BaseItem[]
                    {
                        new Mask(),
                    };
                }
                return _availableItems;
            }
        }

        public abstract void Use(InGamePlayerInfo currentPlayer, List<InGamePlayerInfo> playersList);
    }
}
