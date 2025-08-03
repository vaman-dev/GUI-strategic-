using UnityEngine;

public class MapButtonController : MonoBehaviour
{
    public IndiaMapZoom mapZoom;
    

    // Original map center (you can change these values as needed)
    private float originalLat = 28.6139f; // New Delhi
    private float originalLon = 77.2090f;
    private int originalZoom = 6;

    public void GoToMumbai()
    {
        float lat = 19.0760f;
        float lon = 72.8777f;
        int zoomLevel = 8;
        mapZoom.CenterAndZoomToCity(lat, lon, zoomLevel);
    }

    public void GoToDelhi()
    {
        float lat = 28.6139f;
        float lon = 77.2090f;
        int zoomLevel = 8;
        mapZoom.CenterAndZoomToCity(lat, lon, zoomLevel);
    }

    public void GoToSrinagar()
    {
        float lat = 34.0837f;
        float lon = 74.7973f;
        int zoomLevel = 8;
        mapZoom.CenterAndZoomToCity(lat, lon, zoomLevel);
    }

    public void GoToKolkata()
    {
        float lat = 22.5726f;
        float lon = 88.3639f;
        int zoomLevel = 8;
        mapZoom.CenterAndZoomToCity(lat, lon, zoomLevel);
    }

    public void GoToPune()
    {
        float lat = 18.5204f;
        float lon = 73.8567f;
        int zoomLevel = 8;
        mapZoom.CenterAndZoomToCity(lat, lon, zoomLevel);
    }

    public void GoToJaipur()
    {
        float lat = 26.9124f;
        float lon = 75.7873f;
        int zoomLevel = 8;
        mapZoom.CenterAndZoomToCity(lat, lon, zoomLevel);
    }

    public void ResetView()
    {
        mapZoom.CenterAndZoomToCity(originalLat, originalLon, originalZoom);
    }
}
