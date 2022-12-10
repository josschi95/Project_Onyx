using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CraftingManager : MonoBehaviour
{
    PlayerInventory playerInventory;
    [SerializeField] Transform recipePanelParent;

    private void Awake()
    {
        playerInventory = PlayerInventory.instance;
    }
    private void Start()
    {
        //playerInventory = PlayerInventory.instance;
    }

    void ClearPanelItems()
    {
        foreach (Transform child in recipePanelParent.transform.Cast<Transform>().ToList())
        {
            ObjectPooler.ReturnToPool_Static("infoPanelElement", child.gameObject);
        }
    }

    public void DisplayRecipes(CraftingType type)
    {
        ClearPanelItems();

        if (playerInventory == null)
        {
            Debug.LogWarning("Player inventory null");
            playerInventory = PlayerInventory.instance;
        }

        foreach (Recipe recipe in playerInventory.cookBook)
        {
            if (recipe.craftingType == type)
            {
                GameObject newItem = ObjectPooler.SpawnFromPool_Static("infoPanelElement", transform.position, Quaternion.identity);
                InfoPanelChild txtItem = newItem.GetComponent<InfoPanelChild>();
                txtItem.button.onClick.RemoveAllListeners();

                ButtonSettings(txtItem, recipe);
                txtItem.activeQuestIcon.enabled = false;
                txtItem.infoPanelTitle.text = recipe.craftedItem.name;

                newItem.transform.SetParent(recipePanelParent);
            }
        }
    }

    private void ButtonSettings(InfoPanelChild txtItem, Recipe recipe)
    {

    }
}
