using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents currrent = null;

    private void Awake()
    {
        if (currrent == null)
        {
            currrent = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    // 문에 다가가면 문이 열리는 이벤트
    public event Action<int> onDoorTriggerEnter;
    public void DoorTriggerEnter(int id)
    {
        if (onDoorTriggerEnter != null)
        {
            onDoorTriggerEnter(id);
        }
    }

    // 문에서 멀어지면 문이 닫히는 이벤트
    public event Action<int> onDoorTriggerExit;
    public void DoorTriggerExit(int id)
    {
        if (onDoorTriggerExit != null)
        {
            onDoorTriggerExit(id);
        }
    }


    // 보스전 시작 이벤트
    public event Action<int> onBossStageStart;
    public void BossStageStart(int idx)
    {
        if (onBossStageStart != null)
        {
            onBossStageStart(idx);
        }
    }


    public event Action<int> onBackgroundChange;
    public void BackgroundChange(int idx)
    {
        if (onBackgroundChange != null)
        {
            onBackgroundChange(idx);
        }
    }



}
