using UnityEngine;
using UnityEngine.AI;

public class MonsterAI : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float chaseRange = 20f; 
    [SerializeField] private float stopDistance = 1.5f; 

    private NavMeshAgent agent;
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

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