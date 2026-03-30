using UnityEngine;
using UnityEngine.AI;

public class SpiderStateController : MonoBehaviour
{
    [Header("References")]
    public SpiderAlertSystem alertSystem;
    public SpiderVisionDetector visionDetector;
    public NavMeshAgent agent;

    [Header("Player Auto Find")]
    [SerializeField] private string playerTag = "Player";

    private Transform player;
    private PlayerRespawn playerRespawn;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 1.5f;
    public float patrolReachThreshold = 0.35f;
    public float patrolWaitTime = 1.2f;

    [Header("Yellow Investigate")]
    public float yellowSpeed = 2.2f;
    public float investigateReachThreshold = 0.4f;
    public float investigateWaitTime = 1.5f;

    [Header("Red Chase")]
    public float chaseSpeed = 3.5f;
    public float catchDistance = 1.2f;

    [Header("Debug")]
    public Vector3 lastKnownPlayerPosition;
    public bool hasStoredYellowTarget = false;
    public int currentPatrolIndex = 0;

    [Header("Runtime Debug")]
    [SerializeField] private bool playerFound = false;
    [SerializeField] private bool playerRespawnFound = false;

    private SpiderAlertSystem.AlertState previousState;
    private float patrolWaitTimer = 0f;
    private float investigateWaitTimer = 0f;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        FindPlayerReferences();

        if (alertSystem != null)
            previousState = alertSystem.currentState;

        TryStartPatrol();
    }

    void Update()
    {
        // 如果玩家是跨场景后来才出现，这里会持续补找
        if (player == null || playerRespawn == null)
        {
            FindPlayerReferences();
        }

        playerFound = (player != null);
        playerRespawnFound = (playerRespawn != null);

        if (alertSystem == null || agent == null) return;

        SpiderAlertSystem.AlertState currentState = alertSystem.currentState;

        HandleStateTransitions(currentState);
        HandleMovementByState(currentState);
        CheckCatchPlayer(currentState);

        previousState = currentState;
    }

    void FindPlayerReferences()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObj != null)
        {
            player = playerObj.transform;

            if (playerRespawn == null)
                playerRespawn = playerObj.GetComponent<PlayerRespawn>();
        }
    }

    void HandleStateTransitions(SpiderAlertSystem.AlertState currentState)
    {
        // 第一次进入黄色：记录那一刻玩家位置
        if (currentState == SpiderAlertSystem.AlertState.Yellow &&
            previousState != SpiderAlertSystem.AlertState.Yellow)
        {
            if (player != null)
            {
                lastKnownPlayerPosition = player.position;
                hasStoredYellowTarget = true;
                investigateWaitTimer = 0f;
            }
        }

        // 切回绿色时，清理黄色调查状态
        if (currentState == SpiderAlertSystem.AlertState.Green &&
            previousState != SpiderAlertSystem.AlertState.Green)
        {
            hasStoredYellowTarget = false;
            investigateWaitTimer = 0f;
            patrolWaitTimer = 0f;

            // 回绿时重新进入巡逻
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                if (!agent.pathPending && (!agent.hasPath || agent.remainingDistance <= patrolReachThreshold))
                {
                    SetPatrolDestination(currentPatrolIndex);
                }
            }
        }
    }

    void HandleMovementByState(SpiderAlertSystem.AlertState currentState)
    {
        switch (currentState)
        {
            case SpiderAlertSystem.AlertState.Green:
                HandlePatrol();
                break;

            case SpiderAlertSystem.AlertState.Yellow:
                HandleYellowInvestigate();
                break;

            case SpiderAlertSystem.AlertState.Red:
                HandleRedChase();
                break;
        }
    }

    void HandlePatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        agent.speed = patrolSpeed;

        if (!agent.hasPath)
        {
            SetPatrolDestination(currentPatrolIndex);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= patrolReachThreshold)
        {
            patrolWaitTimer += Time.deltaTime;

            if (patrolWaitTimer >= patrolWaitTime)
            {
                patrolWaitTimer = 0f;
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                SetPatrolDestination(currentPatrolIndex);
            }
        }
        else
        {
            patrolWaitTimer = 0f;
        }
    }

    void HandleYellowInvestigate()
    {
        if (!hasStoredYellowTarget) return;

        agent.speed = yellowSpeed;
        agent.SetDestination(lastKnownPlayerPosition);

        if (!agent.pathPending && agent.remainingDistance <= investigateReachThreshold)
        {
            investigateWaitTimer += Time.deltaTime;

            // 到点后先短暂停一下
            if (investigateWaitTimer >= investigateWaitTime)
            {
                // 状态仍然交给 alertSystem 自己控制
            }
        }
        else
        {
            investigateWaitTimer = 0f;
        }
    }

    void HandleRedChase()
    {
        if (player == null) return;

        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    void CheckCatchPlayer(SpiderAlertSystem.AlertState currentState)
    {
        if (currentState != SpiderAlertSystem.AlertState.Red) return;
        if (player == null || playerRespawn == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= catchDistance)
        {
            playerRespawn.Respawn();
            ResetSpiderToPatrol();
        }
    }

    void ResetSpiderToPatrol()
    {
        if (alertSystem != null)
        {
            alertSystem.alertValue = 0f;
        }

        hasStoredYellowTarget = false;
        investigateWaitTimer = 0f;
        patrolWaitTimer = 0f;

        if (agent != null)
        {
            agent.ResetPath();
        }

        TryStartPatrol();
    }

    void TryStartPatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        if (agent == null) return;

        currentPatrolIndex = Mathf.Clamp(currentPatrolIndex, 0, patrolPoints.Length - 1);
        SetPatrolDestination(currentPatrolIndex);
    }

    void SetPatrolDestination(int index)
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;
        if (index < 0 || index >= patrolPoints.Length) return;

        agent.SetDestination(patrolPoints[index].position);
    }
}