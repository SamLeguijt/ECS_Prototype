
namespace ECS_Prototyping
{
    public enum CollisionLayerECS
    {
        Default = 1 << 0,
        Projectile = 1 << 10,
        Enemy = 1 << 25,
        Environment = 1 << 26,
    }
}
