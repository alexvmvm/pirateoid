using System;
using UnityEngine;

[Serializable]
public partial class CompProperties_EquipmentTracker : CompProperties
{
    public CompProperties_EquipmentTracker()
	{
		compClass = typeof(CompEquipmentTracker);
	}
}

public class CompEquipmentTracker : ThingComp
{
    //Working vars
    private CompEquipment equippedWeapon;

    //Props
    public CompProperties_EquipmentTracker Props => (CompProperties_EquipmentTracker)props;
    public CompEquipment EquippedWeapon => equippedWeapon;

    public CompEquipmentTracker(Thing parent) : base(parent)
    {
    }

    public CompEquipmentTracker(Thing parent, CompProperties props) : base(parent, props)
	{
	}

    public bool IsEquipped(Thing equipment)
    {
        var eq = equipment.GetComp<CompEquipment>();
        if( eq == null )
            return false;

        return equippedWeapon == eq;
    }

    public bool CanEquip(Thing equipment, bool allowUnequip = false)
    {
        var eq = equipment.GetComp<CompEquipment>();
        if( eq == null )
            return false;

        if( eq.Props.slot == EquipmentSlot.Weapon 
            && equippedWeapon != null 
            && !allowUnequip )
        {
            return false;
        }
    
        return true;
    }

    public void Equip(Thing equipment)
    {
        var eq = equipment.GetComp<CompEquipment>();
        if( eq == null )
            return;
        
        if( eq.Props.slot == EquipmentSlot.Weapon )
        {
            if( equippedWeapon != null )
                UnEquip(equippedWeapon.parent);

            var container = parent.GetComp<CompContainer>();
            if( container == null )
            {
                UnityEngine.Debug.LogError($"Tried to equip something to {parent} but this thing has no container.");
                return;    
            }

            if( equipment.Spawned )
                equipment.DeSpawn();

            if( !container.Contains(equipment) )
                container.Add(equipment);

            equippedWeapon = eq;
        }
        else
            throw new NotImplementedException();
    }

    public void UnEquip(Thing thing)
    {        
        if( IsEquipped(thing) )
        {
            equippedWeapon = null;
        }
    }   

    public override void Tick()
    {
        
    }
}

#if UNITY_EDITOR
public partial class CompProperties_EquipmentTracker
{
    public override void DrawEditorFields()
    {
    }
}
#endif