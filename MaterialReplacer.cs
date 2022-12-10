using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialReplacer : MonoBehaviour
{
    public Material mat;

    [ContextMenu("Replace Skins")]
    private void ReplaceMaterials()
    {
        SkinnedMeshRenderer[] skins = GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < skins.Length; i++)
        {
            skins[i].material = mat;
        }
        Debug.Log(skins.Length);
    }
}
