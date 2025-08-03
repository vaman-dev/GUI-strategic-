using UnityEngine;

public class CityMarker : MonoBehaviour
{
    public string cityName;
    public float latitude;
    public float longitude;
    public IndiaMapZoom mapZoom;

    void OnMouseDown()
    {
        Debug.Log($"[Marker Clicked] Zooming to city: {cityName} at ({latitude}, {longitude})");

        int zoomLevel = Mathf.Min(mapZoom.GetCurrentZoom() + 1, mapZoom.maxZoom);
        mapZoom.CenterAndZoomToCity(latitude, longitude, zoomLevel);

        Debug.Log($"[Zoom Action] New zoom level: {zoomLevel} for city: {cityName}");
    }
}
