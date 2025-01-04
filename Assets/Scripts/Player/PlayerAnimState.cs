using UnityEngine;

namespace Player
{
    
    public class Idle : State<PlayerControl>
    {

        public override void Enter(PlayerControl player)
        {
            //player.PlayerAnimator.Play("Idle");
            player.ChangeAnimation("Idle");
        }

        public override void Execute(PlayerControl player)
        {
            if (player.IsMove)
            {
                player.ChangeState(PlayerStates.Run);
            }
        }

        public override void Exit(PlayerControl player)
        {
            
        }
    }

    public class Run : State<PlayerControl>
    {
        public override void Enter(PlayerControl player)
        {
            //player.PlayerAnimator.Play("Run");
            player.ChangeAnimation("Run");
        }

        public override void Execute(PlayerControl player)
        {
            if (!player.IsMove)
            {
                player.ChangeState(PlayerStates.Idle);
            }
        }

        public override void Exit(PlayerControl player)
        {

        }
    }

    public class Attack : State<PlayerControl>
    {
        public override void Enter(PlayerControl player)
        {
            //player.PlayerAnimator.Play("Attack");
            if (player.IsSkill)
            {
                player.ChangeAnimation("Skill");
            }
            else
            {
                player.ChangeAnimation("Attack");
            }
        }

        public override void Execute(PlayerControl player)
        {
            float animTime = Mathf.Repeat(player.PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime,1.0f);    
            if ( animTime >= 0.98f && player.IsAttack) //애니메이션 종료 체크//개선필요?
            //Attack Animation End, State return to Idle. 공격 애니메이션 종료 시 상태종료 Idle로 돌아감
            {
                player.IsAttack = false;
                if(player.IsSkill) 
                    player.IsSkill = false;
                player.ChangeState(PlayerStates.Idle); 
            }
        }

        public override void Exit(PlayerControl player)
        {
           
        }
    }

    public class Skill : State<PlayerControl>
    {
        public override void Enter(PlayerControl player)
        {
            
        }

        public override void Execute(PlayerControl player)
        {
            
        }

        public override void Exit(PlayerControl player)
        {
            
        }
    }
    
    public class Dodge : State<PlayerControl>
    {
        public override void Enter(PlayerControl player)
        {
            //player.PlayerAnimator.Play("Run");
            player.ChangeAnimation("Run");
        }

        public override void Execute(PlayerControl player)
        {
            if (!player.IsDodge)
            {
                player.ChangeState(PlayerStates.Idle);
            }
        }

        public override void Exit(PlayerControl player)
        {

        }
    }

    public class StateGlobal : State<PlayerControl>
    {
        public override void Enter(PlayerControl player)
        {
            
        }
        public override void Execute(PlayerControl player)
        {
    
            if (player.CurrentState is PlayerStates.Attack or PlayerStates.Dodge)
            {
                return;
            }
            if (player.IsDodge)
            {
                if (player.IsAttack)
                    player.IsAttack = false;
                player.ChangeState(PlayerStates.Dodge);
            }
            
            else if (player.IsAttack)
            {
                
                player.ChangeState(PlayerStates.Attack);
            }
            
        }
        public override void Exit(PlayerControl player)
        {
            
        }
    }

}
