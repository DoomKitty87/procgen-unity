using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcGen;

namespace ProcGen
{

  public static class ComplexGeneration
  {

    public static GameObject GenerateSphere(string noiseType, Vector3 position, int seed, int resolution, float radius, float scale, int waves, float amplitude, float frequency) {
      float offsetX = -position.x;
      float offsetZ = -position.z;

      float[,] heightMap;

      switch (noiseType) {
        case "perlin":
          heightMap = NoiseMapGeneration.PerlinNoiseJobs(resolution, resolution, scale, offsetX, offsetZ, waves, amplitude, frequency, seed);
          break;
        case "cellular":
          heightMap = NoiseMapGeneration.CellularNoiseJobs(resolution, resolution, scale, offsetX, offsetZ, waves, amplitude, frequency, seed);
          break;
        default:
          heightMap = NoiseMapGeneration.PerlinNoiseJobs(resolution, resolution, scale, offsetX, offsetZ, waves, amplitude, frequency, seed);
          break;
      }

      GameObject sphere = new GameObject();
      sphere.AddComponent<MeshFilter>();
      sphere.AddComponent<MeshRenderer>();
      UpdateSphereVerticesLatLong(sphere, heightMap, radius);
      sphere.transform.position = position;

      return sphere;
    }

    public static GameObject UpdateSphereVerticesLatLong(int numLatLines, int numLongLines, GameObject obj, float[,] heightMap, float radius) {
      int vertCount = (numLatLines * (numLongLines + 1)) + 2;
      Vector3[] vertices = new Vector3[vertCount];
      vertices[0] = Vector3.up * radius * heightMap[0, 0];
      vertices[vertCount - 1] = Vector3.down * radius * heightMap[0, numLatLines - 1];
      float latSpacing = 1f / (numLatLines + 1f);
      float longSpacing = 1f / numLongLines;

      int v = 1;
      for (int i = 0; i < numLatLines; i++) {
        for (int j = 0; j < numLongLines; j++) {
          float theta = (float)((j * longSpacing) * 2f * Mathf.PI);
          float phi = (float)(((1f - ((i + 1) * latSpacing)) - 0.5f) * Mathf.PI);
          float c = (float)Mathf.Cos(phi);
          vertices[v] = new Vector3(c * Mathf.Cos(theta), Mathf.Sin(phi), c * Mathf.Sin(theta)) * heightMap[j, i] * radius;
          v++;
        }
      }
      
      int[] triangles = TriangleWinding.WindSphere(numLatLines, numLongLines, vertices);
      MeshFilter mf = obj.GetComponent<MeshFilter>();
      mf.mesh.vertices = vertices;
      mf.mesh.triangles = triangles;
      mf.mesh.RecalculateBounds();
      mf.mesh.RecalculateNormals();
      if (obj.GetComponent<MeshCollider>() != null) obj.GetComponent<MeshCollider>().sharedMesh = mf.mesh;

      return obj;
    }

    public static GameObject UpdateSphereVerticesFibbonaci(GameObject obj, float[,] heightMap, float radius) {
      List<Vector3> vertices = new List<Vector3>();
      float phi = Mathf.PI * (Mathf.Sqrt(5f) - 1f);
      float samples = heightMap.GetLength(0) * heightMap.GetLength(1);
      for (int i = 0; i < samples; i++) {
        float y = 1f - (i / (samples - 1)) * 2f;
        float r = Mathf.Sqrt(1 - y * y);
        float theta = phi * i;
        float x = Mathf.Cos(theta) * r;
        float z = Mathf.Sin(theta) * r;
        vertices.Add(new Vector3(x, y, z) * heightMap[i % heightMap.GetLength(0), i / heightMap.GetLength(0)] * radius);
      }

      int[] triangles = TriangleWinding.WindClosestFiltered(vertices.ToArray(), Vector3.zero);

      MeshFilter mf = obj.GetComponent<MeshFilter>();
      mf.mesh.vertices = vertices.ToArray();
      mf.mesh.triangles = triangles;
      mf.mesh.RecalculateBounds();
      mf.mesh.RecalculateNormals();
      if (obj.GetComponent<MeshCollider>() != null) obj.GetComponent<MeshCollider>().sharedMesh = mf.mesh;
      return obj;
    }
  }
}