using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    public enum AlarmLevel
    {
        None,
        GasDetected,
        LowAlarm,
        HighAlarm,
        PeerAlarm,
        PanicAlarm,
    }

    public List<Sensor> sensors;
    AlarmLevel alarm_level;

    // Update is called once per frame
    void Update()
    {
        GetHighestAlarm();
    }

    private void GetHighestAlarm()
    {
        AlarmLevel highest_alarm = AlarmLevel.None;
        foreach(Sensor sensor in sensors)
        {
            AlarmLevel temp_alarm = DetectGas(sensor);
            if (temp_alarm > highest_alarm)
                highest_alarm = temp_alarm;
        }

        if (alarm_level != AlarmLevel.PanicAlarm)
        {
            if (alarm_level != highest_alarm)
            {
                // ALARM CHANGE EVENT
                alarm_level = highest_alarm;
            }
        }
    }

    private AlarmLevel DetectGas(Sensor sensor)
    {
        if (sensor.reading > sensor.high_alarm_threshhold)
            return AlarmLevel.HighAlarm;
        else if (sensor.reading > sensor.low_alarm_threshhold)
            return AlarmLevel.LowAlarm;
        else if (sensor.reading > 0)
            return AlarmLevel.GasDetected;
        else return AlarmLevel.None;
    }

    public void PanicAlarm()
    {
        alarm_level = AlarmLevel.PanicAlarm;
    }

}
