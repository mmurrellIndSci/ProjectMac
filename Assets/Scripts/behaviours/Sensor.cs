using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fortive.Mac
{
    public class Sensor : MonoBehaviour
    {
        public enum SensorReadingType
        {
            PPM,
            PercentVolume
        }

        public double Reading;
        public Gas GasType;
        public SensorReadingType ReadingType;
        public double LowAlarmThreshold = 10;
        public double HighAlarmThreshold = 20;
        public AlarmLevel AlarmLevel = AlarmLevel.None;


        // Update is called once per frame
        void Update()
        {
            this.Reading = CalculateReading();
            this.AlarmLevel = CalculateAlarmLevel();

        }

        private List<GasEmitter> GetGasSources()
        {
            var sources = GameObject.FindGameObjectsWithTag("Emitter")
                .SelectMany(o => o.GetComponents<GasEmitter>())
                .ToList();
            return sources;
        }

        private AlarmLevel CalculateAlarmLevel()
        {
            if (Reading <= 0)
            {
                return AlarmLevel.None;
            }
            if (Reading < LowAlarmThreshold)
            {
                return AlarmLevel.GasDetected;
            }
            else if (Reading < HighAlarmThreshold)
            {
                return AlarmLevel.LowAlarm;
            }
            else
            {
                return AlarmLevel.HighAlarm;
            }
        }

        private double CalculateReading()
        {
            double gasLevel = 0;
            var sources = GetGasSources()
                .Where(s => s.GasType == this.GasType)
                .Where(s => s.State == GasEmitter.EmitterState.Open).ToList();
            foreach (var source in sources)
            {
                Vector3 difference = this.transform.position - source.transform.position;
                var level = source.GasLevel / Mathf.Sqrt(difference.x * difference.x + difference.y * difference.y + difference.z * difference.z);
                gasLevel += level;
            }

            return gasLevel;
        }
    }
}
