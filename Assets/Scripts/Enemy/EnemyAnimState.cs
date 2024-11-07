
namespace EnemyAnimState
{
    public class Idle : State<EnemyManager>
    {
        public override void Enter(EnemyManager enemy)
        {
            enemy.EnemyAnimator.Play("Idle");
        }

        public override void Execute(EnemyManager enemy)
        {
            if (enemy.IsMove)
            {
                enemy.ChangeState(EnemyStates.Move);
            }
                
        }

        public override void Exit(EnemyManager enemy)
        {
            
        }
    }
    
    public class Move : State<EnemyManager>
    {
        public override void Enter(EnemyManager enemy)
        {
            enemy.EnemyAnimator.Play("Move");
        }

        public override void Execute(EnemyManager enemy)
        {
            if (!enemy.IsMove)
            {
                enemy.ChangeState(EnemyStates.Idle);
            }
        }

        public override void Exit(EnemyManager enemy)
        {
            
        }
    }
    
    public class Attack : State<EnemyManager>
    {
        public override void Enter(EnemyManager enemy)
        {
           
            
        }

        public override void Execute(EnemyManager enemy)
        {
            
            enemy.EnemyAnimator.Play("Attack");
            if (!enemy.IsAttack)
            {
                enemy.ChangeState(EnemyStates.Idle);
            }
        }

        public override void Exit(EnemyManager enemy)
        {
            
        }
    }

    public class Damaged : State<EnemyManager>
    {
        public override void Enter(EnemyManager enemy)
        {
           enemy.EnemyAnimator.Play("Damage");
            
        }

        public override void Execute(EnemyManager enemy)
        {
            
        }

        public override void Exit(EnemyManager enemy)
        {
            
        }
    }
    
    public class StateGlobal : State<EnemyManager>
    {
        public override void Enter(EnemyManager enemy)
        {
            
        }

        public override void Execute(EnemyManager enemy)
        {
            if (enemy.CurrentState != EnemyStates.Attack && enemy.IsAttack)
            {
                enemy.ChangeState(EnemyStates.Attack);
            }
            
            if (enemy.CurrentState != EnemyStates.Damaged && enemy.WasDamaged)
            {
                enemy.ChangeState(EnemyStates.Damaged);
            }
        }

        public override void Exit(EnemyManager enemy)
        {
            
        }
    }
}


