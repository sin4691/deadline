using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float chaseRange = 20f;  // 이 거리 안에 들어오면 추적
    [SerializeField] private float stopDistance = 1.5f; // 플레이어 바로 앞 멈춤

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        // player 슬롯 비어있으면 자동으로 찾기
        if (player == null)
            player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < chaseRange && dist > stopDistance)
        {
            agent.SetDestination(player.position);
            animator.SetBool("isWalking", true);
        }
        else
        {
            agent.ResetPath();
            animator.SetBool("isWalking", false);
        }
    }
}