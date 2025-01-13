using System.Collections;
using System.Collections.Generic;
using LearnXR.Core.Utilities;
using Meta.XR.MRUtilityKit;
using UnityEngine;
using UnityEngine.Serialization;
using static EnumsMRUK;

public class ChestPlacement : MonoBehaviour
{
    [Tooltip("When the scene data is loaded, this controls what room(s) the prefabs will spawn in.")]
    public MRUK.RoomFilter SpawnOnStart = MRUK.RoomFilter.CurrentRoomOnly;

    [SerializeField, Tooltip("Prefab to be placed into the scene, or object in the scene to be moved around.")]
    public GameObject SpawnObject;

    [SerializeField, Tooltip("Number of SpawnObject(s) to place into the scene per room, only applies to Prefabs.")]
    public int SpawnAmount = 8;

    [SerializeField, Tooltip("Maximum number of times to attempt spawning/moving an object before giving up.")]
    public int MaxIterations = 1000;

    [FormerlySerializedAs("selectedSnapOption")]
    [SerializeField, Tooltip("Attach content to scene surfaces.")]
    public SpawnLocation SpawnLocations = SpawnLocation.Floating;

    [SerializeField, Tooltip("When using surface spawning, use this to filter which anchor labels should be included. Eg, spawn only on TABLE or OTHER.")]
    public MRUKAnchor.SceneLabels Labels = ~(MRUKAnchor.SceneLabels)0;

    [SerializeField, Tooltip("If enabled then the spawn position will be checked to make sure there is no overlap with physics colliders including themselves.")]
    public bool CheckOverlaps = true;

    [SerializeField, Tooltip("Required free space for the object (Set negative to auto-detect using GetPrefabBounds)")]
    public float OverrideBounds = -1; // default to auto-detect. This value represents the extents of the bounding box

    [FormerlySerializedAs("layerMask")]
    [SerializeField, Tooltip("Set the layer(s) for the physics bounding box checks, collisions will be avoided with these layers.")]
    public LayerMask LayerMask = -1;

    [SerializeField, Tooltip("The clearance distance required in front of the surface in order for it to be considered a valid spawn position")]
    public float SurfaceClearanceDistance = 0.1f;

    /// <summary>
    /// Starts the spawning process for all rooms.
    /// </summary>
    public void StartSpawn()
    {
        foreach (var room in MRUK.Instance.Rooms)
        {
            StartSpawn(room);
        }
    }
    /// <summary>
    /// Starts the spawning process for a specific room.
    /// </summary>
    /// <param name="room">The room to spawn objects in.</param>
    public void StartSpawn(MRUKRoom room)
    {
        Vector3 spawnPosition = Vector3.zero;
        Vector3 spawnNormal = new Vector3(1, 1, 1);
        /*Quaternion spawnRotation = Quaternion.identity;*/
        /*    spawnRotation = Quaternion.FromToRotation(Vector3.up, spawnNormal);*/
        var surfaceType = MRUKAnchor.SceneLabels.TABLE;
        var largestSurface = MRUK.Instance?.GetCurrentRoom()?.FindLargestSurface(surfaceType);
        spawnPosition = largestSurface.GetAnchorCenter();
        SpatialLogger.Instance.LogInfo($"{nameof(MRUKDemo)} wall center anchor: {spawnPosition}");
        /*  var prefabBounds = Utilities.GetPrefabBounds(largestSurface.gameObject);
          spawnPosition.x = prefabBounds.Value.max.x;*/




        var prefabBounds = Utilities.GetPrefabBounds(SpawnObject);
        float minRadius = 0.0f;
        float baseOffset = -prefabBounds?.min.y ?? 0.0f;
        float centerOffset = prefabBounds?.center.y ?? 0.0f;
        const float clearanceDistance = 0.01f;
        minRadius = Mathf.Min(-prefabBounds.Value.min.x, -prefabBounds.Value.min.z, prefabBounds.Value.max.x, prefabBounds.Value.max.z);
        if (minRadius < 0f)
        {
            minRadius = 0f;
        }
        if (room.GenerateRandomPositionOnSurface(MRUK.SurfaceType.FACING_UP, minRadius, new LabelFilter(Labels), out var pos, out var normal))
        {
            spawnPosition = pos + normal * baseOffset;
            spawnNormal = normal;
        }
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, spawnNormal);
        Bounds adjustedBounds = new();
        if (prefabBounds.HasValue)
        {
            minRadius = Mathf.Min(-prefabBounds.Value.min.x, -prefabBounds.Value.min.z, prefabBounds.Value.max.x, prefabBounds.Value.max.z);
            if (minRadius < 0f)
            {
                minRadius = 0f;
            }

            var min = prefabBounds.Value.min;
            var max = prefabBounds.Value.max;
            min.y += clearanceDistance;
            if (max.y < min.y)
            {
                max.y = min.y;
            }

            adjustedBounds.SetMinMax(min, max);
            if (OverrideBounds > 0)
            {
                Vector3 center = new Vector3(0f, clearanceDistance, 0f);
                Vector3 size = new Vector3(OverrideBounds * 2f, clearanceDistance * 2f, OverrideBounds * 2f); // OverrideBounds represents the extents, not the size
                adjustedBounds = new Bounds(center, size);
            }
        }
        if (CheckOverlaps && prefabBounds.HasValue)
        {

            Instantiate(SpawnObject, spawnPosition, spawnRotation, transform);

        }
    }

}
