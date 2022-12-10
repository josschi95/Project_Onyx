using System.Collections;
using UnityEngine;

public class DeathState : _State
{
    public override _State OnStateEnter(NPCController controller)
    {
        controller.StopAgent(true);
        controller.agent.enabled = false;
        controller.charDetect.detectionArea.enabled = false;
        controller.charDetect.enabled = false;
        controller.characterStats.enabled = false;
        controller.characterCombat.enabled = false;
        StartCoroutine(DisableDelay(controller));
        return this;
    }

    public override _State Tick(NPCController controller, NPCStats stats, NPCCombat combat)
    {
        controller.agent.enabled = false;
        return this;
    }

    public override void OnStateExit(NPCController controller)
    {

    }

    private IEnumerator DisableDelay(NPCController controller)
    {
        //I don't know why I need this, but I do.
        //Probably some other script enabling the agent 
        yield return new WaitForSeconds(2);
        controller.agent.enabled = false;
        controller.enabled = false;
    }
}
