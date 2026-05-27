using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCoopServer
{
    public class LootTableManager
    {
        public static Dictionary<string, DataStr.PrefabTable> s_LootTables = new Dictionary<string, DataStr.PrefabTable> ();

        public const bool c_DebugLogs = false;

        public static string GetRandomLoot(string LootTableName = "Main")
        {
            if (c_DebugLogs)
            {
                SkyCoopServer.Logger.Log($"GetRandomLoot from table {LootTableName}");
            }
            
            
            DataStr.PrefabTable LootTable = null;

            if(s_LootTables.TryGetValue(LootTableName, out LootTable))
            {
                string Prefab = LootTable.GetRandomItemPrefab();
                if (c_DebugLogs)
                {
                    SkyCoopServer.Logger.Log($"Got loot {Prefab}");
                }
                
                return Prefab;
            }

            return string.Empty;
        }

        public static void Load()
        {
            s_LootTables.Clear();

            Dictionary<string, DataStr.PrefabTableJSON> Jsons = FilesManager.GetAllLootTables();

            // Первый проход: создаем все таблицы без связей
            foreach (var jsonPair in Jsons)
            {
                string tableName = jsonPair.Key;
                DataStr.PrefabTableJSON jsonTable = jsonPair.Value;

                DataStr.PrefabTable tableInstance = new DataStr.PrefabTable();

                // Заполняем Items
                if (jsonTable.Items != null)
                {
                    foreach (var item in jsonTable.Items)
                    {
                        tableInstance.Items.Add(new DataStr.Loot
                        {
                            Prefab = item.Prefab,
                            Chance = item.Chance
                        });
                    }
                }

                // Создаем пустые LootTables (связи установим во втором проходе)
                if (jsonTable.LootTables != null)
                {
                    foreach (var lootTableJson in jsonTable.LootTables)
                    {
                        tableInstance.LootTables.Add(new DataStr.LootTableInLootTable
                        {
                            Name = lootTableJson.LootTable,
                            LootTable = null, // Установим во втором проходе
                            Chance = lootTableJson.Chance
                        });
                    }
                }

                s_LootTables[tableName] = tableInstance;
            }

            // Второй проход: устанавливаем связи между таблицами
            foreach (var Pair in s_LootTables)
            {
                if(Pair.Value.LootTables == null)
                {
                    Pair.Value.LootTables = new List<DataStr.LootTableInLootTable>();
                }
                else
                {
                    foreach (DataStr.LootTableInLootTable SubTable in Pair.Value.LootTables)
                    {
                        if (!string.IsNullOrEmpty(SubTable.Name))
                        {
                            SubTable.LootTable = s_LootTables[SubTable.Name];
                        }
                    }
                }
                Pair.Value.CalculateWeights();
            }

            if (c_DebugLogs)
            {
                SkyCoopServer.Logger.Log($"Loaded {s_LootTables.Count} loot tables");

                foreach (var item in s_LootTables)
                {
                    string tableName = item.Key;
                    DataStr.PrefabTable table = item.Value;

                    SkyCoopServer.Logger.Log($"=================={tableName}==================");
                    foreach (var _item in table.Items)
                    {
                        SkyCoopServer.Logger.Log($"Prefab - {_item.Prefab} Chance - {_item.Chance}");
                    }

                    foreach (var _subtables in table.LootTables)
                    {
                        if (_subtables != null)
                        {
                            if (_subtables.LootTable != null)
                            {
                                SkyCoopServer.Logger.Log($"SubTable - {_subtables.Name} Chance {_subtables.Chance}");
                                //foreach (var _item in _subtables.LootTable.Items)
                                //{
                                //    if (_item != null)
                                //    {
                                //        SkyCoopServer.Logger.Log($"============> Prefab - {_item.Prefab} Chance - {_item.Chance}");
                                //    }
                                //}
                            }
                        }
                    }
                }
            }
        }
    }
}
