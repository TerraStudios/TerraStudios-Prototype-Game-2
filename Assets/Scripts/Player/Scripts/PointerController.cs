using UnityEngine;

public class PointerController : MonoBehaviour
{
    public bool enableGroundPointer;
    public Transform pointerStatic;
    public Transform pointerGround;
    public Transform pointerLine;
    public LayerMask voxelLayer;

    private void Update()
    {
        if (enableGroundPointer)
        {
            ApplyGroundPointer();
            ApplyPointerLine();
        }
    }

    /// <summary>
    /// Changes Pointer Ground's position depending on the Voxel impact point.
    /// </summary>
    private void ApplyGroundPointer() 
    {
        Vector3 hitPosDown = GetVoxelImpact(Vector3.down);
        pointerGround.transform.position = hitPosDown;
    }

    /// <summary>
    /// Stretches the Pointer Line between Pointer Static and Pointer Ground.
    /// </summary>
    private void ApplyPointerLine()
    {
        pointerLine.localScale = new Vector3(0.5f, 0.5f, Vector3.Distance(pointerStatic.position, pointerGround.position));
        pointerLine.position = new Vector3(pointerGround.position.x - pointerLine.localScale.x / 2, pointerGround.position.y, pointerGround.position.z);

        Vector3 rotationDirection = (pointerGround.position - pointerStatic.position); //Change Rotation
        pointerLine.rotation = Quaternion.LookRotation(-rotationDirection);
    }

    /// <summary>
    /// Returns the impact point on the Voxel Terrain based on the direction provided. 
    /// The direction is oriented based on Pointer Static.
    /// </summary>
    /// <param name="direction">The direction from which the ray should be projected from Pointer Static.</param>
    /// <returns></returns>
    private Vector3 GetVoxelImpact(Vector3 direction)
    {
        if (Physics.Raycast(new Vector3(pointerStatic.position.x, 100000 * -direction.y, pointerStatic.position.z), direction, out RaycastHit hit, Mathf.Infinity, voxelLayer))
            return hit.point;
        else
            return Vector3.zero;
    }
}
