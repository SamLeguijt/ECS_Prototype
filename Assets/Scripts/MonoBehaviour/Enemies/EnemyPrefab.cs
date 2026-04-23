using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrefab : MonoBehaviour
{
    [field: SerializeField] public GameObject Prefab = null;
    [field: SerializeField] public BaseEnemyData Data = null;
    [field: SerializeField] public MeshFilter meshFilter = null;
    [field: SerializeField] public Renderer renderers = null;

}
