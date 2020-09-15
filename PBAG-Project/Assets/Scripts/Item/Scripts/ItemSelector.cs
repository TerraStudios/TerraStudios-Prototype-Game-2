using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSelector : MonoBehaviour
{
    [HideInInspector] public Transform SelectedItem;
    public LayerMask ItemLayer;

    [Header("Components")]
    public Camera MainCamera;
    public ItemInfoUI itemInfoUI;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !RemoveSystem.instance.removeModeEnabled)
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
        {
            ItemBehaviour behaviour = SelectedItem.GetComponent<ItemBehaviour>();

            if (behaviour)
            {
                itemInfoUI.OnUIOpen(); //enable UI
                itemInfoUI.SetData(SelectedItem.gameObject, behaviour.data);
            }

        }
        else
        {
            itemInfoUI.OnUIExit();
        }
    }
}
