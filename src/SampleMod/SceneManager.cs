using KSP.UI.Screens;
using SampleMod.Unity;
using SampleMod.Unity.Interface;
using ToolbarControl_NS;
using UnityEngine;

namespace SampleMod
{
    internal abstract class BaseSceneManager : MonoBehaviour, IMainWindowPanel
    {
        private ToolbarControl? _toolbarControl = null;
        private MainWindowPanel? _mainWindow = null;

        private void ToggleOn()
        {
            // Show window
            Utils.Log("Show main GUI window");

            if (_toolbarControl == null) return;

            //toolbarButtonPosition = _toolbarControl.buttonClickedMousePos;
            var menuPosition = new Vector2(1, 1);

            Utils.Log($"Toolbar position: {menuPosition}");
            _mainWindow = Instantiate(ModManager.GetPrefab("MainWindow"),
                menuPosition,
                Quaternion.identity)?.GetComponent<MainWindowPanel>();

            if (_mainWindow == null) return;

            Utils.Log($"Loading window");

            _mainWindow.gameObject.transform.SetParent(MainCanvasUtil.MainCanvas.transform);
            _mainWindow.SetInterface(this);
        }

        private void ToggleOff()
        {
            // Hide window
            Utils.Log("Hide main GUI window");
        }

        private void HoverOn()
        {
            // Show summary window with brief stats on status, or possibly just show window in place
            Utils.Log("Show main GUI window (hover)");
        }

        private void HoverOff()
        {
            // Hide window
            Utils.Log("Hide main GUI window (hover-off)");
        }

        private void OnEnable()
        {
            // ??
            Utils.Log("Enable Toolbar button?");
        }

        private void OnDisable()
        {
            // ??
            Utils.Log("Disable Toolbar button?");
        }

        protected virtual void Start()
        {
                if (_toolbarControl == null)
                    _toolbarControl = gameObject.AddComponent<ToolbarControl>();
                Utils.Log("Add toolbar button");

                if (_toolbarControl != null)
                    _toolbarControl.AddToAllToolbars(ToggleOn, ToggleOff, HoverOn, HoverOff, OnEnable, OnDisable,
                        ApplicationLauncher.AppScenes.ALWAYS, ModManager.Name,
                        "sampleModAppButton", ModManager.GetRelativeResourcePath("icon_lg_active"),
                        ModManager.GetRelativeResourcePath("icon_lg_inactive"),
                        ModManager.GetRelativeResourcePath("icon_sm_active"),
                        ModManager.GetRelativeResourcePath("icon_sm_inactive"),
                        "SampleMod Dashboard");
        }

        private void OnDestroy()
        {
            Utils.Log("Destroy toolbar button controller");
            if (_toolbarControl == null) return;

            _toolbarControl.OnDestroy();
            Destroy(_toolbarControl);
        }

        public bool IsReady
        {
            get => true;
        }
        public string Version
        {
            get => "v1.0";
            set { }
        }
    }

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class EditorManager : BaseSceneManager
    {
        protected override void Start()
        {
            base.Start();
            if (!Utils.Settings.AssemblyTime)
            {
                Utils.Log("Do nothing - AssemblyTime disabled");
                return;
            }

            Utils.Log("AssemblyTime is enabled - override launch button");
            // Set up overrides for:
            // - Launch vessel; Start build/assembly of current blueprint
            // - Save? Load? New?
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class FlightManager : BaseSceneManager
    {
        protected override void Start()
        {
            base.Start();
            Utils.Log("Flight scene - no action required.");
        }
    }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    class SpaceCentreManager : BaseSceneManager
    {
        protected override void Start()
        {
            base.Start();
            if (!Utils.Settings.AssemblyTime)
            {
                Utils.Log("AssemblyTime disabled - no action in KSC required.");
                return;
            }
            Utils.Log("KSC - AssemblyTime enabled - override launchpad and runways.");

            // Set up overrides for:
            // - Open Launchpad (display GUI with list of launchpads and available vessels to roll out)
            // - Open Runway (display GUI with list of runways and available vessels to roll out)
        }
    }

    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    class TrackingStationManager : BaseSceneManager
    {
        protected override void Start()
        {
            base.Start();
            Utils.Log("Tracking Center scene - no action required.");
        }
    }
}