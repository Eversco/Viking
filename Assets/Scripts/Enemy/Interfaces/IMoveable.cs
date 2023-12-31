using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable
{
    Rigidbody2D rb { get; set; }
    bool isFacingRight { get; set; }
    void MoveEnemy(Vector3 velocity);
}
