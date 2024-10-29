using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerAnimState
{
    
    public class Idle : State<PlayerManager>
    {

        public override void Enter(PlayerManager player)
        {
            player.PlayerAnimator.Play("Idle");
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
            player.PlayerAnimator.Play("Run");
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
            player.PlayerAnimator.Play("Attack");
        }

        public override void Execute(PlayerManager player)
        {
            if (player.PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack")  
                &&  player.PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            //���� �ִϸ��̼� ������ �������� Idle�� ���ư�
            {
                player.IsAttack = false;
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
