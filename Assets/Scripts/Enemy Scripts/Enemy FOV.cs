using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask friendlyMask;
    [SerializeField] private LayerMask obstacleMask;

    [Range(0, 360)] public float angle;
    public float radius;
    public GameObject player;
    public bool playerVisable;
    public bool friendlyVisable;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        Collider[] friendlyRangeChecks = Physics.OverlapSphere(transform.position, radius, friendlyMask);

        if(rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;

            Vector3 directionToTarget = (target.position - transform.position).normalized;

            //this checks to see if the player is within the fov cone
            if(Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                //then when inside fov cone, the enemy detects the player
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    playerVisable = true;
                    friendlyVisable = false;
                }
                else
                {
                    playerVisable = false;
                }
            }
            else
            {
                playerVisable = false;
            }
        }
        else if (playerVisable)
        {
            playerVisable = false;
        }

        if (friendlyRangeChecks.Length != 0)
        {
            friendlyVisable = false;

            for (int i = 0; i < friendlyRangeChecks.Length; i++)
            {
                if (friendlyRangeChecks[i].gameObject == this.gameObject)
                {
                    continue;
                }

                Transform friendly = friendlyRangeChecks[i].transform;
                Vector3 directionToFriendly = (friendly.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToFriendly) < angle / 2)
                {
                    float distanceToFriendly = Vector3.Distance(transform.position, friendly.position);

                    if (!Physics.Raycast(transform.position, directionToFriendly, distanceToFriendly, obstacleMask))
                    {
                        friendlyVisable = true;
                        break;
                    }
                    //else
                    //{
                    //    friendlyVisable = false;
                    //}
                }
                //else
                //{
                //    friendlyVisable = false;
                //}
            }
        }
        else if (friendlyVisable)
        {
            friendlyVisable = false;
        }
    }

    public bool IsPlayerVisible()
    {
        return playerVisable;
    }

    public bool IsFriendlyVisible()
    {
        return friendlyVisable;
    }

    public void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, radius);

        Vector3 viewAngle1 = DirectionFromAngle(transform.eulerAngles.y, -angle / 2);
        Vector3 viewAngle2 = DirectionFromAngle(transform.eulerAngles.y, angle / 2);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + viewAngle1 * radius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngle2 * radius);

        if (playerVisable && player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
