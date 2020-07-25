using System;
using System.Collections.Generic;
using System.Linq;
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
    public BuildingIOManager IOManager;

    [Header("Input Configuration")]
    [Tooltip("Determines whether an IO is a trashcan output")]
    public bool isTrashcanOutput;
    [Tooltip("Determines the items allowed to enter the building")]
    public ItemData[] itemsAllowedToEnter;
    [Tooltip("Determines the item categories allowed to enter the building")]
    public ItemCategory[] itemCategoriesAllowedToEnter;

    //[Header("Dynamic variables")]
    [HideInInspector] public BuildingIO attachedIO;

    private BuildingIO onPort;

    [HideInInspector] public bool visualizeIO = true;
    [HideInInspector] public Transform arrow;
    private LayerMask IOMask;
    private List<Collider> iosInside = new List<Collider>();

    #region Initialization

    /// <summary>
    /// Initializes the BuildingIO by setting the visualization to false
    /// </summary>
    public void Init()
    {
        visualizeIO = false;
        Devisualize();
    }

    private void Awake()
    {
        IOMask = LayerMask.GetMask("IOPort");
        VisualizeArrow(BuildingManager.instance.blueArrow);
    }

    #endregion

    #region IO Trigger Events

    /// <summary>
    /// Event for when the visualization is moved. Currently it does the following:
    /// - Call <see cref="Physics.OverlapBox(Vector3, Vector3, Quaternion, int)"/> with a layer mask limiting to only <see cref="BuildingIO"/>s 
    /// - Loop through all of the resulting colliders and check whether they're inside or outside the <see cref="iosInside"/> list
    /// Depending on the result of the previous bullet point the method may call either <see cref="OnIOEnter(Collider)"/> or <see cref="OnIOExit(Collider)"/>.
    /// 
    /// </summary>
    public void OnVisualizationMoved(Building b)
    {
        //check for any collisions inside of box 
        Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale * .5f, Quaternion.identity, IOMask);
        

        foreach (Collider inside in iosInside.ToList())
        {
            if (!this.IOManager.Equals(inside.GetComponent<BuildingIO>().IOManager) && !hitColliders.Contains(inside) && !inside.Equals(coll)) // inside the list, but not inside
            {
                iosInside.Remove(inside);
                OnIOExit(inside);
            }
        }

        foreach (Collider hit in hitColliders) //loop through each collider that was found
        {
            if (!this.IOManager.Equals(hit.GetComponent<BuildingIO>().IOManager) && !iosInside.Contains(hit) && !hit.Equals(coll)) // not in the list, and isn't this collider
            {
                iosInside.Add(hit);
                OnIOEnter(hit); //on enter
            }
        }


    }

    /// <summary>
    /// Used for debugging the IO detection by drawing a box around each physics check (seen in Scene view)
    /// </summary>
    public void DrawIODetectionBox()
    {
        Color toDraw = Color.white;
        if (onPort) toDraw = Color.green;
        if (attachedIO) toDraw = Color.magenta;
        ExtDebug.DrawBox(gameObject.transform.position, coll.bounds.size / 2, Quaternion.identity, toDraw);
    }

    /// <summary>
    /// Sends an update that a collider has entered one of the colliders
    /// </summary>
    /// <param name="other">The collider that entered</param>
    private void OnIOEnter(Collider other)
    {
        BuildingIO io = other.GetComponent<BuildingIO>();

        if (io)
        {
            //if (io.visualizeIO)
            //return;

            onPort = io;

            if (IsInputSupported(io))
            {
                VisualizeArrow(BuildingManager.instance.greenArrow); //visualize green arrow
            }
            else
            {
                VisualizeArrow(BuildingManager.instance.redArrow); //visualize red arrow
            }
        }
    }

    /// <summary>
    /// Sends an update that a collider has exited one of the colliders
    /// </summary>
    /// <param name="other">The collider that exited</param>
    private void OnIOExit(Collider other)
    {
        BuildingIO io = other.GetComponent<BuildingIO>();

        if (io)
        {
            onPort = null;
            //TODO: Perhaps have a cached building component for visualization in grid manager to avoid calling get component here
            if (io.visualizeIO && !visualizeIO)
            {
                Devisualize();
            }
            else
            {
                VisualizeArrow(BuildingManager.instance.blueArrow);
            }
        }

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

        IOManager.ProceedItemEnter(item.gameObject, item.data, Array.FindIndex(IOManager.inputs, row => this));
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

        if (IOManager.isConveyor)
            spawnPos = transform.position + Vector3.up * 0.25f;
        else
            spawnPos = itemIO.transform.position;

        ObjectPoolManager.instance.ReuseObject(itemToSpawn.obj.gameObject, spawnPos, Quaternion.identity);

        //Instantiate(itemToSpawn.obj, spawnPos, Quaternion.identity);
    }

    #endregion

    #region Indicator Visualization
    /// <summary>
    /// Attempts to visualize an arrow with a specific material
    /// 
    /// - If the arrow is already visible, only the material will be updated 
    /// - If the arrow is <b>not</b> visible, an arrow will be instantiated using the <see cref="ObjectPoolManager"/>. 
    /// </summary>
    /// <param name="material">The material for the <see cref="arrow"/> <see cref="GameObject"/>.</param>
    public void VisualizeArrow(Material material)
    {
        if (arrow != null)
        {
            arrow.GetComponent<MeshRenderer>().material = material;
        }
        else
        {
            arrow = ObjectPoolManager.instance.ReuseObject(BuildingManager.instance.ArrowIndicator.gameObject, gameObject.transform.position, gameObject.transform.rotation).transform;
            arrow.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            arrow.transform.position += new Vector3(0, 1, 0);
            arrow.GetComponent<MeshRenderer>().material = material;
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

    }

    #endregion

    #region IO Update

    /// <summary>
    /// Attempts to make a link if the <see cref="onPort"/> variable isn't null
    /// 
    /// - If the IO was attached successfully, the IO will call <see cref="Devisualize()"/> and set the attachedIO for the other IO
    /// </summary>
    public void MakeLink()
    {
        attachedIO = onPort;

        if (attachedIO)
        {
            Devisualize();
            attachedIO.attachedIO = this;
        }

    }

    /// <summary>
    /// Returns whether an input should be attached or not
    ///
    /// - If two inputs are trying to connect it should return false, as they shouldn't be linked or else no items would flow between the two
    /// - Likewise, if two outputs are trying to connect it should also return false.
    /// - If there are two opposite types of BuildingIOs (e.g. male to female connection or female to male connection) are checked the method will return true, as they will connect properly.
    /// </summary>
    /// <param name="other">The other BuildingIO being checked</param>
    /// <returns>Whether the two BuildingIOs are a suitable input</returns>
    private bool IsInputSupported(BuildingIO other)
    {
        bool toReturn = true;

        if (!IOManager.isConveyor && !other.IOManager.isConveyor) //Both buildings aren't conveyors (incorrect)
            toReturn = false;
        if (isInput && other.isInput) //Female to female connection (incorrect)
            toReturn = false;
        if (isOutput && other.isOutput) //Male to male connection (incorrect)
            toReturn = false;

        if (other.attachedIO) return false; //Building already has an attached IO there, return red

        Debug.Log($"Can place: {GridManager.instance.canPlace}");
        if (!GridManager.instance.canPlace) return false; //Building is red, arrows shouldn't be anything other than red

        //Needs to be replaced with something that can correctly identify if the buildings are on top of each other
        //if (other.IOManager.mc.Building.renderer.bounds.Intersects(this.IOManager.mc.Building.renderer.bounds)) return false;

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