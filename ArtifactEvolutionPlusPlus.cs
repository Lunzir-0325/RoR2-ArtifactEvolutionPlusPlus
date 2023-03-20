using BepInEx;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System;
using RoR2.Artifacts;

namespace ArtifactEvolutionPlusPlus
{
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInPlugin("com.Lunzir.ArtifactEvolutionPlusPlus", "Artifact Evolution Plus Plus", "1.1.0")]
    [NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]
    public class ArtifactEvolutionPlusPlus : BaseUnityPlugin
    {
        System.Collections.Hashtable ItemTable = new System.Collections.Hashtable();
        System.Random Random = new System.Random();

        private static ItemController ItemController_Instance;
        private static List<MonsterItemConcreteStruct> SaveMonsterItems;
        private static List<MonsterItemConcreteStruct> CurrentMonsterItems;

        private static ArtifactEvolutionPlusPlus instance;
        
        public void Awake()
        {
            instance = this;
            ModConfig.InitConfig(Config);

            if (ModConfig.EnableMod.Value)
            {
                On.RoR2.Run.Start += Run_Start;
                On.RoR2.Run.OnDestroy += Run_OnDestroy;

                On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.GrantMonsterTeamItem += MonsterTeamGainsItemsArtifactManager_GrantMonsterTeamItem;
                On.RoR2.SceneDirector.Start += SceneDirector_Start;
            }
        }

        private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            Config.Reload();
            ModConfig.InitConfig(Config);
        }
        private void Run_OnDestroy(On.RoR2.Run.orig_OnDestroy orig, Run self)
        {
            orig(self);
            SaveMonsterItems.Clear();
            //ItemController = null;
        }

        private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            StartCoroutine(DelayLoad());
        }
        public IEnumerator DelayLoad()
        {
            yield return new WaitForSeconds(2f);
            SHowMonsterItemInfoByStage();
        }
        private void MonsterTeamGainsItemsArtifactManager_GrantMonsterTeamItem(On.RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.orig_GrantMonsterTeamItem orig)
        {
            orig();
            if (ModConfig.EnableMod.Value)
            {
                if (Run.instance.stageClearCount + 1 == 1)
                {
                    if (ItemController_Instance is null)
                    {
                        ItemController_Instance = new ItemController();
                    }
                    SaveMonsterItems = new List<MonsterItemConcreteStruct>();
                }
                if (ModConfig.EnableMessage.Value)
                {
                    if (!(CurrentMonsterItems is null)) CurrentMonsterItems.Clear();
                    CurrentMonsterItems = new List<MonsterItemConcreteStruct>(); 
                }
                
                bool artifactEnable = RunArtifactManager.instance.IsArtifactEnabled(ArtifactCatalog.FindArtifactDef("MonsterTeamGainsItems"));
                if (artifactEnable)
                {
                    // 1、生成结果 ， 追加保存结果
                    InitItemData();
                    // 2、排序
                    // 3、清空界面
                    ClearItems();
                    // 4、添加界面
                    AddItems();
                } 
            }
        }
        public void InitItemData()
        {
            int stageIndex = Run.instance.stageClearCount + 1;
            string[] itemCodes = null;
            List<MonsterItemInterface> monsterItemDefs = new List<MonsterItemInterface>();
            for (int i = 0; i < ModConfig.StageCustomItemList.Length; i++)
            {
                if (stageIndex == (i + 1))
                {
                    itemCodes = ModConfig.StageCustomItemList[i].Value.Split(',');
                    Logger.LogDebug("itemCodes = " + itemCodes.Length);
                    monsterItemDefs = SplitAndAddList(itemCodes);
                    InitItemData_Handle(monsterItemDefs);
                    break; // 找到对应关卡就退出循环
                }
            }
        }

        /// <summary>
        /// 分解字符串，获得具体内容
        /// </summary>
        /// <param name="itemCodes"></param>
        /// <param name="currentStage"></param>
        /// <returns></returns>
        private List<MonsterItemInterface> SplitAndAddList(string[] itemCodes, int currentStage = 0)
        {
            List<MonsterItemInterface> _customItems = new List<MonsterItemInterface>();

            int baseNum = currentStage;
            //int offset = ModConfig.ItemCountOffset.Value;
            //int mulitiple = ModConfig.ItemMuitipleCount.Value;
            //int totalCal = baseNum * mulitiple + offset; // (7 - 5) * 2 + 0 = 2

            try
            {
                for (int i = 0; i < itemCodes.Length; i++)
                {
                    string[] codes = itemCodes[i].Split('&');
                    string itemName = codes[0].ToString().Trim();
                    int poolrange = int.MaxValue;
                    int count = 1;
                    ItemDef itemClass = ItemController_Instance.ItemAll.Find(t => t.name.ToLower() == itemName.ToLower());
                    if (itemName.IsNullOrWhiteSpace())
                    {
                        continue;
                    }
                    if (codes.Count() == 2) // 如果是这个格式：ItemName-Count 或者 KeyName-ItemPoolRange
                    {
                        poolrange = int.MaxValue;
                        count = int.Parse(codes[1].ToString().Trim());
                        if (IsKeyWord(itemName))
                        {
                            poolrange = int.Parse(codes[1].ToString().Trim());
                            if(poolrange >= 0)
                            {
                                count = 1;
                            } else
                            {
                                count = -1;
                            }
                            _customItems.Add(new MonsterItemClassStruct(itemName, poolrange, count));
                        }
                        else if(itemClass != null)
                        {
                            _customItems.Add(new MonsterItemConcreteStruct(itemClass, count));
                        } 
                        else
                        {
                            Logger.LogError("Item code " + itemCodes[i] + " could not be parsed. " + itemName + " is not a keyword and not a known item.");
                        }
                    }
                    if (codes.Count() == 3) // 如果是这个格式：KeyName-ItemPoolRange-Count
                    {
                        poolrange = int.Parse(codes[1].ToString().Trim());
                        count = int.Parse(codes[2].ToString().Trim());
                        if (IsKeyWord(itemName))
                        {
                            if (poolrange >= 0 && count < 0)
                            {
                                poolrange = -poolrange;
                            }
                            _customItems.Add(new MonsterItemClassStruct(itemName, poolrange, count));
                        }
                        else
                        {
                            Logger.LogError("Item code " + itemCodes[i] + " could not be parsed. " + itemName + " is not a keyword.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
            }
            finally
            {

            }
            return _customItems;
        }
        private bool IsKeyWord(string name)
        {
            bool isWhat = true;
            if (name.ToLower() == "AllWhite".ToLower() || name.ToLower() == "AllTier1".ToLower())
            {
            }
            else if (name.ToLower() == "AllGreen".ToLower() || name.ToLower() == "AllTier2".ToLower())
            {
            }
            else if (name.ToLower() == "AllRed".ToLower() || name.ToLower() == "AllTier3".ToLower())
            {
            }
            else if (name.ToLower() == "AllYellow".ToLower() || name.ToLower() == "AllBoss".ToLower())
            {
            }
            else if (name.ToLower() == "AllPurple".ToLower() || name.ToLower() == "AllVoid".ToLower())
            {
            }
            else if (name.ToLower() == "AllBlue".ToLower() || name.ToLower() == "AllLunar".ToLower())
            {
            }
            else if (name.ToLower() == "AllRandom".ToLower())
            {
            }
            else
            {
                isWhat = false;
            }
            return isWhat;
        }
        private void InitItemData_Handle(List<MonsterItemInterface> monsterItemDefs)
        {
            foreach (MonsterItemInterface monsterItemDef in monsterItemDefs)
            {
                if (monsterItemDef is MonsterItemClassStruct)
                {
                    MonsterItemClassStruct classItem = monsterItemDef as MonsterItemClassStruct;
                    Logger.LogDebug("item.Name = " + classItem.Name);
                    var pool = ItemController_Instance.GetPool(classItem.Name);
                    if (pool == null)
                    {
                        Logger.LogError("Unknown keyword " + classItem.Name);
                    }
                    HandleClassItem(pool, classItem.PoolRange, classItem.Count);
                } 
                else if (monsterItemDef is MonsterItemConcreteStruct)
                {
                    MonsterItemConcreteStruct item = monsterItemDef as MonsterItemConcreteStruct;
                    if (ModConfig.EnableMessage.Value) CurrentMonsterItems.Add(item);
                    UpdateItems(item);
                    //AddMonsterTeamItem(item.Name, item.Count);
                }
            }
        }
        /// <summary>
        /// 针对关键字的处理方放
        /// </summary>
        private void HandleClassItem(List<ItemDef> itemDefs, int poolRange, int count)
        {
            if(poolRange < 0)
            {
                var tempItemDefs = new List<ItemDef>();
                foreach(var item in SaveMonsterItems)
                {
                    if(!tempItemDefs.Contains(item.ItemDef) && itemDefs.Contains(item.ItemDef))
                        tempItemDefs.Add(item.ItemDef);
                }
                itemDefs = tempItemDefs;
                poolRange = Math.Abs(poolRange);
            }
            List<MonsterItemConcreteStruct> tempSaveItems = new List<MonsterItemConcreteStruct>();
            string[] itemString;
            int totalPoolRange = itemDefs.Count;
            string message = "";
            if (poolRange > totalPoolRange) 
            { 
                poolRange = totalPoolRange; 
            }
            itemString = GetRandomPoolNum(itemDefs, totalPoolRange, poolRange); // 开始随机抽取
            foreach (string codeName in itemString)
            {
                ItemDef ItemDef = itemDefs.AsEnumerable().FirstOrDefault(t => t.name == codeName);
                message += NameToLocal(ItemDef.name) + "-" + count + " ";
                AddMonsterTeamItem(ItemDef.name, count);
                tempSaveItems.Add(new MonsterItemConcreteStruct(ItemDef, count)); // 生成结果

                if (ModConfig.EnableMessage.Value) CurrentMonsterItems.Add(new MonsterItemConcreteStruct(ItemDef, count));
                Logger.LogDebug(message);
            }
            UpdateItems(tempSaveItems); // 更新数量
        }
        /// <summary>
        /// 随机数方法
        /// </summary>
        /// <param name="totalPoolCount">物品池总数</param>
        /// <param name="range">抽取数量</param>
        /// <returns></returns>
        private string[] GetRandomPoolNum(List<ItemDef> itemClasses, int totalPoolCount, int range)
        {
            ItemTable.Clear();
            int nValue;
            string[] randomString = new string[range];
            bool style = ModConfig.ItemPoolRandomStyle.Value;
            if (style)
            {
                for (int i = 0; i < range; i++)
                {
                    nValue = Random.Next(totalPoolCount);
                    randomString[i] = itemClasses[nValue].name;
                }
            }
            else
            {
                for (int i = 0; ItemTable.Count < range; i++)
                {
                    nValue = Random.Next(totalPoolCount + 1);
                    if (!ItemTable.ContainsValue(nValue))
                    {
                        randomString[i] = itemClasses[i].name;
                        ItemTable.Add(nValue, nValue);
                    }
                    else
                    {
                        i--;
                    }
                }
            }
            
            return randomString;
        }

        public static string NameToLocal(string name)
        {
            ItemIndex index = ItemCatalog.FindItemIndex(name);
            ItemDef item = ItemCatalog.GetItemDef(index);
            string nameToken = item.nameToken;
            return Language.GetString(nameToken);
        }

        private void UpdateItems(List<MonsterItemConcreteStruct> monsterItemDefs)
        {
            foreach (MonsterItemConcreteStruct item in monsterItemDefs)
            {
                // 更新已有物品数量
                (from t in SaveMonsterItems
                 where t.Name == item.Name
                 select t).ToList().ForEach(y => y.Count += item.Count);
                // 添加新的物品
                MonsterItemConcreteStruct addDiffnet = SaveMonsterItems.Find(x => x.Name == item.Name);
                if (addDiffnet is null)
                {
                    SaveMonsterItems.Add(item);
                }
                if (item.Count < 0)
                {
                    item.Count = 0;
                }
            }
            //SaveMonsterItems = (from t in SaveMonsterItems orderby t.Order descending, t.Count descending select t).ToList();
        }
        private void UpdateItems(MonsterItemConcreteStruct item)
        {
            SaveMonsterItems.Add(item);
            //SaveMonsterItems = (from t in SaveMonsterItems orderby t.Order descending, t.Count descending select t).ToList();
        }
        private void ClearItems()
        {
            foreach (var item in ItemController_Instance.ItemAll)
            {
                RemoveMonsterTeamItem(item.name, 99999);
            }
        }
        private void AddItems()
        {
            SaveMonsterItems = (from t in SaveMonsterItems orderby t.Order descending, t.Count descending select t).ToList();
            foreach (var item in SaveMonsterItems)
            {
                AddMonsterTeamItem(item.Name, item.Count);
            }
            //foreach (MonsterItemDef  monsterItem in SaveMonsterItems)
            //{
            //    if (monsterItem.Count < 0) monsterItem.Count = 0; // 避免负数
            //}
        }

        #region Command
        [ConCommand(commandName = "monster_showitem", flags = ConVarFlags.ExecuteOnServer, helpText = "显示当前怪物所有物品")]
        public static void Command_ShowMonsterItems(ConCommandArgs args)
        {
            if (args.TryGetArgInt(0) == 1)
            {
                ChatHelper.Open = true;
                ShowMonsterItemTotalInfo();
            }
            if (args.TryGetArgInt(0) == 0)
            {
                //ChatHelper.Open = false;
                SHowMonsterItemInfoByStage();
            }
        }
        [ConCommand(commandName = "monster_additem", flags = ConVarFlags.ExecuteOnServer, helpText = "添加怪物物品 [物品名称code 数量]")]
        public static void Command_AddMonsterItems(ConCommandArgs args)
        {
            string itemString = args.TryGetArgString(0);
            if (args.Count >= 2)
            {
                int? itemCount = args.TryGetArgInt(1);
                if (itemCount.HasValue)
                {
                    AddMonsterTeamItem(itemString, itemCount.Value);
                    return;
                }
            }
            AddMonsterTeamItem(itemString);
        }
        [ConCommand(commandName = "monster_addequip", flags = ConVarFlags.ExecuteOnServer, helpText = "添加怪物主动装备 [物品名称code]")]
        public static void Command_AddMonsterEquiment(ConCommandArgs args)
        {
            string equitString = args.TryGetArgString(0);
            AddMonsterTeamEquit(equitString);
        }
        [ConCommand(commandName = "monster_removeitem", flags = ConVarFlags.ExecuteOnServer, helpText = "移除怪物物品 [物品名称code 数量]")]
        public static void Command_RemoveMonsterItems(ConCommandArgs args)
        {
            string itemString = args.TryGetArgString(0);
            if (args.Count >= 2)
            {
                int? itemCount = args.TryGetArgInt(1);
                if (itemCount.HasValue)
                {
                    RemoveMonsterTeamItem(itemString, itemCount.Value);
                    return;
                }
            }
            RemoveMonsterTeamItem(itemString);
        }
        [ConCommand(commandName = "monster_resetitem", flags = ConVarFlags.ExecuteOnServer, helpText = "重置怪物物品 [物品名称code]")]
        public static void Command_ResetMonsterItems(ConCommandArgs args)
        {
            string itemString = args.TryGetArgString(0);
            if (args.Count >= 1)
            {
                ResetMonsterTeamItem(itemString);
            }

        }
        [ConCommand(commandName = "show_allitem", flags = ConVarFlags.ExecuteOnServer, helpText = "显示all")]
        public static void Command_ShowAll(ConCommandArgs args)
        {
            foreach (ItemDef item in ItemController_Instance.ItemAll)
            {
                ChatHelper.Send($"name = {item.name}, tier = {item.tier}, index = {item.itemIndex}, order = {ItemController_Instance.GetItemOrder(item)}");
            }

        }
        [ConCommand(commandName = "show_allitem_ban", flags = ConVarFlags.ExecuteOnServer, helpText = "显示all_ban")]
        public static void Command_ShowAll_Ban(ConCommandArgs args)
        {
            foreach (ItemDef item in ItemController_Instance.ItemAll_Ban)
            {
                ChatHelper.Send($"name = {item.name}, tier = {item.tier}, index = {item.itemIndex}, order = {ItemController_Instance.GetItemOrder(item)}");
            }

        }
        // 添加物品
        public static void AddMonsterTeamItem(string itemName, int count = 1)
        {
            ItemIndex itemIndex = ItemCatalog.FindItemIndex(itemName);
            if (itemIndex != ItemIndex.None)
            {
                AddMonsterTeamItem(itemIndex, count);
            }
            else
            {
                Debug.LogError("找不到物品，添加怪物物品失败");
            }
        }
        public static void AddMonsterTeamItem(ItemIndex itemIndex, int count = 1)
        {
            if (itemIndex != ItemIndex.None)
            {
                MonsterTeamGainsItemsArtifactManager.monsterTeamInventory.GiveItem(itemIndex, count);
            }
            else
            {
                Debug.LogError("找不到物品，添加怪物物品失败");
            }

        }
        // 移除物品
        public static void RemoveMonsterTeamItem(string itemName, int count = 1)
        {
            ItemIndex itemIndex = ItemCatalog.FindItemIndex(itemName);
            if (itemIndex != ItemIndex.None)
            {
                RemoveMonsterTeamItem(itemIndex, count);
            }
        }
        public static void RemoveMonsterTeamItem(ItemIndex itemIndex, int count = 1)
        {
            if (itemIndex != ItemIndex.None)
            {
                MonsterTeamGainsItemsArtifactManager.monsterTeamInventory.RemoveItem(itemIndex, count);
            }
        }
        // 重设物品
        public static void ResetMonsterTeamItem(string itemName)
        {
            ItemIndex itemIndex = ItemCatalog.FindItemIndex(itemName);
            if (itemIndex != ItemIndex.None)
            {
                ResetMonsterTeamItem(itemIndex);
            }
        }
        public static void ResetMonsterTeamItem(ItemIndex itemIndex)
        {
            if (itemIndex != ItemIndex.None)
            {
                MonsterTeamGainsItemsArtifactManager.monsterTeamInventory.ResetItem(itemIndex);
            }
        }
        // 添加主动装备
        public static void AddMonsterTeamEquit(string equitName)
        {
            EquipmentIndex equitIndex = EquipmentCatalog.FindEquipmentIndex(equitName);
            if (equitIndex != EquipmentIndex.None)
            {
                //MonsterTeamGainsItemsArtifactManager.monsterTeamInventory.GiveEquipmentString(equitName);
            }
            else
            {
                Debug.LogError("找不到物品，添加怪物主动装备失败");
            }
        }
        #endregion
        public static void SHowMonsterItemInfoByStage()
        {
            if (ModConfig.EnableMessage.Value)
            {
                if (!(CurrentMonsterItems is null) && CurrentMonsterItems.Count > 0)
                {
                    instance.Logger.LogDebug($"CurrentMonsterItems.Count = {CurrentMonsterItems.Count}");
                    string info = "<style=cUserSetting>======= 怪物获得新物品 =======</style>\n";
                    if (!ModConfig.GetIsCN())
                    {
                        info = "<style=cUserSetting>======= Monster New Items =======</style>\n";
                    }
                    
                    //CurrentMonsterItems = (from t in CurrentMonsterItems orderby t.Order descending, t.Count descending select t).ToList();
                    var qTable = from t in CurrentMonsterItems
                                 group t by new { t.Name, t.Order }
                                 into y
                                 select new
                                 {
                                     y.Key.Order,
                                     y.Key.Name,
                                     Count = y.Sum(x => x.Count)
                                 };
                    //qTable = qTable.ToList().OrderByDescending(x => x.Order);
                    qTable = (from t in qTable orderby t.Order descending, t.Count descending select t);
                    foreach (var monsterItem in qTable)
                    {
                        instance.Logger.LogDebug($"{monsterItem.Name} = {monsterItem.Count}, order{monsterItem.Order}");
                        ItemDef item = ItemController_Instance.ItemAll.FirstOrDefault(x => x.name == monsterItem.Name);
                        string color = "";
                        if (monsterItem.Count > 0)
                            color = $"<color=red>{monsterItem.Count}</color>";
                        else
                            color = $"<color=green>{monsterItem.Count}</color>";
                        //ChatHelper.Send(" tier = " + item.ItemTier);
                        if (!(item is null)) info += $"<style={ItemTierColor(item.tier)}>{NameToLocal(item.name)}</style>{color}, ";
                    }
                    info = info.Substring(0, info.Length - 2);
                    ChatHelper.Send(info);
                }
            }
        }
        public static string ItemTierColor(ItemTier tier)
        {
            switch (tier)
            {
                case ItemTier.Tier1:
                    return "cSub";
                case ItemTier.Tier2:
                    return "cIsHealing";
                case ItemTier.Tier3:
                    return "cDeath";
                case ItemTier.Lunar:
                    return "cIsUtility";
                case ItemTier.Boss:
                    return "cShrine";
                case ItemTier.VoidTier1:
                case ItemTier.VoidTier2:
                case ItemTier.VoidTier3:
                case ItemTier.VoidBoss:
                    return "cWorldEvent";
                default:
                    return "cKeywordName";
            }
        }
        public static void ShowMonsterItemTotalInfo()
        {
            foreach (ItemIndex itemIndex in MonsterTeamGainsItemsArtifactManager.monsterTeamInventory.itemAcquisitionOrder)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                string name = Language.GetString(itemDef.nameToken);
                int count = MonsterTeamGainsItemsArtifactManager.monsterTeamInventory.GetItemCount(itemIndex);
                ChatHelper.Send($"{name} = {count}");
            }
        }
        interface MonsterItemInterface {}
        internal class MonsterItemConcreteStruct : MonsterItemInterface
        {
            public ItemDef ItemDef;
            public string Name
            {
                get { return ItemDef.name; }
            }
            public int Count;
            public int Order
            {
                get { return ItemController_Instance.GetItemOrder(ItemDef); }
            }

            public MonsterItemConcreteStruct(ItemDef itemClass, int count)
            {
                Count = count;
                ItemDef = itemClass;
            }
        }
        internal class MonsterItemClassStruct : MonsterItemInterface
        {
            public string Name;
            public int PoolRange;
            public int Count;
            // public int Order { get { return ItemController_Instance.GetItemOrder(ItemController_Instance.GetPool(Name).FirstOrDefault()); } }

            public MonsterItemClassStruct(string name, int poolrange, int count)
            {
                Name = name;
                PoolRange = poolrange;
                Count = count;
            }
        }
    }
}
