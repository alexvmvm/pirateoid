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
        def.sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", def.sprite, typeof(Sprite), false);
        def.scale = EditorGUILayout.FloatField("Scale", def.scale);
        def.traversability = (Traversability)EditorGUILayout.EnumPopup("Traversability", def.traversability);
        def.thingType = (ThingType)EditorGUILayout.EnumPopup("Thing type", def.thingType);
        def.moveSpeed = EditorGUILayout.FloatField("Move speed", def.moveSpeed);

        // Conditional fields
        switch( def.thingType )
        {
            case ThingType.Pawn:
                DoPawnFields(def);
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
        def.playerControllable = EditorGUILayout.Toggle("Player controllable", def.playerControllable);
    }

    private static void DoCompsField(ThingDef def)
    {
        EditorGUILayout.LabelField("Comps", EditorStyles.boldLabel);

        for (int i = 0; i < def.comps.Count; i++)
        {
            var comp = def.comps[i];
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(comp?.GetType()?.Name.Replace("CompProperties_", ""), EditorStyles.boldLabel);

            if (comp is CompProperties_Flammable flammable)
            {
                flammable.burnTime = EditorGUILayout.FloatField("Burn Time", flammable.burnTime);
                flammable.burnTime = EditorGUILayout.FloatField("Burn Time", flammable.burnTime);
                flammable.burnTime = EditorGUILayout.FloatField("Burn Time", flammable.burnTime);
            }

            if (GUILayout.Button("Remove"))
                def.comps.RemoveAt(i);

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Component"))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Flammable"), false, () => def.comps.Add(new CompProperties_Flammable()));
            menu.ShowAsContext();
        }
    }
}