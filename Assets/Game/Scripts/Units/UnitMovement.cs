using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] float chaseRange = 10f;
    [SerializeField] Animator animator;

    #region Server

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        if(target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                agent.SetDestination(target.transform.position);
                animator.SetTrigger("walk");

            }
            else if(agent.hasPath)
            {
                agent.ResetPath();
                animator.ResetTrigger("walk");

            }



            return;
        }
        if (!agent.hasPath) return;
        if (agent.remainingDistance > agent.stoppingDistance) return;
        agent.ResetPath();
    }


    [Server]
    public void ServerMove(Vector3 position)
    {
        targeter.ClearTargets();
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
        animator.SetTrigger("walk");
        agent.SetDestination(hit.position);

    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }

    public override void OnStartServer()
    {
        GameOverHandeler.serverOnGameOver += serverHandelOnGameOver;
    }
    public override void OnStopServer()
    {
        GameOverHandeler.serverOnGameOver -= serverHandelOnGameOver;
    }
    [Server]
    void serverHandelOnGameOver()
    {
        agent.ResetPath();
    }
    #endregion
}
