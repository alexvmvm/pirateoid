

public static class ThingMaker
{
    public static Thing Make(ThingDef def)
    {
        if( def == null )
            throw new System.Exception("Tried to spawn thign with null def");

        var thing = new Thing();
        thing.def = def;
        thing.PostMake();
        return thing;
    }
}