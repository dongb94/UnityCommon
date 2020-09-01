using System.Collections.Generic;
using k514;
using UnityEngine;

/// <summary>
/// TODO 퀘스트 그룹 단위로 변경 (한 퀘스트 안에 여러종류의 미션이 있는 경우 대응) 미션 그룹 -> 미션 -> 세부 작업
///
/// 추가 필요한 변수
/// 1. 미션 그룹 key (id는 미션id 로 대체)
/// 2. 세부 미션 그룹 (미션 그룹 내부의 진행 단계별 미션 그룹)
///
/// 미션 그룹 추가시 각 미션은 간략화 할 수 있음. 이거 중요
/// </summary>
public class QuestData : GameData<QuestData, int, QuestData.TableRecord>
{
    public class TableRecord : GameDataInstance
    {
        public int id { get; set; } // 미션 ID로 쓸 예정
        public int type { get; set; } // 1 = main, 2 = sub, 3 = daily (반복퀘스트는 sub)
        public int eventId { get; set; } // 수행의뢰 종류
        public int needLevel { get; set; }
        public bool repeat { get; set; } // 반복퀘스트 여부 (daily는 해당 없음)
        public List<int> precededQuest { get; set; } // 선행 퀘스트 목록
        public List<int> nextQuest { get; set; } // 후행 퀘스트 목록
        public string questName { get; set; } // 퀘스트 이름
        public string questSummary { get; set; } // 퀘스트 요약
        public List<int> questReward { get; set; } // 퀘스트 보상 목록
        public List<int> targetLocation { get; set; }
        public List<int> targetLocationSize { get; set; }
        public List<int> questTarget { get; set; }
        public List<int> questCondition { get; set; }
        public List<int> sceneNumber { get; set; }

        
        /// ////////////////////////////////
        
        
        private List<Vector3> targetVector;
    
        public List<Vector3> TargetVector
        {
            get
            {
                if (targetVector == null)
                {
                    targetVector = new List<Vector3>();
                    for (int i = 0; i < targetLocation.Count; i+=3)
                    {
                        targetVector.Add(new Vector3(targetLocation[i], targetLocation[i+1], targetLocation[i+2]));
                    }
                }
                return targetVector;
            }
        }

        private List<Vector3> targetVectorSize;

        public List<Vector3> TargetVectorSize
        {
            get
            {
                if (targetVectorSize == null)
                {
                    targetVectorSize = new List<Vector3>();
                    for (int i = 0; i < targetLocationSize.Count; i+=3)
                    {
                        targetVectorSize.Add(new Vector3(targetLocationSize[i], targetLocationSize[i+1], targetLocationSize[i+2]));
                    }
                }
                return targetVectorSize;
            }
        }

        private List<int> questProgress;

        public List<int> QuestProgress
        {
            get
            {
                if (questProgress == null)
                {
                    questProgress = new List<int>();
                    for (var i = 0; i < questCondition.Count; i++)
                        questProgress.Add(0);
                }
                return questProgress;
            }
            set
            {
                questProgress = value; 
                // TODO send questProgress to server
            }
        }
    }
    
    public static QuestData.TableRecord GetQuestData(int id)
    {
        if(GetInstance.GetTable().ContainsKey(id))
            return GetInstance.GetTable()[id];
        return new QuestData.TableRecord();
    }

    public override TableFileType GetTableFileType()
    {
        return TableFileType.Xml;
    }

    public override string GetTableFileName()
    {
        return "Quest";
    }
}