namespace Player
{
    
    public class Idle : State<PlayerControl>
    {

        public override void Enter(PlayerControl player)
        {
            player.PlayerAnimator.Play("Idle");
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
            player.PlayerAnimator.Play("Run");
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
            player.PlayerAnimator.Play("Attack");
        }

        public override void Execute(PlayerControl player)
        {
            // &&  player.PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack")  // 공격 애니메이션 체크
            float animTime = player.PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;    
            if ( animTime >= 1.0f && player.IsAttack) //애니메이션 종료 체크
            //Attack Animation End, State return to Idle. 공격 애니메이션 종료 시 상태종료 Idle로 돌아감
            {
                player.IsAttack = false;
                player.ChangeState(PlayerStates.Idle); 
            }
            
            
        }

        public override void Exit(PlayerControl player)
        {
            
        }
    }

    public class Dodge : State<PlayerControl>
    {
        public override void Enter(PlayerControl player)
        {
            player.PlayerAnimator.Play("Run");
        }

        public override void Execute(PlayerControl player)
        {

        }

        public override void Exit(PlayerControl player)
        {

        }
    }

    public class UseItem : State<PlayerControl>
    {
        public override void Enter(PlayerControl player)
        {
            //
        }

        public override void Execute(PlayerControl player)
        {

        }

        public override void Exit(PlayerControl player)
        {

        }
    }

    public class Damaged : State<PlayerControl>
    {
        public override void Enter(PlayerControl player)
        {
            //player.PlayerAnimator.Play("Damaged");
        }

        public override void Execute(PlayerControl player)
        {
            if (!player.IsDamaged)
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
            /*
             
             
            else if (player.WasDamaged)
            {
                if (player.IsAttack)
                   player.IsAttack = false;
                player.ChangeState(PlayerStates.Damaged);
            }*/
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
