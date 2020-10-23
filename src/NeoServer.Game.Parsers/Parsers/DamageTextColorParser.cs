﻿using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Parsers.Effects
{
    public class DamageTextColorParser
    {
        public static TextColor Parse(DamageType damageType) => damageType switch
        {
            DamageType.Fire => TextColor.Orange,
            DamageType.Energy => TextColor.Purple,
            DamageType.Melee => TextColor.Red,
            _ => TextColor.None
        };
    }
}