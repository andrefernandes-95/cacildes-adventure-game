using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public abstract class CharacterBaseAppearance : MonoBehaviour
    {
        public abstract bool IsMale();

        public abstract List<string> GetHairs();
        public abstract List<string> GetEyebrows();
        public abstract List<string> GetBeard();
        public abstract List<string> GetFace();
        public abstract List<string> GetTorso();
        public abstract List<string> GetHands();
        public abstract List<string> GetLegs();
        public abstract Color GetHairColor();
        public abstract Color GetSkinColor();
        public abstract Color GetEyesColor();
        public abstract Color GetTattooColor();

    }
}
