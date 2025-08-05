//using UnityEngine;
//using UnityEngine.UI;
//using System.IO;
//using System.Collections.Generic;
//using UnityEngine.EventSystems;

//public class TileLoader : MonoBehaviour, IDragHandler, IBeginDragHandler
//{
//    public RectTransform mapContainer;
//    public GameObject tilePrefab;
//    public int minZoom = 5, maxZoom = 10, currentZoom = 5;
//    public int tileSize = 256;
//    //[SerializeField] private Transform tileContainer;

//    private Dictionary<Vector2Int, GameObject> loadedTiles = new();
//    private Vector2 dragStart;
//    private Vector2 originalAnchoredPosition;

//    void Start()
//    {
//        originalAnchoredPosition = mapContainer.anchoredPosition;
//        LoadTiles(currentZoom);
//    }

//    void Update()
//    {
//        if (Input.mouseScrollDelta.y != 0)
//        {
//            bool zoomIn = Input.mouseScrollDelta.y > 0;
//            Zoom(zoomIn);
//        }
//    }

//    public void Zoom(bool zoomIn)
//    {
//        int prevZoom = currentZoom;
//        currentZoom += zoomIn ? 1 : -1;
//        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

//        if (currentZoom != prevZoom)
//        {
//            ReloadTiles(currentZoom);
//            mapContainer.anchoredPosition = originalAnchoredPosition; // Reset to original position
//        }
//    }
//    void ReloadTiles(int zoom)
//    {
//        // Only destroy tile GameObjects, skip PinsContainer
//        foreach (Transform child in mapContainer)
//        {
//            if (child.name != "PinsContainer")  // replace with actual name of your container
//            {
//                Destroy(child.gameObject);
//            }
//        }

//        loadedTiles.Clear();
//        LoadTiles(zoom);
//    }

//    void LoadTiles(int z)
//    {
//        string zoomFolder = Path.Combine(Application.streamingAssetsPath, "Map Tiles", z.ToString());
//        if (!Directory.Exists(zoomFolder))
//        {
//            Debug.LogError("Zoom folder not found: " + zoomFolder);
//            return;
//        }

//        List<(int x, int y, string path)> tileInfos = new();

//        foreach (string xDir in Directory.GetDirectories(zoomFolder))
//        {
//            if (!int.TryParse(Path.GetFileName(xDir), out int x)) continue;

//            foreach (string imgFile in Directory.GetFiles(xDir, "*.png"))
//            {
//                if (!int.TryParse(Path.GetFileNameWithoutExtension(imgFile), out int y)) continue;
//                tileInfos.Add((x, y, imgFile));
//            }
//        }

//        if (tileInfos.Count == 0) return;

//        int minX = int.MaxValue, maxX = int.MinValue;
//        int minY = int.MaxValue, maxY = int.MinValue;

//        foreach ((int x, int y, _) in tileInfos)
//        {
//            minX = Mathf.Min(minX, x); maxX = Mathf.Max(maxX, x);
//            minY = Mathf.Min(minY, y); maxY = Mathf.Max(maxY, y);
//        }

//        int width = (maxX - minX + 1) * tileSize;
//        int height = (maxY - minY + 1) * tileSize;
//        mapContainer.sizeDelta = new Vector2(width, height);

//        foreach ((int x, int y, string path) in tileInfos)
//        {
//            GameObject tile = Instantiate(tilePrefab, mapContainer);
//            tile.name = $"Tile_{x}_{y}";

//            byte[] fileData = File.ReadAllBytes(path);
//            Texture2D tex = new Texture2D(2, 2);
//            tex.LoadImage(fileData);
//            tile.GetComponent<RawImage>().texture = tex;

//            RectTransform rt = tile.GetComponent<RectTransform>();
//            rt.sizeDelta = new Vector2(tileSize, tileSize);
//            rt.anchorMin = Vector2.up;
//            rt.anchorMax = Vector2.up;
//            rt.pivot = new Vector2(0, 1);
//            rt.anchoredPosition = new Vector2((x - minX) * tileSize, -(y - minY) * tileSize);

//            loadedTiles[new Vector2Int(x, y)] = tile;
//        }

//        CenterMapOnStart();
//    }

//    void CenterMapOnStart()
//    {
//        mapContainer.anchoredPosition = originalAnchoredPosition;
//    }

//    public void OnBeginDrag(PointerEventData eventData)
//    {
//        dragStart = eventData.position;
//    }

//    public void OnDrag(PointerEventData eventData)
//    {
//        if (eventData.button == PointerEventData.InputButton.Left)
//        {
//            Vector2 delta = eventData.position - dragStart;
//            dragStart = eventData.position;
//            mapContainer.anchoredPosition += delta;
//        }
//    }
//}


using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TileLoader : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    public RectTransform mapContainer;
    public GameObject tilePrefab;
    public int minZoom = 5, maxZoom = 10, currentZoom = 5;
    public int tileSize = 256;

    public PinManager pinManager;

    private Dictionary<Vector2Int, GameObject> loadedTiles = new();
    private Vector2 dragStart;
    private Vector2 originalAnchoredPosition;

    void Start()
    {
        // Center pivot/anchors for center-based zoom
        mapContainer.pivot = new Vector2(0.5f, 0.5f);
        mapContainer.anchorMin = mapContainer.anchorMax = new Vector2(0.5f, 0.5f);
        originalAnchoredPosition = mapContainer.anchoredPosition;
        LoadTiles(currentZoom);
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            Zoom(Input.mouseScrollDelta.y > 0);
        }
    }

    public void Zoom(bool zoomIn)
    {
        int prev = currentZoom;
        currentZoom = Mathf.Clamp(currentZoom + (zoomIn ? 1 : -1), minZoom, maxZoom);
        if (currentZoom == prev) return;
        ReloadTiles(currentZoom);
    }

    void ReloadTiles(int zoom)
    {
        foreach (Transform child in mapContainer)
            if (child.name != "PinsContainer") Destroy(child.gameObject);
        loadedTiles.Clear();
        // Do not reset mapContainer.anchoredPosition—keep center
        LoadTiles(zoom);
    }

    void LoadTiles(int z)
    {
        string zoomFolder = Path.Combine(Application.streamingAssetsPath, "Map Tiles", z.ToString());
        if (!Directory.Exists(zoomFolder))
        {
            Debug.LogError("Zoom folder not found: " + zoomFolder);
            return;
        }

        var infos = new List<(int x, int y, string path)>();
        foreach (var xDir in Directory.GetDirectories(zoomFolder))
        {
            if (!int.TryParse(Path.GetFileName(xDir), out int x)) continue;
            foreach (var img in Directory.GetFiles(xDir, "*.png"))
            {
                if (!int.TryParse(Path.GetFileNameWithoutExtension(img), out int y)) continue;
                infos.Add((x, y, img));
            }
        }
        if (infos.Count == 0) return;

        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;
        foreach (var (x, y, _) in infos)
        {
            minX = Mathf.Min(minX, x); maxX = Mathf.Max(maxX, x);
            minY = Mathf.Min(minY, y); maxY = Mathf.Max(maxY, y);
        }

        int w = (maxX - minX + 1) * tileSize;
        int h = (maxY - minY + 1) * tileSize;
        mapContainer.sizeDelta = new Vector2(w, h);

        foreach (var (x, y, path) in infos)
        {
            var tile = Instantiate(tilePrefab, mapContainer);
            tile.name = $"Tile_{x}_{y}";

            var data = File.ReadAllBytes(path);
            var tex = new Texture2D(2, 2);
            tex.LoadImage(data);
            tile.GetComponent<RawImage>().texture = tex;

            var rt = tile.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(tileSize, tileSize);
            rt.anchorMin = rt.anchorMax = Vector2.up;
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = new Vector2((x - minX) * tileSize, -(y - minY) * tileSize);

            loadedTiles[new Vector2Int(x, y)] = tile;
        }

        CenterMapOnStart();

        if (pinManager != null)
        {
            pinManager.RepositionPins();
        }
    }

    public void OnBeginDrag(PointerEventData e)
    {
        dragStart = e.position;
    }
    public void OnDrag(PointerEventData e)
    {
        if (e.button == PointerEventData.InputButton.Left)
        {
            var delta = e.position - dragStart;
            dragStart = e.position;
            mapContainer.anchoredPosition += delta;
        }
    }


    void CenterMapOnStart()
    {
        mapContainer.anchoredPosition = originalAnchoredPosition;
    }

}
