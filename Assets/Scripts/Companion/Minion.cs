using System.Linq;
using UnityEngine;

namespace AF
{
    public class Minion : MonoBehaviour, IAbilityInstance
    {
        public void CastAbility(CharacterBaseManager caster, CharacterBaseManager target)
        {

            if (TryGetComponent<CharacterManager>(out var minion))
            {
                minion.characterFactions = caster.characterFactions;

                transform.position = caster.transform.position + caster.transform.forward;

                if (caster is PlayerManager playerManager && playerManager.GetTarget() != null)
                {
                    minion.targetManager.SetTarget(playerManager.GetTarget());
                }
                else
                {
                    minion.targetManager.SetPlayerAsTarget();
                }
            }
        }
    }
}
