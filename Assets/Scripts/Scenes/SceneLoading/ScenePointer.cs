using System;
using UnityEngine.SceneManagement;
using Utils.Core.SceneManagement;

namespace SceneLoadingManagement
{
    [Serializable]
    public class ScenePointer : SceneReference
    {
        public bool IsLoading => isLoading;

        private bool isLoading = false;

        public void SetIsLoading(bool isLoading)
        {
            this.isLoading = isLoading;
        }

        public Scene Scene
        {
            get
            {
                return SceneManager.GetSceneByName(this.SceneName);
            }
        }
    }
}
