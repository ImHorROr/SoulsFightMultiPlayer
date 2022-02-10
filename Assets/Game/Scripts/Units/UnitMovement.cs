using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] float chaseRange = 10f;
    #region Server

    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();
        if(target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                agent.SetDestination(  target.transform.position);
            }
            else if(agent.hasPath)
            {
                agent.ResetPath();

            }



            return;
        }
        if (!agent.hasPath) return;
        if (agent.remainingDistance > agent.stoppingDistance) return;
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        targeter.ClearTargets();
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
        
        agent.SetDestination(hit.position);
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
