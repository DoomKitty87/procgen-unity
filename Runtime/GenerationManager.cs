using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcGen;

public class GenerationManager : MonoBehaviour
{

  void Start()
  {
    var arry = NoiseMapGeneration.PerlinNoiseJobs(5, 5, 5, 0, 0, 2, 5, 1, 3);
    foreach (float f in arry) print(f);
    TileGeneration.GenerateTile("perlin", Vector3.zero, 1, 1, 100, 100, 5, 2, 5, 1);
  }

  void Update()
  {
    
  }
}
