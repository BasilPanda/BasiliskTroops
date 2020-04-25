
using MBOptionScreen.Attributes;
using MBOptionScreen.Settings;

namespace BasiliskTroops
{
    public class Settings : AttributeSettings<Settings>
    {
        public override string Id { get; set; } = "BasiliskGuild_v1";
        public override string ModName => "Basilisk Guild";
        public override string ModuleFolderName => "BasiliskGuild";

        [SettingProperty("Female Tree Enabled", false, hintText: "Allows recruitment of female Basilisk troops")]
        [SettingPropertyGroup("General Settings")]
        public bool FemaleTreeEnabled { get; set; } = false;

        [SettingProperty("Cost for Troops", 0, 10000, false, hintText: "Sets the base cost of immediate troops.")]
        [SettingPropertyGroup("General Settings")]
        public int BaseCostForImmediateTroops { get; set; } = 1250;


        [SettingProperty("Weekly Cost", 0, 4, false, hintText: "Sets the weekly cost modifier")]
        [SettingPropertyGroup("General Settings")]
        public float WeeklyCostModifier { get; set; } = 1f;
    }
}
