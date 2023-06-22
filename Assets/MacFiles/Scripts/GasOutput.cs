using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasOutput : MonoBehaviour
{
    public enum GasType
    {
        CO,
        H2S,
        O2,
        C5H12
    };

    public GasType gas_type;
    public float gas_level;
    public Transform gas_transform;

}
