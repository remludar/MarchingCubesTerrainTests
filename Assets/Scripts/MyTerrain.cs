
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MyTerrain : MonoBehaviour {

    public const int WIDTH =  4;
    public const int DEPTH =  4;
    public const int HEIGHT = 4;

    public static double min = double.MaxValue;
    public static double max = double.MinValue;

    public const int NOISE_WIDTH = (WIDTH * (Chunk.WIDTH + 1)); 
    public const int NOISE_HEIGHT = (HEIGHT * (Chunk.HEIGHT + 1)); 
    public const int NOISE_DEPTH = (DEPTH * (Chunk.DEPTH + 1)); 
    public static double[] noise = new double[NOISE_WIDTH * NOISE_HEIGHT * NOISE_DEPTH];

    public float isolevel;

    private Dictionary<Vector3, Chunk> chunksDict = new Dictionary<Vector3, Chunk>();

    //Overloads
    void Start()
    {
        Generate2DNoise();
        //Generate3DNoise();
        GenerateChunks();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RenderChunks();
        }

        foreach (KeyValuePair<Vector3, Chunk> kvp in chunksDict)
        {
            kvp.Value.Render();
        }

    }

    //Helpers

    #region Manual Logging
    //private void Log(string message)
    //{
    //    System.IO.File.AppendAllText(path, message + "\n");
    //}
    #endregion

    private void Generate2DNoise()
    {
        int index = 0;

        for (int z = 0; z < NOISE_DEPTH; z++)
        {
            for (int y = 0; y < NOISE_HEIGHT; y++)
            {
                for (int x = 0; x < NOISE_WIDTH; x++)
                {
                    var octaves = 3;
                    var deltaFrequency = 1.5f;
                    var deltaAmplitude = 0.85f;
                    var deltaScale = 1.0;

                    var frequency = 0.35f;
                    var amplitude = 100.0;
                    var scale = 1.0;


                    double sample = 0.0;
                    for (int i = 0; i < octaves; i++)
                    {

                        sample += Mathf.PerlinNoise((x / (float)Chunk.WIDTH) * frequency,
                                                    (z / (float)Chunk.DEPTH) * frequency
                                                    ) * amplitude * scale - y * 1.5f;

                        frequency *= deltaFrequency;
                        amplitude *= deltaAmplitude;
                        scale *= deltaScale;
                    }

                    if (sample < min) min = sample;
                    if (sample > max) max = sample;

                    index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    noise[index] = sample;

                }
            }
        }
    }
    private void Generate3DNoise()
    {
        var simplexNoise = new OpenSimplexNoise();
     
        for (int z = 0; z < NOISE_DEPTH; z++)
        {
            for (int y = 0; y < NOISE_HEIGHT; y++)
            {
                for (int x = 0; x < NOISE_WIDTH; x++)
                {
                    var octaves = 3;
                    var deltaFrequency = 2.0;
                    var deltaAmplitude = 1.0;
                    var deltaScale = 1.0;

                    var frequency = 0.75;
                    var amplitude = 10.0;
                    var scale = 1.0;

                    double sample = 0.0;
                    for (int i = 0; i < octaves; i++)
                    {
                        
                        sample += simplexNoise.eval((x / (double)(1 * Chunk.WIDTH)) * frequency,
                                                (y / (double)(1 * Chunk.HEIGHT)) * frequency,
                                                (z / (double)(1 * Chunk.DEPTH)) * frequency) * amplitude * scale -y * 0.5f;

                        frequency *= deltaFrequency;
                        amplitude *= deltaAmplitude;
                        scale *= deltaScale;
                    }

                    if (sample < min) min = sample;
                    if (sample > max) max = sample;

                    var index = x + y * NOISE_WIDTH + z * NOISE_HEIGHT * NOISE_WIDTH;
                    noise[index] = sample;
                }
            }
        }
    }
    private void GenerateChunks()
    {
        var materials = Resources.LoadAll<Material>("Materials");
        for (int z = 0; z < DEPTH; z++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    var position = new Vector3(x * Chunk.WIDTH, y * Chunk.HEIGHT, z * Chunk.DEPTH);
                    var rotation = Quaternion.identity;

                    var go = new GameObject(position.ToString());
                    go.transform.position = position;
                    go.transform.rotation = rotation;
                    go.transform.parent = GameObject.Find("Terrain").transform;

                    var newChunk = new Chunk(go, materials[1]);
                    chunksDict.Add(position, newChunk);
                }
            }
        }
    }

    private void RenderChunks()
    {
        foreach(KeyValuePair<Vector3, Chunk> kvp in chunksDict)
        {
            kvp.Value.Clear();

            var renderThread = new Thread(() => kvp.Value.TestTerrain());
            renderThread.Start();

        }
    }
}
