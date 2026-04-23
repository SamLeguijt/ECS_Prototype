using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="EnemyData", menuName = "ScriptableObjects/new Enemy data")]
public class BaseEnemyData : ScriptableObject
{
    [field: SerializeField] public float MovementSpeed {  get; private set; } 
    [field: SerializeField] public float PlayerInRangeMoveThreshold {  get; private set; }
    [field: SerializeField] public float PlayerInRangeRotationThreshold {  get; private set; }
    [field: SerializeField] public float StoppingDistance {  get; private set; }
}
