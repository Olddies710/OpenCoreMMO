﻿using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;

namespace NeoServer.Game.Creatures.Factories;

public class CreatureFactory : ICreatureFactory
{
    private readonly IEnumerable<ICreatureEventSubscriber> _creatureEventSubscribers;

    //factories
    private readonly IMonsterFactory _monsterFactory;
    private readonly INpcFactory _npcFactory;

    public CreatureFactory(
        IMonsterFactory monsterFactory,
        IEnumerable<ICreatureEventSubscriber> creatureEventSubscribers, INpcFactory npcFactory)
    {
        _monsterFactory = monsterFactory;
        _creatureEventSubscribers = creatureEventSubscribers;
        Instance = this;
        _npcFactory = npcFactory;
    }

    public static ICreatureFactory Instance { get; private set; }

    public IMonster CreateMonster(string name, ISpawnPoint spawn = null)
    {
        var monster = _monsterFactory.Create(name, spawn);
        if (monster is null) return null;

        AttachEvents(monster);
        return monster;
    }

    public IMonster CreateSummon(string name, IMonster master)
    {
        var monster = _monsterFactory.CreateSummon(name, master);
        if (monster is null) return null;

        AttachEvents(monster);
        return monster;
    }

    public INpc CreateNpc(string name, ISpawnPoint spawn = null)
    {
        var npc = _npcFactory.Create(name, spawn);
        if (npc is null) return null;

        AttachEvents(npc);
        return npc;
    }

    public IPlayer CreatePlayer(IPlayer player)
    {
        return AttachEvents(player) as IPlayer;
    }

    private ICreature AttachEvents(ICreature creature)
    {
        foreach (var gameSubscriber in _creatureEventSubscribers.Where(x =>
                     x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //register game events first
            gameSubscriber?.Subscribe(creature);

        foreach (var subscriber in _creatureEventSubscribers.Where(x =>
                     !x.GetType().IsAssignableTo(typeof(IGameEventSubscriber)))) //than register server events
            subscriber?.Subscribe(creature);

        return creature;
    }
}