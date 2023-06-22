using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mirrortransform : MonoBehaviour
{
    
    public Transform vpro_transform;
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(vpro_transform);
    }

    // Update is called once per frame
    void Update()
    {
        transform. = vpro_transform.transform;    
    }
}
