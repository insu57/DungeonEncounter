
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
            if (player.PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack")  // 공격 애니메이션 체크
                &&  player.PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) //애니메이션 종료 체크
            //Attack Animation End, State return to Idle. 공격 애니메이션 종료 시 상태종료 Idle로 돌아감
            {
                player.IsAttack = false;
                player.ChangeState(PlayerStates.Idle); 
            }
            //if(player.IsDodge)
                //player.PlayerAnimator.Stop
        }

        public override void Exit(PlayerManager player)
        {
            
        }
    }

    public class Dodge : State<PlayerManager>
    {
        public override void Enter(PlayerManager player)
        {
            player.PlayerAnimator.Play("Run");
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

    public class Damaged : State<PlayerManager>
    {
        public override void Enter(PlayerManager player)
        {
            player.PlayerAnimator.Play("Damaged");
        }

        public override void Execute(PlayerManager player)
        {
            if (!player.WasDamaged)
            {
                player.ChangeState(PlayerStates.Idle);
            }
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
            /*if (player.CurrentState != PlayerStates.Dodge)
            {
                if (player.IsDodge)
                {
                    if (player.IsAttack)
                        player.IsAttack = false;
                    player.ChangeState(PlayerStates.Dodge);
                }

                if (player.CurrentState != PlayerStates.Attack)
                {
                    if (player.IsAttack)
                    {
                        player.ChangeState(PlayerStates.Attack);
                    }
                }
            }*/
            
    
            if (player.CurrentState is PlayerStates.Attack or PlayerStates.Dodge or PlayerStates.Damaged)
            {
                return;
            }
            if (player.IsDodge)
            {
                if (player.IsAttack)
                    player.IsAttack = false;
                player.ChangeState(PlayerStates.Dodge);
            }
            else if (player.WasDamaged)
            {
                if (player.IsAttack)
                    player.IsAttack = false;
                player.ChangeState(PlayerStates.Damaged);
            }
            else if (player.IsAttack)
            {
                player.ChangeState(PlayerStates.Attack);
            }
            
        }
        public override void Exit(PlayerManager player)
        {
            
        }
    }

}
