using UnityEngine;
using UnityEngine.Rendering;

public interface IDamageable
{
    public void Damage(float damageAmount, Vector2 attackDirection);
    public bool HasTakenDamage { get; set; }
}
