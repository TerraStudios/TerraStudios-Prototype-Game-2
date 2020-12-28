using Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ItemManagement
{
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

        private Vector2 mouseDelta;

        public void SelectItem(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                if (!RemoveSystem.instance.removeModeEnabled)
                {
                    Transform hit = GetItemHit(mouseDelta);

                    if (selectedItem != hit)
                        SelectItem(hit);
                }
            }
        }

        public void SelectPosition(InputAction.CallbackContext context) => mouseDelta = context.ReadValue<Vector2>();

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
}
