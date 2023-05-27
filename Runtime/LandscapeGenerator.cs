using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcGen;

public class LandscapeGenerator : MonoBehaviour
{

  enum noiseType {
    perlin,
    cellular
  };

  [SerializeField] private noiseType noiseSelection = noiseType.perlin;
  [Header("Generation Settings")]
  [SerializeField] private bool generateOnStart;
  [SerializeField] private bool generateOnUpdate;
  [SerializeField] private int tilesX;
  [SerializeField] private int tilesZ;
  [SerializeField] private int tileResolution;
  [SerializeField] private float tileScale;
  [SerializeField] private float noiseScale;
  [SerializeField] private int waves;
  [SerializeField] private float amplitude;
  [SerializeField] private float frequency;
  [SerializeField] private int seed;
  [SerializeField] private float yOffset;

  [Header("LOD Settings")]
  [SerializeField] private bool useLOD;
  [SerializeField] private Transform lodTarget;
  [SerializeField] private float lodStartDistance;

  [Header("Visual Settings")]
  [SerializeField] private Material groundMaterial;

  private void Start() {
    if (generateOnStart) GenerateLandscape();
  }

  private void Update() {
    if (generateOnUpdate) GenerateLandscape();
  }

  public void TriggerGenerate() {
    GenerateLandscape();
  }

  private void GenerateLandscape() {
    string noise;
    switch (noiseSelection) {
      case noiseType.perlin:
        noise = "perlin";
        break;
      case noiseType.cellular:
        noise = "cellular";
        break;
    }
    for (int x = 0; x < tilesX; x++) {
      for (int z = 0; z < tilesZ; z++) {
        
        Vector3 position = new Vector3(x * tileResolution * tileScale, yOffset, z * tileResolution * tileScale);
        GameObject tile;
        if (useLOD) {
          lodFac = (lodTarget.position - position).magnitude > lodStartDistance ? (((lodTarget.position - position).magnitude - lodStartDistance) > tileResolution ? tileResolution : (lodTarget.position - position).magnitude - lodStartDistance) : 1;
          tile = TileGeneration.GenerateTile(noise, position, seed, tileScale * lodFac, tileResolution / lodFac, noiseScale, waves, amplitude, frequency);
        }
        else {
          tile = TileGeneration.GenerateTile(noise, position, seed, tileScale, tileResolution, tileResolution, noiseScale, waves, amplitude, frequency);
        }
        tile.transform.parent = transform;
        tile.GetComponent<Renderer>().material = groundMaterial;
      }
    }
  }
}
