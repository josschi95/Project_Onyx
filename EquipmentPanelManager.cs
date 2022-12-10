using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanelManager : MonoBehaviour
{
    #region - Singleton -

    public static EquipmentPanelManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of EquipmentPanelManager found");
            return;
        }
        instance = this;
    }

    #endregion

    private PlayerEquipmentManager playerEquip;
    public EquipmentPanel[] equipmentPanels;
    [SerializeField] private Button primaryWeaponButton, secondaryWeaponButton;

    private void Start()
    {
        playerEquip = PlayerEquipmentManager.instance;
        playerEquip.onEquipmentChanged += delegate { UpdatePanels(); };

        primaryWeaponButton.onClick.AddListener(delegate
        {
            playerEquip.SwapWeaponSets(false, true);
        });

        secondaryWeaponButton.onClick.AddListener(delegate
        {
            playerEquip.SwapWeaponSets(true, true);
        });

        equipmentPanels[2].gameObject.SetActive(false);
        equipmentPanels[3].gameObject.SetActive(false);

        playerEquip.onWeaponSetChange += OnWeaponSetChange;
    }

    //Toggle the primary and secondary weapon set equipment panels
    private void OnWeaponSetChange(bool toSecondary)
    {
        equipmentPanels[0].gameObject.SetActive(!toSecondary);
        equipmentPanels[1].gameObject.SetActive(!toSecondary);

        equipmentPanels[2].gameObject.SetActive(toSecondary);
        equipmentPanels[3].gameObject.SetActive(toSecondary);
    }

    public void UpdatePanels()
    {
        for (int i = 0; i < equipmentPanels.Length; i++)
        {
            if (playerEquip.currentEquipment[i] == null) equipmentPanels[i].UpdatePanel(null);
            //else equipmentPanels[i].UpdatePanel(playerEquip.currentEquipment[i].item as Equipment);
            else
            {
                Equipment equip = DatabaseManager.GetItem(playerEquip.currentEquipment[i].itemID) as Equipment;
                equipmentPanels[i].UpdatePanel(equip);
            }
        }
    }
}
