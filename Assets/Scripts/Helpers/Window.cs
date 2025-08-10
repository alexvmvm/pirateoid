using UnityEngine;

public abstract class Window
{
    //Const
    public static readonly Texture2D CloseTexture = Resources.Load<Texture2D>("Textures/UI/delete");
    public const int HeaderHeight = 15;

    //Props
    public virtual float Width { get; }   = 250;
    public virtual float Height { get; }  = 200;

    //Config
    public bool pausesGame = false;
    public bool closeOnEsc = true;
    public bool doHeader = true;
    public Color32? backgroundColor;
    
    public abstract void DoWindowContents(Rect rect);

    public virtual void DoWindow(Rect rect)
    {
        GUI.color = backgroundColor ?? UI.PanelBackgroundColor;
        UI.Box(rect);
        GUI.color = Color.white;

        if( doHeader )
        {
            rect.SplitVerticallyAmount(out Rect top, out Rect bottom, HeaderHeight);

            var closeBtnRect = new Rect(top.xMax - top.height, top.y, top.height, top.height);

            if( RootUI.IsOver(closeBtnRect) )
                UI.Highlight(closeBtnRect);
            
            if( UI.Button(closeBtnRect, CloseTexture) )
            {
                Find.WindowManager.Remove(this);
            }

            DoWindowContents(bottom.ContractBy(UI.GapLarge));
        }
        else
            DoWindowContents(rect.ContractBy(UI.GapLarge));
    }

    public virtual Vector2 GetPosition()
    {
        var h = Height;
        var w = Width;
            
        var x = (UIScreen.Width/2f)  - (w/2f);
        var y = (UIScreen.Height/2f) - (h/2f);

        return new Vector2(x, y);
    }

    public virtual Rect GetDrawRect()
    {
        var h = Height;
        var w = Width;
        
        var p = GetPosition();

        return new Rect(p.x, p.y, w, h);
    }

    public virtual void Close()
    {
        Find.WindowManager.Remove(this);
    }
}