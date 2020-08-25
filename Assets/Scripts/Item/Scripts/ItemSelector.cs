using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSelector : MonoBehaviour
{
    [HideInInspector] public Transform SelectedItem;
    public LayerMask ItemLayer;

    [Header("Components")]
    public Camera MainCamera;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Transform hit = GetItemHit(Input.mousePosition);

            if (SelectedItem != hit)
                SelectItem(hit);
        }
    }

    private Transform GetItemHit(Vector3 mousePos) 
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return SelectedItem;

        Ray ray = MainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ItemLayer))
            return hit.transform;
        else
            return null;
    }

    public void SelectItem(Transform hit) 
    {
        SelectedItem = hit;
        if (hit)
            Debug.Log("Selected " + hit.gameObject.name);
        else
            Debug.Log("Unselected item");
    }
}
