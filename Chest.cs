using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Container, IInteractable, IEffectReceiver
{
    [Header("Chest Properties")]
    [SerializeField] private Animator anim;
    [SerializeField] Lock chestLock;
    private bool isOpen = false;
    [SerializeField] private bool isOwned;

    protected override void OpenContainer()
    {
        anim.SetTrigger("rotate_x");
        isOpen = true;
        interactionMethod = "Close";
        if (isEmpty) return;
        UI.interactionInventoryDisplay.onInteractionEnd += CloseContainer;
        base.OpenContainer();
        //Play chest open AFX
    }

    public override void CloseContainer()
    {
        if (IsEmpty() == false)
        {
            anim.SetTrigger("rotate_x");
            isOpen = false;
            base.interactionMethod = "Search";
        }
        UI.interactionInventoryDisplay.onInteractionEnd -= CloseContainer;
    }

    private void CloseLid()
    {
        anim.SetTrigger("rotate_x");
        isOpen = false;
        base.interactionMethod = "Search ";
    }

    #region - IInteractable -
    public override void Interact(CharacterController controller)
    {
        if (controller is PlayerController)
        {
            if (isOpen == false)
            {
                if (chestLock.isLocked)
                {
                    if (chestLock.LockCascade())
                        OpenContainer();
                }
                else OpenContainer();
            }
            else CloseLid();
        }

        if (trapEffects.Count > 0)
        {
            //I can do this better, a better method would be to have it play as a touch spell, so particleFX are also played, I also need to trigger this at a different time
            for (int i = 0; i < trapEffects.Count; i++)
            {
                trapEffects[i].ApplyStatEffect(controller.characterStats, transform.name);
            }
            trapEffects.Clear();
        }
    }

    public override DoubleString GetInteractionDisplay()
    {
        if (chestLock.isLocked)
        {
            return new DoubleString("Unlock", containerName, isOwned);
        }
        else return base.GetInteractionDisplay();
    }
    #endregion

    #region - Spell Reception -
    public override void TransferEffects(EffectHolder[] effects, string sourceName)
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
            else if (effect.spellEffect is MagicLock)
            {
                OnLockSpell(effect.magnitude);
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
        if (magnitude >= chestLock.difficulty)
        {
            //Trigger any traps
            chestLock.isLocked = false;
            if (isOpen == false)
            {
                anim.SetTrigger("rotate_x");
                isOpen = true;
            }
        }
        else
        {
            UIManager.instance.DisplayPopup("Open Spell Failed");
        }
    }

    private void OnLockSpell(float magnitude)
    {
        if (chestLock.isLocked == false || chestLock.difficulty < magnitude)
        {
            chestLock.isLocked = true;
            int newMag = Mathf.RoundToInt(magnitude);
            chestLock.difficulty = newMag;
            if (isOpen)
            {
                anim.SetTrigger("rotate_x");
                //anim.Play("close_x");
                isOpen = false;
            }
        }
        else
        {
            UIManager.instance.DisplayPopup("Magic Lock Spell Failed");
        }
    }

    private void OnTelekinesisSpell()
    {
        if (chestLock.isLocked)
        {
            UIManager.instance.DisplayPopup("Telekinesis Spell Failed");
            return;
        }
        else
        {
            anim.SetTrigger("rotate_x");
            if (isOpen)
            {
                //anim.Play("close_x");
                isOpen = false;
            }
            else
            {
                //anim.Play("open_x");
                isOpen = true;
            }
            //Trigger any traps
        }
    }
    #endregion
}
