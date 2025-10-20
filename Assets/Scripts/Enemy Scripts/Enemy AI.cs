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
    private Vector3 destination;
    private float distance;
    private bool reversePath = false;
    private int curWaypoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Patrol();
    }

    public void Patrol()
    {
        distance = Vector3.Distance(gameObject.transform.position, waypoints[curWaypoint].position);
        destination = playerPosition.position;
        float playerDistance = Vector3.Distance(gameObject.transform.position, destination);

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
}
