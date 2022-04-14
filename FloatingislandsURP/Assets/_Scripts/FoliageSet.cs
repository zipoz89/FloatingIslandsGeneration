using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/FoliageSet")]
public class FoliageSet : ScriptableObject
{
    public FoliageObject[] objects;

    public FoliageSet(FoliageObject[] objects)
    {
        this.objects = objects;
    }
}
