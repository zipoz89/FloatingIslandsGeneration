using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OutlineGenerator 
{
    private static void FloodFillMap(int x,int y,float[,] inputMap, ref int[,] outputMap , float minValue) 
    {
        int[,] wasVisitedFlagMap = new int[inputMap.GetLength(0), inputMap.GetLength(1)];
        Queue<Tuple<int, int>> toCheck = new Queue<Tuple<int, int>>();
        toCheck.Enqueue(new Tuple<int, int>(x, y));

        while (toCheck.Count > 0)
        {
            Tuple<int, int> t = toCheck.Dequeue();
            int xToCheck = t.Item1;
            int yToCheck = t.Item2;

            if (inputMap[xToCheck, yToCheck] >= minValue)
            {
                outputMap[xToCheck, yToCheck] = 1;
                wasVisitedFlagMap[xToCheck, yToCheck] = 1;
            }

            //najpierw trzeba sprawdzić jakie zostały odwiedzone
            if (xToCheck >= 1 && yToCheck >= 1 && xToCheck < inputMap.GetLength(0) - 1 && yToCheck < inputMap.GetLength(1) - 1) 
            {
                if (wasVisitedFlagMap[xToCheck + 1, yToCheck] != 1)
                {
                    wasVisitedFlagMap[xToCheck + 1, yToCheck] = 1;
                    toCheck.Enqueue(new Tuple<int, int>(xToCheck + 1, yToCheck));
                }
                if (wasVisitedFlagMap[xToCheck, yToCheck + 1] != 1)
                {
                    wasVisitedFlagMap[xToCheck, yToCheck + 1] = 1;
                    toCheck.Enqueue(new Tuple<int, int>(xToCheck, yToCheck + 1));
                }
                if (wasVisitedFlagMap[xToCheck - 1, yToCheck] != 1)
                {
                    wasVisitedFlagMap[xToCheck - 1, yToCheck] = 1;
                    toCheck.Enqueue(new Tuple<int, int>(xToCheck - 1, yToCheck));
                }
                if (wasVisitedFlagMap[xToCheck, yToCheck - 1] != 1)
                {
                    wasVisitedFlagMap[xToCheck, yToCheck - 1] = 1;
                    toCheck.Enqueue(new Tuple<int, int>(xToCheck, yToCheck - 1));
                }
            }
        }

        // to był rekurencyjny flood fill ale wolny był
        //if (inputMap[x, y] >= minValue)
        //{
        //    outputMap[x, y] = 1;
        //}
        //else 
        //{
        //    outputMap[x, y] = -1;
        //}

        //if (x >= 1 && y >= 1 && x < inputMap.GetLength(0)-1 && y < inputMap.GetLength(1)-1) 
        //{
        //    if (outputMap[x + 1, y] == 0) 
        //    {
        //        FloodFillMap(x + 1, y, inputMap, ref outputMap, minValue);
        //    }
        //    if (outputMap[x , y + 1] == 0)
        //    {
        //        FloodFillMap(x, y + 1, inputMap, ref outputMap, minValue);
        //    }
        //    if (outputMap[x - 1, y] == 0)
        //    {
        //        FloodFillMap(x - 1, y, inputMap, ref outputMap, minValue);
        //    }
        //    if (outputMap[x, y - 1] == 0)
        //    {
        //        FloodFillMap(x, y - 1, inputMap, ref outputMap, minValue);
        //    }
        //}
    }


    public static int[,] generateIslandOutline(float[,] map,float minValueToInclude) 
    {
        float maxValue = float.MinValue;
        int ix = 0;
        int iy = 0;
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                if (map[x, y] > maxValue) 
                {
                    maxValue = map[x, y];
                    ix = x;
                    iy = y;
                }
            }
        }
        int[,] outlineMap = new int[map.GetLength(0), map.GetLength(1)];

        if (map[ix, iy] >= minValueToInclude) 
        {
            FloodFillMap(ix, iy, map,ref outlineMap, minValueToInclude);
        }

        for (int y = 1; y < map.GetLength(1)-1; y++)
        {
            for (int x = 1; x < map.GetLength(0)-1; x++)
            {
                if (outlineMap[x, y] == 1) 
                {
                    int empty = 0;
                    if (outlineMap[x - 1, y] != 1)
                        empty++;
                    if (outlineMap[x + 1, y] != 1)
                        empty++;
                    if (outlineMap[x , y -1] != 1)
                        empty++;
                    if (outlineMap[x, y + 1] != 1)
                        empty++;

                    if (empty >= 3) 
                    {
                        outlineMap[x, y] = 0;
                    }
                }
            }
        }

        return outlineMap;
    }


}
