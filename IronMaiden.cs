using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronMaiden : MonoBehaviour, IInteractable, IEffectReceiver
{
    [SerializeField] private Animator anim;
    [SerializeField] private Outline outline;
    [SerializeField] private List<EffectHolder> trapEffects = new List<EffectHolder>();
    private bool isOpen = false;
    public float interactionDistance = 2;
    private bool acceptTraps = false;

    private void Start()
    {
        outline.enabled = false;
    }

    private void ToggleHinges()
    {
        anim.SetTrigger("rotate");
        isOpen = !isOpen;
    }

    #region - IInteractable -
    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance) return true;
        return false;
    }

    public bool CanBeAccessed(float distance)
    {
        return true;
    }

    public void Interact(CharacterController controller)
    {
        ToggleHinges();
    }

    public DoubleString GetInteractionDisplay()
    {
        string method = "Open";
        if (isOpen) method = "Close";
        return new DoubleString(method, transform.name, false);
    }
    #endregion

    #region - Spell Reception -
    public void TransferEffects(EffectHolder[] effects, string sourceName)
    {
        foreach (EffectHolder effect in effects)
        {
            if (effect.spellEffect is TrapObject)
            {
                acceptTraps = true;
            }
            else if (effect.spellEffect is Open)
            {
                OnOpenSpell(effect.magnitude);
            }
            else if (effect.spellEffect is Telekinesis)
            {
                OnTelekinesisSpell();
            }
            else if (acceptTraps == true)
            {
                trapEffects.Add(effect);
            }

        }
        acceptTraps = false;
    }

    private void OnOpenSpell(float magnitude)
    {
        if (isOpen == false)
        {
            ToggleHinges();
        }
        else
        {
            UIManager.instance.DisplayPopup("Open Spell Failed");
        }
    }

    private void OnTelekinesisSpell()
    {
        ToggleHinges();
        Debug.Log("Not fully implemented yet");
        //Spring any traps
    }
    
    public void OnDivinationEnter(DivinedObjects targetType)
    {
        if (targetType == DivinedObjects.Traps && trapEffects.Count > 0)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.red;
        }
    }

    public void OnDivinationExit(DivinedObjects targetType)
    {
        if (targetType == DivinedObjects.Traps)
        {
            outline.enabled = false;
        }
    }
    #endregion
}