using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float searchDuration;

    private NavMeshAgent navAgent;
    private PlayerControls player;
    private Transform playerPosition;
    private EnemyFOV enemyView;

    private Vector3 destination;
    private enum Behaviours { Patrol, Chase, Search };
    private Behaviours aiState = Behaviours.Patrol;

    private float distance;
    private bool reversePath = false;
    private int curWaypoint = 0;
    private int waypointsCount;
    private float searchTimer;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        enemyView = GetComponent<EnemyFOV>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        waypointsCount = waypoints.Count;
    }

    // Update is called once per frame
    void Update()
    {
        switch (aiState)
        {
            case Behaviours.Patrol:
                Patrol();

                if (enemyView.IsPlayerVisible())
                {
                    aiState = Behaviours.Chase;
                }

                break;
            case Behaviours.Chase:
                if (enemyView.IsPlayerVisible())
                {
                    ChaseTarget();
                }
                else if (!enemyView.IsPlayerVisible())
                {
                    searchTimer = searchDuration;
                    navAgent.SetDestination(destination);
                    aiState = Behaviours.Search;
                }

                break;
            case Behaviours.Search:
                SearchTarget();

                if (enemyView.IsPlayerVisible())
                {
                    aiState = Behaviours.Chase;
                }
                else if(searchTimer <= 0)
                {
                    aiState= Behaviours.Patrol;
                }

                break;
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
                if(curWaypoint >= waypoints.Count - 1)
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

    //after player escapes sight, they'll go towards where the player was last seen and rotate until timer is done
    public void SearchTarget()
    {
        if(navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            searchTimer -= Time.deltaTime;
            this.transform.Rotate(0, 80 * Time.deltaTime, 0);
        }
    }
}
