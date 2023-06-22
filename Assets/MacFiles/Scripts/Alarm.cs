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
    public AlarmLevel alarm_level;
    public GameObject matgameobject;
    private Material mat;

    private void Start()
    {
        mat = matgameobject.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        GetHighestAlarm();
        switch (alarm_level)
        {
            case AlarmLevel.GasDetected:
                mat.color = new Color(1, 1, 1, 1);
                break;
            case AlarmLevel.None:
                mat.color = new Color(1, 0, 1, 1);
                break;
            case AlarmLevel.LowAlarm:
                mat.color = new Color(1, 0, 0, 1);
                break;
            case AlarmLevel.HighAlarm:
                mat.color = new Color(0, 1, 1, 1);
                break;
            case AlarmLevel.PanicAlarm:
                mat.color = new Color(0, 0, 1, 1);
                break;
            case AlarmLevel.PeerAlarm:
                mat.color = new Color(1, 1, 1, 1);
                break;

        }
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
        //Disable for now until button detection is better
        //alarm_level = AlarmLevel.PanicAlarm;
    }

}
