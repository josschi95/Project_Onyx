using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNodeState : _State
{
    public float searchRadius = 10f;
    private INPCInteractNode node;
    private Transform nodeTransform;
    private bool occupyingNode = false;
    public float maxDistanceToInteract = 2;
    private float minDist = 0.5f;

    public override _State OnStateEnter(NPCController controller)
    {
        node = null;
        nodeTransform = null;
        occupyingNode = false;
        minDist = 0.5f;
        return this;
    }

    public override void OnStateExit(NPCController controller)
    {
        node = null;
        nodeTransform = null;
        occupyingNode = false;
        minDist = 0.5f;

        controller.StopAgent(false);
        controller.agent.enabled = true;
    }


    //The other way to do this would be to track last position and if it's less than some ammount (e.g. the controller hasn't moved, then interact. Of course this could come into some issues 
    //So also will need to check if it's also less than maxDist
    public override _State Tick(NPCController controller, NPCStats stats, NPCCombat combat)
    {
        //This is just temporary
        if (occupyingNode) return this;

        if (node != null)
        {
            //The node has been occupied by someone else
            if (!node.CanBeAccessed(searchRadius))
            {
                node = null;
                nodeTransform = null;
                return this;
            }

            //Move the character to the node, and interact if close enough
            controller.UpdateAgentDestination(nodeTransform.position);
            float dist = Vector3.Distance(controller.transform.position, nodeTransform.position);
            if (dist <= maxDistanceToInteract)
            {
                if (dist <= minDist) OccupyNode(controller);
                else
                {
                    minDist += 0.1f;
                    return this;
                }
            }
        }
        //Search for a node to interact with
        else
        {
            Collider[] colls = Physics.OverlapSphere(controller.transform.position, searchRadius);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger == false)
                {
                    var node = colls[i].GetComponent<INPCInteractNode>();
                    if (node != null && node.CanBeAccessed(searchRadius))
                    {
                        this.node = node;
                        nodeTransform = colls[i].transform;
                        break;
                    }
                }
            }

        }

        return handler.defaultState;
    }

    private void OccupyNode(NPCController controller)
    {
        controller.StopAgent(true);
        controller.agent.enabled = false;
        node.Interact(controller);
        occupyingNode = true;
    }
}
