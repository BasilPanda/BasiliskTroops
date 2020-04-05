using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;

namespace BasiliskTroops
{
    public class BasiliskTroops : CampaignBehaviorBase
    {

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
        }

        public void AddTroopDialog(CampaignGameStarter obj)
        {

            obj.AddGameMenuOption("town", "info_troop_type", "Hire Basilisk Guild Troops", (MenuCallbackArgs args) => { args.optionLeaveType = GameMenuOption.LeaveType.Recruit; return true; }, game_menu_troop_type_on_consq, false, 5);

            Town town = Settlement.CurrentSettlement.Town;
            int cost = (int)Math.Ceiling(town.Prosperity * 2);
            obj.AddGameMenu("town_mod_troop_type", "The Basilisk Guild offers its mercenaries, both commoners and nobles, in every town for quite the coin. The guild leader tells you that there is an initial fee of " + cost + " denars here just for the guild to show you their mercenaries.", null);
            obj.AddGameMenuOption("town_mod_troop_type", "pay_fee", "Pay initial cost of " + cost + " denars",
                (MenuCallbackArgs args) => { args.optionLeaveType = GameMenuOption.LeaveType.Recruit; return true; }, game_menu_troop_type_on_consq);

            obj.AddGameMenuOption("town_mod_troop_type", "militia_type", "Hire Commoners", (MenuCallbackArgs args) => { args.optionLeaveType = GameMenuOption.LeaveType.Recruit; return true; }, conversation_miltia_on_consequence);
            obj.AddGameMenuOption("town_mod_troop_type", "noble_type", "Hire Nobles", (MenuCallbackArgs args) => { args.optionLeaveType = GameMenuOption.LeaveType.Recruit; return true; }, conversation_noble_on_consequence);
            obj.AddGameMenuOption("town_mod_troop_type", "nevermind", "I'm done looking", (MenuCallbackArgs args) => { args.optionLeaveType = GameMenuOption.LeaveType.Leave; return true; }, game_menu_switch_to_town_menu);
        }

        private void game_menu_troop_type_on_consq(MenuCallbackArgs args)
        {
            GameMenu.SwitchToMenu("town_mod_troop_type");
        }

        private void game_menu_switch_to_town_menu(MenuCallbackArgs args)
        {
            GameMenu.SwitchToMenu("town");
        }

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

        public override void SyncData(IDataStore dataStore)
        {
            try
            {

            }
            catch(NullReferenceException nope)
            {
                
            }
        }
    }
}
