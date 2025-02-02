
using LearnXR.Core.Utilities;
using Meta.XR.MRUtilityKit;
using UnityEngine;
using static EnumsMRUK;

public class Spawn : MonoBehaviour
{
    /// <summary>
    /// Get the adjusted bounds if necessary.
    /// </summary>
    /// <param name="overrideBounds">To handle bounds manually.</param>
    /// <param name="prefabBounds">Bounds of the mesh renderer of the prefab.</param>
    /// <param name="clearanceDistance">Cleareance to adjust bounds.</param>
    protected Bounds GetAdjustedPrefabBounds(Bounds? prefabBounds, float overrideBounds, float clearanceDistance)
    {
        Bounds adjustedBounds = new();

        var min = prefabBounds.Value.min;
        var max = prefabBounds.Value.max;
        min.y += clearanceDistance;
        if (max.y < min.y)
        {
            max.y = min.y;
        }

        adjustedBounds.SetMinMax(min, max);
        /* PrintToLogger("overrbounds " + overrideBounds);*/
        if (overrideBounds > 0)
        {
            Vector3 center = new Vector3(0f, clearanceDistance, 0f);
            Vector3 size = new Vector3(overrideBounds * 2f, clearanceDistance * 2f, overrideBounds * 2f); // OverrideBounds represents the extents, not the size
            adjustedBounds = new Bounds(center, size);
        }
        return adjustedBounds;
    }

    /// <summary>
    /// Select and retrieve the surfaceType given the spawnLocations.
    /// </summary>
    /// <param name="spawnLocations">Retrieve the surfaceType.</param>
    protected MRUK.SurfaceType GetSurfaceType(SpawnLocation spawnLocations)
    {
        MRUK.SurfaceType surfaceType = 0;
        switch (spawnLocations)
        {
            case SpawnLocation.AnySurface:
                surfaceType |= MRUK.SurfaceType.FACING_UP;
                surfaceType |= MRUK.SurfaceType.VERTICAL;
                surfaceType |= MRUK.SurfaceType.FACING_DOWN;
                break;
            case SpawnLocation.VerticalSurfaces:
                surfaceType |= MRUK.SurfaceType.VERTICAL;
                break;
            case SpawnLocation.OnTopOfSurfaces:
                surfaceType |= MRUK.SurfaceType.FACING_UP;
                break;
            case SpawnLocation.HangingDown:
                surfaceType |= MRUK.SurfaceType.FACING_DOWN;
                break;
        }
        return surfaceType;
    }
    /// <summary>
    /// Print a message on the spatial logger
    /// </summary>
    protected void PrintToLogger(string s)
    {
        SpatialLogger.Instance.LogInfo($"{GetType().Name} > " + s);
    }
}
