using RoR2;
using RoR2.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArtifactEvolutionPlusPlus
{
    public class ItemController
    {
        public List<ItemDef> ItemTier1 = new List<ItemDef>();
        public List<ItemDef> ItemTier2 = new List<ItemDef>();
        public List<ItemDef> ItemTier3 = new List<ItemDef>();
        public List<ItemDef> ItemBoss = new List<ItemDef>();
        public List<ItemDef> ItemVoidTier = new List<ItemDef>();
        public List<ItemDef> ItemLunar = new List<ItemDef>();
        public List<ItemDef> ItemNoTier = new List<ItemDef>();
        public List<ItemDef> ItemAll_Ban = new List<ItemDef>();
        public List<ItemDef> ItemAll = new List<ItemDef>();
        //public static ItemController Instance { get; set; }
        //public static List<ItemDef> ItemCountLimitListAndWeight = new List<ItemDef>();

        public ItemController()
        {
            foreach (ItemDef itemDef in ItemCatalog.allItemDefs)
            {
                switch (itemDef.tier)
                {
                    case ItemTier.Tier1:
                        ItemTier1.Add(itemDef);
                        continue;
                    case ItemTier.Tier2:
                        ItemTier2.Add(itemDef);
                        continue;
                    case ItemTier.Tier3:
                        ItemTier3.Add(itemDef);
                        continue;
                    case ItemTier.Boss:
                        ItemBoss.Add(itemDef);
                        continue;
                    case ItemTier.Lunar:
                        ItemLunar.Add(itemDef);
                        continue;
                    case ItemTier.VoidTier1:
                    case ItemTier.VoidTier2:
                    case ItemTier.VoidTier3:
                    case ItemTier.VoidBoss:
                        ItemVoidTier.Add(itemDef);
                        continue;
                    case ItemTier.NoTier:
                        ItemNoTier.Add(itemDef);
                        continue;
                }
            }

            ItemAll.AddRange(ItemTier1);
            ItemAll.AddRange(ItemTier2);
            ItemAll.AddRange(ItemTier3);
            ItemAll.AddRange(ItemBoss);
            ItemAll.AddRange(ItemLunar);
            ItemAll.AddRange(ItemVoidTier);

            //UpdateAllItemClass();
            //AddLimitAndWeight();
            AddBanItem();
            UpdateAllItem();
        }

        public int GetItemOrder(ItemDef itemDef)
        {
            switch (itemDef.tier)
            {
                case ItemTier.Tier1:
                    return 1;
                case ItemTier.Tier2:
                    return 2;
                case ItemTier.Tier3:
                    return 3;
                case ItemTier.Boss:
                    return 4;
                case ItemTier.Lunar:
                    return 5;
                case ItemTier.VoidTier1:
                case ItemTier.VoidTier2:
                case ItemTier.VoidTier3:
                case ItemTier.VoidBoss:
                    return 6;
                case ItemTier.NoTier:
                    return 7;
                default:
                    return -1;
            }
        }

        public void AddBanItem()
        {
            string[] banCodes = ModConfig.ItemTier1Banlist.Value.Split(',');
            for (int i = 0; i < banCodes.Length; i++)
            {
                ItemTier1.Remove(ItemTier1.AsEnumerable().FirstOrDefault(item => item.name.ToLower() == banCodes[i].Trim().ToLower()));
            }

            banCodes = ModConfig.ItemTier2Banlist.Value.Split(',');
            for (int i = 0; i < banCodes.Length; i++)
            {
                ItemTier2.Remove(ItemTier2.AsEnumerable().FirstOrDefault(item => item.name.ToLower() == banCodes[i].Trim().ToLower()));
            }

            banCodes = ModConfig.ItemTier3Banlist.Value.Split(',');
            for (int i = 0; i < banCodes.Length; i++)
            {
                ItemTier3.Remove(ItemTier3.AsEnumerable().FirstOrDefault(item => item.name.ToLower() == banCodes[i].Trim().ToLower()));
            }

            banCodes = ModConfig.ItemBossBanlist.Value.Split(',');
            for (int i = 0; i < banCodes.Length; i++)
            {
                ItemBoss.Remove(ItemBoss.AsEnumerable().FirstOrDefault(item => item.name.ToLower() == banCodes[i].Trim().ToLower()));
            }

            banCodes = ModConfig.ItemVoidTierBanlist.Value.Split(',');
            for (int i = 0; i < banCodes.Length; i++)
            {
                ItemVoidTier.Remove(ItemVoidTier.AsEnumerable().FirstOrDefault(item => item.name.ToLower() == banCodes[i].Trim().ToLower()));
            }
            
            banCodes = ModConfig.ItemLunarBanlist.Value.Split(',');
            for (int i = 0; i < banCodes.Length; i++)
            {
                ItemLunar.Remove(ItemLunar.AsEnumerable().FirstOrDefault(item => item.name.ToLower() == banCodes[i].Trim().ToLower()));
            }
        }
        //public void AddLimitAndWeight()
        //{
        //    string[] vsItem = ModConfig.ItemCountLimitListAndWeight.Value.Split(',');
        //    string[] vsSplit = null;
        //    for (int i = 0; i < vsItem.Length; i++)
        //    {
        //        vsSplit = vsItem[i].Trim().Split('&');
        //        string name = vsSplit[0];
        //        int limit = int.Parse(vsSplit[1]);
        //        double weight = 1;
        //        if (vsSplit.Count() == 2)
        //        {
        //            ItemCountLimitListAndWeight.Add(new ItemDef(name, limit, weight));
        //        }
        //        else if (vsSplit.Count() == 3)
        //        {
        //            weight = double.Parse(vsSplit[2]);
        //            ItemCountLimitListAndWeight.Add(new ItemDef(name, limit, weight));
        //        }
        //    }
        //    foreach (ItemDef item in ItemCountLimitListAndWeight)
        //    {
        //        ItemDef tempItemClass = ItemAll.AsEnumerable().FirstOrDefault(t => t.Name.Equals(item.Name));
        //        if (tempItemClass.ItemTier == ItemTier.Tier1)
        //        {
        //            AddLimitAndWeight_Handle(ItemTier1, item, tempItemClass);
        //        }
        //        else if (tempItemClass.ItemTier == ItemTier.Tier2)
        //        {
        //            AddLimitAndWeight_Handle(ItemTier2, item, tempItemClass);
        //        }
        //        else if (tempItemClass.ItemTier == ItemTier.Tier3)
        //        {
        //            AddLimitAndWeight_Handle(ItemTier3, item, tempItemClass);
        //        }
        //        else if (tempItemClass.ItemTier == ItemTier.Boss)
        //        {
        //            AddLimitAndWeight_Handle(ItemBoss, item, tempItemClass);
        //        }
        //        else if (tempItemClass.ItemTier == ItemTier.VoidTier1
        //            || tempItemClass.ItemTier == ItemTier.VoidTier2
        //            || tempItemClass.ItemTier == ItemTier.VoidTier3
        //            || tempItemClass.ItemTier == ItemTier.VoidBoss)
        //        {
        //            AddLimitAndWeight_Handle(ItemVoidTier, item, tempItemClass);
        //        }
        //        else if (tempItemClass.ItemTier == ItemTier.Lunar)
        //        {
        //            AddLimitAndWeight_Handle(ItemLunar, item, tempItemClass);
        //        }
        //    }
        //}
        //private void AddLimitAndWeight_Handle(List<ItemDef> listitemClass, ItemDef currentItem, ItemDef tempItemClass)
        //{
        //    ItemDef oldItemClass = listitemClass.AsEnumerable().FirstOrDefault(t => t.Name.Equals(tempItemClass.Name));
        //    ItemDef newItemClass = new ItemDef()
        //    {
        //        Name = oldItemClass.Name,
        //        ItemTier = oldItemClass.ItemTier,
        //        Serveravailble = oldItemClass.Serveravailble,
        //        Limit = currentItem.Limit,
        //        Weight = currentItem.Weight
        //    };
        //    listitemClass.Remove(oldItemClass);
        //    listitemClass.Add(newItemClass);
        //}

        private void UpdateAllItem()
        {
            ItemTier1 = ItemTier1.OrderBy(t => t.name).ToList();
            ItemTier2 = ItemTier2.OrderBy(t => t.name).ToList();
            ItemTier3 = ItemTier3.OrderBy(t => t.name).ToList();
            ItemBoss = ItemBoss.OrderBy(t => t.name).ToList();
            ItemVoidTier = ItemVoidTier.OrderBy(t => t.name).ToList();
            ItemLunar = ItemLunar.OrderBy(t => t.name).ToList();

            ItemAll_Ban.Clear();
            ItemAll_Ban.AddRange(ItemTier1);
            ItemAll_Ban.AddRange(ItemTier2);
            ItemAll_Ban.AddRange(ItemTier3);
            ItemAll_Ban.AddRange(ItemBoss);
            ItemAll_Ban.AddRange(ItemVoidTier);
            ItemAll_Ban.AddRange(ItemLunar);
        }
    }
}
