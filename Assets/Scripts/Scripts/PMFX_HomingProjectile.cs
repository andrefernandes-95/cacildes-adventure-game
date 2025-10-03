using AF;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PMFX_HomingProjectile : MonoBehaviour, IAbilityInstance
{
    public float Speed = 15f;
    public GameObject impact;
    Rigidbody rigidBody => GetComponent<Rigidbody>();

    [Header("Homing Options")]
    CharacterBaseManager homingTarget;
    public float minimumDistanceToStartChasing = 3f;
    public float radius = 5f;
    public float distance = 10f;
    public LayerMask enemyLayer;
    public Vector3 upwardOffset = Vector3.up;

    [Header("Events")]
    public UnityEvent onCollisionEvent;

    public CharacterBaseManager caster;

    bool isHoming = false;

    [Header("Settings")]
    public bool followCasterAround = true;

    [Header("Collision Settings")]
    int currentCollisionCount = 0;
    public int maximumOnCollisionCalls = 999;

    private void Awake()
    {
        currentCollisionCount = 0;
    }

    public void CastAbility(CharacterBaseManager caster, CharacterBaseManager target)
    {
        this.caster = caster;
    }

    bool IsInitialized() => caster != null;

    private void Update()
    {
        if (!IsInitialized()) { return; }

        if (homingTarget == null)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward, distance, enemyLayer);

            float shortestDistance = Mathf.Infinity;

            foreach (RaycastHit hit in hits)
            {
                // Check if the hit object is an enemy
                if (hit.transform != null && hit.transform.root != caster.transform.root)
                {
                    if (hit.transform.gameObject.TryGetComponent<CharacterBaseManager>(out var enemy) && !caster.IsFromSameFaction(enemy))
                    {
                        float currentDistance = Vector3.Distance(enemy.transform.position, transform.position);

                        if (currentDistance < shortestDistance)
                        {
                            shortestDistance = currentDistance;
                            homingTarget = enemy;
                        }
                    }
                }
            }
        }

        if (!isHoming && followCasterAround)
        {
            transform.SetPositionAndRotation(
                caster.characterTransformHelper.torso.transform.position + upwardOffset, caster.transform.rotation);
        }
    }

    void FixedUpdate()
    {
        if (!IsInitialized()) { return; }

        if (homingTarget != null)
        {
            Vector3 targetPosition = homingTarget.characterTransformHelper.torso != null
                ? homingTarget.characterTransformHelper.torso.transform.position
                : homingTarget.transform.position;

            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(directionToTarget);

            if (isHoming)
            {
                rigidBody.linearVelocity = transform.forward * Speed;
            }
            else
            {
                if (Vector3.Distance(transform.position, targetPosition) <= minimumDistanceToStartChasing)
                {
                    isHoming = true;
                }
            }
        }
    }

    public void OnCollision()
    {
        Instantiate(impact, transform.position, transform.rotation, null);
        onCollisionEvent?.Invoke();
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Unity Event
    /// </summary>
    public void IncreaseOnCollisionCount()
    {
        currentCollisionCount++;
        if (currentCollisionCount >= maximumOnCollisionCalls)
        {
            Destroy(this.gameObject);
        }
    }
}
