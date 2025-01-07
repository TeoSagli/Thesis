using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumsMRUK : MonoBehaviour
{
    /// <summary>
    /// Defines possible locations where objects can be spawned.
    /// </summary>
    public enum SpawnLocation
    {
        Floating, // Spawn somewhere floating in the free space within the room
        AnySurface, // Spawn on any surface (i.e. a combination of all 3 options below)
        VerticalSurfaces, // Spawn only on vertical surfaces such as walls, windows, wall art, doors, etc...
        OnTopOfSurfaces, // Spawn on surfaces facing upwards such as ground, top of tables, beds, couches, etc...
        HangingDown // Spawn on surfaces facing downwards such as the ceiling
    }
}
