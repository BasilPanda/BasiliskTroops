using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace BasiliskTroops
{
    public class TroopProperties
    {

        [SaveableField(1)]
        public string settlementID;

        [SaveableField(2)]
        public MobileParty militia;

        [SaveableField(3)]
        public MobileParty nobles;

        [SaveableField(4)]
        public bool paid;

        public TroopProperties(string settlementID, MobileParty militia, MobileParty nobles, bool paid = false)
        {
            this.settlementID = settlementID;
            this.militia = militia;
            this.nobles = nobles;
            this.paid = false;
        }

        public Settlement getSelf()
        {
            return Settlement.Find(this.settlementID);
        }

        public int getCost
        {
            get
            {
                float totalCost = 0;
                totalCost += this.militia.Party.NumberOfMenWithoutHorse * 50 + this.militia.Party.NumberOfMenWithHorse * 100;
                totalCost += this.nobles.Party.NumberOfAllMembers * 250;
                totalCost += (int)Math.Ceiling(getProsperity * 2 + Clan.PlayerClan.Renown * 1.5);

                // 10% discount for player if they own the settlement
                if (getSelf().OwnerClan == Clan.PlayerClan)
                {
                    totalCost *= 0.9f;
                }

                return (int)Math.Ceiling(totalCost);
            }
        }
        
        public int getProsperity
        {
            get
            {
                return (int)Math.Ceiling(getSelf().Town.Prosperity);
            }
        }

    }
}
