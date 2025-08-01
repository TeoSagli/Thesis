/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Meta.XR.MRUtilityKit;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using static EnumsMRUK;


/// <summary>
/// Allows for fast generation of valid (inside the room, outside furniture bounds) random positions for content spawning.
/// Optional method to pin directly to surfaces.
/// </summary>
public class SpawnPuzzlePieces : Spawn
{
    [Tooltip("When the scene data is loaded, this controls what room(s) the prefabs will spawn in.")]
    public MRUK.RoomFilter SpawnOnStart = MRUK.RoomFilter.CurrentRoomOnly;

    [SerializeField, Tooltip("Prefab to be placed into the scene, or object in the scene to be moved around.")]
    private GameObject[] objectArr;

    [SerializeField, Tooltip("Maximum number of times to attempt spawning/moving an object before giving up.")]
    public int MaxIterations = 1000;

    /// <summary>
    /// Defines possible locations where objects can be spawned.
    /// </summary>

    // Attach content to scene surfaces.
    private SpawnLocation SpawnLocations = SpawnLocation.OnTopOfSurfaces;

    // When using surface spawning, use this to filter which anchor labels should be included. Eg, spawn only on TABLE or OTHER.
   // private MRUKAnchor.SceneLabels Labels = ~(MRUKAnchor.SceneLabels)0;
    private MRUKAnchor.SceneLabels Labels = MRUKAnchor.SceneLabels.COUCH | MRUKAnchor.SceneLabels.TABLE | MRUKAnchor.SceneLabels.FLOOR | MRUKAnchor.SceneLabels.OTHER | MRUKAnchor.SceneLabels.STORAGE;

    // If enabled then the spawn position will be checked to make sure there is no overlap with physics colliders including themselves.
    private bool CheckOverlaps = true;

    // Required free space for the object (Set negative to auto-detect using GetPrefabBounds)
    private float OverrideBounds = -1; // default to auto-detect. This value represents the extents of the bounding box

    [FormerlySerializedAs("layerMask")]
    // Set the layer(s) for the physics bounding box checks, collisions will be avoided with these layers.
    private LayerMask LayerMask = -1;

    // The clearance distance required in front of the surface in order for it to be considered a valid spawn position"
    private float SurfaceClearanceDistance = 0.1f;
    //const
    const string PATH_TO_MESH_RENDERER = "Flashlight_LOD0";

    public GameObject[] ObjectArr { get => objectArr; set => objectArr = value; }

    public SpawnPuzzlePieces(MRUK.RoomFilter spawnOnStart, GameObject[] objectArr, int maxIterations, SpawnLocation spawnLocations, MRUKAnchor.SceneLabels labels, bool checkOverlaps, float overrideBounds, LayerMask layerMask, float surfaceClearanceDistance)
    {
        ObjectArr = objectArr;
        SpawnOnStart = spawnOnStart;
        MaxIterations = maxIterations;
        SpawnLocations = spawnLocations;
        Labels = labels;
        CheckOverlaps = checkOverlaps;
        OverrideBounds = overrideBounds;
        LayerMask = layerMask;
        SurfaceClearanceDistance = surfaceClearanceDistance;
    }

    public SpawnPuzzlePieces()

    {
    }

    private void Start()
    {
        if (MRUK.Instance && SpawnOnStart != MRUK.RoomFilter.None)
        {
            MRUK.Instance.RegisterSceneLoadedCallback(() =>
            {
                switch (SpawnOnStart)
                {
                    case MRUK.RoomFilter.AllRooms:
                        StartSpawn();
                        break;
                    case MRUK.RoomFilter.CurrentRoomOnly:
                        StartSpawn(MRUK.Instance.GetCurrentRoom());
                        break;
                }
            });
        }
    }
    private IEnumerator WaitForMRUK()
    {
        yield return new WaitUntil(() => MRUK.Instance != null);

        MRUK.Instance.RegisterSceneLoadedCallback(() =>
        {
            foreach (var room in MRUK.Instance.Rooms)
            {
                StartSpawn(room);
            }
        });
    }
    /// <summary>
    /// Starts the spawning process for all rooms.
    /// </summary>
    public void StartSpawn()
    {
        StartCoroutine(WaitForMRUK());
    }

    /// <summary>
    /// Starts the spawning process for a specific room.
    /// </summary>
    /// <param name="room">The room to spawn objects in.</param>
    public void StartSpawn(MRUKRoom room)
    {
        //to retrieve child mesh
        GameObject obj = ObjectArr[0];
        Bounds? prefabBounds = Utilities.GetPrefabBounds(obj);
        float minRadius = 0.0f;
        const float clearanceDistance = 0.01f;
        Bounds adjustedBounds = new();

        if (prefabBounds.HasValue)
        {
            minRadius = Mathf.Min(-prefabBounds.Value.min.x, -prefabBounds.Value.min.z, prefabBounds.Value.max.x, prefabBounds.Value.max.z);
            if (minRadius < 0f)
                minRadius = 0f;
            adjustedBounds = GetAdjustedPrefabBounds(prefabBounds, OverrideBounds, clearanceDistance);
        }
        GeneratePrefabsOnSurfaces(room, prefabBounds, minRadius, adjustedBounds);

    }

    /// <summary>
    /// Generate prefabs randomly in the room by the selected parameters.
    /// </summary>
    /// <param name="room">The room to spawn objects in.</param>
    /// <param name="prefabBounds">Bounds of the mesh renderer of the prefab.</param>
    /// <param name="adjustedBounds">Adjusted bounds of the mesh renderer of the prefab, if needed.</param>
    /// <param name="minRadius">Min distance between objs.</param>
    private void GeneratePrefabsOnSurfaces(MRUKRoom room, Bounds? prefabBounds, float minRadius, Bounds adjustedBounds)
    {
        float baseOffset = -prefabBounds?.min.y ?? 0.0f;
        float centerOffset = prefabBounds?.center.y ?? 0.0f;
        foreach (var spawnObject in ObjectArr)
        {
            bool foundValidSpawnPosition = false;
            for (int j = 0; j < MaxIterations; ++j)
            {
                Vector3 spawnPosition = Vector3.zero;
                Vector3 spawnNormal = Vector3.zero;
                if (SpawnLocations == SpawnLocation.Floating)
                {
                    var randomPos = room.GenerateRandomPositionInRoom(minRadius, true);
                    if (!randomPos.HasValue)
                    {
                        break;
                    }

                    spawnPosition = randomPos.Value;
                }
                else
                {
                    MRUK.SurfaceType surfaceType = GetSurfaceType(SpawnLocations);

                    if (room.GenerateRandomPositionOnSurface(surfaceType, minRadius, new LabelFilter(Labels), out var pos, out var normal))
                    {
                        // pos += new Vector3(0, 0, -1);
                        spawnPosition = pos + normal * baseOffset;
                        spawnNormal = normal;
                        var center = spawnPosition + normal * centerOffset;
                        // In some cases, surfaces may protrude through walls and end up outside the room
                        // check to make sure the center of the prefab will spawn inside the room
                        if (!room.IsPositionInRoom(center))
                        {
                            continue;
                        }

                        // Ensure the center of the prefab will not spawn inside a scene volume
                        if (room.IsPositionInSceneVolume(center))
                        {
                            continue;
                        }

                        // Also make sure there is nothing close to the surface that would obstruct it
                        if (room.Raycast(new Ray(pos, normal), SurfaceClearanceDistance, out _))
                        {
                            continue;
                        }
                    }
                }

                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.back, spawnNormal);
                //Quaternion spawnRotation = Quaternion.identity;

                foundValidSpawnPosition = true; 
                spawnObject.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
                if (!room.GetRoomBounds().Contains(spawnPosition))
                    continue;

                break;
            }

            if (!foundValidSpawnPosition)
            {
                PrintToLogger($"Failed to find valid spawn position after {MaxIterations} iterations. .");
                break;
            }
        }
    }
}

