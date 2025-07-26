using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public interface ITickable
{
    void Tick();
}

public class Ticker : MonoBehaviour
{
    //Const
    public const int TicksPerRealSecond = 60; // 60 ticks per second at normal speed
    public readonly float TickInterval = 1f / TicksPerRealSecond; 
    public static readonly Vector2Int ButtonSize = new(25, 16);

    //Const - controls
    private const int PausedSpeed = 0;
    private const int NormalSpeed = 1;
    private const int FastSpeed = 2;
    private const int FasterSpeed = 5;

    public float gameSpeed = 1f; // 1f is normal speed, 0f is paused, >1f is faster
    private float tickTimer = 0f;
    private int ticksGame;

    private readonly List<ITickable> tickables = new();

    //Props
    public int GUIOrder => 1;
    public int TicksGame => ticksGame;
    private float GameSpeedNow
    {
        get
        {
            //Dialog pauses game
            if( Find.WindowManager.PauseGame )
                return 0f;

            if( !Application.isFocused )
                return 0f;

            return gameSpeed;
        }
    }

    private void Update()
    {
        if( GameSpeedNow > 0f )
        {
            tickTimer += Time.unscaledDeltaTime * GameSpeedNow;

            while (tickTimer >= TickInterval)
            {
                tickTimer -= TickInterval;
                Tick();
                ticksGame++;
            }
        }
    }

    public void Register(ITickable tickable)
    {
        if( !tickables.Contains(tickable) )
            tickables.Add(tickable);
    }

    public void DeRegister(ITickable tickable)
    {
        if( tickables.Contains(tickable) )
            tickables.Remove(tickable);
    }

    private void Tick()
    {
        for(var i = tickables.Count - 1; i >= 0; i--)
        {
            var tickable = tickables[i];
            
            Profiler.BeginSample("Tick_" + tickable.ToString());
            tickable.Tick();
            Profiler.EndSample();            

            //It's possible this tickable is removed as part of its 
            //tick so we skip an index here.
            if( !tickables.Contains(tickable) )
                i--;
        }
    }

    // public void DoGUI()
    // {
    //     var x = UIScreen.Width - (ButtonSize.x * 4) - (4 * UI.GapSmall);
    //     var y = UIScreen.Height - ButtonSize.y - UI.Gap;

    //     var rect = new Rect(x, y, ButtonSize.x, ButtonSize.y);

    //     //Pause
    //     DoGameSpeedButton(rect, PausedSpeed, UI.PauseTexture);

    //     //Play
    //     rect.x += ButtonSize.x + UI.GapSmall;
    //     DoGameSpeedButton(rect, NormalSpeed, UI.PlayTexture);

    //     //Fast
    //     rect.x += ButtonSize.x + UI.GapSmall;
    //     DoGameSpeedButton(rect, FastSpeed, UI.PlayFastTexture);

    //     //Faster
    //     rect.x += ButtonSize.x + UI.GapSmall;
    //     DoGameSpeedButton(rect, FasterSpeed, UI.PlayFastestTexture);
    // }

    // void DoGameSpeedButton(Rect rect, float speed, Texture2D texture)
    // {
    //     UI.Box(rect, UI.PanelBackgroundColor);
    //     UI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);

    //     if( UI.ButtonHidden(rect) )
    //         SetGameSpeed(speed);

    //     if( RootUI.IsOver(rect) )
    //         UI.Highlight(rect);

    //     if( GameSpeedNow == speed )
    //     {
    //         GUI.color = UI.BorderColor;
    //         UI.Border(rect);
    //         GUI.color = Color.white;
            
    //         UI.Highlight(rect);
    //     }
            
    // }

    public void SetGameSpeed(float speed)
    {
        gameSpeed = speed;
    }

    public void Pause()
    {
        SetGameSpeed(PausedSpeed);
    }
}