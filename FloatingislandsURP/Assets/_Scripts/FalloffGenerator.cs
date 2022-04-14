using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator
{
    public static float[,] GenerateFalloffMap(int size) 
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size *2 -1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }

        return map;
    }

    public static float[,] GenerateRoundFalloffMap(int size,int r = 0)
    {
        float[,] map = new float[size, size];

        if (r > size / 2 || r <= 0)
            r = (size-1) / 2;


        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = j - size / 2;
                float y = i - size / 2;

                var pos = new Vector2(x,y);

                if (x * x + y * y < r * r)
                    map[i, j] = Evaluate(Vector2.Distance(pos,Vector2.zero)/r);
                else
                    map[i, j] = 1;

                //float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                //map[i, j] = Evaluate(value);
            }
        }

        return map;
    }

    static float Evaluate(float value) 
    {
        float a = 3;
        float b = 2.2f;

        return Mathf.Pow(value,a)/(Mathf.Pow(value,a)+ Mathf.Pow(b-b*value,a));
    }
}
