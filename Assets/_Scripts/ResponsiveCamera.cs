using UnityEngine;

public class ResponsiveCamera : MonoBehaviour
{
    private Camera mainCamera;
    public float baseAspectWidth = 16f;
    public float baseAspectHeight = 9f;

    void Start()
    {
        mainCamera = Camera.main;

        // Adjust the camera size based on the screen's aspect ratio
        AdjustCameraSize();
    }

    void AdjustCameraSize()
    {
        float targetAspect = baseAspectWidth / baseAspectHeight;
        float currentAspect = (float)Screen.width / Screen.height;

        // Calculate the desired camera size
        float newSize = mainCamera.orthographicSize * (targetAspect / currentAspect);

        // Apply the new size to the camera
        mainCamera.orthographicSize = newSize;
    }
}
