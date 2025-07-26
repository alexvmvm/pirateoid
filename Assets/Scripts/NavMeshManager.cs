using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClipperLib;

public struct NavRegion
{
    public List<Vector2> points;
    public int id;

    public NavRegion(List<Vector2> points, int id)
    {
        this.points = points;
        this.id = id;
    }
}

public class NavMeshManager : MonoBehaviour
{
    //Const
    private const double ClipperScale = 100000.0;

    //Working vars
    private readonly List<List<IntPoint>> polygons = new();
    public int width = 100;
    public int height = 100;
    private static List<Vector2> tmpPoints = new();
    private readonly List<NavRegion> regions = new();
    private bool dirty = false;

    public void Add(List<Vector2> path)   
    {
        polygons.Add(ClipperUtils.ToIntPath(path, ClipperScale));

        dirty = true;
    }

    public void Register(Thing thing)
    {
        if( thing.def.traversability != Traversability.Impassable )
            return;
        
        Vector2 size = thing.def.size;

        var min = thing.position - (size/2f);
        var max = thing.position + (size/2f);

        tmpPoints.Clear();
        tmpPoints.Add(min);
        tmpPoints.Add(min + Vector2.up * size.y);
        tmpPoints.Add(max);
        tmpPoints.Add(min + Vector2.right * size.x);

        var rot = Quaternion.Euler(0f, 0f, thing.rotation);

        for (int i = 0; i < tmpPoints.Count; i++)
        {
            tmpPoints[i] =
                thing.position +
                (Vector2)(rot * (tmpPoints[i] - thing.position));
        }

        Add(tmpPoints);

        tmpPoints.Clear();
    }

    public void DeRegister(Thing thing)
    {

    }

    public void RegisterBlocker(Rect rect)
    {
        tmpPoints.Clear();
        tmpPoints.Add(rect.min);
        tmpPoints.Add(rect.min + Vector2.up * rect.height);
        tmpPoints.Add(rect.max);
        tmpPoints.Add(rect.min + Vector2.right * rect.width);

        Add(tmpPoints);

        tmpPoints.Clear();
    }

    public List<List<Vector2>> GetUncoveredRegions()
    {
        // 1. Rectangle as outer shape
        List<List<IntPoint>> rectPath = new()
        {
            new List<IntPoint>()
            {
                new IntPoint(0,  height * ClipperScale),
                new IntPoint(width * ClipperScale, height * ClipperScale),
                new IntPoint(width * ClipperScale, 0),
                new IntPoint(0,  0)
            }
        };

        // 2. Union of all polygons
        Clipper clipperUnion = new();
        clipperUnion.AddPaths(polygons, PolyType.ptSubject, true);

        List<List<IntPoint>> unionResult = new();
        clipperUnion.Execute(ClipType.ctUnion, unionResult, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

        polygons.Clear();
        polygons.AddRange(unionResult);

        // 3. Subtract polygons from rectangle
        Clipper clipperDiff = new();
        clipperDiff.AddPaths(rectPath, PolyType.ptSubject, true);
        clipperDiff.AddPaths(unionResult, PolyType.ptClip, true);

        List<List<IntPoint>> diffResult = new();
        clipperDiff.Execute(ClipType.ctDifference, diffResult, PolyFillType.pftNonZero, PolyFillType.pftNonZero);

        // Convert result to Vector2
        return ClipperUtils.ToVectorPaths(diffResult, ClipperScale);
    }

    private static List<Vector2> points = new();
    void Update()
    {
        // if( Input.GetKeyDown(KeyCode.Mouse0) )
        // {
        //     points.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        // }
        // else if( Input.GetKeyDown(KeyCode.Mouse1) )
        // {
        //     if( points.Count > 2 )
        //     {
        //         Add(points);
        //     }

        //     points.Clear();
        // }

        if( dirty )
        {
            regions.Clear();

            int id = 0;
            foreach(var points in GetUncoveredRegions())
            {
                regions.Add(new NavRegion(points, id++));
            }

            dirty = false;
        }
    }

    private static List<Color> colors = new()
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.cyan,
        Color.magenta,
        Color.gray,
        Color.yellow
    };

    void OnDrawGizmos()
    {
        for(var i = 0; i < regions.Count; i++)
        {
            var region = regions[i];

            Gizmos.color = colors[i % colors.Count];
            
            for(var j = 0; j < region.points.Count; j++)
            {
                Gizmos.DrawLine(region.points[j], region.points[(j + 1) % region.points.Count]);
            }
            
            #if UNITY_EDITOR
            var centroid = region.points.Centroid();
            UnityEditor.Handles.Label(centroid, region.id.ToString(), new GUIStyle() { normal = new GUIStyleState() { textColor = Color.black } });
            #endif
        }

        Gizmos.color = Color.green;

        for(var i = 0; i < points.Count; i++)
        {
            Gizmos.DrawLine(points[i], points[(i + 1) % points.Count]);
        }
    }
}

public static class ClipperUtils
{
    public static List<IntPoint> ToIntPath(List<Vector2> path, double scale)
    {
        List<IntPoint> result = new();
        foreach (var v in path)
            result.Add(new IntPoint((long)(v.x * scale), (long)(v.y * scale)));
        return result;
    }

    public static List<List<Vector2>> ToVectorPaths(List<List<IntPoint>> paths, double scale)
    {
        List<List<Vector2>> result = new();
        foreach (var path in paths)
        {
            List<Vector2> poly = new();
            foreach (var p in path)
                poly.Add(new Vector2(p.X / (float)scale, p.Y / (float)scale));
            result.Add(poly);
        }
        return result;
    }
}
