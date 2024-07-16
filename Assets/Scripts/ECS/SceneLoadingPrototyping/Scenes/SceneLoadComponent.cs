using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Collections;

namespace ECS_Prototyping
{
    public struct SceneLoadComponent : IComponentData
    {
        public EntitySceneReference SubSceneReference;
        public FixedString128Bytes MonoSceneName;
        public FixedString128Bytes OwnerScene;
    }

}


