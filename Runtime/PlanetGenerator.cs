using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcGen;

public class PlanetGenerator : MonoBehaviour
{

  private void Start() {
    ComplexGeneration.GenerateSphere("perlin", new Vector3(0, 0, 0), 0, 100, 1, 1, 1, 1, 1);
  }
}