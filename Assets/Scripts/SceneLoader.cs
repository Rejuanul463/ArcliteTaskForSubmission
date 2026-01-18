using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Button[] timeLineButtons;

    int prevIndex = 0;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        for(int i = 0; i < timeLineButtons.Length; i++)
        {
            timeLineButtons[i].onClick.AddListener(() => LoadScene(i));
        }

        // Load first scene additively
        SceneManager.LoadScene(0, LoadSceneMode.Additive);
    }

    void LoadScene(int currentIndex)
    {
        if (currentIndex >= 0 && currentIndex < 3)
            return;

        changeButtonColour(currentIndex);
        prevIndex = currentIndex;
        SceneManager.UnloadSceneAsync(currentIndex);
        SceneManager.LoadScene(currentIndex, LoadSceneMode.Additive);
    }


    private void changeButtonColour(int currentIndex)
    {
        ColorBlock colorBlock = timeLineButtons[prevIndex].colors;
        colorBlock.normalColor = Color.white;
        timeLineButtons[prevIndex].colors = colorBlock;

        colorBlock = timeLineButtons[currentIndex].colors;
        colorBlock.normalColor = Color.green;
        timeLineButtons[currentIndex].colors = colorBlock;
    }

}
