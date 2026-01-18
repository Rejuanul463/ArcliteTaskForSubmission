using DG.Tweening;
using UnityEngine;

public class SphereRotation : MonoBehaviour
{
    public Transform target;

    public Vector3 axis = Vector3.up;
    public float speed = 50f;

    private Tween orbitTween;

    void Start()
    {
        if (target != null)
            StartOrbit();
    }

    public void StartOrbit()
    {
        StopOrbit();

        orbitTween = DOTween.To(() => 0f, x => transform.RotateAround(target.position, axis, speed * Time.deltaTime), 1f, 1f)
                    .SetLoops(-1).SetEase(Ease.Linear);
    }

    public void StopOrbit()
    {
        if (orbitTween != null && orbitTween.IsActive())
            orbitTween.Kill();
    }
}

