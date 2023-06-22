using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

namespace Fortive.Mac
{
    public class Detector : MonoBehaviour
    {
        private AudioSource audioSource;
        private List<Detector> peers;
        private MenuState _lastMenuState = MenuState.Off;


        public List<GameObject> Lights = new List<GameObject>();
        public List<Sensor> Sensors = new List<Sensor>();

        public MeshRenderer mesh;
        public bool IsInPanicMode;
        public MenuState MenuState = MenuState.Off;
        public AlarmLevel AlarmLevel = AlarmLevel.None;
        public AudioClip LowAlarm;
        public AudioClip HighAlarm;
        public AudioClip PanicAlarm;
        public AudioClip PowerOn;
        public AudioClip PowerOff;
        public AudioClip Connected;
        public AudioClip Disconnect;

        public Color LowAlarmColor = Color.yellow;
        public Color HighAlarmColor = Color.red;
        public Color PanicColor = Color.magenta;

        public Material OffScreen;
        public Material StartupScreen;
        public Material NormalReadingScreen;
        public Material PeerScreen;
        public Material ZeroScreen;
        public Material BumpScreen;
        public Material LowAlarmScreen;
        public Material HighAlarmScreen;
        public Material PanicScreen;

        #region event-handlers
        // Start is called before the first frame update
        void Start()
        {
            this.peers = GameObject.FindGameObjectsWithTag("Detector")
                .SelectMany(obj => obj.GetComponents<Detector>())
                .ToList();
            this.audioSource = gameObject.GetComponent<AudioSource>();
            foreach (var light in Lights)
            {
                var mesh = light.GetComponent<MeshRenderer>();
                if (mesh?.material != null)
                {
                    mesh.material.color = Color.clear;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            SetAlarm();
            SetMenuScreen();
        }

        public void TogglePanicMode()
        {
            if (MenuState != MenuState.Off && MenuState != MenuState.Startup)
            {
                IsInPanicMode = !IsInPanicMode;
            }
        }

        public bool IsPoweredOn()
            => MenuState != MenuState.Off && MenuState != MenuState.Startup;

        public void OnModePress()
        {
            
            switch (MenuState)
            {
                case MenuState.Normal:
                    MenuState = MenuState.Peers;
                    break;
                case MenuState.Peers:
                    MenuState = MenuState.Zero;
                    break;
                case MenuState.Zero:
                    MenuState = MenuState.Bump;
                    break;
                case MenuState.Bump:
                    MenuState = MenuState.Normal;
                    break;
            }
            

        }
        #endregion

        #region helpers
        private AlarmLevel GetAlarmLevel()
        {
            switch (MenuState)
            {
                case MenuState.Off:
                case MenuState.Startup:
                    return AlarmLevel.None;
            }

            if (this.IsInPanicMode)
            {
                return AlarmLevel.PanicAlarm;
            }
            else if (this.peers.Any(p => p.IsPoweredOn() && p.IsInPanicMode))
            {
                return AlarmLevel.PeerAlarm;
            }

            return Sensors.Select(s => s.AlarmLevel).DefaultIfEmpty(AlarmLevel.None).Max();
        }


        private void SetMenuScreen()
        {
            Material material = MenuState switch
            {
                MenuState.Off => OffScreen,
                MenuState.Startup => StartupScreen,
                MenuState.Normal => NormalReadingScreen,
                MenuState.Peers => PeerScreen,
                MenuState.Zero => ZeroScreen,
                MenuState.Bump => BumpScreen,
                _ => OffScreen
            };

            if (IsPoweredOn())
            {
                material = AlarmLevel switch
                {
                    AlarmLevel.LowAlarm => LowAlarmScreen,
                    AlarmLevel.HighAlarm => HighAlarmScreen,
                    AlarmLevel.PeerAlarm => PanicScreen,
                    AlarmLevel.PanicAlarm => PanicScreen,
                    _ => material
                };
            }

            if (mesh.material != material)
            {
                mesh.material = material;
            }

            if (_lastMenuState == MenuState)
            {
                return;
            }

            _lastMenuState = MenuState;
            switch (MenuState)
            {
                case MenuState.Startup:
                    audioSource.PlayOneShot(PowerOn);
                    break;
                case MenuState.Off:
                    audioSource.PlayOneShot(PowerOff);
                    break;
            }
        }

        public void ToggleOnOff()
        {
            if (MenuState == MenuState.Off)
            {
                MenuState = MenuState.Startup;
                _ = Task.Delay(TimeSpan.FromSeconds(5))
                    .ContinueWith(tsk =>
                    {
                        if (MenuState == MenuState.Startup)
                        {
                            MenuState = MenuState.Normal;
                        }
                    });
            }
            else
            {
                IsInPanicMode = false;
                MenuState = MenuState.Off;
            }
        }

        private void SetAlarm()
        {
            var alarmLevel = GetAlarmLevel();
            if (alarmLevel != AlarmLevel)
            {
                AlarmLevel = alarmLevel;
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }

                AudioClip audio = null;
                Color color = Color.clear;

                switch (AlarmLevel)
                {
                    case AlarmLevel.LowAlarm:
                        audio = LowAlarm;
                        color = LowAlarmColor;
                        break;
                    case AlarmLevel.HighAlarm:
                        audio = HighAlarm;
                        color = HighAlarmColor;
                        break;
                    case AlarmLevel.PeerAlarm:
                    case AlarmLevel.PanicAlarm:
                        audio = PanicAlarm;
                        color = PanicColor;
                        break;
                }

                if (audio != null)
                {
                    audioSource.clip = audio;
                    audioSource.Play();
                }
                foreach (var light in Lights)
                {
                    var mesh = light.GetComponent<MeshRenderer>();
                    if (mesh?.material != null)
                    {
                        mesh.material.color = color;
                    }
                }
            }
        }
        #endregion
    }
}
