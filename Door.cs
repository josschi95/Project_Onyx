using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable, IEffectReceiver
{
    public string interactionDisplay = "Open ";
    [SerializeField] private Animator anim;
    [SerializeField] Outline outline;
    public Lock doorLock;
    public Door partnerDoor;
    public float interactionDistance = 2;
    public bool isOpen = false;
    [SerializeField] private List<EffectHolder> trapEffects = new List<EffectHolder>();
    private bool acceptTraps = false;
    [SerializeField] private bool canBeLocked = true;
    [SerializeField] private bool isCrimeToUnlock = false;

    private void Start()
    {
        outline.enabled = false;
        if (doorLock == null) canBeLocked = false;
    }

    public void OpenDoor()
    {
        anim.SetTrigger("rotate");
        isOpen = true;
        if (partnerDoor != null && partnerDoor.isOpen == false)
            partnerDoor.OpenDoor();
    }

    public void CloseDoor()
    {
        anim.SetTrigger("rotate");
        isOpen = false;
        if (partnerDoor != null && partnerDoor.isOpen == true)
            partnerDoor.CloseDoor();
    }

    #region - IInteractable -
    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance) return true;
        return false;
    }

    public bool CanBeAccessed(float distance)
    {
        //Probably some other function here for if the door doesn't open from this side
        //Or something is blocking the way
        return true;
    }

    public void Interact(CharacterController controller)
    {
        if (isOpen) CloseDoor();
        else
        {
            if (doorLock != null && doorLock.isLocked)
            {
                if (doorLock.LockCascade()) OpenDoor();
            }
            else OpenDoor();
        }

        if (trapEffects.Count > 0)
        {
            for (int i = 0; i < trapEffects.Count; i++)
            {
                trapEffects[i].ApplyStatEffect(controller.characterStats, transform.name);
            }
            trapEffects.Clear();
        }
    }

    public DoubleString GetInteractionDisplay()
    {
        string method = "Open";
        if (isOpen) method = "Close";
        bool isCrime = false;
        if (doorLock != null && doorLock.isLocked)
        {
            method = "Unlock";
            isCrime = isCrimeToUnlock;
        }
        return new DoubleString(method, "Door", isCrime);
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
        if (doorLock != null && doorLock.isLocked)
        {
            if (magnitude >= doorLock.difficulty)
            {
                doorLock.isLocked = false;
                if (partnerDoor != null)
                    partnerDoor.doorLock.isLocked = false;
                OpenDoor();
            }
            else
            {
                UIManager.instance.DisplayPopup("Open Spell Failed");
            }
        }
        else
        {
            OpenDoor();
        }
    }

    private void OnLockSpell(float magnitude)
    {
        if (canBeLocked && doorLock != null)
        {
            if (doorLock.isLocked == false || doorLock.difficulty < magnitude)
            {
                doorLock.isLocked = true;
                int newMag = Mathf.RoundToInt(magnitude);
                doorLock.difficulty = newMag;
                if (partnerDoor != null)
                {
                    partnerDoor.doorLock.isLocked = true;
                    partnerDoor.doorLock.difficulty = newMag;
                }

                if (isOpen) CloseDoor();
            }
            else
            {
                UIManager.instance.DisplayPopup("Spell Failed");
            }
        }
        else
        {
            UIManager.instance.DisplayPopup("Spell Failed");
        }
    }

    private void OnTelekinesisSpell()
    {
        if (doorLock != null && doorLock.isLocked)
        {
            UIManager.instance.DisplayPopup("Telekinesis Spell Failed");
            return;
        }
        else
        {
            if (isOpen) CloseDoor();
            else OpenDoor();
        }
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
