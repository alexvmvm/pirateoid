using System.Collections.Generic;
using UnityEngine;

public class TileRenderer : MonoBehaviour
{
    public Mesh tileMesh;
    public TileDef sand;
    public TileDef water;
    public int renderDistance = 20;

    private Dictionary<Material, List<Matrix4x4>> materialBatches = new();
    private Camera mainCam;

    void Awake()
    {
        tileMesh = CreateQuadMesh();
        mainCam = Camera.main;
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
        if (mainCam == null)
        {
            mainCam = Camera.main;
            if (mainCam == null)
                return;
        }

        TileMap tileMap = Find.TileMap;
        materialBatches.Clear();

        // Get camera view bounds in world space
        Vector3 camPos  = mainCam.transform.position;
        Vector2 camSize = CameraUtils.GetCameraViewSize(Camera.main);

        float camHeight = camSize.y;
        float camWidth  = camSize.x;

        int maxCameraY = Mathf.CeilToInt(camPos.y + camHeight / 2f) + 1;
        int maxCameraX = Mathf.CeilToInt(camPos.x + camWidth / 2f) + 1;

        int minX = Mathf.Max(0, Mathf.FloorToInt(camPos.x - camWidth / 2f) - 1);
        int maxX = Mathf.Min(tileMap.width - 1, maxCameraX);
        int minY = Mathf.Max(0, Mathf.FloorToInt(camPos.y - camHeight / 2f) - 1);
        int maxY = Mathf.Min(tileMap.height - 1, maxCameraY);

        for (TileLayer layer = TileLayer.Floor; layer < TileLayer.Count; layer++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    ref var tile = ref tileMap.GetTile(layer, x, y);
                    var def = TileDatabase.Get(tile.typeId);
                    var mat = def.material;

                    if (mat == null)
                        continue;

                    if (!materialBatches.TryGetValue(mat, out var list))
                    {
                        list = new List<Matrix4x4>(256);
                        materialBatches[mat] = list;
                    }

                    float z = layer switch
                    {
                        TileLayer.Wall => 1f,
                        TileLayer.Roof => 2f,
                        _ => 0f
                    };

                    list.Add(Matrix4x4.TRS(new Vector3(x, y, z), Quaternion.identity, Vector3.one));
                }
            }

            RenderBatches();
            materialBatches.Clear(); // clear per-layer to reduce memory use
        }
    }

    void RenderBatches()
    {
        const int batchSize = 1023;
        Matrix4x4[] buffer = new Matrix4x4[batchSize];

        foreach (var pair in materialBatches)
        {
            var mat = pair.Key;
            var matrices = pair.Value;

            for (int i = 0; i < matrices.Count; i += batchSize)
            {
                int count = Mathf.Min(batchSize, matrices.Count - i);
                matrices.CopyTo(i, buffer, 0, count);
                Graphics.DrawMeshInstanced(tileMesh, 0, mat, buffer, count);
            }
        }
    }
}