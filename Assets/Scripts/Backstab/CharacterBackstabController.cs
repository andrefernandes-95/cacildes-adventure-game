namespace AF
{
    using UnityEngine;

    public class CharacterBackstabController : MonoBehaviour
    {
        [SerializeField] CharacterManager characterManager;
        public bool isBeingBackstabbed = false;

        public void ResetStates()
        {
            isBeingBackstabbed = false;
        }
    }
}
