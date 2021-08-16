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

using UnityEngine;

public class PointerController : MonoBehaviour
{
    public bool enableGroundPointer;
    public Transform pointerStatic;
    public Transform pointerStaticFake;
    public Transform pointerGround;
    public Transform pointerLine;
    public LayerMask voxelLayer;

    private void Update()
    {
        if (enableGroundPointer)
        {
            ApplyStaticPointer();
            ApplyGroundPointer();
            ApplyPointerLine();
        }
    }

    private void ApplyStaticPointer()
    {
        pointerStatic.transform.position = pointerStaticFake.transform.position;
        pointerStatic.transform.rotation = pointerStaticFake.transform.rotation;
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
        // Scale is incorrect, it doesn't stretch all the way to both points
        pointerLine.localScale = new Vector3(0.5f, 0.5f, Vector3.Distance(pointerStatic.position, pointerGround.position));
        pointerLine.position = new Vector3(pointerGround.position.x - pointerLine.localScale.x / 2, pointerGround.position.y, pointerGround.position.z);

        Vector3 rotationDirection = (pointerGround.position - pointerStatic.position); //Change Rotation
        pointerLine.rotation = Quaternion.LookRotation(-rotationDirection);

        pointerLine.RotateAround(pointerGround.transform.position, Vector3.up, Camera.main.transform.rotation.eulerAngles.y);
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
