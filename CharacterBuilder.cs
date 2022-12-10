using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class CharacterBuilder : MonoBehaviour
{
    #region - Colors -
    [Header("Skin Colors")]
    private Color paleSkin = new Color(0.9607844f, 0.7843138f, 0.7294118f);
    private Color whiteSkin = new Color(1f, 0.8000001f, 0.682353f);
    private Color brownSkin = new Color(0.8196079f, 0.6352941f, 0.4588236f);
    private Color blackSkin = new Color(0.5647059f, 0.4078432f, 0.3137255f);

    [Header("Scar Colors")]
    private Color paleScar = new Color(0.8745099f, 0.6588235f, 0.6313726f);
    private Color whiteScar = new Color(0.9294118f, 0.6862745f, 0.5921569f);
    private Color brownScar = new Color(0.6980392f, 0.5450981f, 0.4f);
    private Color blackScar = new Color(0.4235294f, 0.3176471f, 0.282353f);

    [Header("Stubble Colors")]
    private Color paleStubble = new Color(0.8627452f, 0.7294118f, 0.6862745f);
    private Color whiteStubble = new Color(0.8039216f, 0.7019608f, 0.6313726f);
    private Color brownStubble = new Color(0.6588235f, 0.572549f, 0.4627451f);
    private Color blackStubble = new Color(0.3882353f, 0.2901961f, 0.2470588f);

    [Header("Hair Colors")]
    private Color[] hairColors;
    private Color hair00 = new Color(0.2196079f, 0.2196079f, 0.2196079f); //black, if not close to it
    private Color hair01 = new Color(0.1764706f, 0.1686275f, 0.1686275f); //dark brown
    private Color hair02 = new Color(0.3098039f, 0.254902f, 0.1764706f); //brown
    private Color hair03 = new Color(0.3843138f, 0.2352941f, 0.0509804f); // half orange/half blonde? light brown
    private Color hair04 = new Color(0.6862745f, 0.4f, 0.2352941f); //red/orange
    private Color hair05 = new Color(0.5450981f, 0.427451f, 0.2156863f); //kinda dark blonde
    private Color hair06 = new Color(0.8470589f, 0.4666667f, 0.2470588f); //bright orange?
    private Color hair07 = new Color(0.8313726f, 0.6235294f, 0.3607843f); //yellow blonde
    private Color hair08 = new Color(0.8980393f, 0.7764707f, 0.6196079f); //pale blonde
    private Color hair09 = new Color(0.6196079f, 0.6196079f, 0.6196079f); // more gray 
    private Color hair10 = new Color(0.8000001f, 0.8196079f, 0.8078432f); // gray?
    private Color hair11 = new Color(0.9764706f, 0.9686275f, 0.9568628f); //white

    [Header("Body Art Colors")]
    private Color[] bodyArtColors;
    private Color bodyArt00 = new Color(0f, 0f, 0f);
    private Color bodyArt01 = new Color(0.0509804f, 0.6745098f, 0.9843138f);
    private Color bodyArt02 = new Color(0.7215686f, 0.2666667f, 0.2666667f);
    private Color bodyArt03 = new Color(0.4483752f, 0.03537735f, 0.5f);
    private Color bodyArt04 = new Color(0.1348047f, 0.4433962f, 0.119215f);
    private Color bodyArt05 = new Color(0.6981132f, 0.4815834f, 0.05598078f);
    private Color bodyArt06 = new Color(1f, 1f, 1f);
    #endregion

    #region - References - 
    public bool loadTestingLevel = true;
    private PlayerInventory playerInventory;
    private PlayerController controller;
    private PlayerManager playerManager;
    private PlayerStats stats;

    [SerializeField] private GameObject camHolder;
    [SerializeField] private GameObject characterModel;
    [SerializeField] private Material mat;
    [SerializeField] private GameObject[] pages;
    [SerializeField] private Button returnToMainConfirm;
    [SerializeField] private Button finalConfirmButton;
    #endregion

    #region - Character Model -
    [Header("Character Model")]
    [SerializeField] private GameObject[] maleHeads;
    [SerializeField] private GameObject[] femaleHeads;
    [SerializeField] private GameObject[] facialHair;
    [SerializeField] private GameObject[] maleEyebrows;
    [SerializeField] private GameObject[] femaleEyebrows;
    [SerializeField] private GameObject[] hairStyles;
    [Space]
    [SerializeField] private GameObject[] defaultMale;
    [SerializeField] private GameObject[] defaultFemale;
    private bool isMale = true;
    private List<GameObject> activeObjects = new List<GameObject>();
    #endregion

    #region - Page 01: Model/General -
    [Header("Page 01: Model/General")]
    [SerializeField] private Button genderButton;
    [SerializeField] private TMP_Text genderButtonText;
    [SerializeField] private Slider faceRotate;
    [SerializeField] private Slider headSlider;
    [SerializeField] private Slider facialHairSlider, eyebrowSlider, hairStyleSlider, skinSlider, hairColorSlider, bodyArtSlider, eyeSlider;
    [Space]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField birthInput, ageInput, virtuesInput, vicesInput, downtime;
    #endregion

    #region - Page 02: Class -
    [Header("Page 02: Class")]
    [SerializeField] private Slider fullRotate;
    [SerializeField] private TMP_Text className;
    [SerializeField] private TMP_Text classDescript;
    [SerializeField] private TMP_Text descriptionField;
    [SerializeField] private Button[] classSelectButtons;
    [SerializeField] private CharacterClass[] classes;
    [SerializeField] private TMP_Text[] primarySkillNames, secondarySkillNames;
    [SerializeField] private DefaultEquip[] classEquip;
    private CharacterClass chosenClass;
    private int currentClassIndex;
    #endregion

    #region - Page 03: Background -
    [Header("Page 03: Background")]
    [SerializeField] private TMP_Text backgroundName;
    [SerializeField] private Button[] backgroundSelectButtons;
    [SerializeField] private CharacterBackground[] backgrounds;
    [SerializeField] private TMP_Text backgroundDescript, backgroundPerkName, perkDescription;
    [SerializeField] private TMP_Text[] bonusSkills;
    private CharacterBackground chosenBackground;
    #endregion

    #region - Page 04: Questionnaire -
    [Header("Page 04: Questionnaire")]
    [SerializeField] private Button[] question00Buttons;
    [SerializeField] private Button[] question01Buttons, question02Buttons, question03Buttons, question04Buttons;
    [SerializeField] private Item[] factionNotes, rings;
    [SerializeField] private string[] startingSceneNames;
    private Lifestyle lifestyle;
    private SpokenLanguage language;
    private Faction faction;
    private MagicalDomain domain;
    private bool noMagic;
    private int startingScenario;
    #endregion

    #region - Page 05: Confirmation -
    [Header("Page 05: Confirmation")]
    [SerializeField] private TMP_Text fName;
    [SerializeField] private TMP_Text fAge, fBirth, fVirtues, fVices, fDowntime, fClass, fBackground, perk, prefLifestyle, newLang;
    [SerializeField] private TMP_Text[] pSkills, sSkills, tSkills, attributes;

    #endregion

    private void Start()
    {
        DateTimeManager.instance.timeIsPaused = true;
        playerManager = PlayerManager.instance;
        stats = PlayerStats.instance;
        playerInventory = PlayerInventory.instance;
        controller = PlayerController.instance;
        InputHandler.TogglePlayerInput_Static(false);
        UIManager.instance.ToggleCursor(true);
        HUDManager.instance.HideImmediate();
        HUDManager.instance.hudLocked = true;

        for (int i = 0; i < pages.Length; i++) pages[i].SetActive(false);
        pages[0].SetActive(true);

        characterModel.GetComponentInChildren<Animator>().SetBool("isGrounded", true);
        ResetMat();
        SetMale();
        OnClassSelect(0);
        OnBackgroundSelect(0);
        RemoveClassItems();

        SliderSettings();
        ButtonSettings();
    }

    #region - UI Settings -
    private void ResetMat()
    {
        //set the skin color
        mat.SetColor("_Color_Skin", paleSkin);
        //set stubble color
        mat.SetColor("_Color_Stubble", paleScar);
        //set scar color
        mat.SetColor("_Color_Scar", paleStubble);
        //set the hair color
        mat.SetColor("_Color_Hair", hair01);
        //set the eye color
        mat.SetColor("_Color_Eyes", bodyArt00);
        //set the bodyArt Color
        mat.SetColor("_Color_BodyArt", bodyArt00);

        hairColors = new Color[12];
        hairColors[0] = hair00;
        hairColors[1] = hair01;
        hairColors[2] = hair02;
        hairColors[3] = hair03;
        hairColors[4] = hair04;
        hairColors[5] = hair05;
        hairColors[6] = hair06;
        hairColors[7] = hair07;
        hairColors[8] = hair08;
        hairColors[9] = hair09;
        hairColors[10] = hair10;
        hairColors[11] = hair11;

        bodyArtColors = new Color[7];
        bodyArtColors[0] = bodyArt00;
        bodyArtColors[1] = bodyArt01;
        bodyArtColors[2] = bodyArt02;
        bodyArtColors[3] = bodyArt03;
        bodyArtColors[4] = bodyArt04;
        bodyArtColors[5] = bodyArt05;
        bodyArtColors[6] = bodyArt06;
    }

    private void ButtonSettings()
    {
        returnToMainConfirm.onClick.AddListener(delegate
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        });
        finalConfirmButton.onClick.AddListener(FinishBuild);
        genderButton.onClick.AddListener(delegate { ChangeGender(); });

        //Class
        for (int i = 0; i < classSelectButtons.Length; i++)
        {
            int num = i;
            classSelectButtons[i].onClick.AddListener(delegate { OnClassSelect(num); });
        }
        //Background
        for (int i = 0; i < backgroundSelectButtons.Length; i++)
        {
            int num = i;
            backgroundSelectButtons[i].onClick.AddListener(delegate { OnBackgroundSelect(num); });
        }

        //Question00
        for (int i = 0; i < question00Buttons.Length; i++)
        {
            int num = i;
            question00Buttons[i].onClick.AddListener(delegate { SetQuestionnairAnswers(0, num); });
        }
        //Question01
        for (int i = 0; i < question01Buttons.Length; i++)
        {
            int num = i;
            question01Buttons[i].onClick.AddListener(delegate { SetQuestionnairAnswers(1, num); });
        }
        //Question02
        for (int i = 0; i < question02Buttons.Length; i++)
        {
            int num = i;
            question02Buttons[i].onClick.AddListener(delegate { SetQuestionnairAnswers(2, num); });
        }
        //Question03
        for (int i = 0; i < question03Buttons.Length; i++)
        {
            int num = i;
            question03Buttons[i].onClick.AddListener(delegate { SetQuestionnairAnswers(3, num); });
        }
    }

    private void SliderSettings()
    {
        faceRotate.onValueChanged.AddListener(delegate 
        { 
            camHolder.transform.rotation = Quaternion.Euler(0, faceRotate.value, 0);
        });
        fullRotate.onValueChanged.AddListener(delegate
        {
            camHolder.transform.rotation = Quaternion.Euler(0, fullRotate.value, 0);
        });

        headSlider.onValueChanged.AddListener(delegate { ChangeHead((int)headSlider.value); });
        facialHairSlider.onValueChanged.AddListener(delegate { ChangeFacialHair((int)facialHairSlider.value); });
        eyebrowSlider.onValueChanged.AddListener(delegate { ChangeEyebrows((int)eyebrowSlider.value); });
        hairStyleSlider.onValueChanged.AddListener(delegate { ChangeHairStyle((int)hairStyleSlider.value); });
        skinSlider.onValueChanged.AddListener(delegate { ChangeSkinColor((int)skinSlider.value); });
        hairColorSlider.onValueChanged.AddListener(delegate { ChangeHairColor((int)hairColorSlider.value); });
        bodyArtSlider.onValueChanged.AddListener(delegate { ChangeBodyArtColor((int)bodyArtSlider.value); });
        eyeSlider.onValueChanged.AddListener(delegate { ChangeEyeColor((int)eyeSlider.value); });

        headSlider.maxValue = 22;
        facialHairSlider.maxValue = 17;
        eyebrowSlider.maxValue = 9;
        hairStyleSlider.maxValue = 37;
        skinSlider.maxValue = 3;
        hairColorSlider.maxValue = 11;
        bodyArtSlider.maxValue = 6;
        eyeSlider.maxValue = 6;
    }
    #endregion

    #region - Body Modification -
    private void ChangeHead(int headNumber)
    {
        if (isMale)
        {
            foreach (GameObject go in maleHeads)
                if (headNumber == System.Array.IndexOf(maleHeads, go)) AddItem(go);
                else RemoveItem(go);
        }
        else
        {
            foreach (GameObject go in femaleHeads)
                if (headNumber == System.Array.IndexOf(femaleHeads, go)) AddItem(go);
                else RemoveItem(go);
        }
    }

    private void ChangeFacialHair(int beardNumber)
    {
        foreach (GameObject go in facialHair)
            if (beardNumber == System.Array.IndexOf(facialHair, go) && go != null) AddItem(go);
            else if (go != null) RemoveItem(go);
    }

    private void ChangeEyebrows(int browNumber)
    {
        if (isMale)
        {
            foreach (GameObject go in maleEyebrows)
                if (browNumber == System.Array.IndexOf(maleEyebrows, go)) AddItem(go);
                else RemoveItem(go);
        }
        else
        {
            foreach (GameObject go in femaleEyebrows)
                if (browNumber == System.Array.IndexOf(femaleEyebrows, go)) AddItem(go);
                else RemoveItem(go);
        }
    }

    private void ChangeHairStyle(int hairNumber)
    {
        foreach (GameObject go in hairStyles)
            if (hairNumber == System.Array.IndexOf(hairStyles, go) && go != null) AddItem(go);
            else if (go != null) RemoveItem(go);
    }

    #region - Colors -
    private void ChangeSkinColor(int colorNumber)
    {
        Color skinColor = Color.black;
        Color scarColor = Color.black;
        Color stubbleColor = Color.black;

        switch (colorNumber)
        {
            case 0:
                skinColor = paleSkin;
                scarColor = paleScar;
                stubbleColor = paleStubble;
                break;
            case 1:
                skinColor = whiteSkin;
                scarColor = whiteScar;
                stubbleColor = whiteStubble;
                break;
            case 2:
                skinColor = brownSkin;
                scarColor = brownScar;
                stubbleColor = brownStubble;
                break;
            case 3:
                skinColor = blackSkin;
                scarColor = blackScar;
                stubbleColor = blackStubble;
                break;
        }
        //set the skin color
        mat.SetColor("_Color_Skin", skinColor);
        // set stubble color
        mat.SetColor("_Color_Stubble", stubbleColor);
        // set scar color
        mat.SetColor("_Color_Scar", scarColor);
    }

    private void ChangeHairColor(int colorNumber)
    {
        mat.SetColor("_Color_Hair", hairColors[colorNumber]);
    }

    private void ChangeBodyArtColor(int colorNumber)
    {
        mat.SetColor("_Color_BodyArt", bodyArtColors[colorNumber]);
    }

    private void ChangeEyeColor(int colorNumber)
    {
        mat.SetColor("_Color_Eyes", bodyArtColors[colorNumber]);
    }
    #endregion

    private void AddItem(GameObject gameObject)
    {
        gameObject.SetActive(true);
        activeObjects.Add(gameObject);
    }

    private void RemoveItem(GameObject gameObject)
    {
        gameObject.SetActive(false);
        activeObjects.Remove(gameObject);
    }

    private void ChangeGender()
    {
        isMale = !isMale;
        if (isMale)
        {
            SetMale();
            genderButtonText.text = "male";
        }
        else
        {
            SetFemale();
            genderButtonText.text = "female";
        }

        MatchPerkNameToGender();
    }
    
    private void SetMale()
    {
        foreach (GameObject gameObject in activeObjects)
            gameObject.SetActive(false);

        activeObjects.Clear();

        foreach (GameObject gameObject in defaultMale)
            if (gameObject != null) AddItem(gameObject);

        ChangeHead((int)headSlider.value);

        facialHairSlider.enabled = true;
        facialHairSlider.transform.parent.gameObject.SetActive(true);

        ChangeFacialHair((int)facialHairSlider.value);
        eyebrowSlider.maxValue = 9;
        ChangeEyebrows((int)eyebrowSlider.value);
        ChangeHairStyle((int)hairStyleSlider.value);
        ChangeSkinColor((int)skinSlider.value);
        ChangeHairColor((int)hairColorSlider.value);
        ChangeBodyArtColor((int)bodyArtSlider.value);
    }

    private void SetFemale()
    {
        foreach (GameObject gameObject in activeObjects)
            gameObject.SetActive(false);

        activeObjects.Clear();

        foreach (GameObject gameObject in defaultFemale)
            if (gameObject != null) AddItem(gameObject);

        ChangeHead((int)headSlider.value);

        facialHairSlider.enabled = false;
        facialHairSlider.transform.parent.gameObject.SetActive(false);

        ChangeFacialHair(0);
        eyebrowSlider.maxValue = 6;
        if (eyebrowSlider.value > 6) eyebrowSlider.value = 6;
        ChangeEyebrows((int)eyebrowSlider.value);
        ChangeHairStyle((int)hairStyleSlider.value);
        ChangeSkinColor((int)skinSlider.value);
        ChangeHairColor((int)hairColorSlider.value);
        ChangeBodyArtColor((int)bodyArtSlider.value);
    }
    #endregion

    #region - Class & Background -
    private void OnClassSelect(int value)
    {
        RemoveClassItems();
        chosenClass = classes[value];
        currentClassIndex = value;
        className.text = chosenClass.name;
        classDescript.text = chosenClass.classDescription;
        EquipClassItems();

        primarySkillNames[0].text = chosenClass.primarySkill_00.ToString();
        primarySkillNames[1].text = chosenClass.primarySkill_01.ToString();
        primarySkillNames[2].text = chosenClass.primarySkill_02.ToString();
        primarySkillNames[3].text = chosenClass.primarySkill_03.ToString();
        primarySkillNames[4].text = chosenClass.primarySkill_04.ToString();

        secondarySkillNames[0].text = chosenClass.secondarySkill_00.ToString();
        secondarySkillNames[1].text = chosenClass.secondarySkill_01.ToString();
        secondarySkillNames[2].text = chosenClass.secondarySkill_02.ToString();
        secondarySkillNames[3].text = chosenClass.secondarySkill_03.ToString();
        secondarySkillNames[4].text = chosenClass.secondarySkill_04.ToString();

        fClass.text = chosenClass.name;
        pSkills[0].text = chosenClass.primarySkill_00.ToString();
        pSkills[1].text = chosenClass.primarySkill_01.ToString();
        pSkills[2].text = chosenClass.primarySkill_02.ToString();
        pSkills[3].text = chosenClass.primarySkill_03.ToString();
        pSkills[4].text = chosenClass.primarySkill_04.ToString();

        sSkills[0].text = chosenClass.secondarySkill_00.ToString();
        sSkills[1].text = chosenClass.secondarySkill_01.ToString();
        sSkills[2].text = chosenClass.secondarySkill_02.ToString();
        sSkills[3].text = chosenClass.secondarySkill_03.ToString();
        sSkills[4].text = chosenClass.secondarySkill_04.ToString();
    }

    [System.Serializable]
    private class DefaultEquip
    {
        [SerializeField] public string className;
        [SerializeField] public GameObject[] equipment;
        [SerializeField] public GameObject[] maleCuirass;
        [SerializeField] public GameObject[] femaleCuirass;
        [Space]
        [SerializeField] public GameObject[] malePartes;
        [SerializeField] public GameObject[] femaleParts;
    }

    [ContextMenu("Foo")]
    public void foo()
    {
        var list = new List<Item>();
        for (int i = 0; i < classes.Length; i++)
        {
            for (int x = 0; x < classes[i].startingEquipment.Length; x++)
            {
                if (!list.Contains(classes[i].startingEquipment[x]))
                    list.Add(classes[i].startingEquipment[x]);
            }
        }

        var newList = new List<Apparel>();
        foreach (Item item in list)
        {
            if (item is Apparel apparel)
            {

                newList.Add(apparel);
            }
        }

        foreach (Apparel apparel in newList)
        {
            if (apparel is Apparel_Body body)
            {
                Instantiate(body.femaleTorso, characterModel.transform);
                Instantiate(body.maleTorso, characterModel.transform);
                for (int x = 0; x < body.lowerArms.Length; x++)
                {
                    Instantiate(body.lowerArms[x], characterModel.transform);
                }
            }
            for (int i = 0; i < apparel.assignedMeshes.Length; i++)
            {
                Instantiate(apparel.assignedMeshes[i], characterModel.transform);
            }
        }
    }

    public void RemoveClassItems()
    {
        for (int i = 0; i < classEquip[currentClassIndex].equipment.Length; i++)
            classEquip[currentClassIndex].equipment[i].SetActive(false);

        if (isMale)
        {
            for (int i = 0; i < classEquip[currentClassIndex].maleCuirass.Length; i++)
                classEquip[currentClassIndex].maleCuirass[i].SetActive(false);

            for (int i = 0; i < classEquip[currentClassIndex].malePartes.Length; i++)
                classEquip[currentClassIndex].malePartes[i].SetActive(true);
        }

        else
        {
            for (int i = 0; i < classEquip[currentClassIndex].femaleCuirass.Length; i++)
                classEquip[currentClassIndex].femaleCuirass[i].SetActive(false);

            for (int i = 0; i < classEquip[currentClassIndex].femaleParts.Length; i++)
                classEquip[currentClassIndex].femaleParts[i].SetActive(true);
        }
    }

    public void EquipClassItems()
    {
        for (int i = 0; i < classEquip[currentClassIndex].equipment.Length; i++)
            classEquip[currentClassIndex].equipment[i].SetActive(true);

        if (isMale)
        {
            for (int i = 0; i < classEquip[currentClassIndex].maleCuirass.Length; i++)
                classEquip[currentClassIndex].maleCuirass[i].SetActive(true);

            for (int i = 0; i < classEquip[currentClassIndex].malePartes.Length; i++)
                classEquip[currentClassIndex].malePartes[i].SetActive(false);
        }
        else
        {
            for (int i = 0; i < classEquip[currentClassIndex].femaleCuirass.Length; i++)
                classEquip[currentClassIndex].femaleCuirass[i].SetActive(true);

            for (int i = 0; i < classEquip[currentClassIndex].femaleParts.Length; i++)
                classEquip[currentClassIndex].femaleParts[i].SetActive(false);
        }

        //So I need... 29 sets of equipment... Why did I do this to myself?
        //Ok so realistically it's only like 3-4 sets of boots... probably only a few hats
        //Some Iron weapons
        //And then of course gloves and cuirass
    }

    public void ResetCamera()
    {
        faceRotate.value = faceRotate.maxValue / 2;
        fullRotate.value = fullRotate.maxValue / 2;
        camHolder.transform.rotation = Quaternion.Euler(0, faceRotate.value, 0);
    }

    public void SkillDescription(int num)
    {
        string descript = "";
        switch (num)
        {
        //Primary Class Skills
            case 0:
                {
                    var skill = chosenClass.primarySkill_00;
                    descript = stats.statSheet.GetSkill(skill).statDescription;
                    break;
                }
            case 1:
                {
                    var skill = chosenClass.primarySkill_01;
                    descript = stats.statSheet.GetSkill(skill).statDescription;
                    break;
                }
            case 2:
                {
                    var skill = chosenClass.primarySkill_02;
                    descript = stats.statSheet.GetSkill(skill).statDescription;
                    break;
                }
            case 3:
                {
                    var skill = chosenClass.primarySkill_03;
                    descript = stats.statSheet.GetSkill(skill).statDescription;
                    break;
                }
            case 4:
                {
                    var skill = chosenClass.primarySkill_04;
                    descript = stats.statSheet.GetSkill(skill).statDescription; 
                    break;
                }
            //Secondary Class Skills
            case 5:
                {
                    var skill = chosenClass.secondarySkill_00;
                    descript = stats.statSheet.GetSkill(skill).statDescription; 
                    break;
                }
            case 6:
                {
                    var skill = chosenClass.secondarySkill_01;
                    descript = stats.statSheet.GetSkill(skill).statDescription;
                    break;
                }
            case 7:
                {
                    var skill = chosenClass.secondarySkill_02;
                    descript = stats.statSheet.GetSkill(skill).statDescription;
                    break;
                }
            case 8:
                {
                    var skill = chosenClass.secondarySkill_03;
                    descript = stats.statSheet.GetSkill(skill).statDescription;
                    break;
                }
            case 9:
                {
                    var skill = chosenClass.secondarySkill_04;
                    descript = stats.statSheet.GetSkill(skill).statDescription;
                    break;
                }
        }
        descriptionField.text = descript;
    }

    public void ResetSkillDescription()
    {
        descriptionField.text = "Your chosen class will determine your starting attributes, skills, and equipment.";
    }

    private void OnBackgroundSelect(int value)
    {
        chosenBackground = backgrounds[value];
        backgroundName.text = "Background: " + chosenBackground.name;
        backgroundDescript.text = backgrounds[value].backgroundDescription;

        bonusSkills[0].text = chosenBackground.skillBonus00.ToString();
        bonusSkills[1].text = chosenBackground.skillBonus01.ToString();
        bonusSkills[2].text = chosenBackground.skillBonus02.ToString();

        backgroundPerkName.text = chosenBackground.backgroundPerk.name;
        perk.text = backgroundPerkName.text;

        MatchPerkNameToGender();

        perkDescription.text = chosenBackground.backgroundPerk.description;
        fBackground.text = chosenBackground.name;
        tSkills[0].text = chosenBackground.skillBonus00.ToString();
        tSkills[1].text = chosenBackground.skillBonus01.ToString();
        tSkills[2].text = chosenBackground.skillBonus02.ToString();
    }

    private void MatchPerkNameToGender()
    {
        if (chosenBackground == backgrounds[3])
        {
            if (isMale) backgroundPerkName.text = chosenBackground.backgroundPerk.name;
            else
            {
                backgroundPerkName.text = "Woman of the People";
                perk.text = "Woman of the People";
            }
        }
        if (chosenBackground == backgrounds[9])
        {
            if (isMale) backgroundPerkName.text = chosenBackground.backgroundPerk.name;
            else
            {
                backgroundPerkName.text = "Woman of the Cloth";
                perk.text = "Woman of the Cloth";
            }
        }
    }
    #endregion

    private void SetQuestionnairAnswers(int question, int answer)
    {
        switch (question)
        {
            case 0: //Lifestyle
                {
                    //Preferred Lifestyle
                    lifestyle = (Lifestyle)answer;
                    prefLifestyle.text = lifestyle.ToString();
                    break;
                }
            case 1: //Language
                {
                    if (answer == 0) language = SpokenLanguage.Elvish;
                    else if (answer == 1) language = SpokenLanguage.Dwarvish;
                    else if (answer == 2) language = SpokenLanguage.Giant;
                    else if (answer == 3) language = SpokenLanguage.Fey;
                    else if (answer == 4) language = SpokenLanguage.Demonic;
                    else language = SpokenLanguage.Common;

                    if (language != SpokenLanguage.Common) newLang.text = language.ToString();
                    else newLang.text = "None";

                    break;
                }
            case 2: //Faction
                {
                    if (answer == 0) faction = Faction.Syndicate;
                    else if (answer == 1) faction = Faction.Clan;
                    else if (answer == 2) faction = Faction.Paladins;
                    else if (answer == 3) faction = Faction.Mages;
                    else if (answer == 4) faction = Faction.Grove;
                    else faction = Faction.Temple;
                    break;
                }
            case 3: //Magic
                {
                    noMagic = false;
                    if (answer == 1) domain = MagicalDomain.Arcane;
                    else if (answer == 2) domain = MagicalDomain.Divine;
                    else if (answer == 3) domain = MagicalDomain.Druidic;
                    else noMagic = true;
                    break;
                }
            case 4: //Starting Scenario
                {
                    startingScenario = answer;
                    //Tavern
                    //Prison
                    //Dungeon
                    //Docks
                    //Execution
                    break;
                }
        }
    }

    public void SetFullDisplayValues()
    {
        fName.text = nameInput.text;
        fBirth.text = birthInput.text;
        fAge.text = ageInput.text;
        fVirtues.text = virtuesInput.text;
        fVices.text = vicesInput.text;
        fDowntime.text = downtime.text;

        int Str = 0;
        int Fin = 0;
        int End = 0;
        int Soc = 0;
        int Int = 0;

        switch (chosenClass.archetype)
        {
            case ClassArchetype.Warrior:
                {
                    Str = 40;
                    End = 40;
                    Fin = 35;
                    Soc = 30;
                    Int = 30;
                    break;
                }
            case ClassArchetype.Mage:
                {
                    Int = 40;
                    Soc = 40;
                    Fin = 35;
                    End = 30;
                    Str = 30;
                    break;
                }
            case ClassArchetype.Thief:
                {
                    Fin = 40;
                    Soc = 40;
                    End = 35;
                    Int = 30;
                    Str = 30;
                    break;
                }
        }

        if (chosenClass.favoredAtt_01 == MajorAttribute.Strength || chosenClass.favoredAtt_02 == MajorAttribute.Strength) Str += 10;
        if (chosenClass.favoredAtt_01 == MajorAttribute.Finesse || chosenClass.favoredAtt_02 == MajorAttribute.Finesse) Fin += 10;
        if (chosenClass.favoredAtt_01 == MajorAttribute.Endurance || chosenClass.favoredAtt_02 == MajorAttribute.Endurance) End += 10;
        if (chosenClass.favoredAtt_01 == MajorAttribute.Social || chosenClass.favoredAtt_02 == MajorAttribute.Social) Soc += 10;
        if (chosenClass.favoredAtt_01 == MajorAttribute.Intellect || chosenClass.favoredAtt_02 == MajorAttribute.Intellect) Int += 10;

        attributes[0].text = Str.ToString();
        attributes[1].text = Fin.ToString();
        attributes[2].text = End.ToString();
        attributes[3].text = Soc.ToString();
        attributes[4].text = Int.ToString();
    }

    #region - Completion -
    void CompleteBodyBuild()
    {

        int[] face = new int[4];
        face[0] = (int)headSlider.value;
        face[1] = (int)hairStyleSlider.value;
        face[2] = (int)eyebrowSlider.value;
        if (isMale) face[3] = (int)facialHairSlider.value;
        PlayerEquipmentManager.instance.SetPlayerModel(isMale, mat.GetColor("_Color_Skin"), mat.GetColor("_Color_Hair"), 
            mat.GetColor("_Color_Stubble"), mat.GetColor("_Color_Scar"), mat.GetColor("_Color_Eyes"), mat.GetColor("_Color_BodyArt"), face);
    }

    private void AddStartingItems()
    {
        playerInventory.ClearInventory();

        for (int i = 0; i < chosenClass.startingEquipment.Length; i++)
            playerInventory.AddItem(chosenClass.startingEquipment[i], 1);

        for (int i = 0; i < chosenBackground.backgroundItems.Length; i++)
            playerInventory.AddItem(chosenBackground.backgroundItems[i], 1);

        //Equips starting equipment
        for (int i = 0; i < playerInventory.apparel.Count; i++)
            playerInventory.apparel[i].Use(controller);
    }

    private void AdjustForQuestions()
    {
        playerManager.playerLifestyle = lifestyle;

        stats.LearnLanguage(language);

        //Just need to make sure these are in order
        //They are but I may re-order the factions to be alphabetical because... why wouldn't I?
        playerInventory.AddItem(factionNotes[(int)faction - 1], 1);

        if (noMagic == true) playerInventory.AddItem(rings[0], 1);
        else playerInventory.AddItem(rings[(int)domain + 1], 1);

        playerManager.playerBirthplace = birthInput.text;
        if (ageInput.text == "") playerManager.playerAge = 25;
        else playerManager.playerAge = int.Parse(ageInput.text);
        playerManager.playerVirtues = virtuesInput.text;
        playerManager.playerVices = vicesInput.text;
        playerManager.playerHobbies = downtime.text;

    }

    private void FinishBuild()
    {
        CompleteBodyBuild();
        AddStartingItems();
        AdjustForQuestions();

        chosenClass.ApplyClassPresets(stats.statSheet);
        chosenBackground.ApplyBackgroundPresets(stats.statSheet);
        stats.statSheet.perks.Add(chosenBackground.backgroundPerk);

        if (nameInput.text == "") playerManager.playerName = "Traveller";
        else playerManager.playerName = nameInput.text;
        playerManager.playerArchetype = chosenClass.archetype.ToString();
        playerManager.playerClass = chosenClass.name;
        playerManager.playerBackground = chosenBackground.name;
        
        Player_Settings.instance.OnGeneralSettingsChanged();
        HUDManager.instance.hudLocked = false;
        HUDManager.instance.ToggleAllActiveElements(false);
        DateTimeManager.instance.timeIsPaused = false;
        InputHandler.TogglePlayerInput_Static(true);
        if (loadTestingLevel == true)
        {
            playerManager.sceneSpawnPoint = "spawnPoint";
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            playerManager.sceneSpawnPoint = "spawnPoint";
            SceneManager.LoadScene(startingSceneNames[startingScenario]);
        }
    }
    #endregion
}