using System;

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
    private Thing equippedWeapon;

    //Props
    public CompProperties_EquipmentTracker Props => (CompProperties_EquipmentTracker)props;
    public Thing EquippedWeapon => equippedWeapon;

    public CompEquipmentTracker(Thing parent) : base(parent)
    {
    }

    public CompEquipmentTracker(Thing parent, CompProperties props) : base(parent, props)
	{
	}

    public bool CanEquip(Thing equipment, bool allowDropExisting = false)
    {
        var eq = equipment.GetComp<CompEquipment>();
        if( eq == null )
            return false;

        if( eq.Props.slot == EquipmentSlot.Weapon 
            && equippedWeapon != null 
            && !allowDropExisting )
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
            {       
                //do register equipped thing     
            }

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

            equippedWeapon = equipment;
        }
        else
            throw new NotImplementedException();
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