using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int WIDTH =  16;
    public const int DEPTH =  16;
    public const int HEIGHT = 16;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    int numTris = 0;

    GameObject terrainGO;
    MyTerrain terrain;
    Vector3 position;

    //Overloads
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        position = new Vector3(transform.position.x / WIDTH, transform.position.y / HEIGHT, transform.position.z / DEPTH);
        terrainGO = GameObject.Find("Terrain");
        terrain = terrainGO.GetComponent<MyTerrain>();
        Render();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Render();
    }

    //Helpers
    void TestTerrain()
    {
        var grids = new MarchingCubes.GridCell[WIDTH * HEIGHT * DEPTH];

        var mc = new MarchingCubes();
        var triangles = new MarchingCubes.Triangle[5];
        var triCount = 0;

        for (int z = 0; z < DEPTH; z++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    var gridCell = new MarchingCubes.GridCell();
                    gridCell.p[0] = new Vector3(x + 0, y + 0, z + 1);
                    gridCell.p[1] = new Vector3(x + 1, y + 0, z + 1);
                    gridCell.p[2] = new Vector3(x + 1, y + 0, z + 0);
                    gridCell.p[3] = new Vector3(x + 0, y + 0, z + 0);
                    gridCell.p[4] = new Vector3(x + 0, y + 1, z + 1);
                    gridCell.p[5] = new Vector3(x + 1, y + 1, z + 1);
                    gridCell.p[6] = new Vector3(x + 1, y + 1, z + 0);
                    gridCell.p[7] = new Vector3(x + 0, y + 1, z + 0);

                    gridCell.val[0] = MyTerrain.noise[((int)position.x * WIDTH + x + 0) + ((int)position.y * HEIGHT + y + 0) * MyTerrain.NOISE_WIDTH + ((int)position.z * DEPTH + z + 1) * MyTerrain.NOISE_HEIGHT * MyTerrain.NOISE_WIDTH];
                    gridCell.val[1] = MyTerrain.noise[((int)position.x * WIDTH + x + 1) + ((int)position.y * HEIGHT + y + 0) * MyTerrain.NOISE_WIDTH + ((int)position.z * DEPTH + z + 1) * MyTerrain.NOISE_HEIGHT * MyTerrain.NOISE_WIDTH];
                    gridCell.val[2] = MyTerrain.noise[((int)position.x * WIDTH + x + 1) + ((int)position.y * HEIGHT + y + 0) * MyTerrain.NOISE_WIDTH + ((int)position.z * DEPTH + z + 0) * MyTerrain.NOISE_HEIGHT * MyTerrain.NOISE_WIDTH];
                    gridCell.val[3] = MyTerrain.noise[((int)position.x * WIDTH + x + 0) + ((int)position.y * HEIGHT + y + 0) * MyTerrain.NOISE_WIDTH + ((int)position.z * DEPTH + z + 0) * MyTerrain.NOISE_HEIGHT * MyTerrain.NOISE_WIDTH];
                    gridCell.val[4] = MyTerrain.noise[((int)position.x * WIDTH + x + 0) + ((int)position.y * HEIGHT + y + 1) * MyTerrain.NOISE_WIDTH + ((int)position.z * DEPTH + z + 1) * MyTerrain.NOISE_HEIGHT * MyTerrain.NOISE_WIDTH];
                    gridCell.val[5] = MyTerrain.noise[((int)position.x * WIDTH + x + 1) + ((int)position.y * HEIGHT + y + 1) * MyTerrain.NOISE_WIDTH + ((int)position.z * DEPTH + z + 1) * MyTerrain.NOISE_HEIGHT * MyTerrain.NOISE_WIDTH];
                    gridCell.val[6] = MyTerrain.noise[((int)position.x * WIDTH + x + 1) + ((int)position.y * HEIGHT + y + 1) * MyTerrain.NOISE_WIDTH + ((int)position.z * DEPTH + z + 0) * MyTerrain.NOISE_HEIGHT * MyTerrain.NOISE_WIDTH];
                    gridCell.val[7] = MyTerrain.noise[((int)position.x * WIDTH + x + 0) + ((int)position.y * HEIGHT + y + 1) * MyTerrain.NOISE_WIDTH + ((int)position.z * DEPTH + z + 0) * MyTerrain.NOISE_HEIGHT * MyTerrain.NOISE_WIDTH];

                    grids[x + y * WIDTH + z * HEIGHT * WIDTH] = gridCell;
                }
            }
        }

        for (int i = 0; i < grids.Length; i++)
        {
            triangles = new MarchingCubes.Triangle[5];
            for (int k = 0; k < triangles.Length; k++)
            {
                triangles[k] = new MarchingCubes.Triangle();
            }

            numTris = mc.Polygonize(grids[i], terrain.isolevel, triangles);
            for (int j = 0; j < numTris; j++)
            {
                verts.Add(triangles[j].p[0]);
                verts.Add(triangles[j].p[1]);
                verts.Add(triangles[j].p[2]);

                tris.Add(triCount * 3 + 0);
                tris.Add(triCount * 3 + 1);
                tris.Add(triCount * 3 + 2);

                triCount++;
            }
        }
    }
    void Render()
    {
        verts.Clear();
        tris.Clear();
        uvs.Clear();
        meshFilter.mesh.Clear();

        TestTerrain();


        var mesh = meshFilter.mesh;
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }
}
