//
// Developed by TerraStudios (https://github.com/TerraStudios)
//
// Copyright(c) 2020-2021 Konstantin Milev (konstantin890 | milev109@gmail.com)
// Copyright(c) 2020-2021 Yerti (UZ9)
//
// The following script has been written by either konstantin890 or Yerti (UZ9) or both.
// This file is covered by the GNU GPL v3 license. Read LICENSE.md for more information.
// Past NDA/MNDA and Confidential notices are revoked and invalid since no agreement took place. Read README.md for more information.
//

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
                if (!RemoveSystem.RemoveModeEnabled)
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
