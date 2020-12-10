using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class handles Item selection.
/// It is used when the player clicks on an item.
/// </summary>
public class ItemSelector : MonoBehaviour
{
    [HideInInspector] public Transform selectedItem;
    public LayerMask itemLayer;

    [Header("Components")]
    public Camera mainCamera;
    public ItemInfoUI itemInfoUI;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !RemoveSystem.instance.removeModeEnabled)
        {
            Transform hit = GetItemHit(Input.mousePosition);

            if (selectedItem != hit)
                SelectItem(hit);
        }
    }

    private Transform GetItemHit(Vector3 mousePos)
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return selectedItem;

        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, itemLayer))
            return hit.transform;
        else
            return null;
    }

    public void SelectItem(Transform hit)
    {
        selectedItem = hit;
        if (hit)
        {
            ItemBehaviour behaviour = selectedItem.GetComponent<ItemBehaviour>();

            if (behaviour)
            {
                itemInfoUI.OnUIOpen(); //enable UI
                itemInfoUI.SetData(selectedItem.gameObject, behaviour.data);
            }

        }
        else
        {
            itemInfoUI.OnUIExit();
        }
    }
}
