using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryUIUtils
{
    public static void DrawInventory(Rect rect, Thing thing)
    {
        var container = thing.CompContainer;
        var equipment = thing.CompEquipmentTracker;

        if( container == null || equipment == null )
            return;
        
        var height = rect.height;

        var things = container.Contents;
        var x = rect.x;

        var count = container.Props.capacity;

        for(var i = 0; i < count; i++)
        {
            var r = new Rect(x, rect.y, height, height);
            GUI.color = new Color32(138, 84, 47, 255);
            UI.Box(r);
            GUI.color = new Color32(77, 47, 26, 255);
            UI.Border(r, thickness: 2);
            GUI.color = Color.white;
            x += r.width + UI.Gap;

            int key = i + 1;

            Text.Size = FontSize.Small;
            Text.Anchor = TextAnchor.LowerRight;
            UI.Label(r.ContractBy(4, 1), key.ToString());
            Text.Anchor = TextAnchor.UpperLeft;

            if( i >= things.Count )
                continue;
            
            var t = things[i];

            GUI.color = Color.white * t.def.graphicData.brightness;
            UI.DrawTexture(r.ContractBy(UI.Gap), t.def.graphicData.sprite.texture, ScaleMode.ScaleToFit);
            GUI.color = Color.white;
        }
    }
}