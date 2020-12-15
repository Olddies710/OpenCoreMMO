﻿using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Networking.Packets.Outgoing
{
    public class CreatureOutfitPacket : OutgoingPacket
    {
        private readonly ICreature creature;

        public CreatureOutfitPacket(ICreature creature)
        {
            this.creature = creature;
        }

        public override void WriteToMessage(INetworkMessage message)
        {
            message.AddByte((byte)GameOutgoingPacketType.CreatureOutfit);

            message.AddUInt32(creature.CreatureId);
            message.AddUInt16(creature.Outfit.LookType);

            if (creature.Outfit.LookType > 0)
            {
                message.AddByte(creature.Outfit.Head);
                message.AddByte(creature.Outfit.Body);
                message.AddByte(creature.Outfit.Legs);
                message.AddByte(creature.Outfit.Feet);
                message.AddByte(creature.Outfit.Addon);
            }
            else
            {
                //message.AddUInt16(creature.Outfit.LookTypeEx) //todo
            }
        }

    }
}
