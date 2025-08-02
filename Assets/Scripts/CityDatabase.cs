using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CityDatabase", menuName = "Map/CityDatabase", order = 2)]
public class CityDatabase : ScriptableObject
{
    public List<CityLocation> cities;
}
