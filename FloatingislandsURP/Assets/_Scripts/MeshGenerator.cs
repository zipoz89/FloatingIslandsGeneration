using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateMesh(float[,] heightMap, float heightMultipier, AnimationCurve meshHeightCurve) 
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);

        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(x + topLeftX, meshHeightCurve.Evaluate(heightMap[x, y]) * heightMultipier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x/(float)width,y/(float)height);

                if (x < width - 1 && y < height - 1) 
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }

    public static MeshData GenerateUpperIslandMesh(float[,] heightMap, float heightMultipier, AnimationCurve meshHeightCurve, int[,] islandOutline,bool edgeSmoothing)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);

        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(x + topLeftX, meshHeightCurve.Evaluate(heightMap[x, y]) * heightMultipier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (islandOutline[x, y] == 1)
                {
                    if (x < width - 1 && y < height - 1)
                    {

                        if (x >= 1 && y >= 1)
                        {
                            if (!(islandOutline[x - 1, y] == 0 && islandOutline[x, y + 1] == 0))
                            {
                                meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                            }
                            if (!(islandOutline[x, y - 1] == 0 && islandOutline[x + 1, y] == 0))
                            {
                                meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                            }
                        }
                        else
                        {
                            meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                            meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                        }

                        //meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                        //meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                    }
                }
                vertexIndex++;
            }
        }
        vertexIndex = 0;

        //krawędzie do y = 0
        if (edgeSmoothing)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (islandOutline[x, y] != 1)
                    {
                        if (x > 1 && y > 1 && x < width - 1 && y < height - 1)
                        {
                            if (islandOutline[x + 1, y] == 1) 
                            {
                                meshData.vertices[vertexIndex + 1].y = 0;
                            }
                            if (islandOutline[x, y + 1] == 1)
                            { 
                                meshData.vertices[vertexIndex + width].y = 0;
                            }
                            if (islandOutline[x + 1, y + 1] == 1)
                            { 
                                meshData.vertices[vertexIndex + width + 1].y = 0;
                            }

                            if (islandOutline[x - 1, y - 1] == 1 || islandOutline[x - 1, y] == 1 || islandOutline[x, y - 1] == 1) 
                            {
                                meshData.vertices[vertexIndex].y = 0;
                            }
                        }
                    }
                    vertexIndex++;
                }
            }


        }

        return meshData;
    }

    public static MeshData GenerateLowerIslandMesh(float[,] heightMap, float heightMultipier, AnimationCurve meshHeightCurve, int[,] islandOutline, bool edgeSmoothing) 
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);

        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(x + topLeftX, -meshHeightCurve.Evaluate(heightMap[x, y]) * heightMultipier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (islandOutline[x, y] == 1)
                {
                    if (x < width - 1 && y < height - 1)
                    {

                        if (x >= 1 && y >= 1)
                        {
                            if (!(islandOutline[x - 1, y] == 0 && islandOutline[x, y + 1] == 0))
                            {
                                meshData.AddTriangle(vertexIndex, vertexIndex + width, vertexIndex + width + 1);
                            }
                            if (!(islandOutline[x, y - 1] == 0 && islandOutline[x + 1, y] == 0))
                            {
                                meshData.AddTriangle(vertexIndex + width + 1, vertexIndex + 1, vertexIndex);
                            }
                        }
                        else
                        {
                            meshData.AddTriangle(vertexIndex, vertexIndex + width, vertexIndex + width + 1);
                            meshData.AddTriangle(vertexIndex + width + 1, vertexIndex + 1, vertexIndex);
                        }
                    }
                }
                vertexIndex++;
            }
        }
        vertexIndex = 0;

        //krawędzie do y = 0
        if (edgeSmoothing)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (islandOutline[x, y] != 1)
                    {
                        if (x > 1 && y > 1 && x < width - 1 && y < height - 1)
                        {
                            if (islandOutline[x + 1, y] == 1)
                            {
                                meshData.vertices[vertexIndex + 1].y = 0;
                            }
                            if (islandOutline[x, y + 1] == 1)
                            {
                                meshData.vertices[vertexIndex + width].y = 0;
                            }
                            if (islandOutline[x + 1, y + 1] == 1)
                            {
                                meshData.vertices[vertexIndex + width + 1].y = 0;
                            }

                            if (islandOutline[x - 1, y - 1] == 1 || islandOutline[x - 1, y] == 1 || islandOutline[x, y - 1] == 1)
                            {
                                meshData.vertices[vertexIndex].y = 0;
                            }
                        }
                    }
                    vertexIndex++;
                }
            }
        }

        return meshData;
    }

    public static IslandMeshData GenerateIslandMesh(float[,] upperHeightMap, float upperHeightMultipier, AnimationCurve upperMeshHeightCurve,
                                              float[,] lowerHeightMap, float lowerHeightMultipier, AnimationCurve lowerMeshHeightCurve, 
                                              int[,] islandOutline, bool edgeSmoothing)
    {
        int width = upperHeightMap.GetLength(0);
        int height = upperHeightMap.GetLength(1);

        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        IslandMeshData meshData = new IslandMeshData(width, height);

        int vertexIndex = 0;

        int lowerArrOffset = width * height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float upperMeshHeight, lowerMeshHeight;

                if (islandOutline[x, y] == 1)
                {
                    upperMeshHeight = upperMeshHeightCurve.Evaluate(upperHeightMap[x, y]) * upperHeightMultipier;
                    lowerMeshHeight = -upperMeshHeightCurve.Evaluate(lowerHeightMap[x, y]) * lowerHeightMultipier;
                }
                else 
                {
                    upperMeshHeight = 0;
                    lowerMeshHeight = 0;
                }

                meshData.vertices[vertexIndex] = new Vector3(x + topLeftX, upperMeshHeight, topLeftZ - y);
                meshData.vertices[vertexIndex + lowerArrOffset] = new Vector3(x + topLeftX, lowerMeshHeight, topLeftZ - y);

                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                meshData.uvs[vertexIndex + lowerArrOffset] = new Vector2(x / (float)width, y / (float)height);

                if (islandOutline[x, y] == 1)
                {
                    if (x < width - 1 && y < height - 1)
                    {

                        if (x >= 1 && y >= 1)
                        {
                            int V1 = vertexIndex;
                            int V2 = vertexIndex + 1;
                            int V3 = vertexIndex + width + 1;
                            int V4 = vertexIndex + width;

                            //upper
                            if (!(islandOutline[x - 1, y] == 0 && islandOutline[x, y + 1] == 0))
                            {
                                meshData.AddTriangle(V1, V3, V4);
                            }
                            if (!(islandOutline[x, y - 1] == 0 && islandOutline[x + 1, y] == 0))
                            {
                                meshData.AddTriangle(V3, V1, V2);
                            }

                            //lower
                            bool isV1Edge = true;
                            bool isV2Edge = true;
                            bool isV3Edge = true;
                            bool isV4Edge = true;

                            if (islandOutline[x - 1, y] == 1 && islandOutline[x - 1, y - 1] == 1 && islandOutline[x, y - 1] == 1)
                                isV1Edge = false;
                            if (islandOutline[x, y - 1] == 1 && islandOutline[x + 1, y - 1] == 1 && islandOutline[x + 1, y ] == 1)
                                isV2Edge = false;
                            if (islandOutline[x + 1, y] == 1 && islandOutline[x + 1, y + 1] == 1 && islandOutline[x, y + 1] == 1)
                                isV3Edge = false;
                            if (islandOutline[x - 1, y] == 1 && islandOutline[x - 1, y + 1] == 1 && islandOutline[x, y + 1] == 1)
                                isV4Edge = false;

                            if (!isV1Edge)
                                V1 += lowerArrOffset;
                            if (!isV2Edge)
                                V2 += lowerArrOffset;
                            if (!isV3Edge)
                                V3 += lowerArrOffset;
                            if (!isV4Edge)
                                V4 += lowerArrOffset;

                            if (!(islandOutline[x - 1, y] == 0 && islandOutline[x, y + 1] == 0))
                            {
                                meshData.AddTriangle(V1, V4, V3);
                            }
                            if (!(islandOutline[x, y - 1] == 0 && islandOutline[x + 1, y] == 0))
                            {
                                meshData.AddTriangle(V3, V2, V1);
                            }

                        }
                        else
                        {
                            meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                            meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);

                            meshData.AddTriangle(vertexIndex + lowerArrOffset, vertexIndex + lowerArrOffset + width, vertexIndex + lowerArrOffset + width + 1);
                            meshData.AddTriangle(vertexIndex + lowerArrOffset + width + 1, vertexIndex + lowerArrOffset + 1, vertexIndex + lowerArrOffset);
                        }

                    }
                }
                vertexIndex++;
            }
        }
        vertexIndex = 0;

        //krawędzie do y = 0
        if (edgeSmoothing)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (islandOutline[x, y] != 1)
                    {
                        if (x > 1 && y > 1 && x < width - 1 && y < height - 1)
                        {
                            if (islandOutline[x + 1, y] == 1)
                            {
                                meshData.vertices[vertexIndex + 1].y = 0;
                                meshData.vertices[vertexIndex + lowerArrOffset + 1].y = 0;
                            }
                            if (islandOutline[x, y + 1] == 1)
                            {
                                meshData.vertices[vertexIndex + width].y = 0;
                                meshData.vertices[vertexIndex + lowerArrOffset + width].y = 0;
                            }
                            if (islandOutline[x + 1, y + 1] == 1)
                            {
                                meshData.vertices[vertexIndex + width + 1].y = 0;
                                meshData.vertices[vertexIndex + lowerArrOffset + width + 1].y = 0;
                            }

                            if (islandOutline[x - 1, y - 1] == 1 || islandOutline[x - 1, y] == 1 || islandOutline[x, y - 1] == 1)
                            {
                                meshData.vertices[vertexIndex].y = 0;
                                meshData.vertices[vertexIndex + lowerArrOffset].y = 0;
                            }

                        }
                    }
                    vertexIndex++;
                }
            }


        }

        return meshData;
    }


}

    

public class MeshData 
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;


    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight) 
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c) 
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex+1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh() 
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}

public class IslandMeshData 
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;


    int triangleIndex;

    public IslandMeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight * 2];
        uvs = new Vector2[meshWidth * meshHeight * 2];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6 * 2];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}
