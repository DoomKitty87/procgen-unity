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

    public static int[] windClosestFiltered(Vector3[] vertices, Vector3 origin) {
      List<int> triangles = new List<int>();
      for (int i = 0; i < vertices.Length; i++) {
        float smallestDist = float.PositiveInfinity;
        List<int> closestPoints = new List<int>();
        for (int j = 0; j < vertices.Length; j++) {
          if (j == i) continue;
          float xdist = Mathf.Abs(vertices[i].x - vertices[j].x);
          float ydist = Mathf.Abs(vertices[i].y - vertices[j].y);
          float zdist = Mathf.Abs(vertices[i].z - vertices[j].z);
          if (xdist + ydist + zdist < smallestDist) {
            closestPoints.Clear();
            closestPoints.Add(j);
          }
          else if (xdist + ydist + zdist == smallestDist) {
            closestPoints.Add(j);
          }
        }
        for (int j = 0; j < closestPoints.Length; j++) {
          float smallestDistN = float.PositiveInfinity;
          List<int> closestPointsN = new List<int>();
          for (int k = 0; k < vertices.Length; k++) {
            if (k == i || k == closestPoints[j]) continue;
            float xdist = Mathf.Abs(vertices[i].x - vertices[k].x) + Mathf.Abs(vertices[closestPoints[j]].x - vertices[k].x);
            float ydist = Mathf.Abs(vertices[i].y - vertices[k].y) + Mathf.Abs(vertices[closestPoints[j]].y - vertices[k].y);
            float zdist = Mathf.Abs(vertices[i].z - vertices[k].z) + Mathf.Abs(vertices[closestPoints[j]].z - vertices[k].z);
            if (xdist + ydist + zdist < smallestDist) {
              closestPointsN.Clear();
              closestPointsN.Add(k);
            }
            else if (xdist + ydist + zdist == smallestDist) {
              closestPointsN.Add(k);
            }
          }
          for (int k = 0; k < closestPointsN.Length; k++) {
            triangles.Add(i);
            triangles.Add(closestPoints[j]);
            triangles.Add(closestPointsN[k]);
          }
        }
      }
      List<int> trianglesCleaned = new List<int>();
      for (int i = 0; i < triangles.Length / 3; i++) {
        bool unique = true;
        for (int j = 0; j < trianglesCleaned.length / 3; j++) {
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
          float greatestDotIndexX = 0;
          for (int j = 0; j < 3; j++) {
            float dot = Vector3.Dot(vertices[triangles[i * 3 + j]] - triOrigin, relVectorCross);
            if (dot > greatestDotX) {
              greatestDotX = dot;
              greatestDotIndexX = j;
            }
          }
          float greatestDotY = 0;
          float greatestDotIndexY = 0;
          for (int j = 0; j < 3; j++) {
            if (j == greatestDotIndexX) continue;
            float dot = Vector3.Dot(vertices[triangles[i * 3 + j]] - triOrigin, perpendicular);
            if (dot > greatestDotY) {
              greatestDotY = dot;
              greatestDotIndexY = j;
            }
          }

          trianglesCleaned.Add(triangles[i * 3 + greatestDotIndexX]);
          trianglesCleaned.Add(triangles[i * 3 + greatestDotIndexY]);
          trianglesCleaned.Add(triangles[i * 3 + 3 - greatestDotIndexX - greatestDotIndexY]);

          //for (int j = 0; j < 3; j++) trianglesCleaned.Add(triangles[i * 3 + j]);
        }
      }
      return trianglesCleaned.ToArray();
    }
  }
}