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
        def.size = EditorGUILayout.Vector2IntField("Size", def.size);
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

        if (GUI.changed)
        {
            EditorUtility.SetDirty(def);
        }
    }
    
    private static void DoPawnFields(ThingDef def)
    {
        def.playerControllable = EditorGUILayout.Toggle("Player controllable", def.playerControllable);
    }
}