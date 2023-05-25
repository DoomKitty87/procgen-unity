using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcGen;
using System.Linq;

namespace ProcGen
{

  public static class TileGeneration
  {

    public static GameObject GenerateTile(string noiseType, Vector3 position, int seed, float vertexOffset, int tileDepth, int tileWidth, float scale, int waves, float amplitude, float frequency) {
      float offsetX = -position.x;
      float offsetZ = -position.z;

      float[,] heightMap;

      switch (noiseType) {
        case "perlin":
          heightMap = NoiseMapGeneration.PerlinNoiseJobs(tileDepth, tileWidth, scale, offsetX, offsetZ, waves, amplitude, frequency, seed);
          break;
        case "cellular":
          heightMap = NoiseMapGeneration.CellularNoiseJobs(tileDepth, tileWidth, scale, offsetX, offsetZ, waves, amplitude, frequency, seed);
          break;
        default:
          heightMap = NoiseMapGeneration.PerlinNoiseJobs(tileDepth, tileWidth, scale, offsetX, offsetZ, waves, amplitude, frequency, seed);
          break;
      }

      GameObject tile = new GameObject();
      tile.AddComponent<MeshFilter>();
      tile.AddComponent<MeshRenderer>();
      UpdateVertices(tile, heightMap, vertexOffset);
      tile.transform.position = position;

      return tile;
    }

    public static GameObject UpdateVertices(GameObject obj, float[,] heightMap, float vertexOffset) {
      Vector3[] meshVertices = new Vector3[heightMap.GetLength(0) * heightMap.GetLength(1)];
      for (int vertexIndex = 0; vertexIndex < meshVertices.Length; vertexIndex++) {
        meshVertices[vertexIndex] = new Vector3(vertexIndex % heightMap.GetLength(0) * vertexOffset - (vertexOffset * (int)(heightMap.GetLength(0) / 2)), heightMap[vertexIndex % heightMap.GetLength(0), (int)(vertexIndex / heightMap.GetLength(0))], vertexIndex / heightMap.GetLength(0) * vertexOffset - (vertexOffset * (int)(heightMap.GetLength(1) / 2)));
      }

      int[] meshTriangles = new int[meshVertices.Length * 6];

      for (int i = 0; i < meshVertices.Length; i++) {
        if (i >= meshVertices.Length - heightMap.GetLength(0) || i % heightMap.GetLength(0) == heightMap.GetLength(0) - 1) continue;
        meshTriangles[i * 6] = i;
        meshTriangles[i * 6 + 1] = i + heightMap.GetLength(0);
        meshTriangles[i * 6 + 2] = i + 1;

        meshTriangles[i * 6 + 3] = i + heightMap.GetLength(0) + 1;
        meshTriangles[i * 6 + 4] = i + 1;
        meshTriangles[i * 6 + 5] = i + heightMap.GetLength(0);
      }

      obj.GetComponent<MeshFilter>().mesh.vertices = meshVertices;
      obj.GetComponent<MeshFilter>().mesh.triangles = meshTriangles;
      obj.GetComponent<MeshFilter>().mesh.RecalculateBounds();
      obj.GetComponent<MeshFilter>().mesh.RecalculateNormals();
      if (obj.GetComponent<MeshCollider>() != null) obj.GetComponent<MeshCollider>().sharedMesh = obj.GetComponent<MeshFilter>().mesh;

      return obj;
    }
  }
}