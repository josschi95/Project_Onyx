using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Keep track of equipment. Has functions for adding and removing items */

public class EquipmentManager : MonoBehaviour
{
    #region - Callbacks -
    //Callback for when an item is equipped/unequipped
    public delegate void OnEquipmentChanged(InventoryEquipment newItem, InventoryEquipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    public delegate void OnWeaponSetChange(bool toSecondary);
    public OnWeaponSetChange onWeaponSetChange;
    #endregion

    protected Inventory inventory;
    protected CharacterCombat characterCombat;

    public Transform root;
    public Transform weapon_L;
    public Transform weapon_R;
    public Transform weaponsBack;
    [Space]
    public InventoryEquipment[] currentEquipment;

    #region - Weaponry -
    public ActiveWeapon[] currentWeapons;
    public ActiveWeapon mainHand;
    public ActiveWeapon offHand;
    public WeaponLoadout currentLoadout;
    public Weapon projectilesMain { get; protected set; }
    public Weapon projectilesOff { get; protected set; }
    [Space]
    #endregion

    public bool canEquipWeapons = true; //should only be false for some monstrous types
    public bool canEquipApparel = true; //is false for any character who doesn't have interchangeable parts
    public bool usingSecondaryWeaponSet = false; 

    [HideInInspector] public float apparelWeight = 1;
    public bool isNaked = false;
    public bool isMale = true;
    protected bool wearingLongGloves = false; //Used to overwrite forearm meshes on body when equipping long-sleeved gloves

    protected Dictionary<EquipmentSet, int> equipmentSets = new Dictionary<EquipmentSet, int>();
    public List<EquipmentSet> fullSets = new List<EquipmentSet>();

    #region - Base Meshes -
    [Header("Base Meshes")]
    public Color skinColor;
    public GameObject[] baseMeshes;

    /* Base Mesh Array
     * 0: Head
     * 1: Hair
     * 2: Eyebrows
     * 3: Beard
     * 4: Chest
     * 5: Hips
     * 6: UA_L
     * 7: UA_R
     * 8: LA_L
     * 9: LA_R
     * 10: Hand_L
     * 11: Hand_R
     * 12: Leg_L
     * 13: Leg_R
    */

    #endregion

    #region - Equipped Meshes -
    protected List<SkinnedMeshRenderer> equippedHeadMeshes = new List<SkinnedMeshRenderer>();
    protected List<SkinnedMeshRenderer> equippedBodyMeshes = new List<SkinnedMeshRenderer>();
    protected List<SkinnedMeshRenderer> equippedHandMeshes = new List<SkinnedMeshRenderer>();
    protected List<SkinnedMeshRenderer> equippedFeetMeshes = new List<SkinnedMeshRenderer>();
    protected SkinnedMeshRenderer equippedBackMesh = null;
    protected GameObject equippedNecklace = null;
    protected GameObject equippedRing_Left = null;
    protected GameObject equippedRing_Right = null;
    #endregion

    protected virtual void Start()
    {
        inventory = GetComponent<Inventory>();
        characterCombat = GetComponent<CharacterCombat>();

        //Initialize currentEquipment based on number of equipment slots
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new InventoryEquipment[numSlots];

        currentWeapons = new ActiveWeapon[4];
        onEquipmentChanged += CheckEquipmentSets;
    }

    public void SwapWeaponSets(bool toSecondary, bool playAnim)
    {
        usingSecondaryWeaponSet = toSecondary;

        if (characterCombat.weaponsDrawn == true && playAnim == true)
            characterCombat.animator.SetTrigger("swapWeapons");
        else
        {
            for (int i = 0; i < currentWeapons.Length; i++)
            {
                if (currentWeapons[i] != null && currentWeapons[i].equipSettings != null)
                    currentWeapons[i].equipSettings.WeaponAlignment();
            }
        }

        if (!usingSecondaryWeaponSet)
        {
            mainHand = currentWeapons[0];
            offHand = currentWeapons[1];
        }
        else
        {
            mainHand = currentWeapons[2];
            offHand = currentWeapons[3];
        }

        SetCurrentLoadout();

        if (onWeaponSetChange != null) onWeaponSetChange.Invoke(toSecondary);
    }

    protected void SetCurrentLoadout()
    {
        characterCombat.shield = null;
        characterCombat.hasBow = false;
        characterCombat.hasTwoHander = false;
        projectilesMain = null;
        projectilesOff = null;

        if (mainHand == null && offHand == null) currentLoadout = WeaponLoadout.Unarmed;
        else if (mainHand == null && offHand.weapon.weaponType == WeaponType.Arrow) currentLoadout = WeaponLoadout.Unarmed;
        else if (mainHand == null && offHand.weapon.weaponType == WeaponType.Shield) currentLoadout = WeaponLoadout.Unarmed;
        else
        {
            if (mainHand != null)
            {
                if (mainHand.weapon.weaponType == WeaponType.Bow)
                {
                    currentLoadout = WeaponLoadout.Bow;
                    characterCombat.hasBow = true;
                }
                else if (mainHand.weapon.weaponType == WeaponType.Heavy || mainHand.weapon.weaponType == WeaponType.Pole)
                {
                    characterCombat.hasTwoHander = true;
                    currentLoadout = WeaponLoadout.TwoHander;
                }
                else currentLoadout = WeaponLoadout.Standard;
                if (mainHand.weapon.weaponType == WeaponType.Thrown)
                    projectilesMain = mainHand.weapon;
            }
            if (offHand != null)
            {
                currentLoadout = WeaponLoadout.Standard;
                if (offHand.weapon.weaponType == WeaponType.Arrow)
                    projectilesOff = offHand.weapon;
                else if (offHand.weapon.weaponType == WeaponType.Thrown)
                    projectilesOff = offHand.weapon;
                else if (offHand.weapon is Shield currentShield)
                    characterCombat.shield = currentShield;
            }
        }
    }

    #region - Equipping - 200 Lines
    public void Equip(InventoryEquipment newEquip, int slotIndex)
    {
        InventoryEquipment oldEquip = Replace(slotIndex);
        Item item = DatabaseManager.GetItem(newEquip.itemID);

        //if (newEquip.item is Apparel newApparel)
        if (item is Apparel newApparel)
        {
            EquipApparel(newApparel, slotIndex);
        }
        //else if (newEquip.item is Weapon newWeapon)
        else if (item is Weapon newWeapon)
        {
            EquipWeapon(newEquip, newWeapon, slotIndex);
        }

        //Insert the item into the corresponding equipment slot
        currentEquipment[slotIndex] = newEquip;
        newEquip.isEquipped = true;

        //An item has been equipped so we trigger the callback
        if (onEquipmentChanged != null)
            onEquipmentChanged.Invoke(newEquip, oldEquip);
    }

    protected virtual void EquipApparel(Apparel newApparel, int slotIndex)
    {
        if (canEquipApparel == false) return;

        if (newApparel.glovesWithForearms == true) wearingLongGloves = true;

        foreach (GameObject go in newApparel.assignedMeshes)
        {
            var helper = go.GetComponent<BoneHelper>(); //Could set the skin in the helper as well
            AttachSkinnedMesh(helper.mesh, helper, slotIndex);
        }

        foreach (GameObject newObj in newApparel.newApparelObjects)
        {
            var helper = newObj.GetComponent<ObjectEquipHelper>();
            AttachApparelObject(newObj, helper);
        }

        if (newApparel is Apparel_Body body)
        {
            GameObject torso = body.maleTorso;
            if (isMale == false) torso = body.femaleTorso;
            var bodyHelper = torso.GetComponent<BoneHelper>();
            AttachSkinnedMesh(bodyHelper.mesh, bodyHelper, slotIndex);

            if (wearingLongGloves == false)
            {
                foreach (GameObject go in body.lowerArms)
                {
                    var armHelper = go.GetComponent<BoneHelper>();
                    AttachSkinnedMesh(armHelper.mesh, armHelper, slotIndex);
                }
            }

            //override for cape
            if (currentEquipment[(int)EquipmentSlot.Back] == null && body.backAttachment != null)
            {
                GameObject go = body.backAttachment;
                var helper = go.GetComponent<BoneHelper>();
                AttachSkinnedMesh(helper.mesh, helper, slotIndex);
            }
        }

        //Add weight of armor to wornEquipmentWeight
        apparelWeight += newApparel.weight;
        apparelWeight = Mathf.Clamp(apparelWeight, 1, 60);
    }

    protected virtual void EquipWeapon(InventoryEquipment newEquip, Weapon newWeapon, int slotIndex)
    {
        if (canEquipWeapons == false) return;

        //Instantiate new go into gameworld
        GameObject newObj = Instantiate<GameObject>(newWeapon.prefab);
        newObj.transform.parent = gameObject.transform;

        ActiveWeapon activeWep = newObj.GetComponentInChildren<ActiveWeapon>();
        if (activeWep != null)
        {
            activeWep.equipSlot = (EquipmentSlot)slotIndex;
            activeWep.equipSettings.equipManager = this;
            activeWep.equipSettings.characterCombat = characterCombat;
            activeWep.equipSettings.characterStats = characterCombat.characterStats;

            if (newEquip.equipmentRunes.Count > 0) activeWep.runed = true;
            for (int i = 0; i < newEquip.equipmentRunes.Count; i++)
            {
                if (newEquip.equipmentRunes[i].runeActivation == RuneActivation.OnStrike)
                    activeWep.onStrikeEffects.Add(newEquip.equipmentRunes[i].runeEffect);
            }
        }

        //Equipping a bow
        if (newWeapon.weaponType == WeaponType.Bow)
        {
            characterCombat.hasBow = true;
            if (offHand != null)
            {
                if (offHand is not ActiveQuiver)
                {
                    if (usingSecondaryWeaponSet) Unequip(3);
                    else Unequip(1);
                }
            }
            if (projectilesOff == null) CheckForArrows();
        }
        //Equipping a heavy weapon, polearm, or staff
        else if (newWeapon.weaponType == WeaponType.Heavy || newWeapon.weaponType == WeaponType.Pole)
        {
            if (offHand != null)
            {
                if (usingSecondaryWeaponSet) Unequip(3);
                else Unequip(1);
            }
        }
        //Equipping an arrrow
        else if (newWeapon.weaponType is WeaponType.Arrow)
            activeWep.weapon = newWeapon;
        //Equipping a one-handed weapon
        else
        {
            if (characterCombat.hasTwoHander == true)
            {
                if (usingSecondaryWeaponSet) Unequip(2);
                else Unequip(0);
            }
        }

        newObj.layer = gameObject.layer;
        foreach (Transform t in newObj.GetComponentsInChildren<Transform>())
            t.gameObject.layer = gameObject.layer;

        //Assign to current Equipped weapons
        currentWeapons[slotIndex] = activeWep;
        if (newWeapon is Shield newShield) characterCombat.shield = newShield;
        if (usingSecondaryWeaponSet)
        {
            if (slotIndex == 2) mainHand = activeWep;
            else if (slotIndex == 3) offHand = activeWep;
        }
        else
        {
            if (slotIndex == 0) mainHand = activeWep;
            else if (slotIndex == 1) offHand = activeWep;
        }
        SwapWeaponSets(usingSecondaryWeaponSet, true);
    }

    public bool CheckForArrows()
    {
        foreach (InventoryItem inventoryItem in inventory.weapons)
        {
            //if (inventoryItem.item is Arrow)
            if (DatabaseManager.GetItem(inventoryItem.itemID) is Arrow)
            {
                inventoryItem.Use(characterCombat.characterController);
                return true;
            }
        }
        return false;
    }
    #endregion

    #region - Unequipping - 150 Lines
    //Call this method when equipping another item in its place
    protected InventoryEquipment Replace(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            //The item that is currently equipped in that slot
            InventoryEquipment oldEquip = currentEquipment[slotIndex];
            //Remove the item from the equipment array
            currentEquipment[slotIndex].isEquipped = false;
            currentEquipment[slotIndex] = null;

            Item item = DatabaseManager.GetItem(oldEquip.itemID);

            //if (oldEquip.item is Apparel oldApparel)
            if (item is Apparel oldApparel)
            {
                UnequipApparel(oldApparel, slotIndex);
            }
            //else if (oldEquip.item is Weapon oldWeapon)
            else if (item is Weapon oldWeapon)
            {
                UnequipWeapon(oldWeapon, slotIndex);
            }
            return oldEquip;
        }
        return null;
    }

    public InventoryEquipment Unequip(int slotIndex)
    {
        if (currentEquipment[slotIndex] != null)
        {
            //The item that is currently equipped in that slot
            InventoryEquipment oldEquip = currentEquipment[slotIndex];
            //Remove the item from the equipment array
            currentEquipment[slotIndex].isEquipped = false;
            currentEquipment[slotIndex] = null;

            Item item = DatabaseManager.GetItem(oldEquip.itemID);

            //if (oldEquip.item is Apparel oldApparel)
            if (item is Apparel oldApparel)
            {
                UnequipApparel(oldApparel, slotIndex);
            }
            //else if (oldEquip.item is Weapon oldWeapon)
            else if (item is Weapon oldWeapon)
            {
                UnequipWeapon(oldWeapon, slotIndex);
            }

            //Equipment has been removed so we trigger the callback
            if (onEquipmentChanged != null) onEquipmentChanged.Invoke(null, oldEquip);
            return oldEquip;
        }
        return null;
    }

    protected virtual void UnequipApparel(Apparel oldApparel, int slotIndex)
    {
        if (canEquipApparel == false) return;

        switch (slotIndex)
        {
            case (int)EquipmentSlot.Head: //4
                {
                    ClearEquipmentMesh(equippedHeadMeshes);
                    ToggleBaseMeshes(CoveredArea.Head_Full, true);
                    break;
                }
            case (int)EquipmentSlot.Body: //5
                {
                    ClearEquipmentMesh(equippedBodyMeshes);
                    ToggleBaseMeshes(CoveredArea.Body, true);
                    ToggleBaseMeshes(CoveredArea.Arms_Upper, true);

                    //If the lower arms were covered, re-enable them, but only if gauntlets aren't covering them
                    if (oldApparel is Apparel_Body body && body.lowerArms.Length > 0)
                    {
                        if (wearingLongGloves == false)
                            ToggleBaseMeshes(CoveredArea.Arms_Lower, true);
                    }
                    break;
                }
            case (int)EquipmentSlot.Hands: //6
                {
                    ClearEquipmentMesh(equippedHandMeshes);
                    ToggleBaseMeshes(CoveredArea.Hands, true);
                    ToggleBaseMeshes(CoveredArea.Arms_Lower, true);

                    if (wearingLongGloves == true)
                    {
                        wearingLongGloves = false;
                        ResetBodyEquip();
                    }
                    break;
                }
            case (int)EquipmentSlot.Feet: //7
                {
                    ClearEquipmentMesh(equippedFeetMeshes);
                    ToggleBaseMeshes(CoveredArea.Feet, true);
                    break;
                }
            case (int)EquipmentSlot.Back: //9
                {
                    Destroy(equippedBackMesh.gameObject);
                    equippedBackMesh = null;
                    ResetBodyEquip();
                    break;
                }
            case (int)EquipmentSlot.Ring_Left: //12
                {
                    Destroy(equippedRing_Left.gameObject);
                    equippedRing_Left = null;
                    break;
                }
            case (int)EquipmentSlot.Ring_Right: //13
                {
                    Destroy(equippedRing_Right.gameObject);
                    equippedRing_Right = null;
                    break;
                }
            case (int)EquipmentSlot.Amulet: //14
                {
                    Destroy(equippedNecklace.gameObject);
                    equippedNecklace = null;
                    break;
                }
        }

        apparelWeight -= oldApparel.weight;
        apparelWeight = Mathf.Clamp(apparelWeight, 1, 60);
    }

    protected virtual void UnequipWeapon(Weapon oldWeapon, int slotIndex)
    {
        if (canEquipWeapons == false) return;

        //Remove the go from the gameworld
        if (currentWeapons[slotIndex] != null)
            Destroy(currentWeapons[slotIndex].equipSettings.gameObject);
        currentWeapons[slotIndex] = null;

        if (oldWeapon.weaponType == WeaponType.Arrow || oldWeapon.weaponType == WeaponType.Thrown)
        {
            SwapWeaponSets(usingSecondaryWeaponSet, false);
        }
        else
        {
            characterCombat.animator.Play("Empty", 1);
            SwapWeaponSets(usingSecondaryWeaponSet, true);
        }
    }

    protected void ResetBodyEquip()
    {
        if (currentEquipment[3] != null)
        {
            Equip(currentEquipment[3], 3);
        }
    }

    public void UnequipAll()
    {
        for (int x = 0; x < currentEquipment.Length; x++) Unequip(x);
    }
    #endregion

    #region - SkinnedMesh Swapping - 200 Lines
    protected void AttachApparelObject(GameObject go, ObjectEquipHelper helper)
    {
        GameObject newObj = Instantiate<GameObject>(go);
        //I need to add this to some list for unequipping

        foreach (var newBone in root.GetComponentsInChildren<Transform>())
        {
            if (newBone.name == helper.parent)
                newObj.transform.parent = newBone.transform;
        }

        newObj.gameObject.layer = gameObject.layer;
        newObj.transform.localPosition = new Vector3(0, 0, 0);
        newObj.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    protected void ClearEquipmentMesh(List<SkinnedMeshRenderer> list)
    {
        foreach (SkinnedMeshRenderer oldMesh in list)
        {
            Destroy(oldMesh.gameObject);
        }
        list.Clear();
    }

    protected void AttachSkinnedMesh(SkinnedMeshRenderer skin, BoneHelper helper, int slot)
    {
        SkinnedMeshRenderer newMesh = Instantiate<SkinnedMeshRenderer>(skin);
        AddToList(slot, newMesh);
        ToggleBaseMeshes(helper.coveredArea, false);

        Transform newParent = null;
        Transform[] newBones = new Transform[helper.meshBoneNames.Length];
        for (int i = 0; i < helper.meshBoneNames.Length; i++)
        {
            foreach (var newBone in root.GetComponentsInChildren<Transform>())
            {
                if (newBone.name == helper.rootBoneName)
                    newParent = newBone;

                if (newBone.name == helper.meshBoneNames[i])
                {
                    newBones[i] = newBone;
                    continue;
                }
            }
        }
        newMesh.rootBone = newParent;
        newMesh.bones = newBones;

        newMesh.transform.parent = newParent.transform;
        newMesh.bones = newBones;
        newMesh.rootBone = newParent;
        newMesh.gameObject.layer = gameObject.layer;
        newMesh.material.SetColor("_Color_Skin", skinColor);
    }

    protected void AddToList(int slot, SkinnedMeshRenderer newMesh)
    {
        switch (slot)
        {
            case (int)EquipmentSlot.Head:
                {
                    equippedHeadMeshes.Add(newMesh);
                    break;
                }
            case (int)EquipmentSlot.Body:
                {
                    equippedBodyMeshes.Add(newMesh);
                    break;
                }
            case (int)EquipmentSlot.Hands:
                {
                    equippedHandMeshes.Add(newMesh);
                    ResetBodyEquip();
                    break;
                }
            case (int)EquipmentSlot.Feet:
                {
                    equippedFeetMeshes.Add(newMesh);
                    break;
                }
            case (int)EquipmentSlot.Back:
                {
                    equippedBackMesh = newMesh;
                    ResetBodyEquip();
                    break;
                }
        }
    }

    protected void ToggleBaseMeshes(CoveredArea area, bool enable)
    {
        switch (area)
        {
            case CoveredArea.Hair:
                {
                    if (baseMeshes[1] != null) baseMeshes[1].SetActive(enable);
                    break;
                }
            case CoveredArea.Hair_Face:
                {
                    baseMeshes[2].SetActive(enable);
                    if (baseMeshes[3] != null) baseMeshes[3].SetActive(enable);
                    break;
                }
            case CoveredArea.Head_Full:
                {
                    baseMeshes[0].SetActive(enable);
                    if (baseMeshes[1] != null) baseMeshes[1].SetActive(enable);
                    baseMeshes[2].SetActive(enable);
                    if (baseMeshes[3] != null) baseMeshes[3].SetActive(enable);
                    break;
                }
            case CoveredArea.Body:
                {
                    baseMeshes[4].SetActive(enable);
                    baseMeshes[5].SetActive(enable);

                    break;
                }
            case CoveredArea.Arms_Upper:
                {
                    baseMeshes[6].SetActive(enable);
                    baseMeshes[7].SetActive(enable);
                    break;
                }
            case CoveredArea.Arms_Lower:
                {
                    baseMeshes[8].SetActive(enable);
                    baseMeshes[9].SetActive(enable);
                    break;
                }
            case CoveredArea.Hands:
                {
                    baseMeshes[10].SetActive(enable);
                    baseMeshes[11].SetActive(enable);
                    break;
                }
            case CoveredArea.Feet:
                {
                    baseMeshes[12].SetActive(enable);
                    baseMeshes[13].SetActive(enable);
                    break;
                }
            case CoveredArea.All:
                {
                    //skinColor = Color.black;

                    //This will run into a major issue though,
                    //only certain items will be able to be equipped because the modular pieces have human parts showing
                    //This works, but find a better way to do it
                    for (int i = 0; i < baseMeshes.Length; i++)
                    {
                        if (baseMeshes[i] != null) baseMeshes[i].SetActive(enable);
                    }
                    break;
                }
        }

        characterCombat.animator.Rebind();
    }
    #endregion

    protected void CheckEquipmentSets(InventoryEquipment newItem, InventoryEquipment oldItem)
    {
        if (newItem != null)
        {
            //var newEquip = newItem.item as Equipment;
            var newEquip = DatabaseManager.GetItem(newItem.itemID) as Equipment;
            if (newEquip.setPiece != null)
            {
                if (equipmentSets.ContainsKey(newEquip.setPiece) == false)
                {
                    equipmentSets.Add(newEquip.setPiece, 1);
                }
                else
                {
                    equipmentSets.TryGetValue(newEquip.setPiece, out int count);
                    equipmentSets.Remove(newEquip.setPiece);
                    equipmentSets.Add(newEquip.setPiece, count + 1);
                }
            }
        }

        if (oldItem != null)
        {
            //var oldEquip = oldItem.item as Equipment;
            var oldEquip = DatabaseManager.GetItem(newItem.itemID) as Equipment;
            if (oldEquip != null && oldEquip.setPiece != null)
            {
                equipmentSets.TryGetValue(oldEquip.setPiece, out int count);
                equipmentSets.Remove(oldEquip.setPiece);
                if (count - 1 > 0)
                    equipmentSets.Add(oldEquip.setPiece, count - 1);
                else
                {
                    if (fullSets.Contains(oldEquip.setPiece))
                        fullSets.Remove(oldEquip.setPiece);
                }
            }
        }


        foreach (KeyValuePair<EquipmentSet, int> pair in equipmentSets)
        {
            if (pair.Value == pair.Key.setPieces.Length)
            {
                fullSets.Add(pair.Key);
            }
            else
            {
                if (fullSets.Contains(pair.Key)) 
                    fullSets.Remove(pair.Key);
            }
        }
    }
}
public enum WeaponLoadout { Unarmed, Standard, Bow, TwoHander }

/* 0: Primary main hand
* 1: Primary off hand
* 2: Secondary main hand
* 3: Secondary off hand
* 4: Head
* 5: Body
* 6: Hands
* 7: Feet
* 8: Back
* 9: Ring_Right
* 10: Ring_Left
* 11: Amulet */