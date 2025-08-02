using UnityEngine;

[CreateAssetMenu(fileName = "CityLocation", menuName = "Map/CityLocation", order = 1)]
public class CityLocation : ScriptableObject
{
    public string cityName;
    public float latitude;
    public float longitude;
}
