using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGen
{

  public static class TriangleWinding
  {

    public static int[] windSquarePlane(Vector3[] vertices) {
      int[] triangles = new int[(vertices.Length * 6) - (int)Mathf.Sqrt(vertices.Length) * 2 + 1];

      for (int i = 0; i < vertices.Length; i++) {
        if (i >= vertices.Length - Mathf.Sqrt(vertices.Length) || i % Mathf.Sqrt(vertices.Length) == Mathf.Sqrt(vertices.Length) - 1) continue;
        triangles[i * 6] = i;
        triangles[i * 6 + 1] = i + (int)Mathf.Sqrt(vertices.Length);
        triangles[i * 6 + 2] = i + 1;

        triangles[i * 6 + 3] = i + (int)Mathf.Sqrt(vertices.Length) + 1;
        triangles[i * 6 + 4] = i + 1;
        triangles[i * 6 + 5] = i + (int)Mathf.Sqrt(vertices.Length);
      }

      return triangles;
    }
  }
}