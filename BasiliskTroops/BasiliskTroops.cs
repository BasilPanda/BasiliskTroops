using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using System.Collections.Generic;

namespace BasiliskTroops
{
    public class BasiliskTroops : CampaignBehaviorBase
    {

        Dictionary<string, TroopProperties> troopDic = new Dictionary<string, TroopProperties>();


        public static string[] militiaTroopIDs = new string[]
        {
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


        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.AddTroopDialog));
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, this.trackWeekly);
        }

        public void AddTroopDialog(CampaignGameStarter obj)
        {

            obj.AddGameMenuOption("town", "info_troop_type", "Hire Basilisk Guild Troops", game_menu_just_add_recruit_conditional, game_menu_troop_type_on_consq, false, 5);

            Town town = Settlement.CurrentSettlement.Town;
            int cost = (int)Math.Ceiling(town.Prosperity * 2);
            obj.AddGameMenu("town_mod_pay", "The Basilisk Guild offers its mercenaries, both commoners and nobles, in every town for quite the coin. The guild leader tells you that their mercenaries travel among their locations weekly. She also tells you that there is an initial fee of " + cost + " denars here just for the guild to show you their mercenaries. It is however the only fee besides upkeep cost you pay for.", null);

            obj.AddGameMenu("town_mod_troop_type", "The guild leader shows you their list of mercenaries and ask which you want. She will send the ones you paid for to wait by the gates for when you leave town.", null);

            obj.AddGameMenuOption("town_mod_pay", "pay_fee", "Pay initial cost of " + cost + " denars", this.game_menu_pay_intial_fee, this.game_menu_troop_type_on_consq);
            obj.AddGameMenuOption("town_mod_troop_type", "notpaying", "Nevermind", this.game_menu_just_add_leave_conditional, this.game_menu_switch_to_town_menu);

            obj.AddGameMenuOption("town_mod_troop_type", "militia_type", "Hire Commoners", this.game_menu_just_add_recruit_conditional, this.conversation_miltia_on_consequence);
            obj.AddGameMenuOption("town_mod_troop_type", "noble_type", "Hire Nobles", this.game_menu_just_add_recruit_conditional, this.conversation_noble_on_consequence);
            obj.AddGameMenuOption("town_mod_troop_type", "mod_leave", "I'm done looking", this.game_menu_just_add_leave_conditional, this.game_menu_switch_to_town_menu);
        }

        private bool game_menu_pay_intial_fee(MenuCallbackArgs args)
        {
            bool flag = true;

            return flag;
        }

        private void game_menu_troop_type_on_consq(MenuCallbackArgs args)
        {
            GameMenu.SwitchToMenu("town_mod_troop_type");
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

        // REDO
        public void conversation_miltia_on_consequence(MenuCallbackArgs args)
        {
            MobileParty troops = new MobileParty();
            Town town = Settlement.CurrentSettlement.Town;
            int troopAmount = (int)Math.Ceiling(town.Prosperity / 200);

            foreach(string id in militiaTroopIDs)
            {
                if ((int) Math.Floor(troopAmount / 2f) >= 1)
                {
                    troopAmount = (int)Math.Floor(troopAmount / 2f);
                    troops.AddElementToMemberRoster(CharacterObject.Find(id), troopAmount);
                } else
                {
                    break;
                }
            }
            PartyScreenManager.OpenScreenAsManageTroops(troops);
        }

        // REDO
        public void conversation_noble_on_consequence(MenuCallbackArgs args)
        {
            MobileParty troops = new MobileParty();
            Town town = Settlement.CurrentSettlement.Town;
            int troopAmount = (int)Math.Ceiling(town.Prosperity / 800);

            foreach (string id in nobleTroopIDs)
            {
                if ((int)Math.Floor(troopAmount / 2f) >= 1)
                {
                    troopAmount = (int)Math.Floor(troopAmount / 2f);
                    troops.AddElementToMemberRoster(CharacterObject.Find(id), troopAmount);
                }
                else
                {
                    break;
                }
            }
            PartyScreenManager.OpenScreenAsManageTroops(troops);
        }

        public void trackWeekly()
        {

        }

        private void populateGuilds()
        {
            if(troopDic.Count == 0)
            {
                Random random = new Random();
                foreach(Settlement settlement in Settlement.All)
                {
                    if(settlement.IsTown)
                    {
                        troopDic.Add(settlement.StringId, new TroopProperties(settlement.StringId, generateParty(settlement.Town, militiaTroopIDs), generateParty(settlement.Town, nobleTroopIDs), 0));
                    }
                }
            }
        }

        // Makes a party depending on town prosperity
        public MobileParty generateParty(Town town, string[] type)
        {
            MobileParty party = new MobileParty();
            int troopAmount = (int)Math.Ceiling(town.Prosperity / 800);

            foreach (string id in nobleTroopIDs)
            {
                if ((int)Math.Floor(troopAmount / 2f) >= 1)
                {
                    troopAmount = (int)Math.Floor(troopAmount / 2f);
                    party.AddElementToMemberRoster(CharacterObject.Find(id), troopAmount);
                }
                else
                {
                    break;
                }
            }
            return party;
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("troopDic", ref troopDic);
        }
    }
}
