using System;
using SampleMod.Unity.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace SampleMod.Unity
{
    [RequireComponent(typeof(RectTransform))]
    public class MainWindowPanel : MonoBehaviour
    {
        [NonSerialized] private IMainWindowPanel? _kspInterface = null;
        [NonSerialized] private bool _loaded;
        [SerializeField] private Text? mVersionText = null;

        public void UpdateVersion(string newVersion)
        {
            if (_loaded && mVersionText != null) mVersionText.text = newVersion;
        }

        public void SetInterface(IMainWindowPanel newInterface)
        {
            _kspInterface = newInterface;

            // Initialize
            UpdateVersion(_kspInterface.Version);

            _loaded = true;
        }
    }
}