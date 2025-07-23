using UnityEngine;

public class Window_MainMenu : Window
{
    //Const
    private const float BtnWidth = 90;
    private const float BtnHeight = 20;
    private const int BtnCount = 2;

    //Props
    public override float Width => BtnWidth + (2 * UI.GapLarge);
    public override float Height => (BtnCount * BtnHeight) + HeaderHeight + ((BtnCount - 1) * UI.Gap) + (2 * UI.GapLarge);

    public Window_MainMenu()
    {
        this.pausesGame = true;
    }

    public override void DoWindowContents(Rect rect)
    {
        var x = rect.x + (rect.width/2f) - (BtnWidth/2);
        var y = rect.y;

        Text.Size = FontSize.Medium;

        // Resume
        {
            if( UI.Button(new Rect(x, y, BtnWidth, BtnHeight), "Resume") )
            {
                Close();
                Event.current.Use();
            }
            
            y += BtnHeight + UI.Gap;     
        }

        // // Save
        // {
        //     if( UI.Button(new Rect(x, y, BtnWidth, BtnHeight), "Save") )
        //     {
        //         // TODO
        //         Event.current.Use();
        //     }
            
        //     y += BtnHeight + UI.Gap;     
        // }

        // // Load
        // {
        //     if( UI.Button(new Rect(x, y, BtnWidth, BtnHeight), "Load") )
        //     {
        //         // TODO
        //         Event.current.Use();
        //     }
            
        //     y += BtnHeight + UI.Gap;     
        // }

        // Exit
        {
            if( UI.Button(new Rect(x, y, BtnWidth, BtnHeight), "Exit") )
            {
                Application.Quit();
                Event.current.Use();
            }
               
            y += BtnHeight;   
        }

        Text.Size = FontSize.Small;
    }   
}