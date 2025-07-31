using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Region
{
    //Config
    public ThingLister thingLister;
    public RectInt rect;
    private List<Vector2> corners = new();

    //Props
    public List<Vector2> Corners
    {
        get
        {
            if( corners.Count == 0 )
            {
                corners.Add(rect.min);
                corners.Add(rect.min + Vector2.up * rect.height);
                corners.Add(rect.max);
                corners.Add(rect.min + Vector2.right * rect.width);
            }

            return corners;
        }
    }

    public Region(RectInt rect)
    {
        this.rect = rect;
        this.thingLister = new();
    }

    public void Add(Thing thing)
    {
        if( !thingLister.Contains(thing) )
        {
            thingLister.Add(thing);
            Find.NavManager.UpdateRegionNodes(this);
        }
    }
    
    public void Remove(Thing gameObject)
    {
        if( thingLister.Contains(gameObject) )
        {
            thingLister.Remove(gameObject);
            Find.NavManager.UpdateRegionNodes(this);
        }
    }

    // Override GetHashCode to base it on the RectInt
    public override int GetHashCode()
    {
        unchecked // Overflow is fine in hash codes
        {
            int hash = 17;
            hash = hash * 23 + rect.x.GetHashCode();
            hash = hash * 23 + rect.y.GetHashCode();
            hash = hash * 23 + rect.width.GetHashCode();
            hash = hash * 23 + rect.height.GetHashCode();
            return hash;
        }
    }

    // Override Equals to compare Regions based on RectInt
    public override bool Equals(object obj)
    {
        if (obj is Region other)
        {
            return this.rect.Equals(other.rect);
        }

        return false;
    }

    public void DrawGizmos()
    {
        foreach(var thing in thingLister.AllThings)
        {
            Gizmos.DrawLine(thing.position, rect.center);
        }

        var things = thingLister.AllThings;

        #if UNITY_EDITOR
        var label = "\nThing count: " + things.Count;
        foreach(var g in Enum.GetValues(typeof(ThingGroup)).Cast<ThingGroup>())
        {
            label += "\n -> " + g + ": " + thingLister.GetThingCountInGroup(g);
        }
        UnityEditor.Handles.Label(rect.center, label);
        #endif
    }
}

public class RegionManager : MonoBehaviour
{
    //Const
    public const int RegionSize = 10;

    //Working vars
    public readonly List<Region> regions = new();
    private readonly Dictionary<Vector2Int, Region> regionsByGridCell = new();

    //Property
    public List<Region> All => regions;

    public void Awake()
    {
        CheckRegionsCorrect();
    }

    public Region GetRegion(Vector2Int cell)
    {
        var regionCell = PositionToRegionCell(cell.x, cell.y);
        
        if( regionsByGridCell.ContainsKey(regionCell) )
            return regionsByGridCell[regionCell];
        
        return null;
    }

    void CheckRegionsCorrect()
    {
        var width  = Find.TileMap.width;
        var height = Find.TileMap.height;

        var xMin = Mathf.FloorToInt(0 / (float)RegionSize) * RegionSize;
        var yMin = Mathf.FloorToInt(0 / (float)RegionSize) * RegionSize;
        
        var xMax = Mathf.CeilToInt(width / (float)RegionSize) * RegionSize;
        var yMax = Mathf.CeilToInt(height / (float)RegionSize) * RegionSize;

        for(var x = xMin; x < xMax; x += RegionSize)
        {
            for(var y = yMin; y < yMax; y += RegionSize)
            {
                var cell = PositionToRegionCell(x, y);
                if( regionsByGridCell.ContainsKey(cell) )
                    continue;

                regionsByGridCell[cell] = new Region(new RectInt(x, y, RegionSize, RegionSize));
                regions.Add(regionsByGridCell[cell]);

                Find.NavManager.UpdateRegionNodes(regionsByGridCell[cell]);
            }
        }
    }

    public Region GetRegionOrNull(Vector2Int cell)
    {
        var regionCell = PositionToRegionCell(cell.x, cell.y);
        if( regionsByGridCell.ContainsKey(regionCell) )
            return regionsByGridCell[regionCell];
            
        return null;
    }

    private Vector2Int PositionToRegionCell(int x, int y)
    {
        return new Vector2Int(
            Mathf.FloorToInt(x / (float)RegionSize), 
            Mathf.FloorToInt(y / (float)RegionSize));
    }

    public void Notify_ThingAdded(Thing thing)
    {
        var corners = thing.Corners;

        for(var i = 0; i < corners.Count; i++)
        {
            var pos = corners[i];

            Vector2Int cell = new(
                Mathf.FloorToInt(pos.x), 
                Mathf.FloorToInt(pos.y));

            Region region = GetRegionOrNull(cell);
            
            region?.Add(thing);

            var p1 = corners[i];
            var p2 = corners[(i + 1) % corners.Count];

            for(var j = 0; j < regions.Count; j++)
            {
                var r = regions[j];
                var rCorners = r.Corners;

                for(var k = 0; k < corners.Count; k++)
                {
                    var p3 = rCorners[k];
                    var p4 = rCorners[(k + 1) % rCorners.Count];

                    if( GeomUtils.LinesIntersect(p1, p2, p3, p4, out _, out _) )
                    {
                        r.Add(thing);
                    }
                }
            }
        }
    }

    public void Notify_ThingRemoved(Thing thing)
    {
        for(var i = 0; i < regions.Count; i++)
        {
            if( regions[i].thingLister.Contains(thing) )
                regions[i].thingLister.Remove(thing);
        }
    }

    public void Notify_ThingMoved(Thing thing)
    {
        Notify_ThingRemoved(thing);
        Notify_ThingAdded(thing);
    }

    void OnDrawGizmos()
    {
        foreach(var region in regionsByGridCell.Values)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(region.rect.center, region.rect.size.ToVector3());

            region.DrawGizmos();
        }
    }
}