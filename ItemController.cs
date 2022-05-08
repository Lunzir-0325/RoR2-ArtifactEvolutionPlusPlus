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
        public List<ItemStruct> ItemTier1 = new List<ItemStruct>();
        public List<ItemStruct> ItemTier2 = new List<ItemStruct>();
        public List<ItemStruct> ItemTier3 = new List<ItemStruct>();
        public List<ItemStruct> ItemBoss = new List<ItemStruct>();
        public List<ItemStruct> ItemVoidTier = new List<ItemStruct>();
        public List<ItemStruct> ItemLunar = new List<ItemStruct>();
        public List<ItemStruct> ItemNoTier = new List<ItemStruct>();
        public List<ItemStruct> ItemAll_Ban = new List<ItemStruct>();
        public List<ItemStruct> ItemAll = new List<ItemStruct>();
        //public static ItemController Instance { get; set; }
        //public static List<ItemStruct> ItemCountLimitListAndWeight = new List<ItemStruct>();

        public ItemController()
        {
            foreach (ItemDef itemDef in ItemCatalog.allItemDefs)
            {
                switch (itemDef.tier)
                {
                    case ItemTier.Tier1:
                        ItemTier1.Add(new ItemStruct(itemDef.name, itemDef.tier, 0, -1, 1, 1, itemDef.itemIndex));
                        continue;
                    case ItemTier.Tier2:
                        ItemTier2.Add(new ItemStruct(itemDef.name, itemDef.tier, 0, -1, 1, 2, itemDef.itemIndex));
                        continue;
                    case ItemTier.Tier3:
                        ItemTier3.Add(new ItemStruct(itemDef.name, itemDef.tier, 0, -1, 1, 3, itemDef.itemIndex));
                        continue;
                    case ItemTier.Boss:
                        ItemBoss.Add(new ItemStruct(itemDef.name, itemDef.tier, 0, -1, 1, 4, itemDef.itemIndex));
                        continue;
                    case ItemTier.Lunar:
                        ItemLunar.Add(new ItemStruct(itemDef.name, itemDef.tier, 0, -1, 1, 5, itemDef.itemIndex));
                        continue;
                    case ItemTier.VoidTier1:
                    case ItemTier.VoidTier2:
                    case ItemTier.VoidTier3:
                    case ItemTier.VoidBoss:
                        ItemVoidTier.Add(new ItemStruct(itemDef.name, itemDef.tier, 0, -1, 1, 6, itemDef.itemIndex));
                        continue;
                    case ItemTier.NoTier:
                        ItemNoTier.Add(new ItemStruct(itemDef.name, itemDef.tier, 0, -1, 1, 7, itemDef.itemIndex));
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
        

        public void AddBanItem()
        {
            string[] banCodes = ModConfig.ItemTier1Banlist.Value.Split(',');
            for (int i = 0; i < banCodes.Length; i++)
            {
                ItemTier1.Remove(ItemTier1.AsEnumerable().FirstOrDefault(item => item.Name.ToLower() == banCodes[i].Trim().ToLower()));
            }

            banCodes = ModConfig.ItemTier2Banlist.Value.Split(',');
            for (int i = 0; i < banCodes.Length; i++)
            {
                ItemTier2.Remove(ItemTier2.AsEnumerable().FirstOrDefault(item => item.Name.ToLower() == banCodes[i].Trim().ToLower()));
            }

            banCodes = ModConfig.ItemTier3Banlist.Value.Split(',');
            for (int i = 0; i < banCodes.Length; i++)
            {
                ItemTier3.Remove(ItemTier3.AsEnumerable().FirstOrDefault(item => item.Name.ToLower() == banCodes[i].Trim().ToLower()));
            }

            banCodes = ModConfig.ItemBossBanlist.Value.Split(',');
            for (int i = 0; i < banCodes.Length; i++)
            {
                ItemBoss.Remove(ItemBoss.AsEnumerable().FirstOrDefault(item => item.Name.ToLower() == banCodes[i].Trim().ToLower()));
            }

            banCodes = ModConfig.ItemVoidTierBanlist.Value.Split(',');
            for (int i = 0; i < banCodes.Length; i++)
            {
                ItemVoidTier.Remove(ItemVoidTier.AsEnumerable().FirstOrDefault(item => item.Name.ToLower() == banCodes[i].Trim().ToLower()));
            }
            
            banCodes = ModConfig.ItemLunarBanlist.Value.Split(',');
            for (int i = 0; i < banCodes.Length; i++)
            {
                ItemLunar.Remove(ItemLunar.AsEnumerable().FirstOrDefault(item => item.Name.ToLower() == banCodes[i].Trim().ToLower()));
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
        //            ItemCountLimitListAndWeight.Add(new ItemStruct(name, limit, weight));
        //        }
        //        else if (vsSplit.Count() == 3)
        //        {
        //            weight = double.Parse(vsSplit[2]);
        //            ItemCountLimitListAndWeight.Add(new ItemStruct(name, limit, weight));
        //        }
        //    }
        //    foreach (ItemStruct item in ItemCountLimitListAndWeight)
        //    {
        //        ItemStruct tempItemClass = ItemAll.AsEnumerable().FirstOrDefault(t => t.Name.Equals(item.Name));
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
        //private void AddLimitAndWeight_Handle(List<ItemStruct> listitemClass, ItemStruct currentItem, ItemStruct tempItemClass)
        //{
        //    ItemStruct oldItemClass = listitemClass.AsEnumerable().FirstOrDefault(t => t.Name.Equals(tempItemClass.Name));
        //    ItemStruct newItemClass = new ItemStruct()
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
            ItemTier1 = ItemTier1.OrderBy(t => t.Name).ToList();
            ItemTier2 = ItemTier2.OrderBy(t => t.Name).ToList();
            ItemTier3 = ItemTier3.OrderBy(t => t.Name).ToList();
            ItemBoss = ItemBoss.OrderBy(t => t.Name).ToList();
            ItemVoidTier = ItemVoidTier.OrderBy(t => t.Name).ToList();
            ItemLunar = ItemLunar.OrderBy(t => t.Name).ToList();

            ItemAll_Ban.Clear();
            ItemAll_Ban.AddRange(ItemTier1);
            ItemAll_Ban.AddRange(ItemTier2);
            ItemAll_Ban.AddRange(ItemTier3);
            ItemAll_Ban.AddRange(ItemBoss);
            ItemAll_Ban.AddRange(ItemVoidTier);
            ItemAll_Ban.AddRange(ItemLunar);
        }
    }
    public class ItemStruct
    {
        public ItemIndex ItemIndex;
        public string Name;
        public ItemTier ItemTier;
        public int Serveravailble;
        public int Limit;
        public double Weight;
        public int Order;

        public ItemStruct() { }

        public ItemStruct(ItemIndex index, string name, ItemTier itemTier, int serveravailble)
        {
            ItemIndex = index;
            Name = name;
            ItemTier = itemTier;
            Serveravailble = serveravailble;
        }

        public ItemStruct(string name, ItemTier itemTier, int serveravailble)
        {
            Name = name;
            ItemTier = itemTier;
            Serveravailble = serveravailble;
        }

        public ItemStruct(string name, int limit, double weight)
        {
            Name = name;
            Limit = limit;
            Weight = weight;
        }

        public ItemStruct(string name, ItemTier itemTier, int serveravailble, ItemIndex index, int limit, double weight)
        {
            ItemIndex = index;
            Name = name;
            ItemTier = itemTier;
            Serveravailble = serveravailble;
            Limit = limit;
            Weight = weight;
        }

        public ItemStruct(string name, ItemTier itemTier, int serveravailble, int limit, double weight, int order, ItemIndex itemIndex) : this(name, itemTier, serveravailble)
        {
            Limit = limit;
            Weight = weight;
            Order = order;
        }
    }

}
