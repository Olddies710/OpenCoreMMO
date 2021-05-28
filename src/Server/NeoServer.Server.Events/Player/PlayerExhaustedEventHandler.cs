﻿using NeoServer.Game.Common.Texts;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts;

namespace NeoServer.Server.Events.Player
{
    public class PlayerExhaustedEventHandler
    {
        private readonly IMap map;
        private readonly IGameServer game;

        public PlayerExhaustedEventHandler(IMap map, IGameServer game)
        {
            this.map = map;
            this.game = game;
        }
        public void Execute(IPlayer player)
        {
            if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;
            connection.OutgoingPackets.Enqueue(new MagicEffectPacket(player.Location, Enums.Creatures.Enums.EffectT.Puff));
            connection.OutgoingPackets.Enqueue(new TextMessagePacket(TextConstants.YouAreExhausted, TextMessageOutgoingType.Small));
            connection.Send();
        }
    }
}