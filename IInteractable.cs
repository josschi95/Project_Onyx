public interface IInteractable
{
    bool DisplayPopup(float distance);

    bool CanBeAccessed(float distance);

    void Interact(CharacterController controller);

    DoubleString GetInteractionDisplay();

    //Another way to do this would be to just have some sort of funciton that subscribes to an OnInteractCallback event
    //which is set off isntead of funneling it through TriggerEvents
}

public struct DoubleString
{
    public string interactionMethod;
    public string interactionTarget;
    public bool isCrime;

    public DoubleString(string method, string target, bool isCrime = false)
    {
        this.interactionMethod = method;
        this.interactionTarget = target;
        this.isCrime = isCrime;
    }
}