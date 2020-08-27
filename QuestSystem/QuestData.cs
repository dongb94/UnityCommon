using System.Collections.Generic;
using Almond.GameData;
using Almond.Util;
using k514;
using UnityEngine;

public class QuestData : GameData<QuestData, int, QuestData.TableRecord>
{
    public class TableRecord : GameDataInstance
    {
        public int id { get; set; }
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