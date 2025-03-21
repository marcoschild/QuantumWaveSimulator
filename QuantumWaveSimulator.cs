using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumWaveSimulator : MonoBehaviour
{
    public int gridSize = 100;
    public float timeStep = 0.005f;
    public float waveSpeed = 2.0f;
    public float amplitudeScale = 2.0f;

    public Material quantumWaveMaterial;
    private Texture2D waveTexture;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Vector3[] baseVertices;
    private Vector3[] modifiedVertices;
    private float[,] waveFunctionReal;
    private float[,] waveFunctionImag;
    private float[,] potential;
    private Mesh mesh;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        InitializeWaveFunction();
        SetupDoubleSlitPotential();
        SetupMesh();
        SetupWaveTexture();
    }

    void Update()
    {
        UpdateWaveFunction();
        ApplyWaveToMesh();
        UpdateWaveTexture();
    }

    void InitializeWaveFunction()
    {
        waveFunctionReal = new float[gridSize, gridSize];
        waveFunctionImag = new float[gridSize, gridSize];
        potential = new float[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                float dx = (x - gridSize / 4) * 0.1f;
                float dy = (y - gridSize / 2) * 0.1f;
                waveFunctionReal[x, y] = Mathf.Exp(-dx * dx - dy * dy);
                waveFunctionImag[x, y] = 0.0f;
            }
        }
    }

    void SetupMesh()
    {
        mesh = new Mesh();
        meshFilter.mesh = mesh;

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                vertices.Add(new Vector3(x * 0.1f, 0, y * 0.1f));
                uvs.Add(new Vector2((float)x / gridSize, (float)y / gridSize));

                if (x < gridSize - 1 && y < gridSize - 1)
                {
                    int index = x * gridSize + y;
                    triangles.Add(index);
                    triangles.Add(index + gridSize);
                    triangles.Add(index + 1);

                    triangles.Add(index + 1);
                    triangles.Add(index + gridSize);
                    triangles.Add(index + gridSize + 1);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        baseVertices = mesh.vertices;
        modifiedVertices = new Vector3[baseVertices.Length];
    }

    void ApplyWaveToMesh()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                int index = x * gridSize + y;
                modifiedVertices[index] = baseVertices[index] + Vector3.up * (waveFunctionReal[x, y] * amplitudeScale);
            }
        }

        mesh.vertices = modifiedVertices;
        mesh.RecalculateNormals();
    }

    void SetupWaveTexture()
    {
        waveTexture = new Texture2D(gridSize, gridSize, TextureFormat.RFloat, false);
        waveTexture.filterMode = FilterMode.Bilinear;
        quantumWaveMaterial.SetTexture("_WaveTex", waveTexture);
    }

    void UpdateWaveTexture()
    {
        Color[] pixels = new Color[gridSize * gridSize];
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                float intensity = Mathf.Abs(waveFunctionReal[x, y]);
                pixels[x * gridSize + y] = new Color(intensity, 0, 0);
            }
        }
        waveTexture.SetPixels(pixels);
        waveTexture.Apply();
    }

    void SetupDoubleSlitPotential()
    {
        int barrierX = gridSize / 2;
        for (int y = 0; y < gridSize; y++)
        {
            if (y < gridSize / 3 || y > 2 * gridSize / 3)
                continue;

            potential[barrierX, y] = 5.0f;
        }
    }

    void UpdateWaveFunction()
    {
        for (int x = 1; x < gridSize - 1; x++)
        {
            for (int y = 1; y < gridSize - 1; y++)
            {
                float laplacian = waveFunctionReal[x + 1, y] + waveFunctionReal[x - 1, y] +
                                  waveFunctionReal[x, y + 1] + waveFunctionReal[x, y - 1] -
                                  4 * waveFunctionReal[x, y];
                waveFunctionImag[x, y] += timeStep * (-0.5f * laplacian - potential[x, y] * waveFunctionReal[x, y]);
            }
        }

        for (int x = 1; x < gridSize - 1; x++)
        {
            for (int y = 1; y < gridSize - 1; y++)
            {
                float laplacian = waveFunctionImag[x + 1, y] + waveFunctionImag[x - 1, y] +
                                  waveFunctionImag[x, y + 1] + waveFunctionImag[x, y - 1] -
                                  4 * waveFunctionImag[x, y];
                waveFunctionReal[x, y] -= timeStep * (-0.5f * laplacian - potential[x, y] * waveFunctionImag[x, y]);
            }
        }
    }
}