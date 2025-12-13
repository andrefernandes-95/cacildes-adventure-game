using UnityEngine;

namespace AF.Conditions
{
    public class ChristmasDependant : MonoBehaviour
    {
        private void Awake()
        {
            Utils.UpdateTransformChildren(transform, false);
        }

        private void Start()
        {
            Evaluate();
        }

        public void Evaluate()
        {
            Utils.UpdateTransformChildren(transform, SeasonalEvents.IsChristmasTime());
        }
    }
}
