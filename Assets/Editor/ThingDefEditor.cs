using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ThingDef))]
public class ThingDefEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var def = (ThingDef)target;

        // All
        def.label = EditorGUILayout.TextField("Label", def.label);
        def.size = EditorGUILayout.Vector2Field("Size", def.size);
        DoGraphicDataFields("Graphic", def.graphicData);
        def.thingType = (ThingType)EditorGUILayout.EnumPopup("Thing type", def.thingType);

        // Conditional fields
        switch( def.thingType )
        {
            case ThingType.Pawn:
                DoPawnFields(def);
            break;
            case ThingType.Building:
                DoBuildingFields(def);
            break;
            case ThingType.Projectile:
                DoProjectileFields(def);
            break;
        }

        DoCompsField(def);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(def);
        }
    }
    
    private static void DoPawnFields(ThingDef def)
    {
        DoGraphicDataFields("Graphics back", def.graphicBack);
        DoGraphicDataFields("Graphics side", def.graphicSide);

        def.moveSpeed = EditorGUILayout.FloatField("Move speed", def.moveSpeed);
    }

    private static void DoBuildingFields(ThingDef def)
    {
        def.traversability = (Traversability)EditorGUILayout.EnumPopup("Traversability", def.traversability);
    }

    private static void DoProjectileFields(ThingDef def)
    {
        def.moveSpeed = EditorGUILayout.FloatField("Move speed", def.moveSpeed);
    }

    public static void DoGraphicDataFields(string label, GraphicData graphicData)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical("box");
        graphicData.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", graphicData.sprite, typeof(Sprite), false);
        graphicData.scale = EditorGUILayout.FloatField("Scale", graphicData.scale);
        graphicData.brightness = EditorGUILayout.FloatField("Brightness", graphicData.brightness);
        EditorGUILayout.EndVertical();
    }

    private static void DoCompsField(ThingDef def)
    {
        EditorGUILayout.LabelField("Comps", EditorStyles.boldLabel);

        for (int i = 0; i < def.comps.Count; i++)
        {
            var comp = def.comps[i];
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(comp?.GetType()?.Name.Replace("CompProperties_", ""), EditorStyles.boldLabel);

            comp.DrawEditorFields();

            if (GUILayout.Button("Remove"))
                def.comps.RemoveAt(i);

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Component"))
        {
            GenericMenu menu = new GenericMenu();
            
            var compTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(CompProperties).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                .OrderBy(t => t.Name);

            foreach (var type in compTypes)
            {
                menu.AddItem(new GUIContent(type.Name), false, () =>
                {
                    var instance = (CompProperties)Activator.CreateInstance(type);
                    def.comps.Add(instance);
                });
            }
            menu.ShowAsContext();
        }
    }
}