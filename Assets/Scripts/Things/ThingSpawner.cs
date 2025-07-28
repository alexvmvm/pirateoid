using UnityEngine;

public static class ThingSpawner
{
    public static Thing SpawnThing(Thing thing, Vector2 pos, float rotation = 0f)
    {
        thing.position = pos;
        thing.rotation = rotation;
        thing.PostSpawn();
        return thing;
    }

    public static Thing SpawnThing(ThingDef def, Vector2 pos, float rotation = 0f)
    {        
        var thing = ThingMaker.Make(def);
        return SpawnThing(thing, pos, rotation);
    }
}