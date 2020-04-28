using MBOptionScreen.Attributes;
using MBOptionScreen.Attributes.v2;
using MBOptionScreen.Settings;

namespace BasiliskTroops
{
    public class Settings : AttributeSettings<Settings>
    {
        public override string Id { get; set; } = "BasiliskGuild_v1";
        public override string ModName => "Basilisk Guild";
        public override string ModuleFolderName => "BasiliskGuild";

        [SettingPropertyBool("Female Tree Enabled",RequireRestart = false, HintText = "Allows recruitment of female Basilisk troops")]
        [SettingPropertyGroup("General Settings")]
        public bool FemaleTreeEnabled { get; set; } = false;

        [SettingPropertyInteger("Cost for Troops", 0, 10000, RequireRestart = false, HintText = "Sets the base cost of immediate troops.")]
        [SettingPropertyGroup("General Settings")]
        public int BaseCostForImmediateTroops { get; set; } = 1250;


        [SettingPropertyFloatingInteger("Weekly Cost", 0, 4, RequireRestart = false, HintText = "Sets the weekly cost modifier")]
        [SettingPropertyGroup("General Settings")]
        public float WeeklyCostModifier { get; set; } = 1f;
    }
}
