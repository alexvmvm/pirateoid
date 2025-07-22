using System.Collections.Generic;
using UnityEngine;

public class TileRenderer : MonoBehaviour
{
    public Mesh tileMesh;
    public TileDef sand;
    public TileDef water;
    public int width = 100;
    public int height = 100;

    public TileMap tileMap;
    public int renderDistance = 20;

    // Reused buffer to avoid GC
    private Dictionary<Material, List<Matrix4x4>> materialBatches = new();

    void Awake()
    {
        tileMap = new TileMap(width, height);
        tileMesh = CreateQuadMesh();

        for(var x = 0; x < width; x++)
        {
            for(var y = 0; y < height; y++)
            {
                tileMap.SetTile(TileLayer.Floor, x, y, x < 6 + Random.Range(0, 4) ? water : sand);
            }
        }
    }

    private Mesh CreateQuadMesh()
    {
        Mesh mesh = new Mesh();

        mesh.vertices = new Vector3[]
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0)
        };

        mesh.uv = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        mesh.triangles = new int[]
        {
            0, 2, 1,
            2, 3, 1
        };

        mesh.RecalculateNormals();

        return mesh;
    }

    void Update()
    {
        Vector3 camPos = Camera.main.transform.position;
        int camTileX = Mathf.FloorToInt(camPos.x);
        int camTileY = Mathf.FloorToInt(camPos.z);

        int minX = 0; //Mathf.Max(0, camTileX - renderDistance);
        int maxX = width; //Mathf.Min(tileMap.mapWidth - 1, camTileX + renderDistance);
        int minY = 0; //Mathf.Max(0, camTileY - renderDistance);
        int maxY = height; //Mathf.Min(tileMap.mapHeight - 1, camTileY + renderDistance);

        for (TileLayer layer = TileLayer.Floor; layer < TileLayer.Count; layer++)
        {
            materialBatches.Clear();

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    ref var tile = ref tileMap.GetTile(layer, x, y);
                    var def = TileDatabase.Get(tile.typeId);
                    var mat = def.material;
                    
                    if( mat == null )
                        continue;

                    if (!materialBatches.TryGetValue(mat, out var list))
                    {
                        list = new List<Matrix4x4>(256);
                        materialBatches[mat] = list;
                    }

                    Vector3 pos = new Vector3(x, y, 0);
                    if (layer == TileLayer.Wall) pos.z += 1; // elevate walls
                    if (layer == TileLayer.Roof) pos.z += 2;

                    list.Add(Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one));
                }
            }

            RenderBatches();
        }
    }

    void RenderBatches()
    {
        const int batchSize = 1023;
        Matrix4x4[] buffer = new Matrix4x4[batchSize];

        foreach (var pair in materialBatches)
        {
            Material mat = pair.Key;
            List<Matrix4x4> matrices = pair.Value;

            for (int i = 0; i < matrices.Count; i += batchSize)
            {
                int count = Mathf.Min(batchSize, matrices.Count - i);
                matrices.CopyTo(i, buffer, 0, count);
                Graphics.DrawMeshInstanced(tileMesh, 0, mat, buffer, count);
            }
        }
    }
}