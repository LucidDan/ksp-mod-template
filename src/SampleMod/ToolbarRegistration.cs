using ToolbarControl_NS;
using UnityEngine;

namespace SampleMod
{
    /// <summary>
    /// Monobehaviour to register mod with toolbar controller during mainmenu startup.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class ToolbarRegistration: MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(ModManager.Name, ModManager.DisplayName);
        }
    }
}
