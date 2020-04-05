using System;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace BasiliskTroops
{
    public class BasiliskTroops : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.AddTroopDialog));
        }

        public void AddTroopDialog(CampaignGameStarter obj)
        {

            obj.AddGameMenuOption("town", "info_troop_type", "Hire Basilisk Guild Troops", null, this.game_menu_troop_type_on_consq, false, 5);
            obj.AddGameMenu("town_mod_troop_type", "Basilisk Guild hiring here", (MenuCallbackArgs args) =>{ args.optionLeaveType = GameMenuOption.LeaveType.Recruit; });
            obj.AddGameMenuOption("town_mod_troop_type", "militia_type", "Commoners", (MenuCallbackArgs args) => { args.optionLeaveType = GameMenuOption.LeaveType.Recruit; return true; }, this.conversation_miltia_on_consequence);
            obj.AddGameMenuOption("town_mod_troop_type", "noble_type", "Nobles", (MenuCallbackArgs args) => { args.optionLeaveType = GameMenuOption.LeaveType.Recruit; return true; }, this.conversation_noble_on_consequence);
            obj.AddGameMenuOption("town_mod_troop_type", "nevermind", "Nevermind", (MenuCallbackArgs args) => { args.optionLeaveType = GameMenuOption.LeaveType.Leave; return true; }, this.game_menu_switch_to_town_menu);
        }

        private void game_menu_troop_type_on_consq(MenuCallbackArgs args)
        {
            GameMenu.SwitchToMenu("town_mod_troop_type");
        }

        private void game_menu_switch_to_town_menu(MenuCallbackArgs args)
        {
            GameMenu.SwitchToMenu("village");
        }

        public void conversation_miltia_on_consequence(MenuCallbackArgs args)
        {
            MobileParty troops = new MobileParty();
            Village village = Settlement.CurrentSettlement.Village;
            int num = village.HearthLevel();
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_trainee"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_militia"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_light_infantry"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_heavy_infantry"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_slayer"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_spearman"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_armored_spearman"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_skirmisher"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_archer"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_ranger"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_light_cavalry"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_vanguard"), num);
            PartyScreenManager.OpenScreenAsManageTroops(troops);
        }

        public void conversation_noble_on_consequence(MenuCallbackArgs args)
        {
            MobileParty troops = new MobileParty();
            Village village = Settlement.CurrentSettlement.Village;
            int num = village.HearthLevel();
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_nobleman"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_squire"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_knight"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_master"), num);
            troops.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_grandmaster"), num);
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
