using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class RaycastUtils
{
    public static bool UIToMapPosition(Vector2 clickPos, out Vector3 position)
    {
        if( Find.CameraController.Mode == CameraMode.Orthographic )
        {
            position = Camera.main.ScreenToWorldPoint(clickPos);
            return true;
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(clickPos);
            Plane plane = new(new Vector3(0, 0, -1), Vector3.zero);

            if (plane.Raycast(ray, out float enter))
            {
                position = ray.GetPoint(enter);
                return true;
            }

            position = default;
            return false;
        }
    }
}