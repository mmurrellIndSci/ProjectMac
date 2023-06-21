using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Fortive.Mac
{
    public class Detector : MonoBehaviour
    {
        private AudioSource audioSource;
        private List<GasEmitter> emitters = new List<GasEmitter>();
        private GasLevel currentLevel = GasLevel.None;

        public AudioClip LowAlarm;
        public AudioClip HighAlarm;
        public AudioClip PanicAlarm;
        public AudioClip PowerOn;
        public AudioClip PowerOff;
        public AudioClip Connected;
        public AudioClip Disconnect;

        public void OnLeftButtonPress()
        {
        }
        public void OnRightButtonPress()
        {

        }
        public void OnCenterButtonPress()
        {

        }
        public void OnPanicButtonPress()
        {

        }


        // Start is called before the first frame update
        void Start()
        {
            LoadEmitters();
            LoadAudioSource();
        }

        // Update is called once per frame
        void Update()
        {
            SetAlarm();
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
            var alarmLevel = emitters.Select(e => e.Level).DefaultIfEmpty(GasLevel.None).Max();
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
                        break;
                    case GasLevel.High:
                        audioSource.clip = HighAlarm;
                        audioSource.Play();
                        break;
                }
            }
        }
    }
}
