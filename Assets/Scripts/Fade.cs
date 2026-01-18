using UnityEngine;
using DG.Tweening;

public class Fade : MonoBehaviour
{
    public float duration = 1f;

    public void FadeIn()
    {
        if (TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, duration);
        }
        else if (TryGetComponent<Renderer>(out var r))
        {
            Color c = r.material.color;
            c.a = 0f;
            r.material.color = c;
            r.material.DOFade(1f, duration);
        }
    }

    public void FadeOut()
    {
        if (TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup.DOFade(0f, duration);
        }
        else if (TryGetComponent<Renderer>(out var r))
        {
            r.material.DOFade(0f, duration);
        }
    }
}
