using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Terrain/FoliageObject")]
public class FoliageObject : ScriptableObject
{
    [SerializeField] private List<GameObject> prefabs;
    public Vector3 Offset;
    public float Radius = 1;
    public float AcceptableAngle;

    public bool AllowToGenerateOnTop;

    public LayerMask layers;

    public GameObject Prefab 
    {
        get {
            int r = (int)Random.Range(0,prefabs.Count);
            return prefabs[r];
        }
    }
}
