using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageChest : StorageContainer, IInteractable, IEffectReceiver
{
    [Header("Chest Properties")]
    [SerializeField] private Animator anim;
    [SerializeField] Lock chestLock;
    private bool isOpen = false;
    [SerializeField] private bool isOwned;

    protected override void OpenContainer()
    {
        base.OpenContainer();
        interactionMethod = "Close ";
        anim.SetTrigger("rotate_x");
        isOpen = true;
        //Play chest open AFX
    }

    public override void CloseContainer()
    {
        anim.SetTrigger("rotate_x");
        isOpen = false;
        interactionMethod = "Search ";
        UI.interactionInventoryDisplay.onInteractionEnd -= CloseContainer;
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
            chestLock.isLocked = false;
            if (isOpen == false)
            {
                anim.SetTrigger("rotate_x");
                //anim.Play("open_x");
                isOpen = true;
            }
            //Trigger any traps
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
        }
    }
    #endregion
}
