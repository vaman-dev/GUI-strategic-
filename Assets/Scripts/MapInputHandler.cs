using UnityEngine;
using UnityEngine.InputSystem;

public class MapInputHandler : MonoBehaviour
{
    private IndiaMapZoom mapZoom;

    void Start()
    {
        mapZoom = FindObjectOfType<IndiaMapZoom>();
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Scroll to zoom
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            mapZoom.OnScrollZoom(scroll);
        }

        // Click functionality removed
    }
}
