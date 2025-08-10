using System;
using System.Collections.Generic;
using UnityEngine;

public interface IFrameUpdate
{
    void FrameUpdate();
}

public class FrameUpdate : MonoBehaviour
{
    private readonly List<IFrameUpdate> update = new();


    private void Update()
    {
        for(var i = 0; i < update.Count; i++)
        {
            update[i].FrameUpdate();
        }
    }

    public void Register(IFrameUpdate tickable)
    {
        if( !update.Contains(tickable) )
            update.Add(tickable);
    }

    public void DeRegister(IFrameUpdate tickable)
    {
        if( update.Contains(tickable) )
            update.Remove(tickable);
    }
}