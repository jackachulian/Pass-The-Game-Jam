using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteAlways]
public class TerrainManager : MonoBehaviour
{
    [SerializeField] private Terrain terrain;

    [SerializeField] private float[] _landTypeHeights;

    public enum LandType
    {
        Empty,
        Grass,
        Water,
        Mountain // gray
    }

    private int resolution;

    // since this is an ExecuteAlways script this will reset the map both when starting play mode and leaving play mode/starting edit mode
    private void Awake()
    {
        resolution = terrain.terrainData.heightmapResolution;

        // Set all heights to height of empty
        float[,] initialHeights = new float[resolution, resolution];
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                initialHeights[x, y] = _landTypeHeights[0];
            }
        }
        terrain.terrainData.SetHeights(0, 0, initialHeights);
    }

    /// <summary>
    /// Sets the height of a ragion on the terrain, cinvering the given texture-space coordinated into terrain space coordinates, 
    /// and setting height based on the land type provided.
    /// </summary>
    /// <param name="xCoord">X texture coordinate on canvas, from 0 to 1</param>
    /// <param name="yCoord">Y texture coordinate on canvas, from 0 to 1</param>
    /// <param name="paintSize">brush size relative to canvas - Size of the brush divided by the total width of the canvas</param>
    /// <param name="landType"></param>
    public void SetLandTypeRegion(float xCoord, float yCoord, float paintSize, LandType landType)
    {
        float targetHeight = _landTypeHeights[(int)landType];

        // keep above 0, center heightmap change area on coordinate
        int x = Mathf.Max(0, (int)((xCoord - paintSize/2f) * resolution)); 
        int y = Mathf.Max(0, (int)((yCoord - paintSize/2f) * resolution));
        int size = (int)(paintSize * resolution);

        // Keep upper bound coordinates below size of heightmap
        int xMax = Mathf.Min(x + size, resolution);
        int yMax = Mathf.Min(y + size, resolution);

        Debug.Log(xCoord+", "+yCoord+", "+x + ", " + y + ", " + xMax + ", " + yMax);

        float[,] heights = new float[xMax-x, yMax-y];
        for (int hx = 0; hx < xMax - x; hx++) 
        {
            for (int hy = 0; hy < yMax - y; hy++)
            {
                heights[hx, hy] = targetHeight;
            }
        }

        terrain.terrainData.SetHeights(x, y, heights);
    }
}
