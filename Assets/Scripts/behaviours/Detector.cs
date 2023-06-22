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
        private List<Detector> Peers = new List<Detector>();
        private GasLevel currentLevel = GasLevel.None;
        public MeshRenderer mesh;

        public List<GameObject> lights = new List<GameObject>();

        public bool IsInPanicMode;
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
        public Color PanicColor = Color.magenta;

        public Alarm AlarmLevel;

        #region event-handlers
        // Start is called before the first frame update
        void Start()
        {
            LoadEmitters();
            LoadAudioSource();
            mesh.material.color = OffColor;
        }

        // Update is called once per frame
        void Update()
        {
            SetAlarm();
            SetMenuScreen();
        }

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
        private bool PanicActivated()
        {
            return this.Peers.Any(p => p.MenuState != MenuState.Off && p.MenuState != MenuState.Startup && p.IsInPanicMode);
        }

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

            if (PanicActivated())
            {
                color = PanicColor;
            }

            if (mesh.material.color != color)
            {
                mesh.material.color = color;
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
            else if (!IsInPanicMode)
            {
                MenuState = MenuState.Off;
            }
        }


        private void LoadEmitters()
        {
            this.emitters = GameObject.FindGameObjectsWithTag("Emitter")
                .SelectMany(obj => obj.GetComponents<GasEmitter>())
                .ToList();
            this.Peers = GameObject.FindGameObjectsWithTag("Detector")
                .SelectMany(obj => obj.GetComponents<Detector>())
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
                    alarmLevel = PanicActivated() ? GasLevel.Panic :  emitters.Select(e => e.Level).DefaultIfEmpty(GasLevel.None).Max();
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
                    case GasLevel.Panic:
                        audioSource.clip = PanicAlarm;
                        audioSource.Play();
                        foreach (var light in lights)
                        {
                            var mesh = light.GetComponent<MeshRenderer>();
                            if (mesh?.material != null)
                            {
                                mesh.material.color = PanicColor;
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
