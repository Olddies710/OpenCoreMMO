﻿using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Game.Items.Items.Attributes
{
    public sealed class SkillBonus: ISkillBonus
    {
        public SkillBonus(Dictionary<SkillType, byte> skillBonuses)
        {
            SkillBonuses = skillBonuses;
        }

        public Dictionary<SkillType, byte> SkillBonuses { get; }

        public void AddSkillBonus(IPlayer player)
        {
            if (Guard.AnyNull(SkillBonuses, player)) return;
            foreach (var (skillType, bonus) in SkillBonuses) player.AddSkillBonus(skillType, bonus);
        }

        public void RemoveSkillBonus(IPlayer player)
        {
            if (Guard.AnyNull(SkillBonuses, player)) return;
            foreach (var (skillType, bonus) in SkillBonuses) player.RemoveSkillBonus(skillType, bonus);
        }
    }
}
