using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;

namespace ArtifactEvolutionPlusPlus
{
    class ModConfig
    {
        private static string Section1 = "0 Setting 设置";
        private static string Section3 = "1 Ban List 禁用列表";
        private static string Section2 = "2 Monster Stage Items 关卡物品";
        //private static string Section4 = "3 自定义设置";

        public static ConfigEntry<bool> EnableMod;

        public static ConfigEntry<bool> EnableMessage;
        public static ConfigEntry<bool> ItemPoolRandomStyle;

        //public static ConfigEntry<string> TierCountRange;

        //public static ConfigEntry<string> ItemTierPool;

        //public static ConfigEntry<string> ItemTierExtraPool;
        public static ConfigEntry<string> ItemTier1Banlist;
        public static ConfigEntry<string> ItemTier2Banlist;
        public static ConfigEntry<string> ItemTier3Banlist;
        public static ConfigEntry<string> ItemBossBanlist;
        public static ConfigEntry<string> ItemVoidTierBanlist;
        public static ConfigEntry<string> ItemLunarBanlist;
        //public static ConfigEntry<string> ItemCountLimitListAndWeight;
        
        public static ConfigEntry<int> StageCustomNumber;
        public static ConfigEntry<string>[] StageCustomItemList;
        //public static ConfigEntry<int> AfterStageCustomNumber;
        //public static ConfigEntry<string> AfterStageCustomItemList;
        //public static ConfigEntry<bool> EnableBaseOfGrowth;
        //public static ConfigEntry<int> ItemCountOffset;
        //public static ConfigEntry<int> ItemMuitipleCount;

        //public static ConfigEntry<string> GameItemList;
        //public static ConfigEntry<string> GameEquitList;
        public static void InitConfig(ConfigFile config)
        {
            // 1 全局设置
            EnableMod = config.Bind(Section1, "Enabled", true, "Enable the mod. The configuration file will reload each time after you start a new run.\nfalse = back to game's original logic\n启用模组，每次开局会加载一次配置文件，所以可以在开局前设置好。\n" +
                "游戏原版机制：1、2关卡默认1白装，3、4关卡默认1绿装，5关卡默认1红装");
            if (EnableMod.Value)
            {
                ItemPoolRandomStyle = config.Bind(Section1, "ItemPoolRandomStyle", true, "Set item pool formula mode.\n" +
                    "Values Range: true = Item quantity random and repetitive, drawing the same two items from the pool will add up, false = Random and not repetitive.\n" +
                    "设置物品池抽取方式\n" +
                    "取值范围：ture = 随机而且重复，false = 随机并且不重复\n" +
                    "说明：如果为true随机而且重复，从物品池抽到同样两个物品会叠加数量");

                EnableMessage = config.Bind(Section1, "EnableMessage", true, "If Enable display information in chat box each level what new items do monster get.\n启用每关怪物获得物品信息，在聊天框显示");

                // 2 禁用列表
                ItemTier1Banlist = config.Bind(Section3, "ItemTier1Banlist",
                    "BarrierOnKill,Bear,BossDamageBonus,Firework,GoldOnHurt,Hoof,Medkit,Mushroom,ScrapWhite,Tooth,TreasureCache,WardOnLevel",
                    "White(tier1) items ban list. All banned items only affect keywords.\n白装禁用列表，所有禁用列表只会影响关键词\nItem Code物品代码, https://gist.github.com/Lunzir-0325/8f375c6504a64f6c88f35259470659ee");
                ItemTier2Banlist = config.Bind(Section3, "ItemTier2Banlist",
                    "Bandolier,BonusGoldPackOnKill,ChainLightning,EnergizedOnEquipmentUse,ExecuteLowHealthElite,ExplodeOnDeath,FreeChest,Infusion,JumpBoost,Missile,MoveSpeedOnKill,PrimarySkillShuriken,RegeneratingScrap,ScrapGreen,Squid,StrengthenBurn,TPHealingNova,Thorns",
                    "Green(tier2) items ban list.\n绿装禁用列表");
                ItemTier3Banlist = config.Bind(Section3, "ItemTier3Banlist",
                    "Behemoth,BounceNearby,CaptainDefenseMatrix,Dagger,DroneWeapons,ExtraLife,FallBoots,GhostOnKill,HeadHunter,Icicle,ImmuneToDebuff,KillEliteFrenzy,LaserTurbine,MoreMissile,NovaOnHeal,PermanentDebuffOnHit,Plant,RandomEquipmentTrigger,ScrapRed,ShockNearby,Talisman",
                    "Red(tier3) items ban list.\n红装禁用列表");
                ItemBossBanlist = config.Bind(Section3, "ItemBossBanlist",
                    "ArtifactKey,BeetleGland,BleedOnHitAndExplode,FireballsOnHit,LightningStrikeOnHit,MinorConstructOnKill,RoboBallBuddy,ScrapYellow,SiphonOnLowHealth,SprintWisp,TitanGoldDuringTP",
                    "Yellow(boss) items ban list.\n黄装禁用列表");
                ItemLunarBanlist = config.Bind(Section3, "ItemLunarBanlist",
                    "AutoCastEquipment,FocusConvergence,GoldOnHit,LunarDagger,LunarPrimaryReplacement,LunarSecondaryReplacement,LunarSpecialReplacement,LunarTrinket,LunarUtilityReplacement,MonstersOnShrineUse,RandomDamageZone,RandomlyLunar,RepeatHeal,ShieldOnly",
                    "Blue(Lunar) items ban list.\n蓝装禁用列表");
                ItemVoidTierBanlist = config.Bind(Section3, "ItemVoidTierBanlist",
                    "VoidMegaCrabItem,CritGlassesVoid,TreasureCacheVoid,ChainLightningVoid,ElementalRingVoid,ExplodeOnDeathVoid,MissileVoid,SlowOnHitVoid,CloverVoid,ExtraLifeVoid",
                    "Purple(Void) items ban list.\n紫装禁用列表");

                // 关卡设置
                StageCustomNumber = config.Bind(Section2, "StageCustomNumber", 20, "Customize the number of items in the stage.\n" +
                    "The first thing to do is start the game, and the corresponding number of stages will be generated.\n" +
                    "Usage: [ ItemCode&Count ] or [ KeyWord&PoolRange(&Count) ]\n" +
                    "KeyWord: AllRondom, AllWhite(AllTier1), AllGreen(AllTier2), AllRed(AllTier3), AllYellow(AllBoss), AllBlue(AllLunar), AllPurple(AllVoid)\n" +
                    "Example1: Bear&3 means monsters wiil get 3 bears.\n" +
                    "Example2: AllWhite&3&2 means monsters get 3 random items of 2 each from the white(tier1) channel.\n" +
                    "Example3: AllGreen&4 means monsters get 4 random items from the Green(tier12) channel, each count default is 1, same as AllGreen&4&1\n" +
                    "Example4: AllRandom&10 means Monsters will take 10 items from all item pools, count default is 1.\n" +
                    "Values Range: Positive numbers is the item acquired, negative numbers is the item removed.\n" +
                    "自定义关卡物品数量\n" +
                   "使用说明：先启动一次游戏，自动生成相应关数\n" +
                   "添加例子：[ 物品名称&物品数量 ] 或 [ 关键词&池数值(&物品数量) ]\n" +
                   "关键词这些： AllRondom, AllWhite(AllTier1), AllGreen(AllTier2), AllRed(AllTier3), AllYellow(AllBoss), AllBlue(AllLunar), AllPurple(AllVoid)\n" +
                   "如Bear&3，物品为小熊，数量为3个\n" +
                   "如AllWhite&5&2，从所有白色物品池中随机取5个物品，数量为2\n" +
                   "如AllWhite&4，从所有白色物品池中随机取4个物品，默认数量为1，也可以写成AllWhite&4&1意思一样\n" +
                   "如AllRondom&10，从所有物品池取10个物品，默认数量为1\n" +
                   "取值范围：0至无限, 0 = 不给物品；负数为减少物品");
                if (StageCustomNumber.Value == 0)
                {
                    StageCustomNumber.Value = 1;
                }
                StageCustomItemList = new ConfigEntry<string>[StageCustomNumber.Value];
                int mod = 5;
                for (int i = 0; i < StageCustomItemList.Length; i++)
                {
                    if (i % mod == 0)
                    {
                        StageCustomItemList[i] = config.Bind(Section2, "StageCustomItemList_" + (i + 1), "Pearl&10, AllWhite&1, HealingPotion&1, LunarDagger&1", $"Stage-{i + 1}\n关卡-{i + 1}");
                    }
                    else if (i % mod == 1)
                    {
                        StageCustomItemList[i] = config.Bind(Section2, "StageCustomItemList_" + (i + 1), "Pearl&10, AllTier1&1, OutOfCombatArmor&1, LunarDagger&-1", $"Stage-{i + 1}\n关卡-{i + 1}");
                    }
                    else if (i % mod == 2)
                    {
                        StageCustomItemList[i] = config.Bind(Section2, "StageCustomItemList_" + (i + 1), "Pearl&10, AllGreen&10, Phasing&1", $"Stage-{i + 1}\n关卡-{i + 1}");
                    }
                    else if (i % mod == 3)
                    {
                        StageCustomItemList[i] = config.Bind(Section2, "StageCustomItemList_" + (i + 1), "Pearl&10, AllTier2&-10, Seed&1", $"Stage-{i + 1}\n关卡-{i + 1}");
                    }
                    else if (i % mod == 4)
                    {
                        StageCustomItemList[i] = config.Bind(Section2, "StageCustomItemList_" + (i + 1), "Pearl&10, AllRed&1&2, AllBossw&1, AllVoid&1", $"Stage-{i + 1}\n关卡-{i + 1}");
                    }
                }

                


                //ItemCountLimitListAndWeight = config.Bind(Section3, "ItemCountLimitListAndWeight",
                //    "ExtraLife&1, ShockNearby&1&0.1",
                //    "【未完成】设置物品上限和和概率\n" +
                //    "使用格式：物品代码&数量上限&[生成概率%]" +
                //    "例子：复活熊ExtraLife&1，封顶只有1个" +
                //    "例子：特斯拉ShockNearby&1&0.1，封顶只有1个，同时生成的概率为10%");

                //AfterStageCustomNumber = config.Bind("4 自定义设置", "AfterStageCustomNumber", 21, "从第几关开始，设置额外物品可，可以与上面自定义关卡并列影响，此选项关联下面选项\n" +
                //    "取值范围：-1 = 不开启，下面选项都会失效");
                //AfterStageCustomItemList = config.Bind("4 自定义设置", "AfterFifthStageCustomItemList", "Pearl&1, AllRondom&4&1", "关卡5之后，物品设置\n" +
                //    "说明：从第AfterStageCustomNumber关开始，一直保持这个选项增加相应物品和数量");
                //EnableBaseOfGrowth = config.Bind("4 自定义设置", "EnableBaseOfGrowth", true, "启用物品数量高级计算，以游戏关卡数作为递增\n" +
                //    "计算公式：基础物品数量 + (当前关卡 x 倍数 + 偏差值)\n" +
                //    "比如：Pearl&1，第7关，偏差值=0, 倍数=1，带入公式 1 + (7 x 1 + 0) = 8，第七关会添加8个珍珠给怪物\n" +
                //    "取值范围：true = 开启， false = 不开启");
                //ItemCountOffset = config.Bind("4 自定义设置", "ItemCountOffset", 0, "偏差值");
                //ItemMuitipleCount = config.Bind("4 自定义设置", "ItemMuitipleCount", 1, "倍数");

            }
            if (ModCompatibilityInLobbyConfig.enabled)
            {
                ModCompatibilityInLobbyConfig.CreateFromBepInExConfigFile(config, "Artifact Evolution++");
            }
        }

        public static bool GetIsCN()
        {
            string value = RoR2.Language.currentLanguageName;
            bool isCN = false;
            if (value == "zh-CN")
            {
                isCN = true;
            }
            return isCN;
        }
    }
}
