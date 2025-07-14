namespace AF
{
    using UnityEngine;

    public class AttachToCharacterModel : MonoBehaviour
    {
        [SerializeField] string nameOfParentGameObjectToAttachTo = "Exported Synty Character";
        [Header("Assign Via Name")]
        [SerializeField] string boneName = "";

        [Header("Or Assign Via Direct Reference")]
        [SerializeField] Transform boneToAssignAsParent;

        [Header("Position & Rotation")]
        [SerializeField] Vector3 localPositionWhenParenting;
        [SerializeField] Vector3 localRotationWhenParenting;

        // We neeed to attach OnStart because
        // of SyntyCharacterModelManager.Awake, which disables all character pieces inside the armature
        void Start()
        {
            if (boneToAssignAsParent != null)
            {
                transform.parent = boneToAssignAsParent;
            }
            else if (!string.IsNullOrEmpty(boneName))
            {
                CharacterBaseManager character = GetComponentInParent<CharacterBaseManager>();
                if (character != null)
                {
                    Transform targetModel = FindChildByName(character.transform, nameOfParentGameObjectToAttachTo);
                    Transform foundBone = FindChildByName(targetModel.transform, boneName);
                    if (foundBone != null)
                    {
                        transform.parent = foundBone;
                    }
                    else
                    {
                        Debug.LogWarning($"Bone '{boneName}' not found in character model.");
                    }
                }
                else
                {
                    Debug.LogError("CharacterManager not found in parent hierarchy.");
                }
            }

            // Set local position & rotation after parenting
            transform.localPosition = localPositionWhenParenting;
            transform.localRotation = Quaternion.Euler(localRotationWhenParenting);
        }

        // Recursively searches for a child with the specified name
        Transform FindChildByName(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name.Equals(name, System.StringComparison.Ordinal))
                    return child;

                Transform found = FindChildByName(child, name); // Recursive search
                if (found != null)
                    return found;
            }
            return null;
        }
    }
}
