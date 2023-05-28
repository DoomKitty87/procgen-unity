using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGen
{

  public static class TriangleWinding
  {

    public static int[] WindSquarePlane(Vector3[] vertices) {
      int[] triangles = new int[((vertices.Length - (int)Mathf.Sqrt(vertices.Length) * 2 + 1) * 6) ];

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

    public static int[] WindClosestFiltered(Vector3[] vertices, Vector3 origin) {
      List<int> triangles = new List<int>();
      for (int i = 0; i < vertices.Length; i++) {
        float smallestDist = float.PositiveInfinity;
        List<int> closestPoints = new List<int>();
        for (int j = 0; j < vertices.Length; j++) {
          if (j == i) continue;
          if (Vector3.Distance(vertices[i], vertices[j]) < smallestDist) {
            smallestDist = Vector3.Distance(vertices[i], vertices[j]);
            closestPoints.Clear();
            closestPoints.Add(j);
          }
          else if (Vector3.Distance(vertices[i], vertices[j]) == smallestDist) {
            closestPoints.Add(j);
          }
        }
        for (int j = 0; j < closestPoints.Count; j++) {
          float smallestDistN = float.PositiveInfinity;
          List<int> closestPointsN = new List<int>();
          for (int k = 0; k < vertices.Length; k++) {
            if (k == i || k == closestPoints[j]) continue;
            Vector3 avg = new Vector3((vertices[i].x + vertices[closestPoints[j]].x) / 2, (vertices[i].y + vertices[closestPoints[j]].y) / 2, (vertices[i].z + vertices[closestPoints[j]].z) / 2);
            if (Vector3.Distance(vertices[k], avg) < smallestDistN) {
              smallestDistN = Vector3.Distance(vertices[k], avg);
              closestPointsN.Clear();
              closestPointsN.Add(k);
            }
            else if (Vector3.Distance(vertices[k], avg) == smallestDistN) {
              closestPointsN.Add(k);
            }
          }
          for (int k = 0; k < closestPointsN.Count; k++) {
            triangles.Add(i);
            triangles.Add(closestPoints[j]);
            triangles.Add(closestPointsN[k]);
          }
        }
      }
      List<int> trianglesCleaned = new List<int>();
      for (int i = 0; i < triangles.Count / 3; i++) {
        bool unique = true;
        for (int j = 0; j < trianglesCleaned.Count / 3; j++) {
          List<int> pool = new List<int>();
          for (int k = 0; k < 3; k++) pool.Add(trianglesCleaned[j * 3 + k]);
          for (int k = 0; k < 3; k++) {
            for (int l = 0; l < 3; l++) {
              if (triangles[i * 3 + k] == trianglesCleaned[j * 3 + l]) pool.Remove(trianglesCleaned[j * 3 + l]);
            }
          }
          if (pool.Count == 0) {
            unique = false;
            break;
          }
        }
        if (unique) {
          Vector3 triOrigin = new Vector3();
          triOrigin.x = (vertices[triangles[i * 3]].x + vertices[triangles[i * 3 + 1]].x + vertices[triangles[i * 3 + 2]].x) / 3;
          triOrigin.y = (vertices[triangles[i * 3]].y + vertices[triangles[i * 3 + 1]].y + vertices[triangles[i * 3 + 2]].y) / 3;
          triOrigin.z = (vertices[triangles[i * 3]].z + vertices[triangles[i * 3 + 1]].z + vertices[triangles[i * 3 + 2]].z) / 3;
          Vector3 originToTri = triOrigin - origin;

          Vector3 perpendicular = new Vector3(Vector2.Perpendicular(new Vector2(originToTri.x, originToTri.y)).x, Vector2.Perpendicular(new Vector2(originToTri.x, originToTri.y)).y, triOrigin.z).normalized;
          Vector3 relVectorCross = Vector3.Cross(originToTri.normalized, perpendicular).normalized;
          float greatestDotX = 0;
          int greatestDotIndexX = 0;
          for (int j = 0; j < 3; j++) {
            float dot = Vector3.Dot(vertices[triangles[i * 3 + j]] - triOrigin, relVectorCross);
            if (dot > greatestDotX) {
              greatestDotX = dot;
              greatestDotIndexX = j;
            }
          }
          float greatestDotY = 0;
          int greatestDotIndexY = 0;
          for (int j = 0; j < 3; j++) {
            if (j == greatestDotIndexX) continue;
            float dot = Vector3.Dot(vertices[triangles[i * 3 + j]] - triOrigin, perpendicular);
            if (dot > greatestDotY) {
              greatestDotY = dot;
              greatestDotIndexY = j;
            }
          }

          trianglesCleaned.Add(triangles[i * 3 + 3 - greatestDotIndexX - greatestDotIndexY]);
          trianglesCleaned.Add(triangles[i * 3 + greatestDotIndexY]);
          trianglesCleaned.Add(triangles[i * 3 + greatestDotIndexX]);
          //for (int j = 0; j < 3; j++) trianglesCleaned.Add(triangles[i * 3 + j]);
        }
      }
      return trianglesCleaned.ToArray();
    }
  }
}