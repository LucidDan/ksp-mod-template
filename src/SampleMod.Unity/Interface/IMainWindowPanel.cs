namespace SampleMod.Unity.Interface
{
    /// <summary>
    /// An interface for linking Unity prefabs with KSP assemblies, without a direct reference.
    /// </summary>
    public interface IPrefabLink
    {
        /// <summary>
        /// Is the link ready to be used.
        /// </summary>
        bool IsReady { get; }
    }

    public interface IMainWindowPanel : IPrefabLink
    {
        string Version { get; set; }

        // - Listen methods; receive updates into KSP assemblies, trigger actions
        // - Toggles, etc; form elements updated from KSP assemblies data
    }
}
