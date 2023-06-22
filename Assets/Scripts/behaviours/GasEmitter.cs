using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fortive.Mac
{
    public class GasEmitter : MonoBehaviour
    {
        public enum EmitterState
        {
            Closed,
            Open
        }

        public EmitterState State = EmitterState.Open;
        public Gas GasType = Gas.None;
        public double GasLevel = 0;

        public void ToggleState()
        {
            State = State == EmitterState.Open ? EmitterState.Closed : EmitterState.Open;
        }
    }
}
