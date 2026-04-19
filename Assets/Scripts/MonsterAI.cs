using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float walkFootstepInterval = 0.5f;
    [SerializeField] private float runFootstepInterval = 0.25f;
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float idleWaitTime = 4f;
    [SerializeField] private float arriveDistance = 0.3f;
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float chasePlayerSpeed = 6.5f;
    [SerializeField] private AudioClip screamSound;

    private float footstepTimer = 0f;
    private float waitTimer = 0f;
    private NavMeshAgent agent;
    private PlayerMovement player;
    private Vector3 lastNoisePosition;
    private Animator anim;
    private AudioSource audioSource;
    private Transform[] waypoints;

    private bool isScreaming = false;
    private bool navMeshBlocked = false;

    enum State { Wander, Idle, ChasePlayer, Blocked }
    State state = State.Wander;

    void Start()
    {
        agent   = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = arriveDistance;
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player  = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        GameObject[] points = GameObject.FindGameObjectsWithTag("MonsterMovingPoint");
        waypoints = System.Array.ConvertAll(points, p => p.transform);
        SetWander();
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        bool hearSound = dist < player.noiseRadius; // 소리를 들음!
        anim.SetFloat("Speed", agent.velocity.magnitude);
        switch (state)
        {
            case State.Wander:
                if (hearSound)  {ChangeState(State.ChasePlayer); break; }
                if (!agent.pathPending&&agent.remainingDistance < arriveDistance)
                    SetWander();
                break;

            case State.ChasePlayer:
                if (hearSound)
                    TrySetChaseDestination(player.transform.position);

                if (!isScreaming && !agent.pathPending && agent.remainingDistance < arriveDistance)
                {
                    if (navMeshBlocked)
                        ChangeState(State.Blocked); // 막힌 케이스
                    else
                        ChangeState(State.Idle);    // 정상 도착 케이스
                }
                break;

            case State.Idle:
                if (hearSound) { ChangeState(State.ChasePlayer); break; }
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f) ChangeState(State.Wander);
                break;

            case State.Blocked:
                break;
        }
        HandleFootsteps();
    }
    void ChangeState(State next)
    {
        if (state == next) return;
        State prev = state;
        state = next;

        switch (next)
        {
            case State.ChasePlayer:
                if (prev != State.ChasePlayer && !isScreaming)
                {
                    // 새로 추적 시작할 때만 스크림
                    StartCoroutine(ScreamThenChase());
                }
                else if (prev != State.ChasePlayer)
                {
                    // 이미 스크리밍 중이면 스크림 없이 바로 속도만 설정
                    agent.speed = chasePlayerSpeed;
                }
                break;

            case State.Idle:
                waitTimer = idleWaitTime;
                agent.speed = walkSpeed;
                agent.ResetPath();
                break;

            case State.Wander:
                agent.speed = walkSpeed;
                SetWander();
                break;

            case State.Blocked:
                agent.speed = walkSpeed;
                agent.ResetPath();
                if (!isScreaming)
                    StartCoroutine(BlockedScream());
                break;
        }
    }
    bool TrySetChaseDestination(Vector3 targetPos)
    {
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            lastNoisePosition = hit.position;
            agent.SetDestination(lastNoisePosition);
            navMeshBlocked = false;
            return true;
        }
        navMeshBlocked = true;
        return false;
    }
    void SetWander()
    {
        if (waypoints == null || waypoints.Length == 0) { ChangeState(State.Idle); return; }

        Transform target = waypoints[Random.Range(0, waypoints.Length)];

        if (NavMesh.SamplePosition(target.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            ChangeState(State.Idle);
    }
    IEnumerator BlockedScream()
    {
        isScreaming = true;
        agent.isStopped = true;

        anim.SetTrigger("Scream");
        if (audioSource != null && screamSound != null)
            audioSource.PlayOneShot(screamSound);

        yield return new WaitForSeconds(1f);

        isScreaming = false;
        agent.isStopped = false;
        navMeshBlocked = false;

        ChangeState(State.Wander);
    }
    IEnumerator ScreamThenChase()
    {
        isScreaming = true;
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.isStopped = true;

        anim.SetTrigger("Scream");
        if (audioSource != null && screamSound != null)
            audioSource.PlayOneShot(screamSound);
        yield return new WaitForSeconds(1f);

        isScreaming = false;
        agent.speed = chasePlayerSpeed;
        agent.isStopped = false;
        if (state == State.ChasePlayer)
        {
            TrySetChaseDestination(player.transform.position);
        }
    }
    void HandleFootsteps()
    {
        float speed = agent.velocity.magnitude;

        if (speed < 0.1f || isScreaming)
        {
            footstepTimer = 0f;
            return;
        }

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            if (footstepSounds != null && footstepSounds.Length > 0)
            {
                audioSource.clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
                audioSource.Play();
            }

            footstepTimer = speed >= chasePlayerSpeed * 0.8f ? runFootstepInterval : walkFootstepInterval;
        }
    }
}