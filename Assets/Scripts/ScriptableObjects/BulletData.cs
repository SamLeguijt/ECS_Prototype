using UnityEngine;

[CreateAssetMenu(fileName = "New Bullet data", menuName = "ScriptableObjects/New Bullet data")]
public class BulletData : ScriptableObject
{
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float Lifetime { get; private set; }
    [field: SerializeField] public LayerMask CollisionLayers { get; private set; }
}
