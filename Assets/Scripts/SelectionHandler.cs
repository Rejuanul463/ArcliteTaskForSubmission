using UnityEngine;
using System;

public class SelectionHandler : MonoBehaviour
{
    public static event Action<GameObject> OnObjectClicked;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                OnObjectClicked?.Invoke(hit.collider.gameObject);
            }
        }
    }
}
