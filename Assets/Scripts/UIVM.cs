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
    public string LapTime;
    public string Delta;
    public int Lap;
    public string SessionTime;

    public Texture2D Map;
    public int MapWidth;
    public int MapHeight;

    public int PlayerX;
    public int PlayerY;
    public Color LapIndicator;
    public void UpdateSpeedKmh()
    {
        SpeedKmh = (int)MathF.Round(Speed * 3.6f);
        SpeedKmhString = MathF.Round(Speed * 3.6f, 1).ToString(CultureInfo.InvariantCulture) + "Km/H";
    }
}
