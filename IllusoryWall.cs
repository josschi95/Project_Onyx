using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusoryWall : Destructable, IDamageable, IEffectReceiver
{
    [SerializeField] private Outline outline;
    [SerializeField] private Material transparentMat;
    [SerializeField] new private Collider collider;
    [SerializeField] new private MeshRenderer renderer;
    private bool activated = false;

    private void Start()
    {
        outline.enabled = false;
    }

    public override void ApplyDamage(CharacterStats attacker, float amount, DamageType type, bool isLethal)
    {
        if (activated == false) TurnTransparent();
    }

    private void TurnTransparent()
    {
        collider.enabled = false;
        renderer.material = transparentMat;
        activated = true;
    }

    #region - Spell Reception -
    public void TransferEffects(EffectHolder[] effects, string sourceName)
    {
        foreach (EffectHolder effect in effects)
        {
            if (effect.spellEffect is Telekinesis) OnTelekinesisSpell();
        }
    }

    private void OnTelekinesisSpell()
    {
        if (activated == false) TurnTransparent();
    }

    public void OnDivinationEnter(DivinedObjects targetType)
    {
        if (targetType == DivinedObjects.Hidden)
        {
            if (activated == false)
            {
                outline.enabled = true;
                outline.OutlineColor = Color.blue;
            }
        }
    }

    public void OnDivinationExit(DivinedObjects targetType)
    {
        if (targetType == DivinedObjects.Hidden)
        {
            outline.enabled = false;
        }
    }
    #endregion
}
