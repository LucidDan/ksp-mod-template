namespace SampleMod
{
    public abstract class BaseSettings : GameParameters.CustomParameterNode
    {
        public override string Section => ModManager.Name;
        public override string DisplaySection => ModManager.DisplayName;
        public override int SectionOrder => 1;
        public override bool HasPresets => true;
        public bool autoPersistance = true;
    }

    public class FeatureSettings : BaseSettings
    {
        public override GameParameters.GameMode GameMode => GameParameters.GameMode.ANY;
        public override string Title => "Enable Features";
        public bool newGameOnly = true;

        [GameParameters.CustomParameterUI("Factories", toolTip="Parts need to be manufactured in factories over time, before assembly for launches")]
        public bool PartFactories = true;
        [GameParameters.CustomParameterUI("Resource Farming", toolTip="Resources are mined/harvested over time, before being fueling launches")]
        public bool ResourceFactories = true;
        [GameParameters.CustomParameterUI("Warehousing", toolTip="Only so much parts and resources at a time can be accumulated in KSC warehouses")]
        public bool Warehouses = true;
        [GameParameters.CustomParameterUI("Crew Training", toolTip="New crew need to be trained over time, before being available or joining missions")]
        public bool CrewTraining = true;
        [GameParameters.CustomParameterUI("Assembly Time", toolTip="Parts for a vessel need to be assembled, before rolling out for launch")]
        public bool AssemblyTime = true;
        [GameParameters.CustomParameterUI("Reliability", toolTip="Blueprints, parts, and vessels have reliability that varies depending on quality, age, testing, etc")]
        public bool Reliability = true;
        [GameParameters.CustomParameterUI("Prototyping", toolTip="When designing a blueprint, prototyping the vessel improves reliability")]
        public bool Prototyping = true;
        [GameParameters.CustomParameterUI("Simulations", toolTip="When designing a blueprint, simulations improve the reliability and decrease prototyping costs")]
        public bool Simulations = true;

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            PartFactories = false;
            ResourceFactories = false;
            Warehouses = false;
            CrewTraining = false;
            AssemblyTime = false;
            Reliability = false;
            Prototyping = false;
            Simulations = false;
            switch (preset)
            {
                case GameParameters.Preset.Easy:
                    AssemblyTime = true;
                    break;
                case GameParameters.Preset.Custom:
                case GameParameters.Preset.Normal:
                    PartFactories = true;
                    CrewTraining = true;
                    break;
                case GameParameters.Preset.Moderate:
                    PartFactories = true;
                    ResourceFactories = true;
                    Warehouses = true;
                    CrewTraining = true;
                    AssemblyTime = true;
                    break;
                case GameParameters.Preset.Hard:
                    PartFactories = true;
                    ResourceFactories = true;
                    Warehouses = true;
                    CrewTraining = true;
                    AssemblyTime = true;
                    Reliability = true;
                    Prototyping = true;
                    Simulations = true;
                    break;
                default:
                    // No features enabled
                    break;
            }
        }
    }
}