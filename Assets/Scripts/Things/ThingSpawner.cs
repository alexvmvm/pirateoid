using UnityEngine;

public static class ThingSpawner
{
    public static void SpawnThing(Thing thing, Vector2 pos, float rotation = 0f)
    {
        thing.position = pos;
        thing.rotation = rotation;
        thing.PostSpawn();
    }

    public static void SpawnThing(ThingDef def, Vector2 pos, float rotation = 0f)
    {        
        var thing = ThingMaker.Make(def);
        SpawnThing(thing, pos, rotation);
    }
}