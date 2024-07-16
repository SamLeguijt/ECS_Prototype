using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Scenes;
using Unity.Collections;
using UnityEngine.SceneManagement;
using SceneLoadingManagement;
using Utils.Core.Services;
using Utils.Core;
using Unity.Entities.Serialization;
using Hash128 = Unity.Entities.Hash128;
using Unity.Burst;

namespace ECS_Prototyping
{
    [BurstCompile]
    public partial class SceneLoadSystem : SystemBase
    {
        private EntityQuery newRequests;
        private List<Entity> sceneEntities = new List<Entity>();
        private List<Entity> loadedSubScenes = new List<Entity>();
        private List<Entity> subScenesToUnload = new List<Entity>();
        private List<EntitySceneReference> entitySceneRefs = new List<EntitySceneReference>();

        private bool loadMainSceneCalled = false;
        string MainSceneToLoad = string.Empty;

        CoroutineService coroutineService = null;
        CoroutineTask loadSceneRoutine = null;

        private string currentMainSceneName = string.Empty;
        private string previousMainSceneName = string.Empty;

        private string startGameSceneName = string.Empty;

        private void Reset()
        {
            // Set up the system. 
            Enabled = false;
            newRequests = GetEntityQuery(typeof(SceneLoadComponent));

            // Stage the previously loaded subscenes for unloading later.
            subScenesToUnload.Clear();
            subScenesToUnload.AddRange(loadedSubScenes);

            // Clear the lists because we're in a new scene.
            sceneEntities.Clear();
            entitySceneRefs.Clear();
            loadedSubScenes.Clear();

            // Also reset the other variables for the new scene.
            loadSceneRoutine = null;
            loadMainSceneCalled = false;
            MainSceneToLoad = string.Empty;
        }

        protected override void OnCreate()
        {
            // The scene that is active when entering playmode, because thats when the system gets created.
            startGameSceneName = SceneManager.GetActiveScene().name;

            // SystemBase does not traditionally support coroutines, so use the GlobalServiceLocator.
            coroutineService = GlobalServiceLocator.Instance.Get<CoroutineService>();

            // Set initial value to the field to prevent loading not working correctly on startup.
            currentMainSceneName = startGameSceneName;

            // Setup the rest of the class.
            Reset();
        }

        protected override void OnUpdate()
        {
            var requests = newRequests.ToComponentDataArray<SceneLoadComponent>(Allocator.Temp);

            // Requests holds the entities with a SceneLoadComponent as an array. 
            for (int i = 0; i < requests.Length; i++)
            {
                // Only get scene references if the SceneLoadComponents are present in the scene that currently the main scene target.
                if (requests[i].OwnerScene.ToString() != currentMainSceneName)
                    continue;

                // Save the name of the main scene from the component.
                MainSceneToLoad = requests[i].MonoSceneName.ToString();

                // Add the Subscenes to a list to iterate over.
                if (!entitySceneRefs.Contains(requests[i].SubSceneReference))
                {
                    entitySceneRefs.Add(requests[i].SubSceneReference);
                }

                // Start coroutine to load in the scenes if not started yet.
                if (loadSceneRoutine == null)
                {
                    loadSceneRoutine = coroutineService.StartCoroutine(LoadScenes());
                }
            }

            // Get rid of the requests and native array.
            requests.Dispose();
            EntityManager.DestroyEntity(newRequests);
        }


        private IEnumerator LoadScenes()
        {
            // Not necessary, handy for a develop.
            WaitForSeconds wait = new WaitForSeconds(.5f);

            // Load in all subscenes with a small delay.
            for (int i = 0; i < entitySceneRefs.Count; i++)
            {
                yield return wait;

                // Load in the subscene.
                Entity sceneEntity = SceneSystem.LoadSceneAsync(World.Unmanaged, entitySceneRefs[i]);

                // Add to list to iterate over all loading scenes.
                if (!sceneEntities.Contains(sceneEntity))
                {
                    sceneEntities.Add(sceneEntity);
                }
            }

            // Wait till all scenes are loaded.
            for (int i = 0; i < sceneEntities.Count; i++)
            {
                yield return IsSceneLoaded(sceneEntities[i]);
                loadedSubScenes.Add(sceneEntities[i]);
            }

            // After loading in all subscenes, load in the mono scene using the SceneLoadingController.
            // With a callback to unload the current active scene once the new mono scene is loaded completely.
            if (!loadMainSceneCalled)
            {
                SceneLoadingController.LoadSceneFromName(MainSceneToLoad, OnAllScenesLoaded);

                loadMainSceneCalled = true;
            }
        }

        private void OnAllScenesLoaded()
        {
            previousMainSceneName = currentMainSceneName;
            currentMainSceneName = MainSceneToLoad;

            UnloadPreviousScenes();
            Reset();
        }

        private void UnloadPreviousScenes()
        {
            // We do not want to unload the starting scene of the game, cause it holds the bakery that we need.
            if (previousMainSceneName != startGameSceneName)
            {
                // Unload the previously loaded main scene.
                SceneManager.UnloadSceneAsync(previousMainSceneName);

                // Unload the subscenes used alongside the previous main scene.
                for (int i = 0; i < subScenesToUnload.Count; i++)
                {
                    SceneSystem.UnloadScene(World.Unmanaged, subScenesToUnload[i], SceneSystem.UnloadParameters.DestroyMetaEntities);
                }
            }
        }

        private bool IsSceneLoaded(Entity sceneEntity)
        {
            bool isLoaded = SceneSystem.IsSceneLoaded(World.Unmanaged, sceneEntity);

            bool isValidLoaded = SceneSystem.GetSceneStreamingState(World.Unmanaged, sceneEntity) == SceneSystem.SceneStreamingState.LoadedSuccessfully;

            return isLoaded && isValidLoaded;
        }
    }
}