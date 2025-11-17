using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiAgentManager : MonoBehaviour
{
    [SerializeField] private List<EnemyAI> multiAgentEnemies = new List<EnemyAI>();

    public void RadioEnemies(int enemyID, Vector3 lastPlayerpos)
    {
        for (int i = 0; i < multiAgentEnemies.Count; i++)
        {
            //this is to prevent the agent that found the player from alerting itself
            if(enemyID != i)
            {
                multiAgentEnemies[i].GetAlerted(lastPlayerpos);
            }
        }
    }
}
