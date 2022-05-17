using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoliageGenerator : MonoBehaviour
{
    [SerializeField] private Vector2 regionSize = Vector2.one * 10;
    [SerializeField] private Vector2 offsetOniterations;

    [SerializeField] private Transform parent;


    [SerializeField] private float yRayStart;

    [SerializeField] private FoliageObject[] objects;


    public void GenerateFoliage(int seed = 1) 
    {
        GenerateFoliage(new FoliageSet(objects),regionSize, seed);
    }

    public void GenerateFoliage(FoliageSet set,Vector2 regionSize, int seed = 1)
    {
        ClearFoliage();

        int it = 0;
        
        foreach (var foliage in set.objects)
        {

            List<Vector2> points = PoissonDiskSampler.GeneratePoints(foliage.Radius, regionSize, seed + it);
            foreach (Vector2 point in points)
            {
                RaycastHit hit;

                Vector3 origint = new Vector3(point.x - regionSize.x / 2, yRayStart, point.y - regionSize.y / 2);

                var succesful = Physics.Raycast(origint, Vector3.down, out hit, 100,foliage.layers);

                if (succesful)
                {
                    bool goodLayer = foliage.layers == (foliage.layers | (1 << hit.transform.transform.gameObject.layer));

                    if (Vector3.Angle(hit.normal, Vector3.down) >= foliage.AcceptableAngle && goodLayer)
                    {
                        GameObject o = Instantiate(foliage.Prefab);
                        o.transform.position = hit.point + foliage.Offset;
                        if (foliage.RandomRotation) 
                        {
                            o.transform.rotation = Quaternion.Euler(0,Random.Range(0,360),0);
                        }
                        if (parent)
                            o.transform.parent = parent;
                    }
                }
            }

            it++;
        }
    }

    [ContextMenu("Clear")]
    public void ClearFoliage() 
    {
        var count = parent.childCount;

        for (int i = 0; i < count; i++)
        {
            DestroyImmediate(parent.GetChild(0).gameObject);
        }
    }
}
