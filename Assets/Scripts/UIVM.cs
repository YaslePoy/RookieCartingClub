using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "UIVM", menuName = "Scriptable Objects/UIVM")]
public class UIVM : ScriptableObject
{

    public float Speed;
    
    public int SpeedKmh;
    public string SpeedKmhString;

    public void UpdateSpeedKmh()
    {
        SpeedKmh = (int)MathF.Round(Speed * 3.6f);
        SpeedKmhString = MathF.Round(Speed * 3.6f, 1).ToString(CultureInfo.InvariantCulture) + "Km/H";
        
    }
}
