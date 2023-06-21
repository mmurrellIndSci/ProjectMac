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
        private List<GasEmitter> emitters = new List<GasEmitter>();
        private GasLevel currentLevel = GasLevel.None;
        private MeshRenderer mesh;

        public List<GameObject> lights = new List<GameObject>();
        public MenuState MenuState = MenuState.Normal;
        public AudioClip LowAlarm;
        public AudioClip HighAlarm;
        public AudioClip PanicAlarm;
        public AudioClip PowerOn;
        public AudioClip PowerOff;
        public AudioClip Connected;
        public AudioClip Disconnect;

        public Color OffColor = Color.black;
        public Color StartupColor = Color.gray;
        public Color NormalColor = Color.white;
        public Color PeersColor = Color.green;
        public Color ZeroColor = Color.blue;
        public Color BumpColor = Color.cyan;
        public Color LowAlarmColor = Color.yellow;
        public Color HighAlarmColor = Color.red;

        #region event-handlers
        // Start is called before the first frame update
        void Start()
        {
            mesh = gameObject.GetComponent<MeshRenderer>();
            LoadEmitters();
            LoadAudioSource();
        }

        // Update is called once per frame
        void Update()
        {
            SetAlarm();
            SetMenuScreen();
        }

        void OnModePress(bool isLongPress = false)
        {
            if (isLongPress)
            {
                ToggleOnOff();
            }
            else
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

        }
        #endregion

        #region helpers
        private void SetMenuScreen()
        {
            Color color = MenuState switch
            {
                MenuState.Off => OffColor,
                MenuState.Startup => StartupColor,
                MenuState.Normal => NormalColor,
                MenuState.Peers => PeersColor,
                MenuState.Zero => ZeroColor,
                MenuState.Bump => BumpColor,
                _ => OffColor
            };

            SetMenuScreen(color);
        }

        private void SetMenuScreen(Color color)
        {
            if (mesh.material.color != color)
            {
                mesh.material.color = color;
            }
        }

        private void ToggleOnOff()
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
                MenuState = MenuState.Off;
            }
        }


        private void LoadEmitters()
        {
            this.emitters = GameObject.FindGameObjectsWithTag("Emitter")
                .SelectMany(obj => obj.GetComponents<GasEmitter>())
                .ToList();
        }

        private void LoadAudioSource()
        {
            this.audioSource = gameObject.GetComponent<AudioSource>();
        }

        private void SetAlarm()
        {
            GasLevel alarmLevel; 
            switch (MenuState)
            {
                case MenuState.Off:
                case MenuState.Startup:
                    alarmLevel = GasLevel.None;

                    break;
                default:
                    alarmLevel = emitters.Select(e => e.Level).DefaultIfEmpty(GasLevel.None).Max();
                    break;
            }

            if (alarmLevel != currentLevel)
            {
                currentLevel = alarmLevel;
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }

                switch (currentLevel)
                {
                    case GasLevel.Low:
                        audioSource.clip = LowAlarm;
                        audioSource.Play();
                        foreach (var light in lights)
                        {
                            var mesh = light.GetComponent<MeshRenderer>();
                            if (mesh?.material != null)
                            {
                                mesh.material.color = LowAlarmColor;
                            }
                        }
                        break;
                    case GasLevel.High:
                        audioSource.clip = HighAlarm;
                        audioSource.Play();
                        foreach (var light in lights)
                        {
                            var mesh = light.GetComponent<MeshRenderer>();
                            if (mesh?.material != null)
                            {
                                mesh.material.color = HighAlarmColor;
                            }
                        }
                        break;
                    default:
                        foreach (var light in lights)
                        {
                            var mesh = light.GetComponent<MeshRenderer>();
                            if (mesh?.material != null)
                            {
                                mesh.material.color = Color.clear;
                            }
                        }
                        break;
                }
            }
        }
        #endregion
    }
}
