using UnityEngine;

namespace SampleMod
{
    public static class Utils
    {
        public static FeatureSettings Settings => HighLogic.CurrentGame.Parameters.CustomParams<FeatureSettings>();

        public static void Log(object message)
        {
#if DEBUG
            Debug.Log($"[SampleMod] {message}");
#endif
        }

    }
}