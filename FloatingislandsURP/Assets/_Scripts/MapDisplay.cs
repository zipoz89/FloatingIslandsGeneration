using MarchingCubesProject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] private Renderer textureRenderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

    [SerializeField] private MeshFilter lowerMeshFilter;
    [SerializeField] private MeshRenderer lowerMeshRenderer;

    List<GameObject> meshes = new List<GameObject>();

    [SerializeField] private Transform merrchingParent;

    [SerializeField] private MeshCollider collider;

    public void DrawTexture(Texture2D texture) 
    {
        ClearAll();
        textureRenderer.gameObject.SetActive(true);

        textureRenderer.sharedMaterial.mainTexture = texture;
        //textureRenderer.transform.localScale = new Vector3(texture.width,1,texture.height);

    }

    public void DrawMesh(MeshData meshData,Material material) 
    {
        ClearAll();
        meshRenderer.gameObject.SetActive(true);

        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial = material;
        //meshRenderer.sharedMaterial.mainTexture = texture;

        GenerateCollider();

    }

    public void DrawIsland(MeshData upper, MeshData lower, Material material)
    {
        ClearAll();
        meshRenderer.gameObject.SetActive(true);
        lowerMeshRenderer.gameObject.SetActive(true);

        meshFilter.sharedMesh = upper.CreateMesh();
        meshRenderer.sharedMaterial = material;

        lowerMeshFilter.sharedMesh = lower.CreateMesh();
        lowerMeshRenderer.sharedMaterial = material;

        GenerateCollider();
    }

    public void DrawIsland(IslandMeshData mesh, Material mat) 
    {
        ClearAll();
        meshRenderer.gameObject.SetActive(true);

        meshFilter.sharedMesh = mesh.CreateMesh();
        meshRenderer.sharedMaterial = mat;

        GenerateCollider();
    }

    public void DrawMarching(float[] voxel,int width,int height, int length, Material mat, MARCHING_MODE mode) 
    {
        ClearAll();

        Marching marching = null;
        if (mode == MARCHING_MODE.TETRAHEDRON)
            marching = new MarchingTertrahedron();
        else
            marching = new MarchingCubes();

        marching.Surface = 0.99f;

        List<Vector3> verts = new List<Vector3>();
        List<int> indices = new List<int>();

        marching.Generate(voxel, width, height, length, verts, indices);

        int maxVertsPerMesh = 30000; //must be divisible by 3, ie 3 verts == 1 triangle
        int numMeshes = verts.Count / maxVertsPerMesh + 1;

        for (int i = 0; i < numMeshes; i++)
        {
            meshes = new List<GameObject>();

            List<Vector3> splitVerts = new List<Vector3>();
            List<int> splitIndices = new List<int>();

            for (int j = 0; j < maxVertsPerMesh; j++)
            {
                int idx = i * maxVertsPerMesh + j;

                if (idx < verts.Count)
                {
                    splitVerts.Add(verts[idx]);
                    splitIndices.Add(j);
                }
            }

            if (splitVerts.Count == 0) continue;

            Mesh mesh = new Mesh();
            mesh.SetVertices(splitVerts);
            mesh.SetTriangles(splitIndices, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            GameObject go = new GameObject("Mesh");
            go.transform.parent = merrchingParent;
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.GetComponent<Renderer>().sharedMaterial = mat;
            go.GetComponent<MeshFilter>().sharedMesh = mesh;
            go.transform.localPosition = new Vector3(-width / 2, -length / 2, height / 2);
            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.localRotation = Quaternion.Euler(-90, 0, 0);

            meshes.Add(go);
        }
        merrchingParent.localPosition = new Vector3(0,0, 0);
        merrchingParent.localRotation = Quaternion.Euler(0, 90, 0);

    }

    private void ClearAll()
    {
        meshRenderer.gameObject.SetActive(false);
        lowerMeshRenderer.gameObject.SetActive(false);
        textureRenderer.gameObject.SetActive(false);

        var count = merrchingParent.childCount;

        for (int i =0; i< count; i++)
        {
            DestroyImmediate(merrchingParent.GetChild(0).gameObject);
        }
    }


    private void GenerateCollider() 
    {
        collider.sharedMesh = null;
        collider.sharedMesh = meshFilter.sharedMesh;
    }
}
