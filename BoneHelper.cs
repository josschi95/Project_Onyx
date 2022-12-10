using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneHelper : MonoBehaviour
{
    public SkinnedMeshRenderer mesh;

    public string rootBoneName { get { return rootName; } private set { rootName = value; } }
    public string[] meshBoneNames { get { return boneNames; } private set { boneNames = value; } }

    [SerializeField] private string rootName;
    [SerializeField] private string[] boneNames;
    public CoveredArea coveredArea;

    [ContextMenu("Display Bones")]
    void DisplayBones()
    {
        mesh = GetComponent<SkinnedMeshRenderer>();

        foreach (Transform bone in mesh.bones)
        {
            Debug.Log(gameObject.name + " " + bone.name);
        }
        Debug.Log(gameObject.name + " Root Bone " + mesh.rootBone.name + "________________");
    }

    [ContextMenu("Assign Bones")]
    private void AssignBones()
    {
        mesh = GetComponent<SkinnedMeshRenderer>();

        rootBoneName = mesh.rootBone.name;

        meshBoneNames = new string[mesh.bones.Length];

        for (int i = 0; i < mesh.bones.Length; i++)
        {
            meshBoneNames[i] = mesh.bones[i].name;
        }
    }

    [ContextMenu("Get Skin")]
    private void GetSkin()
    {
        mesh = GetComponent<SkinnedMeshRenderer>();
    }

    [ContextMenu("Attach Mesh")]
    public void AttachMesh(Transform root)
    {
        if (mesh == null) mesh = GetComponent<SkinnedMeshRenderer>();
        //gameObject.transform.SetParent(root);
        gameObject.layer = root.gameObject.layer;

        Transform newParent = null;
        Transform[] newBones = new Transform[meshBoneNames.Length];
        for (int i = 0; i < meshBoneNames.Length; i++)
        {
            foreach (var newBone in root.GetComponentsInChildren<Transform>())
            {
                if (newBone.name == rootBoneName)
                    newParent = newBone;

                if (newBone.name == meshBoneNames[i])
                {
                    newBones[i] = newBone;
                    continue;
                }
            }
        }
        mesh.rootBone = newParent;
        mesh.bones = newBones;

        mesh.transform.parent = newParent.transform;
        mesh.bones = newBones;
        mesh.rootBone = newParent;
    }

    [ContextMenu("Attach Mesh from Root")]
    public void AttachMesh_02()
    {
        Transform root = gameObject.transform.parent;
        AttachMesh(root);
    }
}

public enum CoveredArea { Hair, Hair_Face, Head_Full, Body, Arms_Upper, Arms_Lower, Hands, Feet, None, All }
