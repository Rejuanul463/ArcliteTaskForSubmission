using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    // Global list for the scenes
    public static List<Scene> loadedScenes = new List<Scene>();
    //Buttons that are used in timeline
    [SerializeField] Button[] timelineButtons;
    [SerializeField] List<int> sceneIndicesToLoad;
    [SerializeField] Button RestartButton;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        // Set loading scenes
        StartCoroutine(InitializeScenes(sceneIndicesToLoad));
        // Setup button listeners
        SetupButtons();

        GetComponent<Canvas>().sortingOrder = 1000; // to show it in top

    }

    // Load multiple scenes additively
    IEnumerator InitializeScenes(List<int> sceneIndices)
    {
        foreach (int index in sceneIndices)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            Scene loadedScene = SceneManager.GetSceneByBuildIndex(index);
            if (!loadedScenes.Contains(loadedScene))
            {
                loadedScenes.Add(loadedScene);
            }
            SetSceneActive(loadedScene, false);
        }
        //first scene is activated
        if (loadedScenes.Count > 0)
        {
            ActivateScene(loadedScenes[0].buildIndex);
        }
    }
    // To active Specific scenes
    public void ActivateScene(int sceneIndex)
    {
        for (int i = 0; i < loadedScenes.Count; i++)
        {
            Scene scene = loadedScenes[i];
            bool isActive = scene.buildIndex == sceneIndex;

            SetSceneActive(scene, isActive);

            if (isActive)
            {
                SceneManager.SetActiveScene(scene);
                Debug.Log("Scene " + sceneIndex + " is now active.");

                // RestartButton based on scene index
                UpdateRestartButton(scene.buildIndex);
            }
        }
    }
    void SetSceneActive(Scene scene, bool isActive)
    {
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject go in rootObjects)
        {
            go.SetActive(isActive);
        }
    }

    // Setup Listner to the timelineButtons
    void SetupButtons()
    {
        for (int i = 0; i < timelineButtons.Length; i++)
        {
            int index = i;
            timelineButtons[i].onClick.AddListener(() => ActivateScene(sceneIndicesToLoad[index]));
        }

        RestartButton.onClick.AddListener(ReloadScenes);
    }

    void ReloadScenes()
    {
        StartCoroutine(ReloadScenesCoroutine());
    }

    IEnumerator ReloadScenesCoroutine()
    {
        // at first all loaded scene
        for (int i = loadedScenes.Count - 1; i >= 0; i--)
        {
            Scene scene = loadedScenes[i];
            if (scene.IsValid())
            {
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
                while (!asyncUnload.isDone)
                    yield return null;
            }
        }
        loadedScenes.Clear();

        // Reload all scenes
        yield return StartCoroutine(InitializeScenes(sceneIndicesToLoad));

        Debug.Log("All scenes reloaded.");
    }


    private void UpdateRestartButton(int activeSceneIndex)
    {
        if (RestartButton != null)
        {
            RestartButton.gameObject.SetActive((activeSceneIndex == 3));
        }
    }

}
