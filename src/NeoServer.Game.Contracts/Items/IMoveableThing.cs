﻿using NeoServer.Game.Enums.Location.Structs;

namespace NeoServer.Game.Contracts.Items
{
    public interface IMoveableThing : IThing
    {
        void SetNewLocation(Location location);
    }
}