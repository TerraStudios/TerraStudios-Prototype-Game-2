using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingIO : MonoBehaviour
{
    public bool isInput;
    public bool isOutput;
    public MeshRenderer MeshRenderer;
    public Collider coll;
    public BoxCollider itemIO;
    public BuildingIOManager myManager;

    [Header("Input Configuration")]
    public bool enableDebug;
    public bool isTrashcanOutput;
    public ItemData[] itemsAllowedToEnter;
    public ItemCategory[] itemCategoriesAllowedToEnter;

    [Header("Dynamic variables")]
    public BuildingIO attachedIO;
    [HideInInspector] public bool visualizeIO = true;
    public Transform arrow;

    private void OnTriggerEnter(Collider other)
    {
        OnUpdateIO(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnUpdateIO(other, true);
    }

    public void Init()
    {
        visualizeIO = false;
    }

    public void SpawnItemObj(ItemData itemToSpawn)
    {
        Vector3 spawnPos = itemIO.transform.position;
        Instantiate(itemToSpawn.obj, spawnPos, Quaternion.identity);
    }

    public void Visualize()
    {
        if (!visualizeIO)
        {
            Visualize(Color.blue);
            visualizeIO = true;
        }
    }

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

            if (visualizeIO)
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

            else if (!hit.visualizeIO && !isInputUnsupported)
            {
                attachedIO = hit;
                if (arrow && arrow.gameObject)
                Destroy(arrow.gameObject);
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

        myManager.ProceedItemEnter(item.gameObject, item.data);
    }

    public void OnItemExit(ItemBehaviour item)
    {
        Debug.Log("Item exit " + item.data.name);
        //! probably not needed
    }


    private void Visualize(Color color)
    {

        if (attachedIO) return;
;        //Old cube render
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
            arrow = Instantiate(BuildingManager.instance.ArrowPrefab, gameObject.transform.position, gameObject.transform.rotation);
            arrow.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            arrow.transform.position += new Vector3(0, 1, 0);
            arrow.GetComponent<MeshRenderer>().material.color = color;
        }
    }


    public void Devisualize()
    {
        if (arrow != null)
        {
            Destroy(arrow.gameObject);
            visualizeIO = false;
            arrow = null;
        }
        MeshRenderer.enabled = false;
    }

    /// <summary>
    /// Draws an arrow where the building is faced in the editor. Used for setting the correct rotation (IO)
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
}
