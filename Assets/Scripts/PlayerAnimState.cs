using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAnimState
{
    
    public class Idle : State<PlayerManager>
    {

        public override void Enter(PlayerManager player)
        {
            player.animator.Play("Idle");
        }

        public override void Execute(PlayerManager player)
        {
            if (player.IsMove)
            {
                player.ChangeState(PlayerStates.Run);
            }
        }

        public override void Exit(PlayerManager player)
        {
            
        }
    }

    public class Run : State<PlayerManager>
    {
        public override void Enter(PlayerManager player)
        {
            player.animator.Play("Run");
            Debug.Log("Run....current state " + player.CurrentState);
        }

        public override void Execute(PlayerManager player)
        {
            if (!player.IsMove)
            {
                player.ChangeState(PlayerStates.Idle);
            }
        }

        public override void Exit(PlayerManager player)
        {

        }
    }

    public class Attack : State<PlayerManager>
    {
        public override void Enter(PlayerManager player)
        {
            player.animator.Play("Attack");
        }

        public override void Execute(PlayerManager player)
        {
            if (player.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") 
                &&  player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                
            {
                player.IsAttack = false;
                Debug.Log("IsAttack false");
                player.ChangeState(PlayerStates.Idle); 
            }
        }

        public override void Exit(PlayerManager player)
        {
            
        }
    }

    public class Dodge : State<PlayerManager>
    {
        public override void Enter(PlayerManager player)
        {
            //
        }

        public override void Execute(PlayerManager player)
        {

        }

        public override void Exit(PlayerManager player)
        {

        }
    }

    public class UseItem : State<PlayerManager>
    {
        public override void Enter(PlayerManager player)
        {
            //
        }

        public override void Execute(PlayerManager player)
        {

        }

        public override void Exit(PlayerManager player)
        {

        }
    }

    public class StateGlobal : State<PlayerManager>
    {
        public override void Enter(PlayerManager player)
        {
            
        }
        public override void Execute(PlayerManager player)
        {
            if (player.CurrentState == PlayerStates.Attack)
            {
                return;
            }
            if (player.IsAttack)
            {
                player.ChangeState(PlayerStates.Attack);
            }
        }
        public override void Exit(PlayerManager player)
        {
            
        }
    }

}
