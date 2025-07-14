using UnityEngine;

namespace AF
{
    public abstract class CharacterBaseDodgeController : MonoBehaviour
    {

        [Header("In-game flags")]
        public bool isDodging = false;

        public virtual void ResetStates()
        {
            isDodging = false;
        }

        public abstract void HandleDodge();
        public abstract bool CanDodge();
    }
}
