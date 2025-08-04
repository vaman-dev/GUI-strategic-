using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PinPlacer : MonoBehaviour
{
    [Header("References")]
    public RectTransform mapContainer; // Map container (your tile canvas)
    public GameObject pinPrefab;       // Pin prefab (Image or Button)
    public Transform pinsParent;       // Parent object to hold all pins
    public Camera uiCamera;            // UI camera (for ScreenPoint conversion)

    void Update()
    {
        // Left mouse click and not clicking on UI element
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mapContainer, Input.mousePosition, uiCamera, out localPoint))
            {
                PlacePin(localPoint);
            }
        }
    }

    void PlacePin(Vector2 localPoint)
    {
        GameObject pin = Instantiate(pinPrefab, pinsParent);
        RectTransform pinRect = pin.GetComponent<RectTransform>();

        // Set pivot and position
        pinRect.pivot = new Vector2(0.5f, 1f); // top-middle of the pin
        pinRect.anchoredPosition = localPoint;

        Debug.Log($"📍 Pin placed at map local position: {localPoint}");
    }
}
