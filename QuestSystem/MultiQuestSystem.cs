
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiQuestSystem
{
    public static MultiQuestSystem Instance = new MultiQuestSystem();
    
    private Dictionary<int, QuestData.TableRecord> ClearedQuestDataSet; // 클리어한 퀘스트 목록
    private HashSet<QuestData.TableRecord> FeasibleQuestDataSet; // 시작 가능한 퀘스트 목록
    private HashSet<QuestData.TableRecord> QuestDataSet;

    private enum QuestType
    {
        Main = 1,
        Sub = 2,
        Daily = 3,
    }
    
    private MultiQuestSystem()
    {
        ClearedQuestDataSet = new Dictionary<int, QuestData.TableRecord>();
        FeasibleQuestDataSet = new HashSet<QuestData.TableRecord>();
        QuestDataSet = new HashSet<QuestData.TableRecord>();
    }
    
    // daily quest는 서버에서 처리한다.
    /// <summary>
    /// 서버에서 받아온 정보로 시스템을 초기화 한다.
    /// </summary>
    /// <param name="baseQuestNumList">선행 퀘스트가 없는 퀘스트 목록</param>
    /// <param name="clearedQuestNumList">플레이어가 완수한 퀘스트 목록</param>
    /// <param name="inProgressQuestNumList">플레이어가 진행중인 퀘스트 목록</param>
    /// <param name="questProgress">플레이어가 진행중인 퀘스트들의 진행도</param>
    public void InitQuestInfo(List<int> baseQuestNumList, List<int> clearedQuestNumList, List<int> inProgressQuestNumList, List<List<int>> questProgress)
    {
        for (int i=0; i<inProgressQuestNumList.Count(); i++)
        {
            var questNum = inProgressQuestNumList[i]; 
            var quest = QuestData.GetQuestData(questNum);
            QuestDataSet.Add(quest);
            quest.QuestProgress = questProgress[i];
        }
        
        foreach (var questNum in clearedQuestNumList)
        {
            var quest = QuestData.GetQuestData(questNum);
            ClearedQuestDataSet.Add(quest.id, quest);            
        }

        foreach (var questSet in ClearedQuestDataSet)
        {
            var quest = questSet.Value;
            if (FeasibleQuestDataSet.Contains(quest)) FeasibleQuestDataSet.Remove(quest);
            foreach (var nextQuest in quest.nextQuest)
            {
                CheckFeasible(nextQuest);
            }
        }

        foreach (var questNum in baseQuestNumList)
        {
            CheckFeasible(questNum);
            QuestStart(questNum); // <-- for test 퀘스트 시작 테스트용 코드 TODO Delete
        }
    }
    
    public void QuestStart(int questId)
    {
        var quest = QuestData.GetQuestData(questId);
        // if (quest.needLevel > MogoWorld.thePlayer.level) return; // 레벨 제한 확인 check player level
        if (!FeasibleQuestDataSet.Contains(quest)) return; // 시작 가능한 퀘스트 목록에 포함되어 있는지 확인
        FeasibleQuestDataSet.Remove(quest);
        QuestDataSet.Add(quest);
        
        // TODO send quest start information to server
        
        // TODO 시작 이벤트, 효과 등등, 배달 임무일 경우 아이템 획득
    }

    public void QuestClear(int questId)
    {
        var questData = QuestData.GetQuestData(questId);
        if (!QuestDataSet.Contains(questData)) return;
        QuestDataSet.Remove(questData);
        if(!ClearedQuestDataSet.ContainsKey(questData.id))
            ClearedQuestDataSet.Add(questData.id, questData);
        
        // TODO send questClear information to server
        
        foreach (var nextQuest in questData.nextQuest)
        {
            CheckFeasible(nextQuest);
            if(questData.type == (int)QuestType.Main)
                QuestStart(nextQuest);
        }
        
        // TODO 클리어 이벤트 (대화등)
        
        QuestReward(questData.questReward);
    }

    public void QuestFail(int questId) // 퀘스트 포기, 우편물 분실 등, main 퀘스트는 실패 없음.
    {
        var questData = QuestData.GetQuestData(questId);
        if (!QuestDataSet.Contains(questData)) return;
        QuestDataSet.Remove(questData);
        CheckFeasible(questId);
    }

    public void QuestReward(List<int> rewardList) // TODO + 골드 경험치, 아이템 갯수 표기하도록 변경
    {
        //MogoWorld.thePlayer.inventoryManager.
        //보류
    }
    
    //TODO move to UI
    public void ShowQuestMarkUI()
    {
        // 받을 수 있는 퀘스트가 있을 경우 NPC위에 느낌표 표시
        // FeasibleQuestDataSet에서 레벨 확인 후 표기
    }
    
    //TODO move to UI
    public void ShowFeasibleQuestListUI(int npcId)
    {
        // NPC의 퀘스트 목록에
        // FeasibleQuestDataSet에 있는 퀘스트 목록 표기 (해당 NPC에 해당하는 것만)
        // 단 레벨 제한에 걸릴경우 받을 수 없음
    }
    
    public void UpdateQuestListUI(QuestData.TableRecord quest)
    {
        // 진행중인 퀘스트 목록 업데이트
        bool complete = true;
        for (int i = 0; i < quest.questCondition.Count; i++)
        {
            if (quest.questCondition[i] != quest.QuestProgress[i])
            {
                complete = false;
            }
        }

        QuestProgressUI.Instance.UpdateList(quest, complete);
    }
    
    /// <summary>
    /// 퀘스트 자동진행
    /// </summary>
    /// <param name="questId"></param>
    public void QuestProgressing(int questId)
    {
        var quest = QuestData.GetQuestData(questId);
        
        var eventId = quest.eventId;
        
        var targetList = quest.questTarget;
        var progressList = quest.QuestProgress;
        var targetSceneNumList = quest.sceneNumber;
        int currentTarget;
        int targetSceneNum = MogoWorld.thePlayer.sceneId;
        Vector3 targetVector;

        switch (eventId)
        {
            case 1: // 대화 (대화완료 = 1, 대화미완료 = 0)
                for (var i = 0; i < progressList.Count; i++)
                {
                    if(progressList[i] == quest.questCondition[i]) continue;
                    currentTarget = targetList[i];
                    targetSceneNum = targetSceneNumList[i];
                    break;
                }

                changeScene(targetSceneNum);
                
                // TODO currentTarget 추적
                break;
            case 2 : // 배달 (배달완료 = 1, 배달미완료 = 0) 
                for (var i = 0; i < progressList.Count; i++)
                {
                    if(progressList[i] == quest.questCondition[i]) continue;
                    currentTarget = targetList[2 * i + 1]; // targetList {대상, 물건, 대상 ,물건}
                    targetSceneNum = targetSceneNumList[i]; // {대상위치, 대상위치}
                    break;
                }
                
                changeScene(targetSceneNum);
                
                // TODO currentTarget에게 배달
                break;
            case 3 : // 사냥 (사냥한 수)
                for (var i = 0; i < progressList.Count; i++)
                {
                    if(progressList[i] == quest.questCondition[i]) continue;
                    currentTarget = targetList[i];
                    targetSceneNum = targetSceneNumList[i];
                    break;
                }
                
                changeScene(targetSceneNum);
                
                //TODO currentTarget 사냥
                break;
            case 4 : // 조달 (소지한 물품의 수)
                for (var i = 0; i < progressList.Count; i++)
                {
                    if(progressList[i] == quest.questCondition[i]) continue;
                    currentTarget = targetList[i];
                    targetSceneNum = targetSceneNumList[i];
                    break;
                }
                
                changeScene(targetSceneNum);
                
                // TODO currentTarget 조달
                break;
            case 5 : // 정찰 (정찰완료 = 1, 정찰 미완료 = 0) // 오픈 월드 이전에는 일단 스테이지 클리어 기준으로
                for (var i = 0; i < progressList.Count; i++)
                {
                    if(progressList[i] == quest.questCondition[i]) continue;
                    targetVector = quest.TargetVector[i];
                    targetSceneNum = targetSceneNumList[i];
                    break;
                }
                
                changeScene(targetSceneNum);
                
                // TODO targetVector 정찰
                break;
            case 6 : // 호위 (호위 목표가 목표지점에 도달하면 = 1) // 오픈 월드 이전에는 호위 스테이지 클리어를 기준으로
                for (var i = 0; i < progressList.Count; i++)
                {
                    if(progressList[i] == quest.questCondition[i]) continue;
                    targetVector = quest.TargetVector[i];  
                    targetSceneNum = targetSceneNumList[i];
                    break;
                }
                var escortTarget = targetList[0];
                
                changeScene(targetSceneNum);
                
                // TODO targetVector 향해 이동
                break;
            case 7 : // 파괴 (파괴한 목표의 수) // object.OnDeath()?
                for (var i = 0; i < progressList.Count; i++)
                {
                    if(progressList[i] == quest.questCondition[i]) continue;
                    currentTarget = targetList[i];
                    targetSceneNum = targetSceneNumList[i];
                    break;
                }
                
                changeScene(targetSceneNum);
                
                // TODO currentTarget 파괴
                break;
            case 8 : // 특정 아이템 사용 (소모품 아이템 사용 횟수)
                for (var i = 0; i < progressList.Count; i++)
                {
                    if(progressList[i] == quest.questCondition[i]) continue;
                    currentTarget = targetList[i];
                    break;
                }
                
                // TODO currentTarget 사용
                break;
            case 9 : // 특정 지역에 아이템 사용 (특정 지역에서 아이템을 사용한 횟수)
                for (var i = 0; i < progressList.Count; i++)
                {
                    if(progressList[i] == quest.questCondition[i]) continue;
                    targetVector = quest.TargetVector[i];
                    currentTarget = targetList[i];
                    targetSceneNum = targetSceneNumList[i];
                    break;
                }
                
                changeScene(targetSceneNum);
                
                // TODO targetVector로 이동후 currentTarget 사용
                break;
            case 10 : // 특정 대상에게 아이템 사용 (특정 대상에게 아이템을 사용한 횟수)
                int itemNum;
                for (var i = 0; i < progressList.Count; i++)
                {
                    if(progressList[i] == quest.questCondition[i]) continue;
                    currentTarget = targetList[2 * i]; // {대상, 아이템, 대상, 아이템}
                    itemNum = targetList[2 * i + 1];
                    targetSceneNum = targetSceneNumList[i];
                    break;
                }
                
                changeScene(targetSceneNum);
                
                // TODO currentTarget에게 이동후 itemNum 사용
                break;
            case 11 : // 특정한 장비를 착용하고 사냥 (특정 장비를 착용하고 목표물을 사냥한 횟수)
                for (var i = 0; i < progressList.Count; i++)
                {
                    if(progressList[i] == quest.questCondition[i]) continue;
                    currentTarget = targetList[i + 1]; // {장비, 대상, 대상}
                    targetSceneNum = targetSceneNumList[i];
                    break;
                }
                
                changeScene(targetSceneNum);

                // TODO currentTarget을 착용하고 사냥
                break;
            case 12 : // 특정 대상에게 특정 스킬 사용 (특정 대상에게 스킬을 사용한 횟수)
                int skillNum;
                for (var i = 0; i < progressList.Count; i++)
                {
                    if(progressList[i] == quest.questCondition[i]) continue;
                    currentTarget = targetList[2 * i]; // {대상, 스킬, 대상, 스킬}
                    skillNum = targetList[2 * i + 1];
                    targetSceneNum = targetSceneNumList[i];
                    break;
                }
                
                changeScene(targetSceneNum);
                
                // TODO currentTarget에게 이동후 skillNum 사용
                break;
            
        }
    }

    private bool CheckFeasible(int questId)
    {
        var quest = QuestData.GetQuestData(questId);

        if (QuestDataSet.Contains(quest)) return false; // 진행중이면 false
        if (FeasibleQuestDataSet.Contains(quest)) return false; // 이미 진행 가능한 퀘스트 목록에 포함돼어 있으면 false;
        
        if (quest.repeat); // 반복 퀘스트가 아닐경우 클리어 된 퀘스트 목록에 있으면 false. daily quest는 매일 클리어 목록에서 제거
        else if (ClearedQuestDataSet.ContainsKey(questId)) return false;
        
        foreach (var preQuest in quest.precededQuest) // 선행퀘스트가 완료되지 않았으면 false
        {
            if (ClearedQuestDataSet.ContainsKey(preQuest)) continue;
            return false;
        }
        FeasibleQuestDataSet.Add(quest);
        return true;
    }

    public void QuestEvent(int eventId, int target)
    {
        Debug.Log("QuestEventSend // id = " + eventId + (QuestMissionType)eventId + ", targetId = " + target);
        foreach (var quest in QuestDataSet)
        {
            if (quest.eventId == eventId)
            {
                switch (eventId)
                {
                    case 1 : // 대화 (대화완료 = 1, 대화미완료 = 0)
                    case 3 : // 사냥 (사냥한 수)
                    case 5 : // 정찰 (정찰완료 = 1, 정찰 미완료 = 0) // 오픈 월드 이전에는 일단 스테이지 클리어 기준으로
                    case 6 : // 호위 (호위 목표가 목표지점에 도달하면 = 1) // 오픈 월드 이전에는 호위 스테이지 클리어를 기준으로
                    case 7 : // 파괴 (파괴한 목표의 수) // object.OnDeath()?
                    case 8 : // 특정 아이템 사용 (소모품 아이템 사용 횟수)
                        for (int i = 0; i < quest.questTarget.Count; i++)
                        {
                            var targetId = quest.questTarget[i];
                            if(target == targetId)
                                UpdateQuestProgress(quest, i);
                        }
                        break;
                    case 2 : // 배달 (배달완료 = 1, 배달미완료 = 0)
                        for (int i = 0; i < quest.questTarget.Count; i+=2) // target = entity ID
                        {
                            var entityId = quest.questTarget[i];
                            var itemId = quest.questTarget[i + 1];
                            if (InventoryManager.GetInstance[itemId]._Stack <= 0) break;
                            if (target == entityId)
                            {
                                UpdateQuestProgress(quest, i / 2);
                                // TODO 배달된 재료 아이템 재거 (server쪽 필요)
                            }
                        }
                        break;
                    case 4 : // 조달 (소지한 물품의 수)
                        for (int i = 0; i < quest.questTarget.Count; i++)
                        {
                            var targetId = quest.questTarget[i];
                            if(target == targetId)
                                UpdateNumberOfItem(quest, i, target);
                        }
                        break;
                }
            }
        }
    }
    
    public void QuestEvent(int eventId, int target, Vector3 position)
    {
        foreach (var quest in QuestDataSet)
        {
            if (quest.eventId == eventId)
            {
                switch (eventId)
                {
                    case 9 : // 특정 지역에 아이템 사용 (특정 지역에서 아이템을 사용한 횟수)
                        for (int i = 0; i < quest.questTarget.Count; i++)
                        {
                            var targetId = quest.questTarget[i];
                            if(target == targetId && checkInLocation(position, quest.TargetVector[i], quest.TargetVectorSize[i]))
                                UpdateQuestProgress(quest, i);
                        }
                        break;
                }
            }
        }
    }
    
    public void QuestEvent(int eventId, int target1, int target2)
    {
        Debug.Log("QuestEventSend // id = "+ eventId + (QuestMissionType)eventId + ", targetId = " + target1 + " & " + target2);
        foreach (var quest in QuestDataSet)
        {
            if (quest.eventId == eventId)
            {
                switch (eventId)
                {
                    case 9 : // 특정 지역에 아이템 사용 (특정 지역에서 아이템을 사용한 횟수) (특정 맵에서 아이템을 사용한 횟수)
                        for (int i = 0; i < quest.questTarget.Count; i+=2) // target1 = map ID, target2 = item ID
                        {
                            var mapId = quest.questTarget[i];
                            var itemId = quest.questTarget[i + 1];
                            if(target1 == mapId && target2 == itemId)
                                UpdateQuestProgress(quest, i/2);
                        }
                        break;
                    case 10 : // 특정 대상에게 아이템 사용 (특정 대상에게 아이템을 사용한 횟수)
                        for (int i = 0; i < quest.questTarget.Count; i+=2) // target1 = entity ID, target2 = item ID
                        {
                            var entityId = quest.questTarget[i];
                            var itemId = quest.questTarget[i + 1];
                            if(target1 == entityId && target2 == itemId)
                                UpdateQuestProgress(quest, i/2);
                        }
                        break;
                    case 11 : // 특정한 장비를 착용하고 사냥 (특정 장비를 착용하고 목표물을 사냥한 횟수)
                        if (target2 != quest.questTarget[0]) break; // target1 = monster ID, target2 = weapon ID
                        for (int i = 1; i < quest.questTarget.Count; i++)
                        {
                            var targetId = quest.questTarget[i];
                            if(target1 == targetId)
                                UpdateQuestProgress(quest, i-1);
                        }
                        break;
                    case 12 : // 특정 대상에게 특정 스킬 사용 (특정 대상에게 스킬을 사용한 횟수)
                        for (int i = 0; i < quest.questTarget.Count; i+=2) // target1 = entity ID, target2 = skill ID
                        {
                            var entityId = quest.questTarget[i];
                            var skillId = quest.questTarget[i + 1];
                            if(target1 == entityId && target2 == skillId)
                                UpdateQuestProgress(quest, i/2);
                        }
                        break;
                }
            }
        }
    }

    private void UpdateQuestProgress(QuestData.TableRecord quest, int targetIndex)
    {   
        var progress = quest.QuestProgress[targetIndex];
        var complete = quest.questCondition[targetIndex];
#if UNITY_EDITOR
        Debug.Log($"{quest.id} quest : {targetIndex}->{progress}/{complete}");
#endif
        if (progress < complete)
        {
            ++quest.QuestProgress[targetIndex];
            UpdateQuestListUI(quest);
        }
        else
        {
            // 해당 목표 초과 달성
        }
    }
    
    private void UpdateNumberOfItem(QuestData.TableRecord quest, int targetIndex, int itemIndex)
    {
//        var num = InventoryManager.GetInstance[itemIndex]._Stack;
//        quest.QuestProgress[targetIndex] = num;
//        UpdateQuestListUI(quest);
    }

    /// <summary>
    /// 입력 백터가 지정된 공간(3D) 안 인지 확인
    /// </summary>
    /// <param name="position">확인할 백터(입력 백터)</param>
    /// <param name="location">지정 공간 위치(중앙값)</param>
    /// <param name="range">지정 공간 범위</param>
    /// <returns></returns>
    private bool checkInLocation(Vector3 position, Vector3 location, Vector3 range)
    {
        return location.x - range.x < position.x && location.x + range.x < position.x
               && location.y - range.y < position.y && location.y + range.y < position.y
               && location.z - range.z < position.z && location.z + range.z < position.z;                                       
    }

    private void changeScene(int sceneId)
    {
        if (sceneId != MogoWorld.thePlayer.sceneId)
        {
            // TODO 맵 이동에 관한 UI 알림
            MogoWorld.thePlayer.sceneId = (ushort)sceneId;
        }
    }
}