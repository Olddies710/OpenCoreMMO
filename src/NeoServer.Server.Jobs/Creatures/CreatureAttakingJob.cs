﻿using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Jobs.Creatures
{
    public class CreatureAttakingJob
    {
        private const uint INTERVAL = 1000;
        public static void Execute(ICreature creature, Game game)
        {
            if (creature.IsDead)
            {
                return;
            }
            if (creature.Attacking)
            {
                if (!game.CreatureManager.TryGetCreature(creature.AutoAttackTargetId, out ICreature enemy))
                {
                    return;
                }

                creature.Attack(enemy);
            }
        }
    }
}