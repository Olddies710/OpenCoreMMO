﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Networking.Packets.Outgoing.Creature;
using NeoServer.Networking.Packets.Outgoing.Effect;
using NeoServer.Networking.Packets.Outgoing.Item;
using NeoServer.Networking.Packets.Outgoing.Map;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Server.Events.Creature;

public class CreatureMovedEventHandler
{
    private readonly IGameServer game;

    public CreatureMovedEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IWalkableCreature creature, ICylinder cylinder)
    {
        if (cylinder.IsNull()) return;
        if (cylinder.TileSpectators.IsNull()) return;
        if (creature.IsNull()) return;

        var toTile = cylinder.ToTile;
        var fromTile = cylinder.FromTile;
        if (toTile.IsNull()) return;
        if (fromTile.IsNull()) return;

        var toDirection = fromTile.Location.DirectionTo(toTile.Location, true);

        MoveCreature(toDirection, creature, cylinder);
    }

    private void MoveCreature(Direction toDirection, IWalkableCreature creature, ICylinder cylinder)
    {
        var fromLocation = cylinder.FromTile.Location;
        var toLocation = cylinder.ToTile.Location;
        var fromTile = cylinder.FromTile;

        if (creature is IMonster && creature.IsInvisible) return;

        foreach (var cylinderSpectator in cylinder.TileSpectators)
        {
            var spectator = cylinderSpectator.Spectator;

            if (spectator is not IPlayer player) continue;

            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) continue;

            if (TryMoveMyself(toDirection, creature, cylinder, player, fromLocation, toLocation, connection, fromTile, cylinderSpectator)) continue;

            if (player.CanSee(creature) && player.CanSee(fromLocation) &&
                player.CanSee(toLocation)) //spectator can see old and new location
            {
                MoveCreature(creature, fromLocation, toLocation, connection, fromTile, cylinderSpectator, player);

                connection.Send();

                continue;
            }

            if (player.CanSee(creature) &&
                player.CanSee(fromLocation)) //spectator can see old position but not the new
            {
                //happens when player leaves spectator's view area
                connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(fromTile,
                    cylinderSpectator.FromStackPosition));
                connection.Send();

                continue;
            }

            if (!player.CanSee(creature) || !player.CanSee(toLocation)) continue;
            
            //happens when player enters spectator's view area
            connection.OutgoingPackets.Enqueue(new AddAtStackPositionPacket(creature,
                cylinderSpectator.ToStackPosition));
            
            connection.OutgoingPackets.Enqueue(new AddCreaturePacket(player, creature));
            
            connection.Send();
        }
    }

    private static void MoveCreature(IWalkableCreature creature, Location fromLocation, Location toLocation,
        IConnection connection, ITile fromTile, ICylinderSpectator cylinderSpectator, IPlayer player)
    {
        if (fromLocation.Z != toLocation.Z)
        {
            connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(fromTile,
                cylinderSpectator.FromStackPosition));
            connection.OutgoingPackets.Enqueue(new AddAtStackPositionPacket(creature,
                cylinderSpectator.ToStackPosition));

            connection.OutgoingPackets.Enqueue(new AddCreaturePacket(player, creature));

            return;
        }

        connection.OutgoingPackets.Enqueue(new CreatureMovedPacket(fromLocation, toLocation,
            cylinderSpectator.FromStackPosition));
    }

    private bool TryMoveMyself(Direction toDirection, ICreature creature, ICylinder cylinder, IPlayer player,
        Location fromLocation, Location toLocation, IConnection connection, ITile fromTile,
        ICylinderSpectator cylinderSpectator)
    {
        if (player.CreatureId != creature.CreatureId) return false;
        
        if (fromLocation.Z != toLocation.Z)
        {
            connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(fromTile,
                cylinderSpectator.FromStackPosition));
            connection.OutgoingPackets.Enqueue(new MapDescriptionPacket(player, game.Map));

            return true;
        }

        if (cylinder.IsTeleport)
        {
            connection.OutgoingPackets.Enqueue(new RemoveTileThingPacket(fromTile,
                cylinderSpectator.FromStackPosition));
            connection.OutgoingPackets.Enqueue(new MapDescriptionPacket(player, game.Map));
            connection.OutgoingPackets.Enqueue(new MagicEffectPacket(toLocation, EffectT.BubbleBlue));

            return true;
        }

        connection.OutgoingPackets.Enqueue(new CreatureMovedPacket(fromLocation, toLocation,
            cylinderSpectator.FromStackPosition));
        connection.OutgoingPackets.Enqueue(new MapPartialDescriptionPacket(creature, fromLocation,
            toLocation, toDirection, game.Map));

        connection.Send();
        
        return true;
    }
}