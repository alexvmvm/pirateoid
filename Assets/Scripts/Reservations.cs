using System;
using System.Collections.Generic;
using UnityEngine;

public interface IReservable
{
    Vector2 ReservablePosition { get; }
    void OnReserve(Thing reserver);
    void OnUnreserve(Thing reserver);
}

public readonly struct CellReservation : IReservable, IEquatable<CellReservation>
{
    private readonly Vector2Int cell;

    public Vector2 ReservablePosition => cell.Center();

    public CellReservation(Vector2Int cell)
    {
        this.cell = cell;
    }

    public void OnReserve(Thing reserver)
    {
        
    }

    public void OnUnreserve(Thing reserver)
    {
        
    }

    public override bool Equals(object obj)
    {
        return obj is CellReservation other && Equals(other);
    }

    public bool Equals(CellReservation other)
    {
        return cell.Equals(other.cell);
    }

    public override int GetHashCode()
    {
        return cell.GetHashCode();
    }

    public static bool operator ==(CellReservation left, CellReservation right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CellReservation left, CellReservation right)
    {
        return !(left == right);
    }

    public static implicit operator CellReservation(Vector2Int cell) => new CellReservation(cell);
    public static implicit operator Vector2Int(CellReservation reservation) => reservation.cell;
}

public class ReservationManager : MonoBehaviour
{
    // Dictionary to hold reservations
    private readonly Dictionary<IReservable, Thing> reservations = new();

    // Method to reserve an object
    public bool Reserve(IReservable reservable, Thing reserver)
    {
        if( reservable == null || reserver == null ) 
            return false;

        // Check if the object is already reserved
        if( reservations.ContainsKey(reservable) )
            return true; // Reservation failed, already reserved

        // Reserve the object
        reservations[reservable] = reserver;
        reservable.OnReserve(reserver);

        return true;
    }

    // Method to unreserve an object
    public void Unreserve(IReservable reservable, Thing reserver)
    {
        if( reservable == null || reserver == null ) 
            return;

        // Check if the object is reserved by this reserver
        if( reservations.ContainsKey(reservable) && reservations[reservable] == reserver )
        {
            reservations.Remove(reservable);
            reservable.OnUnreserve(reserver);
        }
    }

    // Check if an object is reserved
    public bool IsReserved(IReservable reservable)
    {
        return reservations.ContainsKey(reservable);
    }

    public bool IsReserved(Vector2Int reservable)
    {
        return reservations.ContainsKey(new CellReservation(reservable));
    }

    // Get the current reserver
    public Thing GetReserver(IReservable reservable)
    {
        if( reservations.ContainsKey(reservable) )
        {
            return reservations[reservable];
        }

        return null;
    }

    void OnDrawGizmos()
    {
        foreach(var (k, v) in reservations)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(k.ReservablePosition, v.position);

            #if UNITY_EDITOR
            UnityEditor.Handles.Label(Vector2.Lerp(k.ReservablePosition, v.position, 0.5f), k + " reserved by " + v.def.label);
            #endif
        }
    }

    void Update()
    {
        #if UNITY_EDITOR
        foreach(var (k, v) in reservations)
        {
            if( k == null )
                Debug.LogError("Null reservable in reservation manager");
            
            if( v == null )
                Debug.LogError("Null reserver in reservation manager");
        }
        #endif
    }
}

public static class ReservationManagerUtils
{
    public static bool IsReserved(this IReservable reservable) 
        => Find.Reservations.IsReserved(reservable);
}
