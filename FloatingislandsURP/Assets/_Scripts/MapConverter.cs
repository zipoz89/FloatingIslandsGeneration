using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapConverter 
{

    public static float[] IslandHeightMapsToVoxels(IslandMeshData meshData, int meshWidth, int meshHeight,out int meshLength) 
    {
        int mapSize = meshWidth * meshHeight;

        float[,] upperMap = new float[meshWidth, meshHeight];
        float[,] lowerMap = new float[meshWidth, meshHeight];

        int vertexIndex = 0;

        float highest = float.MinValue;
        float lowest= float.MaxValue;

        for (int x = 0; x < meshWidth; x++)
        {
            for (int y = 0; y < meshHeight; y++)
            {
                upperMap[x, y] = meshData.vertices[vertexIndex].y;
                lowerMap[x, y] = meshData.vertices[vertexIndex + mapSize].y;

                if (upperMap[x, y] > highest)
                    highest = upperMap[x, y];
                if (lowerMap[x, y] < lowest)
                    lowest = lowerMap[x, y];

                vertexIndex++;
            }
        }

        int zUpper = (int)highest + 1;
        int zLower = (int)Mathf.Abs(lowest) + 1;

        int zSize = zUpper + zLower;

        meshLength = zSize;

        float[,,] voxel3D = new float[meshWidth,  zSize, meshHeight];

        float[] voxels = new float[meshWidth * meshHeight * zSize];

        for (int x = 0; x < meshWidth; x++)
        {
            for (int y = 0; y < meshHeight; y++)
            {

                int z1 = (int)upperMap[x, y];
                int z2 = (int)lowerMap[x, y];
                if (z1 != 0)
                    voxel3D[x, z1 + zLower, y] = 1;
                if (z2 != 0)
                    voxel3D[x, z2 + zLower, y] = 1;

                if (z1 != 0)
                    for (int i = z2 + zLower + 1; i <= z1 + zLower - 1; i++)
                    {
                        voxel3D[x, i, y] = 1;
                    }

                for (int z = 0; z < zSize; z++)
                {

                    int idx = x + y * meshWidth + z * meshWidth * meshHeight;
                    voxels[idx] = voxel3D[x, z, y];
                }
            }
        }



        return voxels;
    }
}
