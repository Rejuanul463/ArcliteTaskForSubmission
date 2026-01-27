using DG.Tweening;
using UnityEngine;

public class SphereRotation : MonoBehaviour
{
    public Transform orbitCenter;
    public float orbitSpeed = 20f;
    public float orbitDistance;
    public Vector3 orbitAxis = Vector3.up;
    public bool clockwise = true;

    void Start()
    {
        if (orbitCenter != null)
        {
            transform.localScale = orbitCenter.localScale * 0.5f;
            Vector3 direction = (transform.position - orbitCenter.position).normalized;
            if (direction == Vector3.zero) direction = Vector3.right;
            transform.position = orbitCenter.position + direction * orbitDistance * transform.localScale.x;
        }
    }

    void Update()
    {
        if (orbitCenter == null) return;

        float direction = clockwise ? 1f : -1f;
        transform.RotateAround(orbitCenter.position, orbitAxis, orbitSpeed * direction * Time.deltaTime);
    }

    void OnEnable()
    {
        SelectionHandler.OnObjectClicked += HandleClick;
    }

    void OnDisable()
    {
        SelectionHandler.OnObjectClicked -= HandleClick;
    }
    // to detect if clicked on sphere
    void HandleClick(GameObject clickedObject)
    {
        if (gameObject != clickedObject)
            GetComponent<Fade>().FadeOut();
        else
        {
            Vector3 centre = transform.root.position;
            transform.SetParent(null);
            transform.DOMove(centre, 2f);
            orbitCenter = null;
            DontDestroyOnLoad(gameObject);
        }
    }
}

