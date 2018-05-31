using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace kawetofe.randomPrefabPlacer {
    [CreateAssetMenu(fileName = "RPPBrush", menuName = "RandomPrefabPlacer/RPPBrush")]
    public class RPPBrush : ScriptableObject {
        public List<PlacementPrefab> prefabs = new List<PlacementPrefab>();
        public int objectsToPlace = 10;
    } }
