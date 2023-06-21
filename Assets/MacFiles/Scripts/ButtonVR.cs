using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonVR : MonoBehaviour
{

    public enum ButtonState
    {
        None,
        Pressed,
        Held,
        Released
    };

    public GameObject button;

    public UnityEvent onPress;
    public UnityEvent onRelease;
    GameObject presser;
    AudioSource sound;

    ButtonState buttonState = ButtonState.None;

    // Start is called before the first frame update
    void Start()
    {
        //sound = GetComponent<AudioSource>();
        buttonState = ButtonState.None;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (buttonState != ButtonState.Pressed || buttonState != ButtonState.Held)
        {
            button.transform.localPosition = new Vector3(0, 0.003f, 0);
            presser = other.gameObject;
            onPress.Invoke();
            //if(sound != null) sound.Play();
            buttonState = ButtonState.Pressed;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if(other == presser)
        //{
        button.transform.localPosition = new Vector3(0, 0.015f, 0);
        onRelease.Invoke();
        buttonState = ButtonState.Released;
        //}
    }

}
