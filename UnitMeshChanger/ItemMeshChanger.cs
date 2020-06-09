
using System.Collections.Generic;
using UnityEngine;
using k514;

public class ItemMeshChanger
{
    private Transform SubjectBone;
    
    private Dictionary<string, Transform> _boneTransforms;
    
    public enum MeshParts
    {
        arm,
        body,
        head,
        boots,
    }

    public ItemMeshChanger(Transform subject)
    {
        SubjectBone = subject;
        _boneTransforms = new Dictionary<string, Transform>();
        ListingTransform();
    }

    public void ApplyMeshRenderer(SkinnedMeshRenderer meshRenderer)
    {
        meshRenderer.rootBone = _boneTransforms[meshRenderer.rootBone.name];
        var bones = meshRenderer.bones;
        Transform[] bone = new Transform[bones.Length];
        for (var i = 0; i < bones.Length; i++)
        {
            if (_boneTransforms.ContainsKey(bones[i].name))
            {
                bone[i] = _boneTransforms[bones[i].name];
            }
            else
            {
                bone[i] = SubjectBone;
                Debug.Log(i + "(false) : "+ bones[i].name);
            }
        }
        meshRenderer.bones = bone;
    }

    private void ListingTransform()
    {
        var transforms = SubjectBone.GetComponentsInChildren<Transform>();
        foreach (var tf in transforms)
        {
            if (!_boneTransforms.ContainsKey(tf.name))
            {
                _boneTransforms.Add(tf.name, tf);
            }
            else
            {
                Debug.LogError($"Transform Name Duplicate {tf.name}");
            }
        }
    }

    /*
     
    public async void ChangeMesh(MeshParts parts, int level)
    {
        //var skins = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        var changeMesh = transform.Find(parts.ToString()).GetComponent<SkinnedMeshRenderer>();
        changeMesh.gameObject.SetActive(false);

        var itemModel = await LoadAssetManager.GetInstance.LoadAssetAsync<GameObject>(ResourceType.GameObjectPrefab, ResourceLifeCycleType.Follwing, transform.name + level);
        //var itemModel = AssetMgr.GetResource(transform.name + level) as GameObject;
        var item = Instantiate(itemModel.transform.Find(parts.ToString()).GetComponent<SkinnedMeshRenderer>());
        var gameObject = new GameObject("arm");
        var newMesh = gameObject.AddComponent<SkinnedMeshRenderer>();
        
        newMesh.transform.parent = changeMesh.transform.parent;
        newMesh.rootBone = changeMesh.rootBone;
        newMesh.sharedMesh = item.sharedMesh;
        newMesh.materials = item.materials; 
        var bones = item.bones;
        Transform[] bone = new Transform[bones.Length];
        for (var i = 0; i < bones.Length; i++)
        {
            if (_boneTransforms.ContainsKey(bones[i].name))
            {
                bone[i] = _boneTransforms[bones[i].name];
            }
            else
            {
                bone[i] = transform;
                Debug.Log(i + "(false) : "+ bones[i].name);
            }
        }
        newMesh.bones = bone;
   
    }

    public void TestEquip()
    {
        var arm = transform.Find("arm").GetComponent<SkinnedMeshRenderer>();
        arm.gameObject.SetActive(false);
        var newArm = Instantiate(item.transform.Find("arm").GetComponent<SkinnedMeshRenderer>());
        var gameObject = new GameObject("arm");
        var newMesh = gameObject.AddComponent<SkinnedMeshRenderer>();

        newMesh.transform.parent = arm.transform.parent;
        newMesh.rootBone = arm.rootBone;
        newMesh.sharedMesh = newArm.sharedMesh;
        newMesh.materials = newArm.materials; 
        var bones = newArm.bones;
        Transform[] bone = new Transform[bones.Length];
        for (var i = 0; i < bones.Length; i++)
        {
            if (_boneTransforms.ContainsKey(bones[i].name))
            {
                bone[i] = _boneTransforms[bones[i].name];
            }
            else
            {
                bone[i] = transform;
                Debug.Log(i + "(false) : "+ bones[i].name);
            }
        }
        newMesh.bones = bone;
        
        string debug = "";
        for (int i = 0; i < newArm.bones.Length; i++)
        {
            debug += i + " : " + newArm.bones[i].name + "   " + newArm.bones[i].transform.position + "\n";
        }
        Debug.Log(debug);
    }
    
    */
}