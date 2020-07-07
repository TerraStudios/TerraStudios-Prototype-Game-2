using System;
using UnityEngine;

/// <summary>
/// MonoBehaviour used for each IO of a building. Controlled by BuildingIOManager
/// </summary>
public class BuildingIO : MonoBehaviour
{
    [Tooltip("Toggles whether the BuildingIO is an input collider")]
    public bool isInput;
    [Tooltip("Toggles whether the BuildingIO is an exit collider")]
    public bool isOutput;
    public MeshRenderer MeshRenderer;
    public Collider coll;
    public BoxCollider itemIO;
    public BuildingIOManager myManager;

    [HideInInspector] public bool ReadyToLink = false;

    [Header("Input Configuration")]
    [Tooltip("Toggles whether debug prints should be printed or not")]
    public bool enableDebug = true;
    [Tooltip("Determines whether an IO is a trashcan output")]
    public bool isTrashcanOutput;
    [Tooltip("Determines the items allowed to enter the building")]
    public ItemData[] itemsAllowedToEnter;
    [Tooltip("Determines the item categories allowed to enter the building")]
    public ItemCategory[] itemCategoriesAllowedToEnter;

    //[Header("Dynamic variables")]
    public BuildingIO attachedIO;
    [HideInInspector] public bool visualizeIO = true;
    [HideInInspector] public Transform arrow;

    #region Initialization

    /// <summary>
    /// Initializes the BuildingIO by setting the visualization to false
    /// </summary>
    public void Init()
    {
        visualizeIO = false;
    }

    #endregion

    #region IO Trigger Events

    /// <summary>
    /// Sends an update that a collider has entered one of the colliders
    /// </summary>
    /// <param name="other">The collider that entered</param>
    private void OnTriggerEnter(Collider other)
    {
        OnUpdateIO(other);
    }

    /// <summary>
    /// Sends an update that a collider has exited one of the colliders
    /// </summary>
    /// <param name="other">The collider that exited</param>
    private void OnTriggerExit(Collider other)
    {
        OnUpdateIO(other, true);
    }

    /// <summary>
    /// Event for when items enter a building.
    ///
    /// - If an object is allowed to enter, the method <code>ProceedItemEnter(GameObject, ItemData)</code> in <see cref="BuildingIOManager"/> is called.
    ///
    /// </summary>
    /// <param name="item">The item data of the game object that entered the building</param>
    public void OnItemEnter(ItemBehaviour item)
    {
        bool allowedToEnter = false;

        foreach (ItemData data in itemsAllowedToEnter)
        {
            if (item.data.ID == data.ID)
                allowedToEnter = true;
        }

        foreach (ItemCategory data in itemCategoriesAllowedToEnter)
        {
            if (item.data.ItemCategory == data)
                allowedToEnter = true;
        }

        if (itemsAllowedToEnter.Length == 0 && itemCategoriesAllowedToEnter.Length == 0)
            allowedToEnter = true;

        if (!allowedToEnter)
            return;

        myManager.ProceedItemEnter(item.gameObject, item.data, Array.FindIndex(myManager.inputs, row => this));
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="item"></param>
    public void OnItemExit(ItemBehaviour item)
    {
        Debug.Log("Item exit " + item.data.name);
        //! probably not needed
    }

    #endregion

    #region Item Instantiation

    /// <summary>
    /// Spawns an <see cref="ItemData"/> at one of the exit IOs
    /// </summary>
    /// <param name="itemToSpawn"></param>
    public void SpawnItemObj(ItemData itemToSpawn)
    {
        Vector3 spawnPos;

        if (myManager.isConveyor)
            spawnPos = transform.position + Vector3.up * 0.25f;
        else
            spawnPos = itemIO.transform.position;

        ObjectPoolManager.instance.ReuseObject(itemToSpawn.obj.gameObject, spawnPos, Quaternion.identity);

        //Instantiate(itemToSpawn.obj, spawnPos, Quaternion.identity);
    }

    #endregion

    #region Indicator Visualization

    /// <summary>
    /// Sends a request to visualize the build indicators
    /// </summary>
    public void Visualize()
    {
        if (!visualizeIO)
        {
            Visualize(Color.blue);
            visualizeIO = true;
        }
    }

    /// <summary>
    /// Visualizes an indicator using the ObjectPoolManager reuse system.
    /// </summary>
    /// <param name="color">The color for the MeshRenderer</param>
    private void Visualize(Color color)
    {

        if (attachedIO) return;
        //Old cube render
        //MeshRenderer.enabled = true;
        //MeshRenderer.material.color = color;

        //New arrow render
        //TODO: Have the arrow part of the IO system before to remove instantiates
        if (arrow != null)
        {
            arrow.GetComponent<MeshRenderer>().material.color = color;
        }
        else
        {
            arrow = ObjectPoolManager.instance.ReuseObject(BuildingManager.instance.ArrowIndicator.gameObject, gameObject.transform.position, gameObject.transform.rotation).transform;
            //Instantiate(BuildingManager.instance.ArrowPrefab, gameObject.transform.position, gameObject.transform.rotation);
            arrow.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            arrow.transform.position += new Vector3(0, 1, 0);
            arrow.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    /// <summary>
    /// Devisualizes the indicator is one is currently visible.
    /// </summary>
    public void Devisualize()
    {
        if (arrow != null)
        {
            ObjectPoolManager.instance.DestroyObject(arrow.gameObject);
            //Destroy(arrow.gameObject);
            visualizeIO = false;
            arrow = null;
        }
        MeshRenderer.enabled = false;
    }

    #endregion

    #region IO Update
    /// <summary>
    /// Event called when a collider either enters or exits an IO collider
    /// </summary>
    /// <param name="other">The collider that entered the collider</param>
    /// <param name="exit">Whether the IO collider was an output or input</param>
    private void OnUpdateIO(Collider other, bool exit = false)
    {
        if (gameObject.name == "Input")
        {
            if (enableDebug) Debug.Log("Am input, exit is " + exit);
        }
        if (!exit)
        {
            BuildingIO hit = other.GetComponent<BuildingIO>();

            if (hit == null || hit == this)
                return;

            bool isInputUnsupported = IsInputUnsupported(hit);

            if (enableDebug) Debug.Log(1);

            if ((!hit.visualizeIO || (hit.ReadyToLink && this.ReadyToLink)) && !isInputUnsupported)
            {
                attachedIO = hit;
                hit.attachedIO = this;

                attachedIO.ReadyToLink = false;
                this.ReadyToLink = false;

                if (arrow && arrow.gameObject)
                {
                    ObjectPoolManager.instance.DestroyObject(arrow.gameObject);
                    arrow = null; //Could be removed, doing this just to make sure it's null
                }

                if (hit.arrow && hit.arrow.gameObject)
                {
                    ObjectPoolManager.instance.DestroyObject(hit.arrow.gameObject);
                    arrow = null;
                }
            }
            else if (visualizeIO)
            {

                if (enableDebug) Debug.Log(2);
                if (isInputUnsupported)
                {
                    if (enableDebug) Debug.Log(3);
                    if (arrow != null)
                    {
                        arrow.GetComponent<MeshRenderer>().material.color = Color.red;
                    }
                }
                else
                {
                    if (arrow != null)
                    {
                        arrow.GetComponent<MeshRenderer>().material.color = Color.green;
                    }

                }
            }

        }
        else
        {
            if (enableDebug) Debug.Log("Resetting arrows");
            if (arrow != null) arrow.GetComponent<MeshRenderer>().material.color = Color.blue; //reset arrow

            BuildingIO hit = other.GetComponent<BuildingIO>();

            if (hit == null || hit == this)
                return;

            if (visualizeIO)
            {
                // subject of change
                if (hit.arrow != null) hit.arrow.GetComponent<MeshRenderer>().material.color = Color.blue;
                //hit.Devisualize();
            }
        }
    }

    /// <summary>
    /// Returns whether an input should be attached or not
    ///
    /// - If two inputs are trying to connect it should return false, as they shouldn't be linked or else no items would flow between the two
    /// - Likewise, if two outputs are trying to connect it should also return false.
    /// - If there are two opposite types of BuildingIOs (e.g. male and female connection or female and male connection) are checked the method will return true, as they will connect properly.
    /// </summary>
    /// <param name="other">The other BuildingIO being checked</param>
    /// <returns>Whether the two BuildingIOs are a suitable input</returns>
    private bool IsInputUnsupported(BuildingIO other)
    {
        bool toReturn = false;

        if (!myManager.isConveyor && !other.myManager.isConveyor)
            toReturn = true;
        if (isInput && other.isInput)
            toReturn = true;
        if (isOutput && other.isOutput)
            toReturn = true;

        return toReturn;
    }

    #endregion

    #region Editor

    /// <summary>
    /// Draws an arrow where the building is faced in the editor.
    /// - Used for setting the correct rotation (IO)
    /// </summary>
    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 0.5f);

        Vector3 right = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(0, 180 + 20.0f, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(0, 180 - 20.0f, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(transform.position + transform.forward * 0.5f, right * 0.15f);
        Gizmos.DrawRay(transform.position + transform.forward * 0.5f, left * 0.15f);
    }

    #endregion
}