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
        LongPressed,
        Released
    };

    public GameObject button;

    public UnityEvent onPress;
    public UnityEvent onRelease;
    public UnityEvent onLongPress;
    public GameObject presser;
    public float press_timer = 0;
    public float max_press_time = 3.0f;

    public GameObject mat_go;
    private Material mat;


    ButtonState buttonState = ButtonState.None;

    // Start is called before the first frame update
    void Start()
    {
        buttonState = ButtonState.None;
        press_timer = 0;
        mat = mat_go.GetComponent<MeshRenderer>().material;
    }

    public void UpdateMaterial()
    {
        switch (buttonState)
        {
            case ButtonState.LongPressed:
                mat.color = new Color(1, 0, 1, 1);
                break;
            case ButtonState.Pressed:
                mat.color = new Color(1, 0, 0, 1);
                break;
            case ButtonState.Released:
                mat.color = new Color(0, 0, 1, 1);
                break;
        }
    }

    private void Update()
    {
        if (buttonState == ButtonState.Pressed)
        {
            press_timer += Time.deltaTime;
            if (press_timer > max_press_time)
            {
                buttonState = ButtonState.LongPressed;
                onLongPress.Invoke();
            }
        }
    }


    public void OnOVRSelect()
    {
        if (buttonState != ButtonState.Pressed || buttonState != ButtonState.LongPressed)
        {
            button.transform.localPosition = new Vector3(0, 0.003f, 0);
            //presser = other.gameObject;
            onPress.Invoke();
            buttonState = ButtonState.Pressed;
            press_timer = 0;
        }
    }

    public void OnOVRUnSelect()
    {
        button.transform.localPosition = new Vector3(0, 0.015f, 0);
        onRelease.Invoke();
        buttonState = ButtonState.Released;
        press_timer = 0;
    }

    /*
     
    private void OnTriggerEnter(Collider other)
    {
        if (buttonState != ButtonState.Pressed || buttonState != ButtonState.LongPressed)
        {
            button.transform.localPosition = new Vector3(0, 0.003f, 0);
            presser = other.gameObject;
            onPress.Invoke();
            if(sound != null) sound.Play();
            buttonState = ButtonState.Pressed;
            press_timer = 0;
        }
    }

     private void OnTriggerStay(Collider other)
    {
        if (buttonState == ButtonState.Pressed)
        {
            press_timer += Time.deltaTime;
            if (press_timer > max_press_time)
            {
                buttonState = ButtonState.LongPressed;
                onLongPress.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if(other == presser)
        //{
        button.transform.localPosition = new Vector3(0, 0.015f, 0);
        onRelease.Invoke();
        buttonState = ButtonState.Released;
        press_timer = 0;
        //}
    }*/

}
