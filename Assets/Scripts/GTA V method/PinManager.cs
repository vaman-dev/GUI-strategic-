using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PinManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform mapContainer;     // Container holding the tile map (zoomed/panned)
    public GameObject pinPrefab;           // Pin prefab (UI Image or Button)
    public RectTransform pinsParent;       // Parent to hold pins (should be child of mapContainer)
    public Camera uiCamera;                // UI Camera
    public RectTransform clickTargetPanel; // Transparent panel catching clicks (MapClickCatcher)

    private class PinData
    {
        public RectTransform rect;
        public Vector2 normalizedPos; // Relative position (0–1) on the map
    }

    private List<PinData> pins = new List<PinData>();

    void Awake()
    {
        if (clickTargetPanel == null)
        {
            clickTargetPanel = mapContainer.Find("MapClickCatcher") as RectTransform;
        }

        if (pinsParent == null)
        {
            var existing = mapContainer.Find("PinsParent") as RectTransform;
            if (existing != null)
            {
                pinsParent = existing;
            }
            else
            {
                GameObject go = new GameObject("PinsParent", typeof(RectTransform));
                go.transform.SetParent(mapContainer, false);
                pinsParent = go.GetComponent<RectTransform>();
                pinsParent.anchorMin = Vector2.zero;
                pinsParent.anchorMax = Vector2.one;
                pinsParent.sizeDelta = Vector2.zero;
                pinsParent.pivot = mapContainer.pivot;
            }
            Debug.LogWarning("[PinManager] pinsParent was null—auto-created/found it under mapContainer.");
        }

        if (clickTargetPanel != null)
        {
            // Optional: Disable RaycastTarget on the catcher panel's Image
            var img = clickTargetPanel.GetComponent<Image>();
            if (img != null) img.raycastTarget = true; // Allow it to block events
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mapContainer, Input.mousePosition, uiCamera, out localPoint))
            {
                if (IsInsideMap(localPoint))
                    PlacePinAtLocalPoint(localPoint);
            }
        }
    }

    bool IsInsideMap(Vector2 localPoint)
    {
        Rect r = mapContainer.rect;
        return localPoint.x >= r.xMin && localPoint.x <= r.xMax
            && localPoint.y >= r.yMin && localPoint.y <= r.yMax;
    }

    void PlacePinAtLocalPoint(Vector2 localPoint)
    {
        float normX = (localPoint.x + mapContainer.rect.width * .5f) / mapContainer.rect.width;
        float normY = (localPoint.y + mapContainer.rect.height * .5f) / mapContainer.rect.height;

        RectTransform pinRect = Instantiate(pinPrefab, pinsParent).GetComponent<RectTransform>();
        pinRect.pivot = new Vector2(0.5f, 1f); // Tip of the pin
        pinRect.anchoredPosition = NormalizedToAnchored(normX, normY);

        pins.Add(new PinData
        {
            rect = pinRect,
            normalizedPos = new Vector2(normX, normY)
        });

        Debug.Log($"\uD83D\uDCCC Pin placed at normalized: ({normX:F2}, {normY:F2})");
    }

    Vector2 NormalizedToAnchored(float normX, float normY)
    {
        float x = normX * mapContainer.rect.width - mapContainer.rect.width * .5f;
        float y = normY * mapContainer.rect.height - mapContainer.rect.height * .5f;
        return new Vector2(x, y);
    }

    public void RepositionPins()
    {
        foreach (var pin in pins)
            pin.rect.anchoredPosition = NormalizedToAnchored(pin.normalizedPos.x, pin.normalizedPos.y);
    }
}
