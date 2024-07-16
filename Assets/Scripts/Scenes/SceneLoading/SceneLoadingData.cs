using UnityEngine;


namespace SceneLoadingManagement
{
    public class SceneLoadingData : ScriptableObject
    {

        /* ----- PROPERTIES ----- */

        public ScenePointer MainSceneToLoad => mainSceneToLoad;
        public ScenePointer[] SubScenesToLoad => subScenesToLoad;
        public ScenePointer[] ScenesToUnload => scenesToUnload;
        public AsyncOperation[] SubSceneLoadOperations { get { return subSceneLoadOperations; } set { subSceneLoadOperations = value; } }
        public bool AutoActivateSubScenes => autoActivateSubScenes;
        public bool UnloadSubSceneOnActivate => unloadSubSceneOnActivate;


        /* ----- FIELDS ----- */

        [Tooltip("The target scene to load in")]
        [SerializeField] private ScenePointer mainSceneToLoad;

        [Tooltip("Optionally load in these scenes, activated once main scene is loaded.")]
        [SerializeField] private ScenePointer[] subScenesToLoad;

        [Tooltip("Check box to auto activate and enable the subscene when loaded, otherwise waits for MainScene to be loaded.")]
        [SerializeField] private bool autoActivateSubScenes = false;
        
        [Tooltip("Check box to unload the subscenes when activated, so contents are stored in memory but scene is not open.")]
        [SerializeField] private bool unloadSubSceneOnActivate = false;

        [Tooltip("The scene(s) that need to be unloaded after loading in the Main scene.")]
        [SerializeField] private ScenePointer[] scenesToUnload;

        private AsyncOperation[] subSceneLoadOperations = null;

    }
}