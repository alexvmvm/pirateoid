using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.Mouse0) )
        {
            var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            ThingSpawner.SpawnThing(ThingDefOf.Barricade, pos, Random.Range(0, 360));
        } 
    }
}
