﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Common.Contracts.World;

public interface IPathFinder
{
    IMap Map { get; set; }

    bool Find(ICreature creature, Location.Structs.Location target, FindPathParams findPathParams,
        ITileEnterRule tileEnterRule,
        out Direction[] directions);

    bool Find(ICreature creature, Location.Structs.Location target, ITileEnterRule tileEnterRule,
        out Direction[] directions);

    Direction FindRandomStep(ICreature creature, ITileEnterRule rule);
}