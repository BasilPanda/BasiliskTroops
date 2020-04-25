using System;
using System.Configuration;
using System.Collections.Generic;
using TaleWorlds.Localization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.SaveSystem;
using System.Windows.Forms;

namespace BasiliskTroops
{
    public class BasiliskTroops : CampaignBehaviorBase
    {
        public static bool femaleTreeEnabled = Settings.Instance.FemaleTreeEnabled;
        public static int baseCostForImmediate = Settings.Instance.BaseCostForImmediateTroops;
        public static float weeklyCostModifier = Settings.Instance.WeeklyCostModifier;
        Dictionary<string, TroopProperties> troopDic = new Dictionary<string, TroopProperties>();
        Random rand = new Random();

        public static Dictionary<string, string> genderPairs = new Dictionary<string, string>()
        {
            { "mod_basilisk_trainee",           "mod_f_basilisk_trainee"},
            { "mod_basilisk_militia",           "mod_f_basilisk_militia" },
            { "mod_basilisk_light_infantry",    "mod_f_basilisk_light_infantry" },
            { "mod_basilisk_skirmisher",        "mod_f_basilisk_skirmisher" },
            { "mod_basilisk_archer",            "mod_f_basilisk_archer" },
            { "mod_basilisk_spearman",          "mod_f_basilisk_spearman" },
            { "mod_basilisk_crossbow",          "mod_f_basilisk_crossbow" },
            { "mod_basilisk_light_cavalry",     "mod_f_basilisk_light_cavalry" },
            { "mod_basilisk_light_horsearcher", "mod_f_basilisk_light_horsearcher" },
            { "mod_basilisk_heavy_infantry",    "mod_f_basilisk_heavy_infantry" },
            { "mod_basilisk_ranger",            "mod_f_basilisk_ranger" },
            { "mod_basilisk_armored_spearman",  "mod_f_basilisk_armored_spearman" },
            { "mod_basilisk_sharpshooter",      "mod_f_basilisk_sharpshooter" },
            { "mod_basilisk_heavy_cavalry",     "mod_f_basilisk_heavy_cavalry" },
            { "mod_basilisk_slayer",            "mod_f_basilisk_slayer" },
            { "mod_basilisk_heavy_horsearcher", "mod_f_basilisk_heavy_horsearcher" },
            { "mod_basilisk_vanguard",          "mod_f_basilisk_vanguard" },
            { "mod_basilisk_nobleman",          "mod_f_basilisk_nobleman" },
            { "mod_basilisk_squire",            "mod_f_basilisk_squire"},
            { "mod_basilisk_knight",            "mod_f_basilisk_knight" },
            { "mod_basilisk_master",            "mod_f_basilisk_master" },
            { "mod_basilisk_grandmaster",       "mod_f_basilisk_grandmaster" }
        };

        public static string[] militiaTroopIDs = new string[]
        {
            "mod_basilisk_trainee",
            "mod_basilisk_militia",
            "mod_basilisk_light_infantry",
            "mod_basilisk_skirmisher",
            "mod_basilisk_archer",
            "mod_basilisk_spearman",
            "mod_basilisk_crossbow",
            "mod_basilisk_light_cavalry",
            "mod_basilisk_light_horsearcher",
            "mod_basilisk_heavy_infantry",
            "mod_basilisk_ranger",
            "mod_basilisk_armored_spearman",
            "mod_basilisk_heavy_cavalry",
            "mod_basilisk_sharpshooter",
            "mod_basilisk_slayer",
            "mod_basilisk_heavy_horsearcher",
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
            try
            {
                populateGuilds();
                AddTroopMenu(obj);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
            CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, this.trackDaily);
        }

        public void AddTroopMenu(CampaignGameStarter obj)
        {

            obj.AddGameMenuOption("town", "info_troop_type", "Hire Basilisk Guild Troops", game_menu_just_add_recruit_conditional, (MenuCallbackArgs args) => { GameMenu.SwitchToMenu("town_mod_pay"); }, false, 5);

            obj.AddGameMenu("town_mod_pay", "The Basilisk Guild offers its powerful mercenaries, both commoners and nobles, in every town for quite the coin. " +
                "The guild manager tells you that their mercenaries favor more wealthy places." +
                " She also tells you that there is a weekly upfront fee of {COST} denars here just to view available mercenaries. The available mercenaries will update weekly." +
                "She then tells you that they also offer immediate contingents for the extreme wealthy. The larger contingents are of higher quality.",
                (MenuCallbackArgs args) =>
                {
                    TroopProperties troopProps;
                    troopDic.TryGetValue(Settlement.CurrentSettlement.StringId, out troopProps);
                    MBTextManager.SetTextVariable("COST", troopProps.getCost * weeklyCostModifier, false);
                    if(Clan.PlayerClan.Tier == 0)
                    {
                        MBTextManager.SetTextVariable("RENOWN_STATUS", "No one here has heard of you before", false);
                    }
                });

            obj.AddGameMenu("town_mod_troop_type", "The guild manager goes to the back and after a while comes back to the front with two lists of available mercenaries. " +
                "{RENOWN_STATUS} " +
                "The ones you paid for will wait by the gates.", 
                (MenuCallbackArgs args) =>
                {
                    if (Clan.PlayerClan.Tier == 0)
                    {
                        MBTextManager.SetTextVariable("RENOWN_STATUS", "She tells you that no one here has heard of you so not many are willing to join.", false);
                    } else if (Clan.PlayerClan.Tier == 1)
                    {
                        MBTextManager.SetTextVariable("RENOWN_STATUS", "She tells you that she cannot remember you or your face but some of their trainees were talking about you.", false);
                    }
                    else if (Clan.PlayerClan.Tier == 2)
                    {
                        MBTextManager.SetTextVariable("RENOWN_STATUS", "She thanks you for waiting and tells you that some of their trainees were waiting for you to come again.", false);
                    }
                    else if (Clan.PlayerClan.Tier == 3)
                    {
                        MBTextManager.SetTextVariable("RENOWN_STATUS", "She thanks you for waiting and tells you that some of their experienced members were discussing your exploits from time to time.", false);
                    }
                    else if (Clan.PlayerClan.Tier == 4)
                    {
                        MBTextManager.SetTextVariable("RENOWN_STATUS", "She thanks you for your patronage and tells you that noblemen are declining other requests to join you instead.", false);
                    }
                    else if (Clan.PlayerClan.Tier == 5)
                    {
                        MBTextManager.SetTextVariable("RENOWN_STATUS", "She bows and thanks you for your continued patronage. She tells you that there is now a waiting list to get on the list to join your party.", false);
                    }
                    else
                    {
                        MBTextManager.SetTextVariable("RENOWN_STATUS", "She bows and thanks you for your continued patronage. The whole guild is entirely maintained with your money at this point.", false);
                    }
                });

            obj.AddGameMenuOption("town_mod_pay", "pay_fee", "Pay {DAILY_COST} denars to see weekly troops",
                (MenuCallbackArgs args) =>
                {
                    TroopProperties troopProps;
                    troopDic.TryGetValue(Settlement.CurrentSettlement.StringId, out troopProps);
                    MBTextManager.SetTextVariable("DAILY_COST", (int)Math.Ceiling(troopProps.getCost * weeklyCostModifier), false);
                    args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                    if ((int)Math.Ceiling(troopProps.getCost * weeklyCostModifier) >= Hero.MainHero.Gold || troopProps.paid)
                    {
                        return false;
                    }
                    return true;
                },
                (MenuCallbackArgs args) =>
                {
                    TroopProperties troopProps;
                    troopDic.TryGetValue(Settlement.CurrentSettlement.StringId, out troopProps);
                    int cost = (int)Math.Ceiling(troopProps.getCost * weeklyCostModifier);
                    if (cost <= Hero.MainHero.Gold)
                    {
                        GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, cost);
                        troopProps.paid = true;
                        troopDic[Settlement.CurrentSettlement.StringId] = troopProps;
                    }
                    GameMenu.SwitchToMenu("town_mod_troop_type");
                });

            obj.AddGameMenuOption("town_mod_pay", "pay_fee_5", "Pay {COST_5} denars for 5 troops",
                (MenuCallbackArgs args) =>
                {
                    int cost = (int)Math.Ceiling(baseCostForImmediate + Settlement.CurrentSettlement.Prosperity / 2 + Clan.PlayerClan.Tier * 1000);
                    MBTextManager.SetTextVariable("COST_5", cost, false);
                    args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                    if (cost >= Hero.MainHero.Gold)
                    {
                        return false;
                    }
                    return true;
                },
                (MenuCallbackArgs args) =>
                {
                    int cost = (int)Math.Ceiling(baseCostForImmediate + Settlement.CurrentSettlement.Prosperity / 2 + Clan.PlayerClan.Tier * 1000);
                    if (cost <= Hero.MainHero.Gold)
                    {
                        GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, cost);
                        giveTroops(5);
                    }
                });

            obj.AddGameMenuOption("town_mod_pay", "pay_fee_15", "Pay {COST_15} denars for 15 troops",
                (MenuCallbackArgs args) =>
                {
                    int cost = (int)Math.Ceiling(baseCostForImmediate * 3 + Settlement.CurrentSettlement.Prosperity / 2 + Clan.PlayerClan.Tier * 2000);
                    MBTextManager.SetTextVariable("COST_15", cost, false);
                    args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                    if (cost >= Hero.MainHero.Gold)
                    {
                        return false;
                    }
                    return true;
                },
                (MenuCallbackArgs args) =>
                {
                    int cost = (int)Math.Ceiling(baseCostForImmediate * 3 + Settlement.CurrentSettlement.Prosperity / 2 + Clan.PlayerClan.Tier * 2000);
                    if (cost <= Hero.MainHero.Gold)
                    {
                        GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, cost);
                        giveTroops(15);
                    }
                });

            obj.AddGameMenuOption("town_mod_pay", "pay_fee_25", "Pay {COST_25} denars for 25 troops",
                (MenuCallbackArgs args) =>
                {
                    int cost = (int)Math.Ceiling(baseCostForImmediate * 10 + Settlement.CurrentSettlement.Prosperity / 2 + Clan.PlayerClan.Tier * 3000);
                    MBTextManager.SetTextVariable("COST_25", cost, false);
                    args.optionLeaveType = GameMenuOption.LeaveType.Trade;
                    if (cost >= Hero.MainHero.Gold)
                    {
                        return false;
                    }
                    return true;
                },
                (MenuCallbackArgs args) =>
                {
                    int cost = (int)Math.Ceiling(baseCostForImmediate * 10 + Settlement.CurrentSettlement.Prosperity / 2 + Clan.PlayerClan.Tier * 3000);
                    if (cost <= Hero.MainHero.Gold)
                    {
                        GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, cost);
                        giveTroops(25);
                    } else
                    {
                        
                        GameMenu.SwitchToMenu("town");
                    }
                });

            /*
            obj.AddGameMenuOption("town_mod_pay", "pay_patrol", "Pay 1 denars for a patrol",
                (MenuCallbackArgs args) =>
                {
                    if (1 >= Hero.MainHero.Gold)
                    {
                        return false;
                    }
                    return true;
                },
                (MenuCallbackArgs args) =>
                {
                    if (1 <= Hero.MainHero.Gold)
                    {
                        GiveGoldAction.ApplyForCharacterToSettlement(Hero.MainHero, Settlement.CurrentSettlement, 1);
                        giveTroops(25);
                    }
                    else
                    {

                        GameMenu.SwitchToMenu("town");
                    }
                });
            */

            obj.AddGameMenuOption("town_mod_pay", "already_paid", "View troops",
                (MenuCallbackArgs args) =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Recruit;
                    TroopProperties troopProps;
                    troopDic.TryGetValue(Settlement.CurrentSettlement.StringId, out troopProps);
                    if (troopProps.paid)
                    {
                        return true;
                    }
                    return false;
                },
                (MenuCallbackArgs args) =>
                {
                    GameMenu.SwitchToMenu("town_mod_troop_type");
                });


            obj.AddGameMenuOption("town_mod_pay", "notpaying", "Leave", this.game_menu_just_add_leave_conditional, this.game_menu_switch_to_town_menu);

            obj.AddGameMenuOption("town_mod_troop_type", "militia_type", "Look at the Commoners list", this.game_menu_just_add_recruit_conditional, this.conversation_miltia_on_consequence);
            obj.AddGameMenuOption("town_mod_troop_type", "noble_type", "Look at the Nobles list", this.game_menu_just_add_recruit_conditional, this.conversation_noble_on_consequence);
            obj.AddGameMenuOption("town_mod_troop_type", "mod_leave", "Finished looking", this.game_menu_just_add_leave_conditional, this.game_menu_switch_to_town_menu);
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

        // Updates the parties daily
        public void trackDaily()
        {
            string id = "";
            foreach (Settlement settlement in Settlement.All)
            {
                if (settlement.IsTown)
                {
                    id = settlement.StringId;
                    TroopProperties townTroopProperties;
                    troopDic.TryGetValue(id, out townTroopProperties);
                    townTroopProperties.militia = generateParty(townTroopProperties.getSelf().Town, militiaTroopIDs, 0);
                    townTroopProperties.nobles = generateParty(townTroopProperties.getSelf().Town, nobleTroopIDs, 1);
                    townTroopProperties.paid = false;
                    troopDic[id] = townTroopProperties;
                }
            }
        }

        // Populates all the guilds in every town
        private void populateGuilds()
        {
            if (troopDic.Count == 0)
            {
                foreach (Settlement settlement in Settlement.All)
                {
                    if (settlement.IsTown)
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

            int troopProspModifier = 0;
            // Militia
            if (type == 0)
            {
                TextObject text = new TextObject("Basilisk Mercenaries");
                party.Name = text;
            }
            // Nobles
            else
            {
                TextObject text = new TextObject("Basilisk Noble Mercenaries");
                party.Name = text;
            }
            troopProspModifier = (int)Math.Floor(town.Prosperity / 100 + Clan.PlayerClan.Renown * 0.05);
            int troopAmount = 0;
            CharacterObject unit;
            CharacterObject unitf;
            foreach (string id in troopIDs)
            {
                unit = CharacterObject.Find(id);
                string f_id = "";
                genderPairs.TryGetValue(unit.StringId, out f_id);
                unitf = CharacterObject.Find(f_id);
                if (unit.Level > 21)
                {
                    continue;
                }
                troopAmount = (int)Math.Floor((double)troopProspModifier / unit.Level);
                troopAmount = balanceTroops(unit.Level, troopAmount);
                if (troopAmount >= 1)
                {
                    troopAmount = rand.Next((int)Math.Ceiling((double)troopAmount / 2), troopAmount + 1);
                    while(troopAmount > 0)
                    {
                        party.AddElementToMemberRoster(randomGender(unit, unitf), 1, true);
                        troopAmount--;
                    }
                }
            }
            return party;
        }

        // Randomizes gender
        public CharacterObject randomGender(CharacterObject unit, CharacterObject unitf)
        {
            if (!femaleTreeEnabled)
            {
                return unit;
            } 
            if (rand.Next(0, 2) == 1)
            {
                return unitf;
            }
            return unit;
        }

        // Allows for 5, 15, 25
        public void giveTroops(int troopAmount)
        {
            int amount = 0;
            CharacterObject unit;
            CharacterObject unitf;
            if (troopAmount == 5)
            {
                foreach(string id in militiaTroopIDs)
                {
                    unit = CharacterObject.Find(id);
                    string f_id = "";
                    genderPairs.TryGetValue(unit.StringId, out f_id);
                    unitf = CharacterObject.Find(f_id);
                    if (unit.Level < 17 && troopAmount > 0)
                    {
                        amount = rand.Next(troopAmount / 2, troopAmount);
                        troopAmount -= amount;
                        while (amount > 0)
                        {
                            MobileParty.MainParty.AddElementToMemberRoster(randomGender(unit, unitf), 1, false);
                            amount--;
                        }
                    }
                }
                if (troopAmount > 0)
                {
                    MobileParty.MainParty.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_militia"), troopAmount);
                }
            }
            else if (troopAmount == 15)
            {
                MobileParty.MainParty.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_nobleman"), 1);
                troopAmount--;
                foreach (string id in militiaTroopIDs)
                {
                    unit = CharacterObject.Find(id);
                    string f_id = "";
                    genderPairs.TryGetValue(unit.StringId, out f_id);
                    unitf = CharacterObject.Find(f_id);
                    if (unit.Level < 30 && troopAmount > 0)
                    {
                        amount = rand.Next(troopAmount / 2, troopAmount);
                        troopAmount -= amount;
                        while (amount > 0)
                        {
                            MobileParty.MainParty.AddElementToMemberRoster(randomGender(unit, unitf), 1, false);
                            amount--;
                        }
                    }
                }
                if (troopAmount > 0)
                {
                    MobileParty.MainParty.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_militia"), troopAmount);
                }
            }
            else
            {
                MobileParty.MainParty.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_knight"), 1);
                MobileParty.MainParty.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_squire"), 1);
                MobileParty.MainParty.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_nobleman"), 1);
                troopAmount -= 3;
                foreach (string id in militiaTroopIDs)
                {
                    unit = CharacterObject.Find(id);
                    string f_id = "";
                    genderPairs.TryGetValue(unit.StringId, out f_id);
                    unitf = CharacterObject.Find(f_id);
                    if (unit.Level < 40 && troopAmount > 0)
                    {
                        amount = rand.Next(troopAmount/2, troopAmount);
                        troopAmount -= amount;
                        while (amount > 0)
                        {
                            MobileParty.MainParty.AddElementToMemberRoster(randomGender(unit, unitf), 1, false);
                            amount--;
                        }
                    }
                }
                if (troopAmount > 0)
                {
                    MobileParty.MainParty.AddElementToMemberRoster(CharacterObject.Find("mod_basilisk_militia"), troopAmount);
                }
            }

        }

        public int balanceTroops(int level, int troopAmount)
        {
            if (level < 12 && troopAmount > (int)Math.Floor(4 + Clan.PlayerClan.Renown * 0.0075))
            {
                troopAmount = (int)Math.Floor(12 + Clan.PlayerClan.Renown * 0.0075);
            }
            else if(level < 17 && troopAmount > (int)Math.Floor(3 + Clan.PlayerClan.Renown * 0.006))
            {
                troopAmount = (int)Math.Floor(6 + Clan.PlayerClan.Renown * 0.006);
            }
            else if(level < 22 && troopAmount > (int)Math.Floor(2 + Clan.PlayerClan.Renown * 0.005))
            {
                troopAmount = (int)Math.Floor(3 + Clan.PlayerClan.Renown * 0.005);
            }
            else if(level < 26 && troopAmount > (int)Math.Floor(1 + Clan.PlayerClan.Renown * 0.0025))
            {
                troopAmount = (int)Math.Floor(1 + Clan.PlayerClan.Renown * 0.0025);
            } else
            {
                return 0;
            }
            return troopAmount;
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
