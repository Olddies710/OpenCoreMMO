﻿using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Players;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Game.Contracts.Items.Types;

namespace NeoServer.Server.Commands.Player
{

    public class PlayerUseItemCommand : Command
    {
        private readonly Game game;
        private UseItemPacket useItemPacket;
        private readonly IPlayer player;

        public PlayerUseItemCommand(IPlayer player, Game game, UseItemPacket useItemPacket)
        {
            this.game = game;
            this.player = player;
            this.useItemPacket = useItemPacket;
        }

        public override void Execute()
        {
            if (useItemPacket.Location.Type == LocationType.Ground)
            {
                if (game.Map[useItemPacket.Location] is not IDynamicTile tile) return;
                if(tile.TopItemOnStack is IContainer container)
                {
                    player.Containers.OpenContainerAt(useItemPacket.Location, useItemPacket.Index, container);
                    return;
                }
            }
            else if (useItemPacket.Location.Slot == Slot.Backpack || useItemPacket.Location.Type == LocationType.Container)
            {
                player.Containers.OpenContainerAt(useItemPacket.Location, useItemPacket.Index);
            }
        }

    }
}
