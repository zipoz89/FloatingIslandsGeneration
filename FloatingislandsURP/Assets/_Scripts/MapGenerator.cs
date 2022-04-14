using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum MARCHING_MODE { CUBES, TETRAHEDRON };
public class MapGenerator : MonoBehaviour
{
    [SerializeField] private enum DrawMode { NoiseMap, Mesh, FalloffMap, IslandOutline , UpperIsland ,LowerIsland,FullIsland, FullIslandCombined,test}

    [SerializeField] private DrawMode drawMode;

    
    //[Header("Heightmap Settings")]
    //[SerializeField] private int seed;
    //[SerializeField] private int mapWidth;
    //[SerializeField] private int mapHeight;

    //[Header("Upper half Settings")]
    //[SerializeField] private float noiseScale;
    //[SerializeField] private float meshHeightMultiplier;
    //[SerializeField] private AnimationCurve meshHeightCurve;
    //[SerializeField] private int octaves;
    //[Range(0, 1)]
    //[SerializeField] private float persistance;
    //[SerializeField] private float lacunarity;
    //[SerializeField] private Vector2 offset;

    //[Header("Lower half Settings")]
    //[SerializeField] private float lowerNoiseScale;
    //[SerializeField] private float lowerMeshHeightMultiplier;
    //[SerializeField] private AnimationCurve lowerMeshHeightCurve;
    //[SerializeField] private int lowerOctaves;
    //[Range(0, 1)]
    //[SerializeField] private float lowerPersistance;
    //[SerializeField] private float lowerLacunarity;
    //[SerializeField] private Vector2 lowerOffset;

    [SerializeField] private IslandPreset island;
    [SerializeField] private MapDisplay display;
    [Header("Modifiers")]
    [SerializeField] private bool autoUpdate = false;
    [SerializeField] private bool edgeDump;
    [Header("Falloff Settings")]
    [SerializeField] private bool useFalloff;
    [SerializeField] private bool useRoundFalloff;
    [SerializeField] private int falloffRadius;
    
    [SerializeField] private MARCHING_MODE mode = MARCHING_MODE.CUBES;

    [SerializeField] private bool useDifferentMapForIsland;

    [Range(0.0001f, 1f)]
    [SerializeField] private float islandMinHeightValue = 0.0001f;
    private float[,] falloffMap;

    //[SerializeField] private Material terrainMaterial;
    public bool AutoUpdate { get => autoUpdate; }


    [SerializeField] private Layer[] layers;

    private const int textureSize = 1024;
    private const TextureFormat textureFormat = TextureFormat.RGB565;

    [SerializeField] private FoliageGenerator fg;
    [SerializeField] private bool autoGenerateFoliage;

    private void GenerateFalloff() 
    {
        if (!island)
            return;

        if (useRoundFalloff)
            falloffMap = FalloffGenerator.GenerateRoundFalloffMap(island.mapWidth, falloffRadius);
        else
            falloffMap = FalloffGenerator.GenerateFalloffMap(island.mapWidth);
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
            return island.upperNoiseScale * island.upperMeshHeightMultiplier * island.upperMeshHeightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return island.upperNoiseScale * island.upperMeshHeightMultiplier * island.upperMeshHeightCurve.Evaluate(1);
        }
    }

    public void GenerateMap() 
    {
        

        //Generuje mape szumów typu "teren"
        float[,] noiseMap = Noise.GenerateTerrainNoiseMap(island.mapWidth, island.mapHeight, island.seed, island.upperNoiseScale, island.upperOctaves, island.upperPersistance, island.upperLacunarity, island.upperOffset);

        Color[] colorMap = new Color[island.mapWidth * island.mapHeight];

        for (int y = 0; y < island.mapHeight; y++)
        {
            for (int x = 0; x < island.mapWidth; x++)
            {
                if (useFalloff) 
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                }
                //ustawia wysokość pod x,y
                float currentHeight = noiseMap[x, y];
            }
        }

        int[,] islandOutline = OutlineGenerator.generateIslandOutline(noiseMap, islandMinHeightValue);

        if (useDifferentMapForIsland) 
        {
            float[,] noiseMap2 = Noise.GenerateTerrainNoiseMap(island.mapWidth, island.mapHeight, island.seed +1, island.upperNoiseScale, island.upperOctaves, island.upperPersistance, island.upperLacunarity, island.upperOffset);
            for (int y = 0; y < island.mapHeight; y++)
            {
                for (int x = 0; x < island.mapWidth; x++)
                {
                    noiseMap2[x, y] = Mathf.Clamp01(noiseMap2[x, y] - falloffMap[x, y]);
                }
            }
            islandOutline = OutlineGenerator.generateIslandOutline(noiseMap2, islandMinHeightValue);
        }

        //GenerateMaterial();

        float[,] lowerNoiseMap = Noise.GenerateTerrainNoiseMap(island.mapWidth, island.mapHeight, -island.seed, island.lowerNoiseScale, island.lowerOctaves, island.lowerPersistance, island.lowerLacunarity, island.lowerOffset);




        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateMesh(noiseMap, island.upperMeshHeightMultiplier, island.upperMeshHeightCurve), island.terrainMaterial);
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
            display.DrawMesh(MeshGenerator.GenerateUpperIslandMesh(noiseMap, island.upperMeshHeightMultiplier, island.upperMeshHeightCurve, islandOutline, edgeDump), island.terrainMaterial);
        }
        else if (drawMode == DrawMode.LowerIsland)
        {
            display.DrawMesh(MeshGenerator.GenerateLowerIslandMesh(lowerNoiseMap, island.lowerMeshHeightMultiplier, island.lowerMeshHeightCurve, islandOutline, edgeDump), island.terrainMaterial);
        }
        else if (drawMode == DrawMode.FullIsland)
        {
            var upper = MeshGenerator.GenerateUpperIslandMesh(noiseMap, island.upperMeshHeightMultiplier, island.upperMeshHeightCurve, islandOutline, edgeDump);
            var lower = MeshGenerator.GenerateLowerIslandMesh(lowerNoiseMap, island.lowerMeshHeightMultiplier, island.lowerMeshHeightCurve, islandOutline, edgeDump);
            display.DrawIsland(upper, lower, island.terrainMaterial);
        }
        else if (drawMode == DrawMode.FullIslandCombined)
        {
            var mesh = MeshGenerator.GenerateIslandMesh(noiseMap, island.upperMeshHeightMultiplier, island.upperMeshHeightCurve, lowerNoiseMap, island.lowerMeshHeightMultiplier, island.lowerMeshHeightCurve, islandOutline, edgeDump);
            display.DrawIsland(mesh, island.terrainMaterial);
        }
        else if (drawMode == DrawMode.test)
        {
            var mesh = MeshGenerator.GenerateIslandMesh(noiseMap, island.upperMeshHeightMultiplier, island.upperMeshHeightCurve, lowerNoiseMap, island.lowerMeshHeightMultiplier, island.lowerMeshHeightCurve, islandOutline, edgeDump);
            int meshLength;
            var voxel = MapConverter.IslandHeightMapsToVoxels(mesh, island.mapWidth, island.mapHeight,out meshLength);
            display.DrawMarching(voxel, island.mapWidth, island.mapHeight,meshLength, island.terrainMaterial, mode);
        }

        if (fg && autoGenerateFoliage && island.foliage)
        {
            fg.GenerateFoliage(island.foliage, island.seed);
        }
    }

    //private void GenerateMaterial()
    //{
    //    terrainMaterial.SetInt("layerCount", layers.Length);
    //    terrainMaterial.SetColorArray("baseColors", layers.Select(x => x.tint).ToArray());
    //    terrainMaterial.SetFloatArray("baseStartHeights", layers.Select(x => x.StartHeight).ToArray());
    //    terrainMaterial.SetFloatArray("baseBlends", layers.Select(x => x.blendStrength).ToArray());
    //    terrainMaterial.SetFloatArray("baseColorStrength", layers.Select(x => x.tintStrength).ToArray());
    //    terrainMaterial.SetFloatArray("baseTextureScales", layers.Select(x => x.textureScale).ToArray());
    //    Texture2DArray textureArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray());
    //    terrainMaterial.SetTexture("baseTextures", textureArray);

    //    terrainMaterial.SetFloat("minHeight", minHeight);
    //    terrainMaterial.SetFloat("maxHeight", maxHeight);
    //}

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
        GenerateFalloff();
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