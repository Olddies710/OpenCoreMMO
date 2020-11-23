﻿using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Loaders.World;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace NeoServer.Loaders.Monsters
{
    public class MonsterLoader
    {
        private readonly IMonsterDataManager _monsterManager;
        private readonly GameConfiguration gameConfiguration;
        public MonsterLoader(IMonsterDataManager monsterManager, GameConfiguration gameConfiguration)
        {
            _monsterManager = monsterManager;
            this.gameConfiguration = gameConfiguration;
        }
        public void Load()
        {
            var monsters = GetMonsterDataList();
            _monsterManager.Load(monsters);
        }

        private IEnumerable<IMonsterType> GetMonsterDataList()
        {
            var basePath = "./data/monsters";
            var jsonString = File.ReadAllText(Path.Combine(basePath, "monsters.json"));
            var monstersPath = JsonConvert.DeserializeObject<List<IDictionary<string, string>>>(jsonString);

            foreach (var monsterFile in monstersPath)
            {
                yield return ConvertMonster(basePath, monsterFile);
            }
        }
        private IMonsterType ConvertMonster(string basePath, IDictionary<string, string> monsterFile)
        {
            var json = File.ReadAllText(Path.Combine(basePath, monsterFile["file"]));

            var monster = JsonConvert.DeserializeObject<MonsterData>(json, new JsonSerializerSettings { Error = (se, ev) => { ev.ErrorContext.Handled = true; Console.WriteLine(ev.ErrorContext.Error); } });

            return MonsterConverter.Convert(monster, gameConfiguration);
        }
    }
}
