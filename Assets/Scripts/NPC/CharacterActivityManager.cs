namespace AF
{
    using UnityEngine;

    public class CharacterActivityManager : CharacterBaseActivityManager
    {
        [SerializeField] CharacterManager characterManager;

        public override CharacterBaseManager GetCharacter()
        {
            return characterManager;
        }
    }
}
