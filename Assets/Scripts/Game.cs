using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Find.CameraController.Mode = CameraMode.Perspective;

        ThingSpawner.SpawnThing(ThingDefOf.Player, new Vector2(50, 50));   
    }
}
