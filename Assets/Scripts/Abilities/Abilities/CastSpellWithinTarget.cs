namespace AF
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Cast Spell Within Target", menuName = "Abilities / Spells / New Cast Spell Within Target", order = 0)]
    public class CastSpellWithinTarget : CastFromSpell
    {
        [Header("Options")]
        [SerializeField] bool parentToOwnerInstead = false;

        protected override GameObject ReleaseSpellGameObject(CharacterBaseManager damageOwner, string[] tagsToDetect)
        {
            GameObject instance = base.ReleaseSpellGameObject(damageOwner, tagsToDetect);

            if (parentToOwnerInstead)
            {
                instance.transform.parent = damageOwner.transform;
            }
            else if (target != null)
            {
                instance.transform.parent = target.transform;
            }

            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;

            return instance;
        }
    }
}
