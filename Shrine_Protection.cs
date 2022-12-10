using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine_Protection : MonoBehaviour, IInteractable, IDungeonElement
{
    [Tooltip("If true, the values of this fountain will not change")]
    [SerializeField] private bool isStatic = false;

    [SerializeField] private SpellEffect[] effects;

    [SerializeField] private TypeResistance resistance;
    [SerializeField] private bool armorShrine = false;
    [SerializeField] private int value;
    [Space]
    [SerializeField] private ParticleSystem particles;

    new private string name = "Shrine of Protection";
    private float interactionDistance = 2;
    private bool hasBeenUsed;

    public void OnActivation(int type, int value)
    {
        if (particles == null) particles = GetComponentInChildren<ParticleSystem>();

        particles.Play();

        if (isStatic) return;

        //Set type of shrine
        //0-11 sets resistance, 12 sets armor
        int i = System.Enum.GetNames(typeof(TypeResistance)).Length + 1;
        if (type > i) type = i;
        else if (type < 0) type = 0;

        if (type == i) armorShrine = true;
        else
        {
            armorShrine = false;
            resistance = (TypeResistance)type;
        }            

        //Set value
        if (value < 10) value = 10;
        else if (value > 100) value = 100;
        this.value = value;
    }

    #region - IInteractable -
    public bool CanBeAccessed(float distance)
    {
        if (distance <= interactionDistance && hasBeenUsed == false) return true;
        return false;
    }

    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance && hasBeenUsed == false) return true;
        return false;
    }

    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString("Use", name);
    }

    public void Interact(CharacterController controller)
    {
        if (hasBeenUsed == true) return;

        if (armorShrine == true)
        {
            controller.characterStats.AddSpellEffect(effects[12],
                controller.characterStats.statSheet.GetMinorAttributeIndex(MinorAttribute.Armor), value, 60, name);
            UIManager.instance.AddNotification("Armor increased by " + value);
        }
        else
        {
            controller.characterStats.AddSpellEffect(effects[(int)resistance],
                controller.characterStats.statSheet.GetResistanceIndex(resistance), value, 60, name);
            UIManager.instance.AddNotification(resistance + " resistance increased by " + value);
        }

        AudioManager.instance.Play("blessing");

        hasBeenUsed = true;
        particles.Stop();
    }
    #endregion
}
