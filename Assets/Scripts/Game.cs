using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Find.CameraController.Mode = CameraMode.Perspective;

        var player = ThingSpawner.SpawnThing(ThingDefOf.Pirate, new Vector2(50, 50));   
        
        player.GetComp<CompControllable>()
            .SetControlled(true);
    }
}
