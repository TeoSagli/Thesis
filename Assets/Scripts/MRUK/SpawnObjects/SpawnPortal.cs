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

using System;
using LearnXR.Core.Utilities;
using Meta.XR.MRUtilityKit;
using Meta.XR.Util;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Serialization;
using static EnumsMRUK;


/// <summary>
/// Allows for fast generation of valid (inside the room, outside furniture bounds) random positions for content spawning.
/// Optional method to pin directly to surfaces.
/// </summary>
public class SpawnPortal : Spawn
{
    [Tooltip("When the scene data is loaded, this controls what room(s) the prefabs will spawn in.")]
    public MRUK.RoomFilter SpawnOnStart = MRUK.RoomFilter.CurrentRoomOnly;

    [SerializeField, Tooltip("Prefab to be placed into the scene, or object in the scene to be moved around.")]
    public GameObject SpawnObject;

    [SerializeField, Tooltip("Number of SpawnObject(s) to place into the scene per room, only applies to Prefabs.")]
    public int SpawnAmount = 1;

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
    //const
    const string PATH_TO_MESH_RENDERER = "Visuals/Mesh";
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
        GameObject toFind = SpawnObject.transform.Find(PATH_TO_MESH_RENDERER).gameObject;
        var prefabBounds = Utilities.GetPrefabBounds(toFind);
        float minRadius = 0.0f;
        const float clearanceDistance = 0.01f;
        float baseOffset = -prefabBounds?.min.y ?? 0.0f;
        float centerOffset = prefabBounds?.center.y ?? 0.0f;
        Bounds adjustedBounds = new();

        if (prefabBounds.HasValue)
        {
            minRadius = Mathf.Min(-prefabBounds.Value.min.x, -prefabBounds.Value.min.z, prefabBounds.Value.max.x, prefabBounds.Value.max.z);
            if (minRadius < 0f)
            {
                minRadius = 0f;
            }

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
        for (int i = 0; i < SpawnAmount; ++i)
        {
            bool foundValidSpawnPosition = false;
            for (int j = 0; j < MaxIterations; ++j)
            {
                Vector3 spawnPosition = Vector3.zero;
                Vector3 spawnNormal = Vector3.zero;
                if (SpawnLocations == SpawnLocation.Floating)
                {
                    //spawn somewhere floating
                    var randomPos = room.GenerateRandomPositionInRoom(minRadius, true);
                    if (!randomPos.HasValue)
                    {
                        break;
                    }

                    spawnPosition = randomPos.Value;
                }
                else
                {
                    //retrieve the selected surface by the user
                    MRUK.SurfaceType surfaceType = GetSurfaceType(SpawnLocations);

                    if (room.GenerateRandomPositionOnSurface(surfaceType, minRadius, new LabelFilter(Labels), out var pos, out var normal))
                    {
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

                Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, spawnNormal);
                if (CheckOverlaps && prefabBounds.HasValue)
                {
                    if (Physics.CheckBox(spawnPosition + spawnRotation * adjustedBounds.center, adjustedBounds.extents, spawnRotation, LayerMask, QueryTriggerInteraction.Ignore))
                    {
                        continue;
                    }
                }

                foundValidSpawnPosition = true;

                if (SpawnObject.gameObject.scene.path == null)
                {
                    Vector2 vec2 = new Vector2(0, 0);
                    spawnPosition = MRUK.Instance.GetCurrentRoom().GetKeyWall(out vec2).GetAnchorCenter();

                    /* PrintToLogger("Portal obj:" + SpawnObject);*/

                    GameObject portal = Instantiate(SpawnObject, spawnPosition, spawnRotation, transform);
                    float px = portal.transform.position.x;
                    float py = portal.transform.position.y;
                    float pz = portal.transform.position.z;
                    float checkBound = 2.5f;
                    int choice = GetPrefabOrientation(px, py, pz, checkBound, room);
                    OrientatePrefabToDirection(portal, choice, px, py, pz, checkBound);
                    PrintToLogger("vec2 key wall:" + vec2);
                    PrintToLogger("Portal coords position:" + spawnPosition);
                    PrintToLogger("Portal coords position:" + spawnRotation);
                    PrintToLogger("choice is:" + choice);
                    PrintToLogger("bounds:" + room.GetRoomBounds().ToString());
                    PrintToLogger("bounds:" + room.GetRoomBounds().ToString());
                }
                else
                {
                    SpawnObject.transform.position = spawnPosition;
                    SpawnObject.transform.rotation = spawnRotation;
                    return; // ignore SpawnAmount once we have a successful move of existing object in the scene
                }

                break;
            }

            if (!foundValidSpawnPosition)
            {
                PrintToLogger($"Failed to find valid spawn position after {MaxIterations} iterations. Only spawned {i} prefabs instead of {SpawnAmount}.");
                break;
            }
        }
    }

    /// <summary>
    /// Get the prefab's orientation.
    /// </summary>
    /// <param name="px">Prefab x coordinate.</param>
    /// <param name="py">Prefab y coordinate.</param>
    /// <param name="pz">Prefab z coordinate.</param>
    /// <param name="offset">Offset used to evaluate which wall it is closer.</param>
    /// <param name="room">The room to spawn objects in.</param>
    private int GetPrefabOrientation(float px, float py, float pz, float offset, MRUKRoom room)
    {
        int choice = -1;
        if (!room.GetRoomBounds().Contains(new Vector3(px + offset, py, pz)))
        {
            //orientated with positive x wall
            choice = 0;
        }
        else
        {
            if (!room.GetRoomBounds().Contains(new Vector3(px - offset, py, pz)))
                //orientated with negative x wall
                choice = 1;
            else
            {
                if (!room.GetRoomBounds().Contains(new Vector3(px, py, pz + offset)))
                    //orientated with positive z wall
                    choice = 2;
                else
                {
                    //orientated with negative z wall
                    if (!room.GetRoomBounds().Contains(new Vector3(px, py, pz - offset)))
                        choice = 3;
                }
            }
        }
        return choice;
    }

    /// <summary>
    /// Orientate the prefab in front of it.
    /// </summary>
    /// <param name="px">Prefab x coordinate.</param>
    /// <param name="py">Prefab y coordinate.</param>
    /// <param name="pz">Prefab z coordinate.</param>
    /// <param name="offset">Offset used to evaluate which wall it is closer.</param>
    /// <param name="choice">Tells in which direction the prefab has to be orientated .</param>
    /// <param name="prefab">Prefab to orientate .</param>
    private void OrientatePrefabToDirection(GameObject prefab, int choice, float px, float py, float pz, float offset)
    {
        switch (choice)
        {
            case 0:
                prefab.transform.LookAt(new Vector3(px - offset, py, pz));
                break;
            case 1:
                prefab.transform.LookAt(new Vector3(px + offset, py, pz));
                break;
            case 2:
                prefab.transform.LookAt(new Vector3(px, py, pz - offset));
                break;
            case 3:
                prefab.transform.LookAt(new Vector3(px, py, pz + offset));
                break;
            default:
                break;
        }
    }

}

