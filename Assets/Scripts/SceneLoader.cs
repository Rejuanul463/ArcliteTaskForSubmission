using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    // list of loaded scenes
    public static List<Scene> LoadedScenes { get; private set; } = new List<Scene>();

    [Header("Timeline Buttons")]
    [SerializeField] private Button[] timelineButtons;
    [SerializeField] private List<int> sceneIndicesToLoad;

    [Header("Restart Button")]
    [SerializeField] private Button restartButton;
    [SerializeField] private int restartSceneIndex = 3;
    private int prevInd = -1;

    [Header("UI Settings")]
    [SerializeField] private int canvasSortingOrder = 1000;

    [SerializeField] private Fade fadeWhileTransition;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // to make canvas of timeline appear top of all the scene
        if (TryGetComponent(out Canvas canvas))
        {
            canvas.sortingOrder = canvasSortingOrder;
        }

        // Initialize all scenes
        StartCoroutine(InitializeScenes(sceneIndicesToLoad));

        // Button Listener
        SetupButtons();
    }

    
    private IEnumerator InitializeScenes(List<int> sceneIndices)
    {
        foreach (int index in sceneIndices)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            Scene loadedScene = SceneManager.GetSceneByBuildIndex(index);

            if (!LoadedScenes.Contains(loadedScene))
            {
                LoadedScenes.Add(loadedScene);
            }

            SetSceneRootObjectsActive(loadedScene, false);
        }

        // Activate the first scene by default
        if (LoadedScenes.Count > 0)
        {
            ActivateScene(LoadedScenes[0].buildIndex);
        }
    }

    public void ActivateScene(int sceneIndex)
    {
        if (sceneIndex == 2 && prevInd == 1) {
            fadeWhileTransition.gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
            fadeWhileTransition.FadeOut(); 
        }
        prevInd = sceneIndex;

        SetActiveButtonColour(sceneIndex);
        foreach (Scene scene in LoadedScenes)
        {
            bool isActive = scene.buildIndex == sceneIndex;

            SetSceneRootObjectsActive(scene, isActive);

            if (isActive)
            {
                SceneManager.SetActiveScene(scene);
                Debug.Log($"Scene {sceneIndex} is now active.");

                UpdateRestartButton(scene.buildIndex);
            }
        }
    }

    private void SetActiveButtonColour(int sceneIndex)
    {
        for (int i = 1; i <= timelineButtons.Length; i++)
        {
            ColorBlock colors = timelineButtons[i-1].colors;
            colors.normalColor = (i == sceneIndex) ? Color.green : Color.white;
            timelineButtons[i-1].colors = colors;
        }
    }


    private void SetSceneRootObjectsActive(Scene scene, bool isActive)
    {
        foreach (GameObject go in scene.GetRootGameObjects())
        {
            go.SetActive(isActive);
        }
    }



    #region Button Setup

    private void SetupButtons()
    {
        if (timelineButtons.Length != sceneIndicesToLoad.Count)
        {
            Debug.LogWarning("Timeline buttons count does not match scene indices count.");
        }

        for (int i = 0; i < timelineButtons.Length; i++)
        {
            int index = i; // Capture index to avoid closure issues
            timelineButtons[i].onClick.AddListener(() =>
            {
                if (index < sceneIndicesToLoad.Count)
                {
                    ActivateScene(sceneIndicesToLoad[index]);
                }
            });
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(ReloadScenes);
        }
    }

    #endregion

    #region Reload Scenes

    private void ReloadScenes()
    {
        StartCoroutine(ReloadScenesCoroutine());
    }

    private IEnumerator ReloadScenesCoroutine()
    {
        // Unload all loaded scenes in reverse order
        for (int i = LoadedScenes.Count - 1; i >= 0; i--)
        {
            Scene scene = LoadedScenes[i];

            if (scene.IsValid())
            {
                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(scene);
                while (!asyncUnload.isDone)
                {
                    yield return null;
                }
            }
        }

        LoadedScenes.Clear();

        // Reload all scenes
        yield return StartCoroutine(InitializeScenes(sceneIndicesToLoad));
    }

    #endregion

    #region UI Helpers

    private void UpdateRestartButton(int activeSceneIndex)
    {
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(activeSceneIndex == restartSceneIndex);
        }
    }

    #endregion
}
