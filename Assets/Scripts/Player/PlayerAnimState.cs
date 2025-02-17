using UnityEngine;

namespace Player
{
    
    public class Idle : State<PlayerControl>
    {

        public override void Enter(PlayerControl player)
        {
            player.ChangeAnimation("Idle");
        }

        public override void Execute(PlayerControl player)
        {
            
        }

        public override void Exit(PlayerControl player)
        {
            
        }
    }

    public class Run : State<PlayerControl>
    {
        public override void Enter(PlayerControl player)
        {
            player.ChangeAnimation("Run");
        }

        public override void Execute(PlayerControl player)
        {
        }

        public override void Exit(PlayerControl player)
        {

        }
    }

    public class Attack : State<PlayerControl>
    {
        private float _startTime;
        public override void Enter(PlayerControl player)
        {
            _startTime = Time.time;//Enter했을 때 시간
            player.ChangeAnimation(player.IsSkill ? "Skill" : "Attack");
        }

        public override void Execute(PlayerControl player)
        {
            var animatorState = player.PlayerAnimator.GetCurrentAnimatorStateInfo(0);
            float animTime = Time.time - _startTime; //현재 애니메이션 시간
            
            bool isAttack = animatorState.IsName("Attack") || animatorState.IsName("Skill");
            if ( animTime >= animatorState.length && isAttack)
            {
                //애니메이션 길이(시간)을 넘으면 Idle
                player.ChangeState(PlayerStates.Idle); 
            }
        }

        public override void Exit(PlayerControl player)
        {
            //player.IsAttack = false;
            if(player.IsSkill) 
                player.IsSkill = false;
        }
        
    }
    
    public class Dodge : State<PlayerControl>
    {
        public override void Enter(PlayerControl player)
        {
            player.ChangeAnimation("Run");
        }

        public override void Execute(PlayerControl player)
        {
            
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
            
        }
        public override void Exit(PlayerControl player)
        {
            
        }
    }

}
