public interface INPCInteractNode 
{
    bool CanBeAccessed(float distance);

    void Interact(CharacterController controller);
}

//Combine this with some sort of decision tree for the NPC to decide what to do
public enum NPCNodeType { Food, Sleep, Leisure, OtherNPC, Work }
