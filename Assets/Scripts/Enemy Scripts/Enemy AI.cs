using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private List<Transform> doorWaypoints;
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject gun;
    [SerializeField] private Transform attackSpawn;
    [SerializeField] private float searchDuration;
    [SerializeField] private float timer;
    [SerializeField] private float generalRadius;
    [SerializeField] private int enemyID;
    [SerializeField] private bool isMultiAgent;
    [SerializeField] private bool isAdvMultiAgent;

    private NavMeshAgent navAgent;
    private PlayerControls player;
    private Transform playerPosition;
    private EnemyFOV enemyView;
    private MultiAgentManager multiAgentManager;
    private SlowTrapManager slowTrapManager;

    private Vector3 destination;
    private Vector3 lastPlayerLocation;
    private Vector3 alertedArea;

    private enum Behaviours { Patrol, Chase, Search, Alerted };
    private Behaviours aiState = Behaviours.Patrol;

    private float distance;
    private float searchTimer;
    private float lastShot;

    private bool reversePath = false;
    [SerializeField] private bool isAlerted = false;
    private bool hasAlerted = false;
    private bool hasAlertedPosition = false;

    private int curWaypoint = 0;
    private int waypointsCount;
    private int trapsPlacedCount;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        enemyView = GetComponent<EnemyFOV>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        multiAgentManager = FindAnyObjectByType<MultiAgentManager>();
        waypointsCount = waypoints.Count;
        slowTrapManager = FindAnyObjectByType<SlowTrapManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMultiAgent && !isAdvMultiAgent)
        {
            MultiAgentBehaviours();
        }
        else if(isAdvMultiAgent && !isMultiAgent)
        {
            AdvMultiAgentBehaviours();
        }
        else if (!isMultiAgent && !isAdvMultiAgent)
        {
            SingeAgentBehaviours();
        }
    }

    public void SingeAgentBehaviours()
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
                    ShootTarget();
                }
                else if (!enemyView.IsPlayerVisible())
                {
                    searchTimer = searchDuration;
                    navAgent.SetDestination(destination);
                    gun.transform.localEulerAngles = new Vector3(0, 0, 0);
                    aiState = Behaviours.Search;
                }

                break;
            case Behaviours.Search:
                SearchTarget();

                if (enemyView.IsPlayerVisible())
                {
                    aiState = Behaviours.Chase;
                }
                else if (searchTimer <= 0)
                {
                    aiState = Behaviours.Patrol;
                }

                break;
        }
    }

    public void MultiAgentBehaviours()
    {
        switch (aiState)
        {
            case Behaviours.Patrol:
                Patrol();

                if (enemyView.IsPlayerVisible())
                {
                    //multiAgentManager.RadioEnemies(enemyID);
                    aiState = Behaviours.Chase;
                }
                else if (isAlerted)
                {
                    aiState = Behaviours.Alerted;
                }

                break;
            case Behaviours.Alerted:
                GoToGeneralTargetArea();
                break;
            case Behaviours.Chase:
                if (enemyView.IsPlayerVisible())
                {
                    ChaseTarget();
                    ShootTarget();
                }
                else if (!enemyView.IsPlayerVisible() /*&& !isAlerted*/)
                {
                    searchTimer = searchDuration;
                    navAgent.SetDestination(destination);
                    gun.transform.localEulerAngles = new Vector3(0, 0, 0);
                    hasAlerted = false;
                    aiState = Behaviours.Search;
                }
                //else if(!enemyView.IsPlayerVisible() && isAlerted)
                //{
                //    GoToGeneralTargetArea();
                //}

                break;
            case Behaviours.Search:
                SearchTarget();

                if (enemyView.IsPlayerVisible())
                {
                    aiState = Behaviours.Chase;
                }
                else if (searchTimer <= 0)
                {
                    aiState = Behaviours.Patrol;
                }

                break;
        }
    }

    public void AdvMultiAgentBehaviours()
    {
        switch (aiState)
        {
            case Behaviours.Patrol:
                TrapperPatrol();

                if (enemyView.IsPlayerVisible())
                {
                    //multiAgentManager.RadioEnemies(enemyID);
                    aiState = Behaviours.Chase;
                }
                else if (isAlerted)
                {
                    aiState = Behaviours.Alerted;
                }

                break;
            case Behaviours.Alerted:
                GoToGeneralTargetArea();
                break;
            case Behaviours.Chase:
                if (enemyView.IsPlayerVisible())
                {
                    ChaseTarget();
                }
                else if (!enemyView.IsPlayerVisible() /*&& !isAlerted*/)
                {
                    searchTimer = searchDuration;
                    navAgent.SetDestination(destination);
                    gun.transform.localEulerAngles = new Vector3(0, 0, 0);
                    hasAlerted = false;
                    aiState = Behaviours.Search;
                }
                //else if(!enemyView.IsPlayerVisible() && isAlerted)
                //{
                //    GoToGeneralTargetArea();
                //}

                break;
            case Behaviours.Search:
                SearchTarget();

                if (enemyView.IsPlayerVisible())
                {
                    aiState = Behaviours.Chase;
                }
                else if (searchTimer <= 0)
                {
                    aiState = Behaviours.Patrol;
                }

                break;
        }
    }

    //this has the enemy follow waypoints
    public void Patrol()
    {
        distance = Vector3.Distance(gameObject.transform.position, waypoints[curWaypoint].position);

        /*
         * add else if where if the enemy isn't able to find the distance to its normal waypoint path due to the doors blocking it
         * then for it to go through the array of door waypoints and find the closest one to it. Which should then make it go
         * back to its original destination of that waypoint that it was going to
         */

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

    //the adv multi-agent enemy will patrol at random points within its radius with a low % chance of them placing a trap
    //unless it has to go to a door to open it then the %age will probably increase when it ge
    public void TrapperPatrol()
    {
        if(navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            int randomNum = UnityEngine.Random.Range(1, 101);

            if(randomNum <= 5)
            {
                PlaceTrap();
            }

            Vector3 randomDestination = UnityEngine.Random.insideUnitSphere * generalRadius;
            randomDestination += navAgent.transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDestination, out hit, generalRadius, NavMesh.AllAreas))
            {
                destination = hit.position;
            }

            navAgent.SetDestination(destination);
        }
    }

    //upon seeing the player, the enemy will then chase after the player
    public void ChaseTarget()
    {
        destination = playerPosition.position;
        lastPlayerLocation = destination;

        //while the method is still used on the single agents, it only works with the multi-agent behaviours
        if (!hasAlerted)
        {
            //this sends this enemy's ID and the player's position of where they were detected to the multi-agent manager 
            multiAgentManager.RadioEnemies(enemyID, lastPlayerLocation);
            hasAlerted = true;
        }

        navAgent.SetDestination(destination);
        distance = Vector3.Distance(gameObject.transform.position, destination);
    }

    //this is for the other multi-agents that were notified of the player's position, but they'll go to a randomized general area of where that is
    public void GoToGeneralTargetArea()
    {
        //this is so they don't keep randomizing the general area they'll go to
        if (!hasAlertedPosition)
        {
            Vector3 generalArea = UnityEngine.Random.insideUnitSphere * generalRadius;
            generalArea += lastPlayerLocation;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(generalArea, out hit, generalRadius, NavMesh.AllAreas))
            {
                alertedArea = hit.position;
            }

            navAgent.SetDestination(alertedArea);
            hasAlertedPosition = true;
        }

        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            searchTimer = searchDuration;
            isAlerted = false;
            hasAlertedPosition = false;
            aiState = Behaviours.Search;
        }
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

    public void ShootTarget()
    {
        gun.transform.LookAt(playerPosition.position);

        if (Time.time - lastShot < timer)
        {
            return;
        }

        lastShot = Time.time;
        GameObject bullet = Instantiate(attack, attackSpawn.position, attackSpawn.rotation);
        bullet.GetComponent<Rigidbody>().velocity = transform.forward * 25;
        Destroy(bullet, 5f);
    }

    public void PlaceTrap()
    {
        if (slowTrapManager.GetTrapsCount() <= 6)
        {
            GameObject trap = Instantiate(attack, attackSpawn.position, attackSpawn.rotation);
        }
    }

    //this is how the other agents will get notified of the player's general area
    public void GetAlerted(Vector3 lastPlayerPos)
    {
        isAlerted = true;
        lastPlayerLocation = lastPlayerPos;
    }

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, generalRadius);
    //}
}
