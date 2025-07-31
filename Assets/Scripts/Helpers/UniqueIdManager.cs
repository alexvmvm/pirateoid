using UnityEngine;

public class UniqueIdManager
{
	//Const
	public const int InvalidId = -1;

	//Working vars
	private int thingID;

	public int GetNextThingID() => GetNextID(ref thingID);

    private int GetNextID(ref int nextID)
	{
		int ret = nextID;
		nextID++;
		return ret;
	}
}
