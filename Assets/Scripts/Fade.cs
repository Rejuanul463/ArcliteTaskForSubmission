using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections;

public class Fade : MonoBehaviour
{
    public float duration = 1f;
    private Renderer rend;
    private Material mat;

    void Start()
    {
        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            mat = rend.material;
        }

    }

    public void FadeIn()
    {
        if (TryGetComponent(out CanvasGroup canvasGroup))
        {
            // make the UI fade in
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, duration);
        }
        else if (mat != null)
        {
            Color c = mat.GetColor("_BaseColor");
            c.a = 0f; // invissible at start
            mat.SetColor("_BaseColor", c);

            DOTween.To(() => mat.GetColor("_BaseColor").a,
                       x =>
                       {
                           Color col = mat.GetColor("_BaseColor");
                           col.a = x;
                           mat.SetColor("_BaseColor", col);
                       },
                       1f, duration);
        }
    }

    public void FadeOut()
    {
        if (TryGetComponent(out CanvasGroup canvasGroup))
        {
            //make the Ui fadeout
            canvasGroup.DOFade(0f, duration);
        }
        else if (mat != null)
        {
            Color c = mat.GetColor("_BaseColor");
            float startAlpha = c.a;
            DOTween.To(() => startAlpha,
                       x =>
                       {
                           Color col = mat.GetColor("_BaseColor");
                           col.a = x;
                           mat.SetColor("_BaseColor", col);
                       },
                       0f, duration);
            StartCoroutine(removeObject());
        }
    }

    IEnumerator removeObject()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    void OnEnable()
    {
        SceneManager.activeSceneChanged += SceneChanged;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= SceneChanged;
    }

    void SceneChanged(Scene prev, Scene next)
    {
        if (next == gameObject.scene) // whenever any scene gets active...
        {
            FadeIn();
        }
    }


}
