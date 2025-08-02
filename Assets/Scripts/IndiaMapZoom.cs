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
    private float tileSize = 5f;
    private float zoomScale = 1f;
    private float scaleStep = 0.1f;

    private Dictionary<string, GameObject> tileCache = new Dictionary<string, GameObject>();

    private float currentCenterLat = 28.6139f; // New Delhi (Default)
    private float currentCenterLon = 77.2090f;

    void Start()
    {
        currentZoom = initialZoom;
        mainCamera.orthographic = true;
        mainCamera.transform.position = new Vector3(0, 0, -10f);

        FitMapToCamera();
        LoadTilesCenteredOn(currentCenterLat, currentCenterLon, currentZoom);
    }

    public void OnScrollZoom(float scrollDelta)
    {
        Vector3 mouseWorldBeforeZoom = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        zoomScale += scrollDelta > 0 ? scaleStep : -scaleStep;
        zoomScale = Mathf.Clamp(zoomScale, 0.5f, 2f);
        transform.localScale = Vector3.one * zoomScale;

        Vector3 mouseWorldAfterZoom = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 delta = mouseWorldBeforeZoom - mouseWorldAfterZoom;
        transform.position += delta;

        int newZoom = Mathf.Clamp(initialZoom + Mathf.FloorToInt((zoomScale - 1f) * 4f), minZoom, maxZoom);
        if (newZoom != currentZoom)
        {
            currentZoom = newZoom;
            ClearTiles();
            LoadTilesCenteredOn(currentCenterLat, currentCenterLon, currentZoom);
        }
    }

    public void LoadTilesCenteredOn(float lat, float lon, int zoom)
    {
        currentCenterLat = lat;
        currentCenterLon = lon;

        int centerX = LonToTileX(lon, zoom);
        int centerY = LatToTileY(lat, zoom);
        int radius = 2; // 5x5 grid

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

        transform.position = Vector3.zero;
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
        int tileCount = 5;
        float mapWidth = tileCount * tileSize;
        float mapHeight = tileCount * tileSize;

        float screenAspect = (float)Screen.width / Screen.height;
        float targetAspect = mapWidth / mapHeight;

        if (screenAspect >= targetAspect)
        {
            mainCamera.orthographicSize = mapHeight / 2f;
        }
        else
        {
            float differenceInSize = targetAspect / screenAspect;
            mainCamera.orthographicSize = (mapHeight / 2f) * differenceInSize;
        }

        zoomScale = 1f;
        transform.localScale = Vector3.one * zoomScale;

        mainCamera.transform.position = new Vector3(0, 0, -10f);
    }

    int LonToTileX(float lon, int zoom)
    {
        return (int)((lon + 180f) / 360f * (1 << zoom));
    }

    int LatToTileY(float lat, int zoom)
    {
        float latRad = lat * Mathf.Deg2Rad;
        return (int)((1f - Mathf.Log(Mathf.Tan(latRad) + 1f / Mathf.Cos(latRad)) / Mathf.PI) / 2f * (1 << zoom));
    }
}
