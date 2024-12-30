namespace Enemy
{
    public class Idle : State<EnemyControl>
    {
        public override void Enter(EnemyControl enemy)
        {
            //enemy.EnemyAnimator.Play("Idle");
            enemy.ChangeAnimation("Idle");
        }

        public override void Execute(EnemyControl enemy)
        {
            if (enemy.IsMove)
            {
                enemy.ChangeState(EnemyStates.Move);
            }
                
        }

        public override void Exit(EnemyControl enemy)
        {
            
        }
    }
    
    public class Move : State<EnemyControl>
    {
        public override void Enter(EnemyControl enemy)
        {
            //enemy.EnemyAnimator.Play("Move");
            enemy.ChangeAnimation("Move");
        }

        public override void Execute(EnemyControl enemy)
        {
            if (!enemy.IsMove)
            {
                enemy.ChangeState(EnemyStates.Idle);
            }
        }

        public override void Exit(EnemyControl enemy)
        {
            
        }
    }
    
    public class Attack : State<EnemyControl>
    {
        public override void Enter(EnemyControl enemy)
        {
            //enemy.EnemyAnimator.Play("Attack");
            enemy.ChangeAnimation("Attack");
        }

        public override void Execute(EnemyControl enemy)
        {
            if (enemy.IsAttack) return;
            enemy.ChangeState(EnemyStates.Idle);
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
            //enemy.EnemyAnimator.Play("Dead");
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
            if (enemy.IsDead)
            {
                enemy.ChangeState(EnemyStates.Dead);
            }
            else
            {
                if (enemy.CurrentState != EnemyStates.Attack && enemy.IsAttack)
                {
                    enemy.ChangeState(EnemyStates.Attack);
                }
                /*
                if (enemy.CurrentState != EnemyStates.Damaged && enemy.WasDamaged)
                {
                    enemy.ChangeState(EnemyStates.Damaged);
                }*/
            }
            
        }

        public override void Exit(EnemyControl enemy)
        {
            
        }
    }
}


