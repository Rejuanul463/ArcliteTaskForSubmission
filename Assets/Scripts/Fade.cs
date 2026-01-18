using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections;

public class Fade : MonoBehaviour
{
    public float duration = 1f;

    private Renderer rend;
    private Material mat;
    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");

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
            // Fade in UI
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, duration);
        }
        else if (mat != null)
        {
            // Fade in 3D object
            Color c = mat.GetColor(BaseColorID);
            c.a = 0f;
            mat.SetColor(BaseColorID, c);

            DOTween.To(
                () => mat.GetColor(BaseColorID).a,
                x =>
                {
                    Color col = mat.GetColor(BaseColorID);
                    col.a = x;
                    mat.SetColor(BaseColorID, col);
                },
                1f,
                duration
            );
        }
    }

    public void FadeOut()
    {
        if (TryGetComponent(out CanvasGroup canvasGroup))
        {
            // fade out UI
            canvasGroup.DOFade(0f, duration);
        }
        else if (mat != null)
        {
            // fade out 3D object
            Color c = mat.GetColor(BaseColorID);
            float startAlpha = c.a;

            DOTween.To(
                () => startAlpha,
                x =>
                {
                    Color col = mat.GetColor(BaseColorID);
                    col.a = x;
                    mat.SetColor(BaseColorID, col);
                },
                0f,
                duration
            );

            StartCoroutine(RemoveObject());
        }
    }

    private IEnumerator RemoveObject()
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
        // remove running tween if any
        DOTween.Kill(this);
    }

    private void SceneChanged(Scene prev, Scene next)
    {
        if (next == gameObject.scene) // Fade in when activating scene
        {
            FadeIn();
        }
    }

}
