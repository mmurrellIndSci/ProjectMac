using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public enum SensorReadingType
    {
        PPM, 
        PercentVolume
    }


    public enum SensorType
    {
        CO,
        H2S,
        O2,
        LEL
    }

    public float reading;
    public SensorType sensor_type;
    public SensorReadingType reading_type;
    public float low_alarm_threshhold = 10;
    public float high_alarm_threshhold = 20;

    public List<GasOutput> gases;

    // Start is called before the first frame update
    void Start()
    {
        // This could potentially be modified to automatically search for all gases in the level 
    }

    // Update is called once per frame
    void Update()
    {
        if (gases.Count > 0)
        {
            foreach (GasOutput gas in gases)
            {
                Calculate_GasLevel(gas);
            }
        }
    }

    private void Calculate_GasLevel(GasOutput gas)
    {
        if (!gas.is_active) return;
        if ((int)gas.gas_type == (int)sensor_type)
        {
            Vector3 difference = transform.position - gas.gas_transform.position;
            reading = gas.gas_level * Mathf.Sqrt(difference.x * difference.x + difference.y * difference.y + difference.z * difference.z);
        }

    }
}
