using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.Character;
using TaleWorlds.SaveSystem;
using static TaleWorlds.CampaignSystem.SettlementComponent;

namespace BasiliskTroops
{
    public class TroopProperties
    {

        [SaveableField(1)]
        public string settlementID;

        public TroopProperties(string settlementID)
        {
            this.settlementID = settlementID;
        }

    }
}
