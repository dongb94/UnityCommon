
using System.Collections.Generic;
using UnityEngine;

public class ClientQuestServer
{
    public static void ClientInitQuest()
    {
        Debug.Log("____INIT____");
        
        var init = new QuestInitializePacket()
        {
            BaseQuest = new byte[2],
            ClearedQuest = new byte[0],
            InprogressQuest = new byte[0],
            QuestProgress = new byte[0][]
        };
        init.BaseQuest[0] = 1;
        init.BaseQuest[1] = 2;

        InitQuest(init);
    }
    
    public static void InitQuest(QuestInitializePacket init)
    {
        List<int> baseQuestNumList = new List<int>();
        List<int> clearedQuestNumList = new List<int>();
        List<int> inProgressQuestNumList = new List<int>();
        List<List<int>> questProgress = new List<List<int>>();

        for (int i = 0; i < init.BaseQuest.Length; i++)
        {
            baseQuestNumList.Add(init.BaseQuest[i]);
        }
        
        for (int i = 0; i < init.ClearedQuest.Length; i++)
        {
            clearedQuestNumList.Add(init.ClearedQuest[i]);
        }
        
        for (int i = 0; i < init.InprogressQuest.Length; i++)
        {
            clearedQuestNumList.Add(init.InprogressQuest[i]);
        }
        
        for (int i = 0; i < init.QuestProgress.Length; i++)
        {
            List<int> progress = new List<int>();
            for (int j = 0; j < init.QuestProgress[i].Length; j++)
            {
                progress.Add(init.QuestProgress[i][j]);
            }
            questProgress.Add(progress);
        }
        
        MultiQuestSystem.Instance.InitQuestInfo(baseQuestNumList,clearedQuestNumList,inProgressQuestNumList,questProgress);
    }
}

public struct QuestInitializePacket
{
    public byte[] BaseQuest;
    public byte[] ClearedQuest;
    public byte[] InprogressQuest;
    public byte[][] QuestProgress;
}