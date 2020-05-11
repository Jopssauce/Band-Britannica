using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IntensityData
{
    public List<float> IDatas;
}

[System.Serializable]
public class Intensity
{
    public List<IntensityData> Levels;
}
