using UnityEngine;

interface IEmnemyMoveable
{
    Rigidbody2D rb { get; set; }
    bool IsFacingRight { get; set; }

    void Move(Vector2 velocity);
    void CheckLeftOrRightFacing(Vector2 velocity);
    }
