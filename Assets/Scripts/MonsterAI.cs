using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float InvestigateWaitTime = 3f;

    private NavMeshAgent agent;
    private PlayerMovement player;

    enum State { Idle, Investigate }
    State state = State.Idle;

    private Vector3 lastNoisePosition;  // 마지막으로 소리 난 위치
    private float waitTimer = 0f;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>(); ;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        bool canHear = dist < player.noiseRadius;
        switch (state)
        {
            case State.Idle:
                if (canHear)
                {
                    lastNoisePosition = player.transform.position;
                    agent.SetDestination(lastNoisePosition);
                    state = State.Investigate;
                    waitTimer = InvestigateWaitTime;
                }
                break;
            case State.Investigate:
                if (canHear)
                {
                    lastNoisePosition = player.transform.position;
                    agent.SetDestination(lastNoisePosition);
                    waitTimer = InvestigateWaitTime;
                }
                float distToTarget = Vector3.Distance(transform.position, lastNoisePosition);
                if (distToTarget < stopDistance)
                {
                    waitTimer -= Time.deltaTime;
                    if (waitTimer <= 0f && !canHear)
                    {
                        agent.ResetPath();
                        state = State.Idle;
                    }
                }
                break;
        }
    }
}