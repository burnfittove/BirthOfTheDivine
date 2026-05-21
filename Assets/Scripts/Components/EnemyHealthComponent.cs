using DefaultNamespace;
using Events;
using UnityEngine;

namespace Components
{ 
    public class EnemyHealthComponent : HealthComponent
    {
        public override void Die()
        {
            if (_currentHealth > 0) return;
            GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            GetComponent<Collider2D>().enabled = false;
            GameEventManager.Instance.resourceEvents.OnRewardBlood(10);
            GameEventManager.Instance.resourceEvents.OnRewardBones(3);
        }
    }
}
