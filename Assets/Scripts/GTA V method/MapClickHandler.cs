using UnityEngine;
using UnityEngine.EventSystems;

public class MapClickHandler : MonoBehaviour, IPointerClickHandler
{
    public RectTransform mapContainer;
    public PinManager pinManager;

    public void OnPointerClick(PointerEventData e)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mapContainer, e.position, e.pressEventCamera, out Vector2 local))
        {
            pinManager.PlacePinAtUIPosition(local);
        }
    }
}
