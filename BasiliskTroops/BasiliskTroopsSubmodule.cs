using System;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace BasiliskTroops
{
    class BasiliskTroopsSubmodule : MBSubModuleBase
    {

        public static readonly string ModuleName = "BasiliskGuild";
        
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (!(game.GameType is Campaign))
                return;
            CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;
            try
            {
                gameInitializer.AddBehavior(new BasiliskTroops());
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}
