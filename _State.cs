using UnityEngine;

public abstract class _State : MonoBehaviour
{
    public _StateHandler handler;

    //This will run once when transitioning to this state
    public abstract _State OnStateEnter(NPCController controller);

    //This will run every frame while the NPC is in this state
    public abstract _State Tick(NPCController controller, NPCStats stats, NPCCombat combat);

    public abstract void OnStateExit(NPCController controller);
}
