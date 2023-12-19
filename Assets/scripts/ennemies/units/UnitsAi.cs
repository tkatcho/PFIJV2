using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class UnitsAi : MonoBehaviour
{
    private NavMeshAgent agent;
    private Node topNode;
    private Animator animator;

    public Transform mainTarget;
    public LayerMask enemyLayer;
    public LayerMask buildingLayer;
    public float detectionRadius = 10f;
    public Transform currentTarget;
    public int Damage;

    Transform target = null;
    HealthComponent targetHealth;
    


    public float attackRange = 5f;
    public float attackRate = 1f;
    private float lastAttackTime;

    void Start()
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag("Flag");
        if (gameObject.tag.Contains("team"))
            targetObject = GameObject.FindGameObjectWithTag("EFlag");

        mainTarget = targetObject.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (target == null)
        {
            target = FindPriorityTarget();
            targetHealth = target.GetComponent<HealthComponent>();

        }
        if (target != null)
        {
            MoveTowardsTarget(target);

            if (Vector3.Distance(transform.position, target.position) <= attackRange)
            {
                if (Time.time > lastAttackTime + 1f / attackRate)
                {
                    AttackTarget();
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    void MoveTowardsTarget(Transform target)
    {
        agent.SetDestination(target.position);
    }

    void AttackTarget()
    {
        if (target.tag.Contains("Flag"))
        {
            Debug.Log("end game");
            SceneManager.LoadScene(1);
            return;
        }
        bool isalive = targetHealth.TakeDamage(Damage);
        if (!isalive)
            target = null;

    }

    Transform FindPriorityTarget()
    {
        Transform enemyTarget = UnitsTargetDetector.GetNearestTarget(transform, detectionRadius, enemyLayer);
        if (enemyTarget != null)
            return enemyTarget;

        Transform buildingTarget = BuildingDetector.GetNearestBuilding(transform, detectionRadius, buildingLayer);
        if (buildingTarget != null)
            return buildingTarget;


        return mainTarget;
    }
}
