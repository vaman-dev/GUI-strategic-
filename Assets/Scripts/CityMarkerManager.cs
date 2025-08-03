using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CityData
{
    public string cityName;
    public float latitude;
    public float longitude;
}

public class CityMarkerManager : MonoBehaviour
{
    public IndiaMapZoom mapZoom;
    public GameObject markerPrefab;
    public List<CityData> cities;

    private List<GameObject> spawnedMarkers = new List<GameObject>();

    void Start()
    {
        SpawnAllCityMarkers();
    }

    public void SpawnAllCityMarkers()
    {
        foreach (CityData city in cities)
        {
            SpawnMarker(city);
        }
    }

    void SpawnMarker(CityData city)
    {
        Vector3 pos = mapZoom.LatLonToUnity(city.latitude, city.longitude);
        GameObject marker = Instantiate(markerPrefab, mapZoom.transform);
        marker.transform.localPosition = pos;
        marker.name = city.cityName;

        CityMarker markerScript = marker.GetComponent<CityMarker>();
        markerScript.cityName = city.cityName;
        markerScript.latitude = city.latitude;
        markerScript.longitude = city.longitude;
        markerScript.mapZoom = mapZoom;

        spawnedMarkers.Add(marker);
    }
}
