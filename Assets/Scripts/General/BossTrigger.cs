using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private EnemyBoss boss;

    bool triggered;

    [SerializeField] private CinemachineVirtualCamera mainCam;
    [SerializeField] private CinemachineVirtualCamera bossCam;

    [SerializeField] private GameObject bossHealthBar;

    [SerializeField] private GameObject wall;

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (triggered)
            return;
 
        triggered = true;

        if (collision.transform.tag == "Player")
        {
            boss.ActivateBoss();
            mainCam.Priority = 0;
            bossCam.Priority = 20;

            bossHealthBar.SetActive(true);
            wall.SetActive(true);

            AudioManager.Instance.PlayMusic("music2", 0.5f);
        }

        
    }
}
