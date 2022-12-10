using UnityEngine;

public class Fountain_Restoration : MonoBehaviour, IInteractable, IDungeonElement
{
    public bool testing = false;
    [Tooltip("If true, the values of this fountain will not change")]
    [SerializeField] private bool isStatic = false;
    [SerializeField] private bool fullRestore = false;

    private enum FountainType { Health, Mana, Stamina }
    [SerializeField] private FountainType fountainType;
    [SerializeField] private int value;
    new private string name;

    private bool hasBeenUsed;
    private float interactionDistance = 2;
    [Space]
    [SerializeField] private MeshRenderer topPool;
    [SerializeField] private MeshRenderer bottomPool;
    [Space]
    [SerializeField] private Material matHealth;
    [SerializeField] private Material matMana;
    [SerializeField] private Material matStamina;

    private void Start()
    {
        if (isStatic) SetFountainType();
    }

    public void OnActivation(int type, int value)
    {
        if (isStatic) return;

        //Set type of fountain
        if (type > 2) type = 2;
        else if (type < 0) type = 0;
        fountainType = (FountainType)type;

        //Set restore value
        if (value < 10) value = 10;
        this.value = value;

        SetFountainType();
    }

    //Changes the name and appearance of the fountain based on its type HP/MP/SP
    private void SetFountainType()
    {
        switch (fountainType)
        {
            case FountainType.Health:
                {
                    name = "Fountain of Health";
                    topPool.material = matHealth;
                    bottomPool.material = matHealth;
                    break;
                }
            case FountainType.Mana:
                {
                    name = "Fountain of Mana";
                    topPool.material = matMana;
                    bottomPool.material = matMana;
                    break;
                }
            case FountainType.Stamina:
                {
                    name = "Fountain of Energy";
                    topPool.material = matStamina;
                    bottomPool.material = matStamina;
                    break;
                }
        }
    }

    #region - IInteractable -
    public bool CanBeAccessed(float distance)
    {
        if (distance <= interactionDistance && hasBeenUsed == false) return true;
        return false;
    }

    public bool DisplayPopup(float distance)
    {
        if (distance <= interactionDistance && hasBeenUsed == false) return true;
        return false;
    }

    public DoubleString GetInteractionDisplay()
    {
        return new DoubleString("Use", name);
    }

    public void Interact(CharacterController controller)
    {
        if (hasBeenUsed == true) return;

        switch (fountainType)
        {
            case FountainType.Health:
                {
                    if (fullRestore) controller.characterStats.FullHeal();
                    else controller.characterStats.RestoreHealth(value);
                    break;
                }
            case FountainType.Mana:
                {
                    if (fullRestore) controller.characterStats.FullMana();
                    else controller.characterStats.RestoreMana(value);
                    break;
                }
            case FountainType.Stamina:
                {
                    if (fullRestore) controller.characterStats.FullStamina();
                    else controller.characterStats.RestoreStamina(value);
                    break;
                }
        }
        AudioManager.instance.Play("blessing");
        if (testing) return;

        hasBeenUsed = true;
        topPool.gameObject.SetActive(false);
        bottomPool.gameObject.SetActive(false);
    }
    #endregion
}