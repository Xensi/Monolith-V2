using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyBehavior : MonoBehaviour
{

    public enum UnseenPlayerPositionBehaviors
    {
        None,
        Wander,
        Patrol
    }
    public enum SeenPlayerPositionBehaviors
    {
        None,
        Chase,
        KeepDistance,
        ReportPlayer
    }
    public enum BlindedBehavior
    {
        None,
        Berserk
    }
    public UnseenPlayerPositionBehaviors unseenBehavior;
    public SeenPlayerPositionBehaviors seenBehavior;
    public IAstarAI pathfinder;

    public Transform targetToFollow;
    public EnemySensor sensor;

    public Vector3 lastSeenPlayerPosition;
    public float timeUntilGiveUpOnSearch = 10;
    public bool justSawPlayer = false;

    public bool canSee = true;

    void OnEnable()
    {
        pathfinder = GetComponent<IAstarAI>(); 
        if (pathfinder != null) pathfinder.onSearchPath += Update;
    } 
    void OnDisable()
    {
        if (pathfinder != null) pathfinder.onSearchPath -= Update;
    } 
    public Vector3 randomPosition;
    public void Start()
    {
        //InvokeRepeating();
    }
    public void ImpairVision()
    {
        canSee = false;
    }
    public void DebuffSpeed(float debuff)
    {
        pathfinder.maxSpeed -= debuff;
    }
    void Update()
    {
        if (!sensor.IsInSight(PlayerHealth.Instance.gameObject) || !canSee) //can't see player
        {
            if (!justSawPlayer)
            {
                switch (unseenBehavior)
                {
                    case UnseenPlayerPositionBehaviors.None:
                        break;
                    case UnseenPlayerPositionBehaviors.Wander:
                        if (pathfinder.reachedEndOfPath || !pathfinder.hasPath) //if we're at dest or no path
                        { 
                            randomPosition = EnemySpawner.Instance.RandomPoint();
                        }
                        pathfinder.destination = randomPosition;  
                        break;
                    case UnseenPlayerPositionBehaviors.Patrol:
                        break;
                    default:
                        break;
                }
            }
            else  //go to last seen position
            { 
                pathfinder.destination = lastSeenPlayerPosition;
                if (pathfinder.reachedEndOfPath)
                {
                    justSawPlayer = false;
                }
            } 
        }
        else
        {
            lastSeenPlayerPosition = PlayerHealth.Instance.transform.position;
            justSawPlayer = true;
            //searchTimerAllowedToStart = true;
            switch (seenBehavior)
            {
                case SeenPlayerPositionBehaviors.None:
                    break;
                case SeenPlayerPositionBehaviors.Chase:
                    pathfinder.destination = PlayerHealth.Instance.transform.position;
                    break;
                case SeenPlayerPositionBehaviors.KeepDistance:
                    break;
                case SeenPlayerPositionBehaviors.ReportPlayer:
                    break;
                default:
                    break;
            }
        }
    }
}
