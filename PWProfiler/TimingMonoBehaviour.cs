using System.Collections.Generic;
using PluginAPI.Core;
using UnityEngine;
namespace PWProfiler
{
    public class TimingMonoBehaviour : MonoBehaviour
    {
        internal float[] DeltaTime;
        internal List<float> SlowDeltaTime;

        internal float AverageDeltaTime
        {
            get
            {
                float total = 0;
                for (int i = 0; i < DeltaTime.Length; i++)
                    total += DeltaTime[i];
                return total / DeltaTime.Length;
            }
        }
        private int _i;
        private static int FramesToCheck => PWProfiler.Singleton.Config.AverageFrameSampleAmount;
        private void Awake()
        {
            DeltaTime = new float[FramesToCheck];
            SlowDeltaTime = new List<float>();
            _i = 0;
        }

        private static float Delta => Time.deltaTime;
        private float LogBelowDeltaTime => PWProfiler.Singleton.Config.LowTps;
        private void Update()
        {
            DeltaTime[_i] = Delta;
            if (Time.deltaTime > LogBelowDeltaTime)
                SlowDeltaTime.Add(Delta);
            //Timing.DeltaTime
            _i++;
            if (_i > FramesToCheck - 2)
                _i = 0;
            if (SlowDeltaTime.Count > 30)
            {
                SlowDeltaTime.Clear();
                Log.Error($"SlowDeltaTime is clogging up. Server is running below the recommended {1/LogBelowDeltaTime} tps ({LogBelowDeltaTime}). Current TPS: {1/AverageDeltaTime} tps ({AverageDeltaTime})");
            }
        }
    }
}