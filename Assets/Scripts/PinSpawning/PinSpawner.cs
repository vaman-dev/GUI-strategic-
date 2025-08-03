using UnityEngine;
using UnityEngine.InputSystem;

public class PinSpawner : MonoBehaviour
{
    public IndiaMapZoom mapZoom;          // Assign your IndiaMapZoom script here in the Inspector
    public GameObject pinPrefab;          // Drag your pin prefab here
    public Transform pinParent;           // Set this to the tileRoot (same as tile parent object)

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 pinPos = hit.point;
                pinPos.z = -0.1f; // Ensure it's rendered above tiles

                GameObject pin = Instantiate(pinPrefab, pinPos, Quaternion.identity);
                pin.transform.SetParent(pinParent != null ? pinParent : mapZoom.transform, true);
            }
        }
    }
}
