using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject gun;
    [SerializeField] private Transform enemyBulletSpawn;
    [SerializeField] private float searchDuration;
    [SerializeField] private float timer;
    [SerializeField] private float generalRadius;
    [SerializeField] private int enemyID;
    [SerializeField] private bool isMultiAgent;

    private NavMeshAgent navAgent;
    private PlayerControls player;
    private Transform playerPosition;
    private EnemyFOV enemyView;
    private MultiAgentManager multiAgentManager;

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

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        enemyView = GetComponent<EnemyFOV>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        multiAgentManager = FindAnyObjectByType<MultiAgentManager>();
        waypointsCount = waypoints.Count;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMultiAgent)
        {
            MultiAgentBehaviours();
        }
        else if (!isMultiAgent)
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
        GameObject newProjectile = Instantiate(projectile, enemyBulletSpawn.position, enemyBulletSpawn.rotation);
        newProjectile.GetComponent<Rigidbody>().velocity = transform.forward * 25;
        Destroy(newProjectile, 5f);
    }

    //this is how the other agents will get notified of the player's general area
    public void GetAlerted(Vector3 lastPlayerPos)
    {
        isAlerted = true;
        lastPlayerLocation = lastPlayerPos;
    }
}
