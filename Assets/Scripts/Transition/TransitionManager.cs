using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LMagent.Transition
{
    public class TransitionManager : Singleton<TransitionManager>
    {
        [SerializeField] public string UISceneName;
        [SerializeField] public string mainSceneName;

        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }

        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }


        private void Start()
        {
            if (IsSceneLoaded(UISceneName) == false) {
                StartCoroutine(LoadSceneSetActive(UISceneName));
            }
        }

        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGo)
        {
            StartCoroutine(Transition(sceneToGo, positionToGo));
        }

        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            
            yield return LoadSceneSetActive(sceneName);

            EventHandler.CallAfterSceneLoadedEvent();
        }

        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            Debug.Log(newScene.name);

            SceneManager.SetActiveScene(newScene);

            EventHandler.CallAfterSceneLoadedEvent();
        }

        public void ReloadCurrentScene()
        {
            string currentSceneName = this.mainSceneName;
            StartCoroutine(ReloadScene(currentSceneName));
        }

        private IEnumerator ReloadScene(string sceneName)
        {
            // Call any events before unloading the current scene
            EventHandler.CallBeforeSceneUnloadEvent();

            // Unload the current scene
            yield return SceneManager.UnloadSceneAsync(sceneName);

            // Reload the same scene
            yield return LoadSceneSetActive(sceneName);

            // Call any events after the scene is loaded
            EventHandler.CallAfterSceneLoadedEvent();
        }

        bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName && scene.isLoaded)
                {
                    return true;
                }
            }
            return false;
        }
    }
}