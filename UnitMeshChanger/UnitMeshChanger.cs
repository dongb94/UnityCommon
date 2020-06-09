using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class UnitMeshChanger
{
    #region <Fields>

    private LamiereUnit _MasterNode;
    private ItemMeshChanger _meshChanger;
    private Dictionary<UnitMeshChangeTool.LamiereUnitParts, LamiereUnitPartWrapper> _MeshWrapperCollection;
    
    #endregion

    #region <Constructors>

    public UnitMeshChanger(LamiereUnit p_LamierUnit)
    {
        _MeshWrapperCollection = new Dictionary<UnitMeshChangeTool.LamiereUnitParts, LamiereUnitPartWrapper>();
        
        _MasterNode = p_LamierUnit;
        
        var rootTransform = p_LamierUnit._Transform;
        var childCount = rootTransform.childCount;
        
        _meshChanger = new ItemMeshChanger(rootTransform);
        
        foreach (var unitParts in UnitMeshChangeTool._Enumerator)
        {
            var unitPartString = UnitMeshChangeTool._NameEnumearator[unitParts];
            var spawnedWrapperObject = new GameObject(unitPartString).transform;
            spawnedWrapperObject.SetParent(rootTransform, false);

            var spawnedWrapper = spawnedWrapperObject.gameObject.AddComponent<LamiereUnitPartWrapper>();
            spawnedWrapper.Initialize(this, unitParts);
            
            _MeshWrapperCollection.Add(unitParts, spawnedWrapper);   
        }
        
        SetUnitPartContent(rootTransform);

        foreach (var wrapper in _MeshWrapperCollection.Values)
        {
            if (!wrapper.Equip(0))
            {
                var errString = $"UnitPartWrapper Initialize Err : \n";

                foreach (var pair in wrapper.contentGroup)
                {
                    errString += $" - {pair.Key} : \n";
                    foreach (var content in pair.Value)
                    {
                        errString += $"   {content.Transform.name}\n";
                    }
                }
            
                Debug.LogError(errString);
            }
        }
    }

    #endregion

    #region <Methods>

    public LamiereUnitPartContent GetParts(UnitMeshChangeTool.LamiereUnitParts p_Type, int p_Index)
    {
        var targetWrapper = _MeshWrapperCollection[p_Type];
        var targetList = targetWrapper.GetUnitPartContent();
        if (targetList.Count > 0)
        {
            return targetList[Mathf.Clamp(p_Index, 0, targetList.Count - 1)];
        }
        else
        {
            return default;
        }
    }

    public bool ChangeUnitPart(int p_TableRecordIndex)
    {
        var targetTable = UnitPartPrefabData.GetInstance.GetTable();
        if (targetTable.TryGetValue(p_TableRecordIndex, out var targetRecord))
        {
            if (targetRecord != null)
            {
                var wrapper = _MeshWrapperCollection[targetRecord.TargetPartType];
                if (!wrapper.Equip(targetRecord.ItemLevel))
                {
                    var asset = LoadAssetManager.GetInstance.LoadAsset<GameObject>(ResourceType.GameObjectPrefab,
                        ResourceLifeCycleType.Scene, targetRecord.PrefabName);
                    var equipObject = GameObject.Instantiate(asset, wrapper.transform);
                    SetUnitPartContent(equipObject.transform);

                    foreach (var content in wrapper.contentGroup[targetRecord.ItemLevel])
                    {
                        _meshChanger.ApplyMeshRenderer(content.SkinnedMeshRenderer);
                    }
                }

                return true;
            }
            else
            {
                Debug.LogError("TargetRecord is null");
                return false;
            }
        }
        else
        {
            Debug.LogError("Table read error");
            return false;
        }
    }

    private void SetUnitPartContent(Transform meshParent)
    {
        var childCount = meshParent.childCount;
        Transform[] child = new Transform[childCount];
        for (var i = 0; i < childCount; i++)
        {
            child[i] = meshParent.GetChild(i);
        }
        
        for (var i = 0; i < childCount; i++)
        {
            foreach (var unitParts in UnitMeshChangeTool._Enumerator)
            {
                // 장비의 이름 규칙 => 직업명[0-장비레벨]_부위{_[1-부위번호]}
                // ex) archer0_weapon    => 아쳐 장비레벨 0 무기 (부위 없음)
                // magician6_arm_1       => 법사 장비레벨 6 팔 (1번째 부위)
                var unitPartString = UnitMeshChangeTool._NameEnumearator[unitParts];
                
                if (child[i].name.Contains(unitPartString) && !child[i].name.Equals(unitPartString)) 
                {
                    var split = child[i].name.Split('_');

                    var number = new[] {'0','1','2','3','4','5','6','7','8','9'};
                    var index = split[0].IndexOfAny(number);
                    if (index < 0)
                    {
                        Debug.LogError($"Input String Err : {child[i].name} \n" +
                                       $"Split 0 : {split[0]}\n" +
                                       $"UnitPart : {unitPartString}\n" +
                                       $"Number Index : {index}");
                    }
                    var level = split[0].Substring(index);
                    
                    _MeshWrapperCollection[unitParts].Add(child[i], int.Parse(level));
                }
            }
        }
    }

    #endregion

    #region <Structs>
    
    public struct LamiereUnitPartContent
    {
        public Transform Transform;
        public UnitMeshChangeTool.LamiereUnitParts PartType;
        public SkinnedMeshRenderer SkinnedMeshRenderer;
        
        public LamiereUnitPartContent(UnitMeshChangeTool.LamiereUnitParts p_PartType, Transform p_Transform)
        {
            Transform = p_Transform;
            PartType = p_PartType;
            SkinnedMeshRenderer = p_Transform.GetComponent<SkinnedMeshRenderer>();
        }
    }

    #endregion

    #region <InnerClass>

    public class LamiereUnitPartWrapper : MonoBehaviour
    {
        public UnitMeshChanger parent;
        public UnitMeshChangeTool.LamiereUnitParts partType;
        public Dictionary<int, List<LamiereUnitPartContent>> contentGroup;

        public int currentLevel;

        public void Initialize(UnitMeshChanger meshChanger, UnitMeshChangeTool.LamiereUnitParts p_PartType)
        {
            parent = meshChanger;
            partType = p_PartType;
            contentGroup = new Dictionary<int, List<LamiereUnitPartContent>>();
            currentLevel = -1;
        }

        public void Add(Transform content, int level)
        {
            if(!contentGroup.ContainsKey(level)) contentGroup.Add(level, new List<LamiereUnitPartContent>());
            contentGroup[level].Add(new LamiereUnitPartContent(partType, content));
            content.SetParent(transform);
        }

        public bool Equip(int level)
        {
            if (contentGroup.ContainsKey(level))
            {
                if(currentLevel != -1) Disrobe();
                foreach (var content in contentGroup[level])
                {
                    content.Transform.gameObject.SetActive(true);
                }
            }
            else
            {
                return false;
            }
            currentLevel = level;

            return true;
        }

        public void Disrobe()
        {
            foreach (var content in contentGroup[currentLevel])
            {
                content.Transform.gameObject.SetActive(false);
            }
        }

        public List<LamiereUnitPartContent> GetUnitPartContent()
        {
            return contentGroup[currentLevel];
        }
    }

    #endregion
}