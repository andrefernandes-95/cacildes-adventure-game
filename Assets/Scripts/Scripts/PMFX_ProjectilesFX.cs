using System.Collections;
using AF;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PMFX_ProjectilesFX : MonoBehaviour
{

    public float Speed = 15f;
    public float lifetime = 3f;

    public GameObject impact;

    public ParticleSystem detachFX;

    public Transform spawnPoint;

    Transform _parent;

    Rigidbody rigidBody => GetComponent<Rigidbody>();

    [Header("Homing Options")]
    public float playerYOffset = 3f;
    public bool isFromPlayer = false;
    public bool shouldHomeOnTarget = false;
    public bool shouldHomeOnPlayer = false;
    public bool isHoming = false;
    public Transform homingTarget;
    [SerializeField] Vector3 homingTargetOffset = Vector3.zero;

    public float delayBeforeSettingHoming = 1f;
    public float maxHomingTargetDistanceToTriggerIsHoming = 3f;
    public float maxHomingTargetDistanceToDeactivate = 1f;
    SphereCollider sphereCollider => GetComponent<SphereCollider>();

    [Header("Homing On Enemies")]
    public float radius = 5f;
    public float distance = 10f;
    public LayerMask enemyLayer;

    [Header("Target Locking")]
    public bool lockOnToNearestEnemy = false;
    public Transform nearestEnemyToLockOn;

    [Header("Events")]
    public UnityEvent onCollisionEvent;

    Transform player;

    private void Awake()
    {
        _parent = this.transform.parent;

        if (shouldHomeOnTarget)
        {
            sphereCollider.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (lockOnToNearestEnemy)
        {
            var playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);

            if (nearestEnemyToLockOn == null)
            {
                nearestEnemyToLockOn = Utils.GetClosestEnemy(playerManager, playerManager.characterFactions[0])?.transform;
            }
        }

        if (isFromPlayer)
        {
            player = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include).transform;
        }

        if (isFromPlayer)
        {
            _parent = player.transform;
            this.transform.position = player.transform.position + new Vector3(0, playerYOffset, 0);
        }
        else if (spawnPoint != null)
        {
            this.transform.position = spawnPoint.transform.position;
        }

        this.transform.parent = _parent;

        if (!shouldHomeOnTarget)
        {
            this.transform.rotation = spawnPoint != null ? spawnPoint.transform.rotation : Quaternion.identity;

            if (isFromPlayer)
            {
                transform.rotation = player.transform.rotation;
            }

            this.transform.parent = null;
            StartCoroutine(DisableAfter());
        }

        if (shouldHomeOnPlayer)
        {
            StartCoroutine(SetPlayerAsHomingTarget_Coroutine());
        }
    }

    private void OnDisable()
    {
        isHoming = false;

        if (shouldHomeOnTarget)
        {
            sphereCollider.enabled = false;
        }
    }

    void SetIsHomingToTrue()
    {
        this.transform.parent = null;
        isHoming = true;
        sphereCollider.enabled = true;
    }

    public IEnumerator SetPlayerAsHomingTarget_Coroutine()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include)?.transform;
        }

        if (player != null)
        {
            Vector3 directionToTarget = (player.position + player.up - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(directionToTarget);

            yield return new WaitForSeconds(delayBeforeSettingHoming);

            SetHomingTarget(player);
        }
    }

    public void SetHomingTarget(Transform target)
    {
        this.homingTarget = target;
    }

    IEnumerator DisableAfter()
    {
        yield return new WaitForSeconds(lifetime);
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (shouldHomeOnTarget && !shouldHomeOnPlayer && !isHoming)
        {
            // Perform the sphere cast
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, radius, transform.forward, distance, enemyLayer);

            // Loop through all the hits
            foreach (RaycastHit hit in hits)
            {
                // Check if the hit object is an enemy
                if (hit.transform != null)
                {
                    CharacterManager enemy = hit.transform.gameObject.GetComponent<CharacterManager>();
                    if (enemy != null)
                    {
                        homingTarget = enemy.transform;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (lockOnToNearestEnemy && nearestEnemyToLockOn != null)
        {
            Vector3 directionToTarget = (nearestEnemyToLockOn.position + homingTargetOffset - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(directionToTarget);
            rigidBody.linearVelocity = transform.forward * Speed;
            return;
        }

        if (shouldHomeOnTarget)
        {
            if (homingTarget != null)
            {
                Vector3 directionToTarget = (homingTarget.position + homingTarget.up - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(directionToTarget);

                if (isHoming == false && Vector3.Distance(transform.position, homingTarget.transform.position) <= maxHomingTargetDistanceToTriggerIsHoming)
                {
                    SetIsHomingToTrue();
                }

                if (isHoming)
                {
                    rigidBody.linearVelocity = transform.forward * Speed;

                    if (Vector3.Distance(transform.position, homingTarget.transform.position + homingTargetOffset) <= maxHomingTargetDistanceToDeactivate)
                    {
                        isHoming = false;
                        gameObject.SetActive(false);
                    }

                    return;
                }
            }

            if (isFromPlayer)
            {
                transform.rotation = player.transform.rotation;
            }

            rigidBody.linearVelocity = Vector3.zero;
            return;
        }

        rigidBody.linearVelocity = transform.forward * Speed;
    }

    public void OnCollision()
    {
        Instantiate(impact, transform.position, transform.rotation, null);
        onCollisionEvent?.Invoke();
    }
}
