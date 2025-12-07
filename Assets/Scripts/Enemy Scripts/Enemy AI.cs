using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private List<GameObject> doorWaypoints;
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
    //[SerializeField] private AlertStatusUI alertStatusUI;

    private Vector3 destination;
    private Vector3 lastPlayerLocation;
    private Vector3 alertedArea;

    [SerializeField] private enum Behaviours { Patrol, Chase, Search, Alerted, Standby };
    [SerializeField] private Behaviours aiState = Behaviours.Patrol;

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
        //alertStatusUI = FindAnyObjectByType<AlertStatusUI>();
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
                else if (enemyView.IsFriendlyVisible())
                {
                    aiState = Behaviours.Standby;
                }
                //alertStatusUI.HiddenStatus();

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
                //alertStatusUI.AlertStatus();

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
                //alertStatusUI.SearchingStatus();

                break;
            case Behaviours.Standby:
                if (enemyView.IsFriendlyVisible())
                {
                    navAgent.SetDestination(gameObject.transform.position);
                }
                else if (!enemyView.IsFriendlyVisible())
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
                    aiState = Behaviours.Chase;
                }
                else if (isAlerted)
                {
                    aiState = Behaviours.Alerted;
                }

                //alertStatusUI.HiddenStatus();

                break;
            case Behaviours.Alerted:
                //alertStatusUI.AlertStatus();

                if (enemyView.IsPlayerVisible())
                {
                    aiState = Behaviours.Chase;
                }
                else if (!enemyView.IsPlayerVisible())
                {
                    GoToGeneralTargetArea();
                }

                break;
            case Behaviours.Chase:

                //alertStatusUI.AlertStatus();
                if (enemyView.IsPlayerVisible())
                {
                    //alertStatusUI.AlertStatus();
                    ChaseTarget();
                    ShootTarget();
                }
                else if (!enemyView.IsPlayerVisible())
                {
                    searchTimer = searchDuration;
                    navAgent.SetDestination(destination);
                    gun.transform.localEulerAngles = new Vector3(0, 0, 0);
                    hasAlerted = false;
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
                //alertStatusUI.SearchingStatus();

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
                    aiState = Behaviours.Chase;
                }
                else if (isAlerted)
                {
                    aiState = Behaviours.Alerted;
                }
                //alertStatusUI.HiddenStatus();

                break;
            case Behaviours.Alerted:
                //alertStatusUI.AlertStatus();

                if (enemyView.IsPlayerVisible())
                {
                    aiState = Behaviours.Chase;
                }
                else if (!enemyView.IsPlayerVisible())
                {
                    GoToGeneralTargetArea();
                }

                break;
            case Behaviours.Chase:
                //alertStatusUI.AlertStatus();
                if (enemyView.IsPlayerVisible())
                {
                    ChaseTarget();
                }
                else if (!enemyView.IsPlayerVisible())
                {
                    searchTimer = searchDuration;
                    navAgent.SetDestination(destination);
                    hasAlerted = false;
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
                //alertStatusUI.SearchingStatus();

                break;
        }
    }

    //this has the enemy follow waypoints
    public void Patrol()
    {
        //alertStatusUI.HiddenStatus();
        destination = waypoints[curWaypoint].position;
        //distance = Vector3.Distance(gameObject.transform.position, waypoints[curWaypoint].position);

        if (!PathReachable(destination))
        {
            GoToClosestDoor();
        }

        distance = Vector3.Distance(gameObject.transform.position, waypoints[curWaypoint].position);

        if (distance > 2.5f)
        {
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
        //alertStatusUI.HiddenStatus();

        if (!PathReachable(destination))
        {
            GoToClosestDoor();
        }

        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            int randomNum = UnityEngine.Random.Range(1, 101);

            if(randomNum <= 3)
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
        //alertStatusUI.AlertStatus();
        destination = playerPosition.position;
        lastPlayerLocation = destination;

        if (!PathReachable(destination))
        {
            GoToClosestDoor();
        }

        if (isAlerted)
        {
            isAlerted = false;
            hasAlertedPosition = false;
        }

        //while the method is still used on the single agents, it only works with the multi-agent behaviours
        if (!hasAlerted && (isAdvMultiAgent || isMultiAgent))
        {
            //this sends this enemy's ID and the player's position of where they were detected to the multi-agent manager 
            multiAgentManager.RadioEnemies(enemyID, lastPlayerLocation);
            hasAlerted = true;
        }

        gameObject.transform.LookAt(destination);
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

            if (!PathReachable(alertedArea))
            {
                GoToClosestDoor();
            }

            navAgent.SetDestination(alertedArea);
            hasAlertedPosition = true;
        }

        if (navAgent.remainingDistance <= navAgent.stoppingDistance)
        {
            if (isAdvMultiAgent)
            {
                int randomNum = UnityEngine.Random.Range(1, 101);

                if (randomNum <= 95)
                {
                    PlaceTrap();
                }
            }

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
            //alertStatusUI.SearchingStatus();
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

    public bool PathReachable(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();

        if (!NavMesh.CalculatePath(transform.position, destination, navAgent.areaMask, path) || path.status != NavMeshPathStatus.PathComplete)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void GoToClosestDoor()
    {
        float closestDoorDistance = float.MaxValue;
        NavMeshPath path = new NavMeshPath();
        NavMeshPath shortestPath = new NavMeshPath();

        for (int i = 0; i < doorWaypoints.Count; i++)
        {
            if (doorWaypoints[i] == null || doorWaypoints[i].activeInHierarchy == false)
            {
                continue;
            }

            path = new NavMeshPath();

            if (NavMesh.CalculatePath(transform.position, doorWaypoints[i].transform.position, navAgent.areaMask, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                float distance = 0f;

                for (int j = 1; j < path.corners.Length; j++)
                {
                    distance += Vector3.Distance(path.corners[j - 1], path.corners[j]);
                }

                if (distance < closestDoorDistance)
                {
                    closestDoorDistance = distance;
                    shortestPath = path;
                }
            }
        }

        if (shortestPath != null)
        {
            navAgent.SetPath(shortestPath);
            return;
        }

        if (isAdvMultiAgent)
        {
            int randomNum = UnityEngine.Random.Range(1, 101);

            if (randomNum <= 50)
            {
                PlaceTrap();
            }
        }
    }

    //this is how the other agents will get notified of the player's general area
    public void GetAlerted(Vector3 lastPlayerPos)
    {
        isAlerted = true;
        lastPlayerLocation = lastPlayerPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, generalRadius);
    }
}
