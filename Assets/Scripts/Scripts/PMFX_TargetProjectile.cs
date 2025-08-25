using System.Collections;
using AF;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class PMFX_TargetProjectile : MonoBehaviour, IAbilityInstance
{
    public float Speed = 15f;
    public float lifetime = 3f;

    public GameObject impact;
    public ParticleSystem detachFX;

    Rigidbody rigidBody => GetComponent<Rigidbody>();

    [Header("Target Locking")]
    public CharacterBaseManager caster;
    public CharacterBaseManager target;

    [Header("Events")]
    public UnityEvent onCollisionEvent;

    public void CastAbility(CharacterBaseManager caster, CharacterBaseManager target)
    {
        this.caster = caster;
        this.target = target;
    }

    private void OnEnable()
    {
        StartCoroutine(DisableAfter());
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 directionToTarget = target.characterTransformHelper.torso.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(directionToTarget);
            rigidBody.linearVelocity = transform.forward * Speed;
        }
    }

    public void OnCollision()
    {
        Instantiate(impact, transform.position, transform.rotation, null);
        onCollisionEvent?.Invoke();
        Destroy(this.gameObject);
    }

    IEnumerator DisableAfter()
    {
        yield return new WaitForSeconds(lifetime);
        this.gameObject.SetActive(false);
    }

}
