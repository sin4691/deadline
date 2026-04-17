using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private float chaseDist = 30f;
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float idleWaitTime = 4f;
    [SerializeField] private float arriveDistance = 0.3f;

    private NavMeshAgent agent;
    private PlayerMovement player;
    private Vector3 lastNoisePosition;
    private float waitTimer = 0f;
    private Animator anim;

    enum State { Chase, Wander, Idle, Investigate }
    State state = State.Chase;

    void Start()
    {
        agent   = GetComponent<NavMeshAgent>();
        anim    = GetComponent<Animator>();
        player  = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>(); 
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        bool hearSound = dist < player.noiseRadius; // 소리를 들음!
        anim.SetFloat("Speed", agent.velocity.magnitude);
        switch (state)
        {
            case State.Chase:
                agent.SetDestination(player.transform.position);
                if (dist < chaseDist)
                    ChangeState(State.Idle);
                break;

            case State.Wander:
                if (hearSound)
                {
                    ChangeState(State.Investigate);
                    break;
                }
                if (agent.remainingDistance < arriveDistance && !agent.pathPending)
                    SetWander();
                break;

            case State.Investigate:
                if (hearSound)
                {
                    lastNoisePosition = player.transform.position;
                    agent.SetDestination(lastNoisePosition);
                }
                if (Vector3.Distance(transform.position, lastNoisePosition) < arriveDistance)
                    ChangeState(State.Idle);
                break;

            case State.Idle:
                if (hearSound)
                    ChangeState(State.Investigate);
                else
                {
                    waitTimer -= Time.deltaTime;
                    if (waitTimer <= 0f)
                        ChangeState(State.Wander);
                }
                break;
        }
    }
    void ChangeState(State next)
    {
        state = next;
        if(next == State.Idle)
            waitTimer = idleWaitTime;
    }
    void SetWander()
    {
        Vector3 randomDir = Random.insideUnitSphere * wanderRadius + transform.position;
        if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }
}