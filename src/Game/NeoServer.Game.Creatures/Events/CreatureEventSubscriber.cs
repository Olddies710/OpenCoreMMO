﻿using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Creatures.Events.Monsters;
using NeoServer.Game.Creatures.Events.Players;

namespace NeoServer.Game.Creatures.Events;

public class CreatureEventSubscriber : ICreatureEventSubscriber, IGameEventSubscriber
{
    private readonly CreatureDamagedEventHandler creatureDamagedEventHandler;
    private readonly CreatureKilledEventHandler creatureKilledEventHandler;
    private readonly CreatureMovedEventHandler creatureMovedEventHandler;
    private readonly CreaturePropagatedAttackEventHandler creaturePropagatedAttackEventHandler;
    public readonly CreatureSayEventHandler creatureSayEventHandler;
    private readonly CreatureTeleportedEventHandler creatureTeleportedEventHandler;
    public readonly MonsterKilledEventHandler monsterKilledEventHandler;
    private readonly PlayerDisappearedEventHandler playerDisappearedEventHandler;
    private readonly PlayerLoggedInEventHandler playerLoggedInEventHandler;
    private readonly PlayerLoggedOutEventHandler playerLoggedOutEventHandler;
    public readonly PlayerOpenedContainerEventHandler playerOpenedContainerEventHandler;

    public CreatureEventSubscriber(CreatureKilledEventHandler creatureKilledEventHandler,
        CreatureDamagedEventHandler creatureDamagedEventHandler,
        CreaturePropagatedAttackEventHandler creaturePropagatedAttackEventHandler,
        CreatureTeleportedEventHandler creatureTeleportedEventHandler,
        PlayerDisappearedEventHandler playerDisappearedEventHandler,
        CreatureMovedEventHandler creatureMovedEventHandler, PlayerLoggedInEventHandler playerLoggedInEventHandler,
        PlayerLoggedOutEventHandler playerLoggedOutEventHandler,
        CreatureSayEventHandler creatureSayEventHandler, MonsterKilledEventHandler monsterKilledEventHandler,
        PlayerOpenedContainerEventHandler playerOpenedContainerEventHandler)
    {
        this.creatureKilledEventHandler = creatureKilledEventHandler;
        this.creatureDamagedEventHandler = creatureDamagedEventHandler;
        this.creaturePropagatedAttackEventHandler = creaturePropagatedAttackEventHandler;
        this.creatureTeleportedEventHandler = creatureTeleportedEventHandler;
        this.playerDisappearedEventHandler = playerDisappearedEventHandler;
        this.creatureMovedEventHandler = creatureMovedEventHandler;
        this.playerLoggedInEventHandler = playerLoggedInEventHandler;
        this.playerLoggedOutEventHandler = playerLoggedOutEventHandler;
        this.creatureSayEventHandler = creatureSayEventHandler;
        this.monsterKilledEventHandler = monsterKilledEventHandler;
        this.playerOpenedContainerEventHandler = playerOpenedContainerEventHandler;
    }

    public void Subscribe(ICreature creature)
    {
        if (creature is ICombatActor combatActor)
        {
            combatActor.OnKilled += creatureKilledEventHandler.Execute;
            combatActor.OnInjured += creatureDamagedEventHandler.Execute;
            combatActor.OnPropagateAttack += creaturePropagatedAttackEventHandler.Execute;
        }

        if (creature is IWalkableCreature walkableCreature)
        {
            walkableCreature.OnTeleported += creatureTeleportedEventHandler.Execute;
            walkableCreature.OnCreatureMoved += creatureMovedEventHandler.Execute;
        }

        if (creature is IPlayer player)
        {
            player.OnLoggedOut += playerDisappearedEventHandler.Execute;
            player.OnLoggedIn += playerLoggedInEventHandler.Execute;
            player.OnLoggedOut += playerLoggedOutEventHandler.Execute;
            player.Containers.OnOpenedContainer += playerOpenedContainerEventHandler.Execute;
        }

        if (creature is IMonster monster) monster.OnKilled += monsterKilledEventHandler.Execute;

        creature.OnSay += creatureSayEventHandler.Execute;
    }

    public void Unsubscribe(ICreature creature)
    {
        if (creature is ICombatActor combatActor)
        {
            combatActor.OnKilled -= creatureKilledEventHandler.Execute;
            combatActor.OnInjured -= creatureDamagedEventHandler.Execute;
            combatActor.OnPropagateAttack -= creaturePropagatedAttackEventHandler.Execute;
        }

        if (creature is IWalkableCreature walkableCreature)
        {
            walkableCreature.OnTeleported -= creatureTeleportedEventHandler.Execute;
            walkableCreature.OnCreatureMoved -= creatureMovedEventHandler.Execute;
        }

        if (creature is IPlayer player)
        {
            player.OnLoggedOut -= playerDisappearedEventHandler.Execute;
            player.OnLoggedIn -= playerLoggedInEventHandler.Execute;
            player.OnLoggedOut -= playerLoggedOutEventHandler.Execute;
            player.Containers.OnOpenedContainer -= playerOpenedContainerEventHandler.Execute;
        }

        if (creature is IMonster monster) monster.OnKilled -= monsterKilledEventHandler.Execute;

        creature.OnSay -= creatureSayEventHandler.Execute;
    }
}