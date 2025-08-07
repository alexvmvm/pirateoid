using UnityEngine;

public static class CameraUtils
{
    private const float PerspectiveCameraExpandFactor = 1.5f;

    /// <summary>
    /// Returns the estimated maximum visible width and height in world units from the camera's current position.
    /// </summary>
    public static Vector2 GetCameraViewSize(Camera cam)
    {
        if (cam.orthographic)
        {
            float height = cam.orthographicSize * 2f;
            float width = height * cam.aspect;
            return new Vector2(width, height);
        }
        else
        {
            // Distance from camera to ground plane (z = 0 assumed)
            float camToPlane = Mathf.Abs(cam.transform.position.z);
            float fovRad = cam.fieldOfView * Mathf.Deg2Rad;

            float height = 2f * camToPlane * Mathf.Tan(fovRad / 2f);
            float width = height * cam.aspect;
            
            return new Vector2(width, height) * PerspectiveCameraExpandFactor;
        }
    }

    public static Rect CalculateCameraScreenRect(this Camera cam)
    {
        // Bottom-left and top-right corners of the screen in pixels
        Vector3 bottomLeft = new Vector3(0, 0, 0);
        Vector3 topRight   = new Vector3(Screen.width, Screen.height, 0);

        // Convert to same coordinate space (optional: clamp z to 0)
        bottomLeft = cam.ScreenToViewportPoint(bottomLeft);
        topRight   = cam.ScreenToViewportPoint(topRight);

        float xMin = bottomLeft.x * Screen.width;
        float yMin = bottomLeft.y * Screen.height;
        float xMax = topRight.x * Screen.width;
        float yMax = topRight.y * Screen.height;

        return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
    }
}