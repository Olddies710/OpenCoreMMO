﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Location;

namespace NeoServer.Game.Combat;

public class CombatTarget
{
    public CombatTarget(ICombatActor creature)
    {
        Creature = creature;
    }

    public ICombatActor Creature { get; set; }
    public Direction[] PathToCreature { get; private set; }
    public bool CanReachCreature { get; private set; } = true;
    public bool CanSee { get; set; } = true;

    public void SetAsUnreachable()
    {
        CanReachCreature = false;
    }

    public void SetAsReachable(Direction[] path)
    {
        CanReachCreature = true;
        PathToCreature = path;
    }
}