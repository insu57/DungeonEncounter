namespace Enemy
{
    public class Idle : State<EnemyControl>
    {
        public override void Enter(EnemyControl enemy)
        {
            
            enemy.ChangeAnimation("Idle");
        }

        public override void Execute(EnemyControl enemy)
        {
            
        }

        public override void Exit(EnemyControl enemy)
        {
            
        }
    }
    
    public class Move : State<EnemyControl>
    {
        public override void Enter(EnemyControl enemy)
        {
            enemy.ChangeAnimation("Move");
        }

        public override void Execute(EnemyControl enemy)
        {
            
        }

        public override void Exit(EnemyControl enemy)
        {
            
        }
    }
    
    public class Attack : State<EnemyControl>
    {
        public override void Enter(EnemyControl enemy)
        {
            enemy.ChangeAnimation("Attack");
        }

        public override void Execute(EnemyControl enemy)
        {
           
        }

        public override void Exit(EnemyControl enemy)
        {
            
        }
    }

    public class Damaged : State<EnemyControl>
    {
        public override void Enter(EnemyControl enemy)
        {
           enemy.EnemyAnimator.Play("Damage");
            
        }

        public override void Execute(EnemyControl enemy)
        {
            
        }

        public override void Exit(EnemyControl enemy)
        {
            
        }
    }
    public class Dead : State<EnemyControl>
    {
        public override void Enter(EnemyControl enemy)
        {
            enemy.ChangeAnimation("Dead");
        }

        public override void Execute(EnemyControl enemy)
        {
            
        }

        public override void Exit(EnemyControl enemy)
        {
            
        }
    }
    
    public class StateGlobal : State<EnemyControl>
    {
        public override void Enter(EnemyControl enemy)
        {
            
        }

        public override void Execute(EnemyControl enemy)
        {
           
        }

        public override void Exit(EnemyControl enemy)
        {
            
        }
    }
}


