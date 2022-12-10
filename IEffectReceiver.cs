using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A base class that will be attached to any component which can receive a spell effect */ 
public interface IEffectReceiver
{
    void TransferEffects(EffectHolder[] effects, string sourceName);

    void OnDivinationEnter(DivinedObjects targetType);

    void OnDivinationExit(DivinedObjects targetType);
}
