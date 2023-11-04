using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Xml.Schema;

public class test : MonoBehaviour
{
    float GetIncrementingValue(float interval, float maxValue) {
        var count = Time.time;
        var fraction = math.frac(count);
        var roundvalue = math.floor(fraction * 10);
        var adjustedvalue = math.floor(roundvalue * (maxValue / 10));
        return adjustedvalue;
}

void Update() {
    float result = GetIncrementingValue(0.1f, 5); // Call the function with a 100 ms interval and a maximum value of 5
    // Use the 'result' variable in your shader calculations
    Debug.Log(result);
}

}
