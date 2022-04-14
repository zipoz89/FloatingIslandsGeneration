using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/IslandPreset")]
public class IslandPreset : ScriptableObject
{
    public Material terrainMaterial;

    [Header("Heightmap Settings")]
    public int seed;
    public int mapWidth;
    public int mapHeight;

    [Header("Upper half Settings")]
    public float upperNoiseScale;
    public float upperMeshHeightMultiplier;
    public AnimationCurve upperMeshHeightCurve;
    public int upperOctaves;
    [Range(0, 1)]
    public float upperPersistance;
    public float upperLacunarity;
    public Vector2 upperOffset;

    [Header("Lower half Settings")]
    public float lowerNoiseScale;
    public float lowerMeshHeightMultiplier;
    public AnimationCurve lowerMeshHeightCurve;
    public int lowerOctaves;
    [Range(0, 1)]
    public float lowerPersistance;
    public float lowerLacunarity;
    public Vector2 lowerOffset;

    public bool useDifferentMapForIsland;

    public FoliageSet foliage;


    private void OnValidate()
    {

        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (mapHeight < 1)
        {
            mapHeight = 1;
        }
        if (upperLacunarity < 1)
        {
            upperLacunarity = 1;
        }
        if (upperOctaves < 0)
        {
            upperOctaves = 0;
        }
        if (lowerLacunarity < 1)
        {
            lowerLacunarity = 1;
        }
        if (lowerOctaves < 0)
        {
            lowerOctaves = 0;
        }
    }
}
