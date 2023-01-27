using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
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
        private int framesToCheck = 120;
        private void Awake()
        {
            DeltaTime = new float[framesToCheck];
            SlowDeltaTime = new List<float>();
            _i = 0;
        }

        private float delta => Time.deltaTime;
        private float LogBelowDeltaTime => PwProfiler.Singleton.ProblematicFPS;
        private void Update()
        {
            DeltaTime[_i] = delta;
            if (Time.deltaTime > LogBelowDeltaTime)
                SlowDeltaTime.Add(delta);
            //Timing.DeltaTime
            _i++;
            if (_i > framesToCheck - 2)
                _i = 0;
            if (SlowDeltaTime.Count > 30)
            {
                SlowDeltaTime.Clear();
                Log.Error($"SlowDeltaTime is clogging up. Server is running below the recommended {1/LogBelowDeltaTime} fps ({LogBelowDeltaTime}). Current FPS: {1/AverageDeltaTime} fps ({AverageDeltaTime})");
            }
        }
    }
}