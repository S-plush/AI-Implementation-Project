using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;

    private NavMeshAgent navAgent;
    private PlayerControls player;
    private Transform playerPosition;
    private EnemyFOV enemyView;
    private Vector3 destination;
    private float distance;
    private bool reversePath = false;
    private int curWaypoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        enemyView = GetComponent<EnemyFOV>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (waypoints.Length > 0 && !enemyView.IsPlayerVisible())
        {
            Patrol();
        }
        else if (enemyView.IsPlayerVisible())
        {
            ChaseTarget();
        }
    }

    //this has the enemy follow waypoints
    public void Patrol()
    {
        distance = Vector3.Distance(gameObject.transform.position, waypoints[curWaypoint].position);

        if(distance > 2f)
        {
            destination = waypoints[curWaypoint].position;
            navAgent.SetDestination(destination);
        }
        else
        {
            if (reversePath)
            {
                if(curWaypoint <= 0)
                {
                    reversePath = false;
                }
                else
                {
                    curWaypoint--;
                    destination = waypoints[curWaypoint].position;
                }
            }
            else
            {
                if(curWaypoint >= waypoints.Length - 1)
                {
                    reversePath = true;
                }
                else
                {
                    curWaypoint++;
                    destination = waypoints[curWaypoint].position;
                }
            }
        }
    }

    //upon seeing the player, the enemy will then chase after the player
    public void ChaseTarget()
    {
        destination = playerPosition.position;
        navAgent.SetDestination(destination);
        distance = Vector3.Distance(gameObject.transform.position, destination);
    }
}
