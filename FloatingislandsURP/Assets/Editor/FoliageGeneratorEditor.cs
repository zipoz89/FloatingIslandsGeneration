using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FoliageGenerator))]
public class FoliageGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FoliageGenerator fg = (FoliageGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Clear"))
        {
            fg.ClearFoliage();
        }
    }
}
