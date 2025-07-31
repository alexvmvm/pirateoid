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
}