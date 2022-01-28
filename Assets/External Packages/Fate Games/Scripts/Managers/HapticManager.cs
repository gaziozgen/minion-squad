using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

namespace FateGames
{
    public class HapticManager : MonoBehaviour
    {
        private float delay = 0.5f;
        private float latestHapticTime = -0.5f;
        private static HapticManager instance;

        public static HapticManager Instance { get => instance; }

        private void Awake()
        {
            if (!instance)
                instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public void DoHaptic()
        {
            if (Time.time > latestHapticTime + delay)
            {
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
                latestHapticTime = Time.time;
            }
        }

    }
}