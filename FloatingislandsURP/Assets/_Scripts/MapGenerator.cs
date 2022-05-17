using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum MARCHING_MODE { CUBES, TETRAHEDRON };
public class MapGenerator : MonoBehaviour
{
    [Header("Island Setting")]
    [SerializeField] private int seed;
    [SerializeField] private enum DrawMode { NoiseMap,LowerNoiseMap, Mesh, FalloffMap, IslandOutline , UpperIsland ,LowerIsland,FullIsland, FullIslandCombined,test}

    [SerializeField] private DrawMode drawMode;
    [SerializeField] private IslandPreset preset;
    [SerializeField] private bool useDifferentMapForIsland;
    [Range(0.0001f, 1f)]
    [SerializeField] private float islandMinHeightValue = 0.0001f;
    [SerializeField] private bool edgeDump = true;

    [Header("Falloff Settings")]
    [SerializeField] private bool useFalloff;
    [SerializeField] private bool useRoundFalloff;
    [SerializeField] private int falloffRadius;
    
    private MARCHING_MODE mode = MARCHING_MODE.CUBES;



    private float[,] falloffMap;

    //[SerializeField] private Material terrainMaterial;
    public bool AutoUpdate { get => autoUpdate; }


    private const int textureSize = 1024;
    private const TextureFormat textureFormat = TextureFormat.RGB565;

    [Header("References")]
    [SerializeField] private MapDisplay display;
    [SerializeField] private FoliageGenerator fg;
    [Header("Auto Update")]
    [SerializeField] private bool autoGenerateFoliage;
    [SerializeField] private bool autoUpdate = false;

    private void GenerateFalloff() 
    {
        if (!preset)
            return;

        if (useRoundFalloff)
            falloffMap = FalloffGenerator.GenerateRoundFalloffMap(preset.mapWidth, falloffRadius);
        else
            falloffMap = FalloffGenerator.GenerateSquareFalloffMap(preset.mapWidth);
    }


    private void Awake()
    {
        GenerateFalloff();
    }

    private void Start()
    {
        GenerateMap();
    }

    public float minHeight {
        get {
            return preset.upperNoiseScale * preset.upperMeshHeightMultiplier * preset.upperMeshHeightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return preset.upperNoiseScale * preset.upperMeshHeightMultiplier * preset.upperMeshHeightCurve.Evaluate(1);
        }
    }

    public void GenerateMap() 
    {
        

        //Generuje mape szumów typu "teren"
        float[,] upperHeightMap = Noise.GenerateTerrainNoiseMap(preset.mapWidth, preset.mapHeight, seed, preset.upperNoiseScale, preset.upperOctaves, preset.upperPersistance, preset.upperLacunarity, preset.upperOffset);

        Color[] colorMap = new Color[preset.mapWidth * preset.mapHeight];

        GenerateFalloff();
        for (int y = 0; y < preset.mapHeight; y++)
        {
            for (int x = 0; x < preset.mapWidth; x++)
            {
                if (useFalloff) 
                {
                    upperHeightMap[x, y] = Mathf.Clamp01(upperHeightMap[x, y] - falloffMap[x, y]);
                }
                //ustawia wysokość pod x,y
                float currentHeight = upperHeightMap[x, y];
            }
        }

        int[,] islandOutline = OutlineGenerator.generateIslandOutline(upperHeightMap, islandMinHeightValue);

        if (useDifferentMapForIsland) 
        {
            float[,] noiseMap2 = Noise.GenerateTerrainNoiseMap(preset.mapWidth, preset.mapHeight, seed +1, preset.upperNoiseScale, preset.upperOctaves, preset.upperPersistance, preset.upperLacunarity, preset.upperOffset);
            for (int y = 0; y < preset.mapHeight; y++)
            {
                for (int x = 0; x < preset.mapWidth; x++)
                {
                    noiseMap2[x, y] = Mathf.Clamp01(noiseMap2[x, y] - falloffMap[x, y]);
                }
            }
            islandOutline = OutlineGenerator.generateIslandOutline(noiseMap2, islandMinHeightValue);
        }

        //GenerateMaterial();

        float[,] lowerHeightMap = Noise.GenerateTerrainNoiseMap(preset.mapWidth, preset.mapHeight, -seed, preset.lowerNoiseScale, preset.lowerOctaves, preset.lowerPersistance, preset.lowerLacunarity, preset.lowerOffset);




        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(upperHeightMap));
        }
        else if (drawMode == DrawMode.LowerNoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(lowerHeightMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateMesh(upperHeightMap, preset.upperMeshHeightMultiplier, preset.upperMeshHeightCurve), preset.terrainMaterial);
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(falloffMap));
        }
        else if (drawMode == DrawMode.IslandOutline)
        {
            display.DrawTexture(TextureGenerator.TextureFromOutlineMap(islandOutline));
        }
        else if (drawMode == DrawMode.UpperIsland)
        {
            display.DrawMesh(MeshGenerator.GenerateUpperIslandMesh(upperHeightMap, preset.upperMeshHeightMultiplier, preset.upperMeshHeightCurve, islandOutline, edgeDump), preset.terrainMaterial);
        }
        else if (drawMode == DrawMode.LowerIsland)
        {
            display.DrawMesh(MeshGenerator.GenerateLowerIslandMesh(lowerHeightMap, preset.lowerMeshHeightMultiplier, preset.lowerMeshHeightCurve, islandOutline, edgeDump), preset.terrainMaterial);
        }
        else if (drawMode == DrawMode.FullIsland)
        {
            var upper = MeshGenerator.GenerateUpperIslandMesh(upperHeightMap, preset.upperMeshHeightMultiplier, preset.upperMeshHeightCurve, islandOutline, edgeDump);
            var lower = MeshGenerator.GenerateLowerIslandMesh(lowerHeightMap, preset.lowerMeshHeightMultiplier, preset.lowerMeshHeightCurve, islandOutline, edgeDump);
            display.DrawIsland(upper, lower, preset.terrainMaterial);
        }
        else if (drawMode == DrawMode.FullIslandCombined)
        {
            var mesh = MeshGenerator.GenerateIslandMesh(upperHeightMap, preset.upperMeshHeightMultiplier, preset.upperMeshHeightCurve, lowerHeightMap, preset.lowerMeshHeightMultiplier, preset.lowerMeshHeightCurve, islandOutline);
            display.DrawIsland(mesh, preset.terrainMaterial);
        }
        else if (drawMode == DrawMode.test)
        {
            var mesh = MeshGenerator.GenerateIslandMesh(upperHeightMap, preset.upperMeshHeightMultiplier, preset.upperMeshHeightCurve, lowerHeightMap, preset.lowerMeshHeightMultiplier, preset.lowerMeshHeightCurve, islandOutline);
            int meshLength;
            var voxel = MapConverter.IslandHeightMapsToVoxels(mesh, preset.mapWidth, preset.mapHeight,out meshLength);
            display.DrawMarching(voxel, preset.mapWidth, preset.mapHeight,meshLength, preset.terrainMaterial, mode);
        }

        if (fg && autoGenerateFoliage && preset.foliage)
        {
            fg.GenerateFoliage(preset.foliage,new Vector2(preset.mapWidth,preset.mapHeight), seed);
        }
    }

    private Texture2DArray GenerateTextureArray(Texture2D[] textures) 
    {
        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
        for (int i = 0; i < textures.Length; i++)
        {
            textureArray.SetPixels(textures[i].GetPixels(),i);
        }
        textureArray.Apply();
        return textureArray;
    }

    private void OnValidate()
    {
        
    }

}

[System.Serializable]
public class Layer 
{
    public Texture2D texture;
    public Color tint;
    [Range(0, 1)]
    public float tintStrength;
    [Range(0, 1)]
    public float StartHeight;
    [Range(0, 1)]
    public float blendStrength;
    public float textureScale;
}