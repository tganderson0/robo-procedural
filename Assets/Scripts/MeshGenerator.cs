using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    public int xSize = 20;
    public int zSize = 20;

    public int resolution = 4; // number of vertices per unit direction (e.g. how many vertices between 0 and 1)

    float distanceBetweenPoints;

    public Gradient gradient;

    float minTerrainHeight;
    float maxTerrainHeight;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        distanceBetweenPoints = 1.0f / resolution;
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize * resolution + 1) * (zSize * resolution + 1)];

        for (int i = 0, z = 0; z <= zSize * resolution; z++)
        {
            for (int x = 0; x <= xSize * resolution; x++)
            {
                float y = Mathf.PerlinNoise(x * distanceBetweenPoints * 0.3f, z * distanceBetweenPoints * 0.3f) * 2f;
                vertices[i] = new Vector3((float)x / resolution, y, (float)z / resolution);

                if (y > maxTerrainHeight)
                    maxTerrainHeight = y;
                if (y < minTerrainHeight)
                    minTerrainHeight = y;

                i++;
            }
        }

        int vert = 0;
        int tris = 0;
        triangles = new int[xSize * resolution * zSize * resolution * 6];
        for (int z = 0; z < zSize * resolution; z++)
        {
            for (int x = 0; x < xSize * resolution; x++)
            {
                triangles[tris] = vert;
                triangles[tris + 1] = vert + xSize * resolution + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize * resolution + 1;
                triangles[tris + 5] = vert + xSize * resolution + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }
}
