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
using UnityEngine;




/// <summary>
/// Allows for fast generation of valid (inside the room, outside furniture bounds) random positions for content spawning.
/// Optional method to pin directly to surfaces.
/// </summary>
public class SpawnMapSockets : Spawn
{
    [Tooltip("When the scene data is loaded, this controls what room(s) the prefabs will spawn in.")]
    public MRUK.RoomFilter SpawnOnStart = MRUK.RoomFilter.CurrentRoomOnly;

    [SerializeField, Tooltip("Prefab to be placed into the scene, or object in the scene to be moved around.")]
    public GameObject SpawnObject;


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
        GeneratePrefabsOnSurfaces();
    }

    /// <summary>
    /// Generate prefabs in the center of the room .
    /// </summary>
    private void GeneratePrefabsOnSurfaces()
    {
        Vector3 spawnNormal = Vector3.zero;
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.up, spawnNormal);
        Vector3 spawnPosition = new Vector3(0, 1.5f, 0.5f);
        GameObject mapHolder = Instantiate(SpawnObject, spawnPosition, spawnRotation, transform);
        /* mapHolder.transform.LookAt(new Vector3(0, 20, 0));*/
    }

}

