using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Overlay;
using System.IO;
using System.Xml.Serialization;
using TaleWorlds.SaveSystem;

namespace BasiliskTroops
{
    public class BasiliskTroops : CampaignBehaviorBase
    {

        Dictionary<string, TroopProperties> troopDic = new Dictionary<string, TroopProperties>();


        public static string[] militiaTroopIDs = new string[]
        {
            "mod_basilisk_trainee",
            "mod_basilisk_militia",
            "mod_basilisk_light_infantry",
            "mod_basilisk_heavy_infantry",
            "mod_basilisk_slayer",
            "mod_basilisk_spearman",
            "mod_basilisk_armored_spearman",
            "mod_basilisk_skirmisher",
            "mod_basilisk_archer",
            "mod_basilisk_ranger",
            "mod_basilisk_light_cavalry",
            "mod_basilisk_vanguard"
        };

        public static string[] nobleTroopIDs = new string[]
        {
            "mod_basilisk_nobleman",
            "mod_basilisk_squire",
            "mod_basilisk_knight",
            "mod_basilisk_master",
            "mod_basilisk_grandmaster"
        };
        
        private void OnSessionLaunched(CampaignGameStarter obj)
        {
            populateGuilds();
            AddTroopMenu(obj);
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, this.trackWeekly);
        }

        public void AddTroopMenu(CampaignGameStarter obj)
        {

            obj.AddGameMenuOption("town", "info_troop_type", "Hire Basilisk Guild Troops", game_menu_just_add_recruit_conditional, (MenuCallbackArgs args) => { GameMenu.SwitchToMenu("town_mod_pay"); }, false, 5);
            
            obj.AddGameMenu("town_mod_pay", "The Basilisk Guild offers its mercenaries, both commoners and nobles, in every town for quite the coin. The guild leader tells you that their mercenaries travel among their locations weekly." +
                " She also tells you that there is a recurring upfront fee of {COST} denars here just to view their mercenaries. It is however the only fee besides upkeep cost you pay for. You do not need to pay per merc.",
                (MenuCallbackArgs args) => 
                {
                    TroopProperties troopProps;
                    troopDic.TryGetValue(Settlement.CurrentSettlement.StringId, out troopProps);
                    MBTextManager.SetTextVariable("COST", troopProps.getCost, false);
                });

            obj.AddGameMenu("town_mod_troop_type", "The guild leader shows you their list of mercenaries and ask which you want. She will send the ones you paid for to wait by the gates for when you leave town.", null);

            obj.AddGameMenuOption("town_mod_pay", "pay_fee", "Pay {COST} denars", 
                (MenuCallbackArgs args) => 
                {
                    TroopProperties troopProps;
                    troopDic.TryGetValue(Settlement.CurrentSettlement.StringId, out troopProps);
                    MBTextManager.SetTextVariable("COST", troopProps.getCost, false);
                    args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                    if (troopProps.getCost >= Hero.MainHero.Gold)
                    {
                        return false;
                    }
                    return true;
                }, 
                (MenuCallbackArgs args) => 
                {
                    TroopProperties troopProps;
                    troopDic.TryGetValue(Settlement.CurrentSettlement.StringId, out troopProps);
                    int cost = troopProps.getCost;
                    if (cost <= Hero.MainHero.Gold) 
                    {
                        GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, cost);
                    }
                    GameMenu.SwitchToMenu("town_mod_troop_type");
                });

            obj.AddGameMenuOption("town_mod_pay", "notpaying", "Nevermind", this.game_menu_just_add_leave_conditional, this.game_menu_switch_to_town_menu);

            obj.AddGameMenuOption("town_mod_troop_type", "militia_type", "Hire Commoners", this.game_menu_just_add_recruit_conditional, this.conversation_miltia_on_consequence);
            obj.AddGameMenuOption("town_mod_troop_type", "noble_type", "Hire Nobles", this.game_menu_just_add_recruit_conditional, this.conversation_noble_on_consequence);
            obj.AddGameMenuOption("town_mod_troop_type", "mod_leave", "I'm done looking", this.game_menu_just_add_leave_conditional, this.game_menu_switch_to_town_menu);
        }

        private void game_menu_switch_to_town_menu(MenuCallbackArgs args)
        {
            GameMenu.SwitchToMenu("town");
        }

        private bool game_menu_just_add_recruit_conditional(MenuCallbackArgs args)
        {
            args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
            return true;
        }

        private bool game_menu_just_add_leave_conditional(MenuCallbackArgs args)
        {
            args.optionLeaveType = GameMenuOption.LeaveType.Leave;
            return true;
        }

        public void conversation_miltia_on_consequence(MenuCallbackArgs args)
        {
            TroopProperties troopProperties;
            this.troopDic.TryGetValue(Settlement.CurrentSettlement.StringId, out troopProperties);
            PartyScreenManager.OpenScreenAsManageTroops(troopProperties.militia);
        }
        
        public void conversation_noble_on_consequence(MenuCallbackArgs args)
        {
            TroopProperties troopProperties;
            this.troopDic.TryGetValue(Settlement.CurrentSettlement.StringId, out troopProperties);
            PartyScreenManager.OpenScreenAsManageTroops(troopProperties.nobles);
        }

        // Updates the parties weekly
        public void trackWeekly()
        {
            foreach(string id in troopDic.Keys)
            {
                TroopProperties townTroopProperties;
                troopDic.TryGetValue(id, out townTroopProperties);
                townTroopProperties.militia = generateParty(townTroopProperties.getSelf().Town, militiaTroopIDs, 0);
                townTroopProperties.militia = generateParty(townTroopProperties.getSelf().Town, nobleTroopIDs, 1);
                troopDic[id] = townTroopProperties;
            }
        }

        // Populates all the guilds in every town
        private void populateGuilds()
        {
            if(troopDic.Count == 0)
            {
                Random random = new Random();
                foreach(Settlement settlement in Settlement.All)
                {
                    if(settlement.IsTown)
                    {
                        troopDic.Add(settlement.StringId, new TroopProperties(settlement.StringId, generateParty(settlement.Town, militiaTroopIDs, 0), generateParty(settlement.Town, nobleTroopIDs, 1)));
                    }
                }
            }
        }

        // Makes a party depending on town prosperity
        public MobileParty generateParty(Town town, string[] troopIDs, int type)
        {
            MobileParty party = new MobileParty();

            int troopProspModifier= 0;
            // Militia
            if (type == 0)
            {
                troopProspModifier = (int)Math.Ceiling(town.Prosperity / 50);
            }
            // Nobles
            else
            {
                troopProspModifier = (int)Math.Ceiling(town.Prosperity / 100);
            }
            int troopAmount = 0;
            foreach (string id in troopIDs)
            {
                troopAmount = (int)Math.Floor((double)troopProspModifier / CharacterObject.Find(id).Level);
                if(troopAmount >= 1)
                {
                    party.AddElementToMemberRoster(CharacterObject.Find(id), troopAmount);
                }
            }
            return party;
        }

        // Loads the mod data
        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("troopDic", ref troopDic);
        }

        // Saves the mod data
        public class BasiliskSaveDefiner : SaveableTypeDefiner
        {
            public BasiliskSaveDefiner() : base(91115119)
            {
            }

            protected override void DefineClassTypes()
            {
                AddClassDefinition(typeof(TroopProperties), 1);
            }

            protected override void DefineContainerDefinitions()
            {
                ConstructContainerDefinition(typeof(Dictionary<string, TroopProperties>));
            }
        }
    }
}
