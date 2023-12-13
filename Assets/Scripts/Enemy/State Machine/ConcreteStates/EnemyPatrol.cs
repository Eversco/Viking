using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : EnemyState
{
    private Vector3 _targetPos;
    private Vector3 _direction;

    // Rotation speed
    private float rotationSpeed = 5f;

    public EnemyPatrol(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void EnterState()
    {
        base.EnterState();
        SetNewRandomTimer();
        _targetPos = GetRandomPoint();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        _direction = (_targetPos - enemy.transform.position).normalized;

        // Rotate towards the target position using Quaternion.Lerp

        enemy.MoveEnemy(_direction * enemy.Rand_Move_Speed);

        enemy._currentTimer -= Time.deltaTime * 0.3f;
        if (enemy._currentTimer <= 0 || (enemy.transform.position - _targetPos).sqrMagnitude < 0.01f)
        {
            _targetPos = GetRandomPoint();
            SetNewRandomTimer();
        }
    }

    private Vector3 GetRandomPoint()
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle;
        return enemy.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y) * enemy.Rand_MoveRange;
    }

    #region Timer
    private void SetNewRandomTimer()
    {
        enemy._changeDirectionTimer = UnityEngine.Random.Range(2f, 5f); // Random time between 2 to 5 seconds
        enemy._currentTimer = enemy._changeDirectionTimer;
    }
    #endregion
}
