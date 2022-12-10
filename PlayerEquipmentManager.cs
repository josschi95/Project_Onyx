using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : EquipmentManager
{
    #region - Singleton -
    public static PlayerEquipmentManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of PlayerEquipmentManager found");
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField] private CharacterModelHandler modelHandler;
    [SerializeField] private Material playerMat;
    //
    public Color hairColor { get; private set; }
    public Color stubbleColor { get; private set; }
    public Color scarColor { get; private set; }
    public Color eyeColor { get; private set; }
    public Color paintColor { get; private set; }
    public int[] facialModels = {0, 0, 0, 0};

    public void SetPlayerModel(bool isMale, Color skinColor, Color hairColor, Color stubbleColor, Color scarColor, Color eyeColor, Color paintColor, int[] facialModels)
    {
        this.isMale = isMale;
        this.skinColor = skinColor;
        this.hairColor = hairColor;
        this.stubbleColor = stubbleColor;
        this.scarColor = scarColor;
        this.eyeColor = eyeColor;
        this.paintColor = paintColor;

        this.facialModels = new int[4];
        facialModels.CopyTo(this.facialModels, 0);

        UpdatePlayerMat();
        UpdatePlayerModel();
        characterCombat.animator.Rebind();
    }

    private void UpdatePlayerMat()
    {
        playerMat.SetColor("_Color_Skin", skinColor);
        playerMat.SetColor("_Color_Hair", hairColor);
        playerMat.SetColor("_Color_Stubble", stubbleColor);
        playerMat.SetColor("_Color_Scar", scarColor);
        playerMat.SetColor("_Color_Eyes", eyeColor);
        playerMat.SetColor("_Color_BodyArt", paintColor);
    }

    private void UpdatePlayerModel()
    {
        //Clear existing base meshes
        for (int i = 0; i < baseMeshes.Length; i++)
        {
            if (baseMeshes[i] != null)
            {
                Destroy(baseMeshes[i].gameObject);
                baseMeshes[i] = null;
            }
        }

        int head = facialModels[0]; //head
        int hair = facialModels[1]; //hair
        int brow = facialModels[2]; //eyebrow
        int beard = facialModels[3]; //beard

        baseMeshes = new GameObject[14];

        //Instantiate player skins
        if (hair != 0) baseMeshes[1] = Instantiate(modelHandler.hairModels[hair]);

        if (isMale)
        {
            baseMeshes[0] = Instantiate(modelHandler.maleHeads[head]);
            baseMeshes[2] = Instantiate(modelHandler.maleBrows[brow]);

            if (beard != 0) baseMeshes[3] = Instantiate(modelHandler.beardModels[beard]);

            baseMeshes[4] = Instantiate(modelHandler.maleBody[0]);
            baseMeshes[5] = Instantiate(modelHandler.maleBody[1]);
            baseMeshes[6] = Instantiate(modelHandler.maleBody[2]);
            baseMeshes[7] = Instantiate(modelHandler.maleBody[3]);
            baseMeshes[8] = Instantiate(modelHandler.maleBody[4]);
            baseMeshes[9] = Instantiate(modelHandler.maleBody[5]);
            baseMeshes[10] = Instantiate(modelHandler.maleBody[6]);
            baseMeshes[11] = Instantiate(modelHandler.maleBody[7]);
            baseMeshes[12] = Instantiate(modelHandler.maleBody[8]);
            baseMeshes[13] = Instantiate(modelHandler.maleBody[9]);
        }
        else
        {
            baseMeshes[0] = Instantiate(modelHandler.femaleHeads[head]);
            baseMeshes[2] = Instantiate(modelHandler.femaleBrows[brow]);

            baseMeshes[4] = Instantiate(modelHandler.femaleBody[0]);
            baseMeshes[5] = Instantiate(modelHandler.femaleBody[1]);
            baseMeshes[6] = Instantiate(modelHandler.femaleBody[2]);
            baseMeshes[7] = Instantiate(modelHandler.femaleBody[3]);
            baseMeshes[8] = Instantiate(modelHandler.femaleBody[4]);
            baseMeshes[9] = Instantiate(modelHandler.femaleBody[5]);
            baseMeshes[10] = Instantiate(modelHandler.femaleBody[6]);
            baseMeshes[11] = Instantiate(modelHandler.femaleBody[7]);
            baseMeshes[12] = Instantiate(modelHandler.femaleBody[8]);
            baseMeshes[13] = Instantiate(modelHandler.femaleBody[9]);
        }

        //Attach to player skeleton
        for (int i = 0; i < baseMeshes.Length; i++)
        {
            if (baseMeshes[i] != null)
            {
                baseMeshes[i].GetComponent<BoneHelper>().AttachMesh(root);
            }
        }

        //Set new base mesh material to player mat
        for (int i = 0; i < baseMeshes.Length; i++)
        {
            if (baseMeshes[i] != null)
            {
                baseMeshes[i].GetComponent<SkinnedMeshRenderer>().material = playerMat;
            }
        }
    }
}