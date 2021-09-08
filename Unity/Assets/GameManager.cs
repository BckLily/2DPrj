using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject[] bosses;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }



    // Start is called before the first frame update
    void Start()
    {
        GameEvents.currrent.onBossStageStart += SetActiveBoss;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetActiveBoss(int bossIdx)
    {
        if (!bosses[bossIdx].activeSelf)
        {
            bosses[bossIdx].SetActive(true);
        }
    }


}
