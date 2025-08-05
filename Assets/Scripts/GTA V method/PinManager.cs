using UnityEngine;
using System.Collections.Generic;

public class PinManager : MonoBehaviour
{
    [Header("References (same RectTransform as tiles)")]
    public RectTransform mapContainer;
    public RectTransform pinParent;   // assign your PinsContainer
    public GameObject pinPrefab;

    private struct PinData
    {
        public RectTransform rect;
        public Vector2 uv;
    }

    private readonly List<PinData> pins = new();

    public void PlacePinAtUIPosition(Vector2 uiLocalPos)
    {
        // 1) Spawn under pinParent (so ReloadTiles never touches it)
        GameObject go = Instantiate(pinPrefab, pinParent);
        RectTransform rt = go.GetComponent<RectTransform>();

        // 2) Compute normalized UV in [0..1]
        Vector2 size = mapContainer.sizeDelta;
        Vector2 half = size * 0.5f;
        Vector2 uv = (uiLocalPos + half) / size;

        // 3) Store and position
        pins.Add(new PinData { rect = rt, uv = uv });
        rt.anchoredPosition = uiLocalPos;
    }

    public void RepositionPins()
    {
        Vector2 size = mapContainer.sizeDelta;
        Vector2 half = size * 0.5f;

        foreach (var p in pins)
        {
            Vector2 pos = (p.uv * size) - half;
            p.rect.anchoredPosition = pos;
        }
    }

    public void ClearPins()
    {
        foreach (var p in pins)
            Destroy(p.rect.gameObject);
        pins.Clear();
    }
}
