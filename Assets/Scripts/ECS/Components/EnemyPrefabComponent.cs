using Unity.Entities;

public struct EnemyPrefabComponent : IComponentData
{
    public Entity basicEnemy;
    public Entity aggresiveEnemy;
    public Entity lurkingEnemy;
}
