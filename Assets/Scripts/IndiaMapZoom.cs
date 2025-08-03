using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.InputSystem;

public class IndiaMapZoom : MonoBehaviour
{
    public int initialZoom = 5;
    public int minZoom = 5;
    public int maxZoom = 10;

    public GameObject tilePrefab;
    public Camera mainCamera;

    private int currentZoom;
    private float tileSize = 1f;
    private float zoomScale = 1f;
    private float scaleStep = 0.1f;
    private float mapCenterLat = 20.5937f; // default India center
    private float mapCenterLon = 78.9629f;

    private Dictionary<string, GameObject> tileCache = new Dictionary<string, GameObject>();

    void Start()
    {
        currentZoom = initialZoom;
        mainCamera.orthographic = true;
        mainCamera.transform.position = new Vector3(0, 0, -10f);
        transform.position = Vector3.zero;

        LoadTilesCenteredOnIndia();
        FitMapToCamera();
    }

    public void OnScrollZoom(float scrollDelta)
    {
        // Adjust scale only
        zoomScale += scrollDelta > 0 ? scaleStep : -scaleStep;
        zoomScale = Mathf.Clamp(zoomScale, 0.5f, 2f);
        transform.localScale = Vector3.one * zoomScale;

        // Recalculate zoom level based on scale
        int newZoom = Mathf.Clamp(initialZoom + Mathf.FloorToInt((zoomScale - 1f) * 4f), minZoom, maxZoom);

        if (newZoom != currentZoom)
        {
            currentZoom = newZoom;
            ClearTiles();
            LoadTilesCenteredOn(GetMapCenterLat(), GetMapCenterLon(), currentZoom);
        }
    }

    void LoadTilesCenteredOnIndia()
    {
        float lat = 20.5937f;
        float lon = 78.9629f;
        LoadTilesCenteredOn(lat, lon, currentZoom);
    }

    void LoadTilesCenteredOn(float lat, float lon, int zoom)
    {
        int centerX = LonToTileX(lon, zoom);
        int centerY = LatToTileY(lat, zoom);
        int radius = 2;

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                int tileX = centerX + dx;
                int tileY = centerY + dy;
                Vector3 localPos = new Vector3(dx * tileSize, -dy * tileSize, 0);
                LoadTile(zoom, tileX, tileY, localPos);
            }
        }
    }

    void LoadTile(int zoom, int x, int y, Vector3 localPos)
    {
        string path = Path.Combine(Application.dataPath, "Map Tiles", zoom.ToString(), x.ToString(), y.ToString() + ".png");
        if (!File.Exists(path)) return;

        string key = $"{zoom}_{x}_{y}";
        if (tileCache.ContainsKey(key)) return;

        byte[] data = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(256, 256);
        tex.LoadImage(data);
        tex.Apply();

        GameObject tile = Instantiate(tilePrefab, transform);
        tile.transform.localPosition = localPos;
        tile.transform.localScale = Vector3.one * tileSize;
        tile.GetComponent<Renderer>().material.mainTexture = tex;

        tileCache[key] = tile;
    }

    void ClearTiles()
    {
        foreach (var tile in tileCache.Values)
        {
            Destroy(tile);
        }
        tileCache.Clear();
    }

    void FitMapToCamera()
    {
        float camHeight = 2f * mainCamera.orthographicSize;
        float camWidth = camHeight * mainCamera.aspect;

        float mapWidth = tileSize * 5;
        float mapHeight = tileSize * 5;

        float scaleX = camWidth / mapWidth;
        float scaleY = camHeight / mapHeight;
        float fitScale = Mathf.Min(scaleX, scaleY);

        zoomScale = fitScale;
        transform.localScale = Vector3.one * zoomScale;
    }

    float GetMapCenterLat() => mapCenterLat;
    float GetMapCenterLon() => mapCenterLon;

    int LonToTileX(float lon, int zoom)
    {
        return (int)((lon + 180f) / 360f * (1 << zoom));
    }

    int LatToTileY(float lat, int zoom)
    {
        float latRad = lat * Mathf.Deg2Rad;
        return (int)((1f - Mathf.Log(Mathf.Tan(latRad) + 1f / Mathf.Cos(latRad)) / Mathf.PI) / 2f * (1 << zoom));
    }

    public int GetCurrentZoom()
    {
        return currentZoom;
    }

    public void CenterAndZoomToCity(float lat, float lon, int zoomLevel)
    {
        mapCenterLat = lat;
        mapCenterLon = lon;
        currentZoom = Mathf.Clamp(zoomLevel, minZoom, maxZoom);
        ClearTiles();
        LoadTilesCenteredOn(lat, lon, currentZoom);
    }


    public Vector3 LatLonToUnity(float lat, float lon)
    {
        int tileX = LonToTileX(lon, currentZoom);
        int tileY = LatToTileY(lat, currentZoom);

        int centerX = LonToTileX(GetMapCenterLon(), currentZoom);
        int centerY = LatToTileY(GetMapCenterLat(), currentZoom);

        int dx = tileX - centerX;
        int dy = tileY - centerY;

        return new Vector3(dx * tileSize, -dy * tileSize, 0f);
    }
}