using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ECS_Prototyping
{
    public class SceneLoaderAuthoring : MonoBehaviour
    {
#if UNITY_EDITOR
    [SerializeField] private ScenePointer monoScene;
    [SerializeField] private ScenePointer[] subScenes;
    [SerializeField] private ScenePointer ownerScene;

    [SerializeField] private SceneLoadingData sceneLoadData;

    class SceneLoaderBaker : Baker<SceneLoaderAuthoring>
    {
        public override void Bake(SceneLoaderAuthoring authoring)
        {
            for (int i = 0; i < authoring.subScenes.Length; i++)
            {
                var subReference = new EntitySceneReference(authoring.subScenes[i].SceneAsset);

                var entity = CreateAdditionalEntity(TransformUsageFlags.None);
                AddComponent(entity, new SceneLoadComponent
                {
                    SubSceneReference = subReference,
                    MonoSceneName = authoring.monoScene.SceneName,
                    OwnerScene = authoring.ownerScene.SceneName
                }) ;

                AddComponent(entity, new EntityIdentifierComponent
                {
                    Name = "SubScene Entity"
                });
            }
        }
    }

#endif
    }
}