using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private float FindPlayerDist = 30f;
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float idleWaitTime = 4f;
    [SerializeField] private float arriveDistance = 0.3f;
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float ChasePlayerSpeed = 6.5f;
    [SerializeField] private AudioClip screamSound;
    private NavMeshAgent agent;
    private PlayerMovement player;
    private Vector3 lastNoisePosition;
    private float waitTimer = 0f;
    private Animator anim;
    private AudioSource audioSource;
    private Transform[] waypoints;

    enum State { FindPlayer, Wander, Idle, ChasePlayer }
    State state = State.FindPlayer;

    void Start()
    {
        agent   = GetComponent<NavMeshAgent>();
        anim    = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player  = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        GameObject[] points = GameObject.FindGameObjectsWithTag("ItemPoint");
        waypoints = System.Array.ConvertAll(points, p => p.transform);
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.transform.position);
        bool hearSound = dist < player.noiseRadius; // 소리를 들음!
        anim.SetFloat("Speed", agent.velocity.magnitude);
        switch (state)
        {
            case State.FindPlayer:
                agent.SetDestination(player.transform.position);
                if (dist < FindPlayerDist)
                    ChangeState(State.Idle);
                break;

            case State.Wander:
                if (hearSound)
                {
                    ChangeState(State.ChasePlayer);
                    break;
                }
                if (agent.remainingDistance < arriveDistance && !agent.pathPending)
                    SetWander();
                break;

            case State.ChasePlayer:
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
                    ChangeState(State.ChasePlayer);
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
        // 아무 상태에서 추적 상태로 변할때 Scream
        if (next == State.ChasePlayer)
        {
            StartCoroutine(PlayScream());
        }
        else
            agent.speed = walkSpeed;
        // Idle 타이머 초기화
        state = next;
        if(next == State.Idle)
            waitTimer = idleWaitTime;
           
        
    }
    void SetWander()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[Random.Range(0, waypoints.Length)];

        if (NavMesh.SamplePosition(target.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            ChangeState(State.Idle);
    }
    IEnumerator PlayScream()
    {
        anim.SetTrigger("Scream");
        agent.speed = 0.1f;
        Debug.Log("멈춤!");
        yield return new WaitForSeconds(1f);
        Debug.Log("멈춤끝!");
        agent.speed = ChasePlayerSpeed;
        if (audioSource != null && screamSound != null)
        {
            //audioSource.PlayOneShot(screamSound);
            
        }
    }
}