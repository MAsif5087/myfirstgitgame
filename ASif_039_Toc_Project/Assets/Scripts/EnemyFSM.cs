using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyFSM : MonoBehaviour
{
  
    public enum ENEMY_STATE {IDLE,MOVING,CHASE,ATTACK,Exit};
    [SerializeField]
    private ENEMY_STATE curentState;

    public ENEMY_STATE CurrentState
    {
        get { return curentState; }
        set { 
            curentState = value;
            StopAllCoroutines();
            switch (curentState)
            {
                case ENEMY_STATE.MOVING:
                    StartCoroutine(EnemyPatrol());
                    break;
                case ENEMY_STATE.IDLE:
                    StartCoroutine(EnemyIdol());
                    break;
                case ENEMY_STATE.Exit:
                    StartCoroutine(EnemyPatrol());
                    break;
                case ENEMY_STATE.CHASE:
                    StartCoroutine(EnemyChase());
                    break;
                case ENEMY_STATE.ATTACK:
                    StartCoroutine(EnemyExit());
                    break;
                    
            }

        
        }
    }

    private CheckMyVision checkMyVision;

    private NavMeshAgent agent = null;

    private Transform playerTransform = null;

    //reference to patrol destination

    private Transform patrolDestoination = null;

    private Health playerHealth = null;

    private float maxDamage =10f;

    private void Awake()
    {
        checkMyVision = GetComponent<CheckMyVision>();
        agent = GetComponent<NavMeshAgent>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        playerTransform = playerHealth.GetComponent<Transform>();

    }

    void Start()
    {
       GameObject[] destinations = GameObject.FindGameObjectsWithTag("Dest");
       patrolDestoination = destinations[Random.Range(0, destinations.Length)]
            .GetComponent<Transform>();
        CurrentState = ENEMY_STATE.MOVING;
        
    }

    public IEnumerator EnemyPatrol()
    {
        while (curentState == ENEMY_STATE.MOVING)
        {
            checkMyVision.sensitivity = CheckMyVision.enumSensitivity.HIGH;
            agent.isStopped=false;
            agent.SetDestination(patrolDestoination.position);
            while (agent.pathPending)
                yield return null;
            if (checkMyVision.targetInSight)
            {
                agent.isStopped = true;
                CurrentState = ENEMY_STATE.CHASE;
                yield break;
            }
            yield break;
        }
        yield break;
    }

    public IEnumerator EnemyChase()
    {
       while(curentState==ENEMY_STATE.CHASE)
        {
            checkMyVision.sensitivity = CheckMyVision.enumSensitivity.LOW;
            agent.isStopped = false;
            agent.SetDestination(checkMyVision.lastknownsighting);
            while(agent.pathPending)
            {
                yield return null;
            }
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.isStopped = true;
                if (!checkMyVision.targetInSight)
                    CurrentState = ENEMY_STATE.MOVING;
                else
                    CurrentState = ENEMY_STATE.ATTACK;

            
            }
            yield return null;
        }
    }

    public IEnumerator EnemyAttack()
    {
        while (curentState == ENEMY_STATE.ATTACK)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);

            while (agent.pathPending)
                yield return null;
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                CurrentState = ENEMY_STATE.CHASE;
            }
            else {
                playerHealth.HealthPoints -= maxDamage * Time.deltaTime; 
               }

            yield return null;
        }
        yield break;
    }

    public IEnumerator EnemyIdol()
    {
        yield break;
       
    }

    public IEnumerator EnemyExit()
    {
        yield break;
    }
        void Update()
    {
        
    }
}
