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

  [SerializeField] private bool generateOnStart;
  [SerializeField] private bool generateOnUpdate;

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

  }
}
