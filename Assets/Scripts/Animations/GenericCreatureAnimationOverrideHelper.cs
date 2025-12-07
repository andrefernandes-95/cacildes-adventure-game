using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class GenericCreatureAnimationOverrideHelper : MonoBehaviour
    {
        [Header("Locomotion")]
        public AnimationClip idle;
        public AnimationClip patrolling;
        public AnimationClip chasing;

        [Header("Combat")]
        public AnimationClip takingDamage;
        public AnimationClip dying;
        public AnimationClip knockdown;

        [Header("Attacks")]
        public AnimationClip attackA;
        public AnimationClip attackB;
        public AnimationClip attackC;
        public AnimationClip attackD;

        [Header("Block & Parry")]
        public AnimationClip parried;
        public AnimationClip blocking;
        public AnimationClip blockingReaction;

        [Header("Ambush")]
        public AnimationClip ambushIdle;
        public AnimationClip ambushExit;

        [Header("Spells")]
        public AnimationClip castSpell;

        [Header("Gestures")]
        public AnimationClip roar;
        public AnimationClip taunt;

        public Dictionary<string, AnimationClip> GetClipOverrides()
        {
            Dictionary<string, AnimationClip> animations = new();

            // Locomotion
            if (idle != null) animations.Add("Generic Creature - Idle", idle);
            if (patrolling != null) animations.Add("Generic Creature - Patrolling", patrolling);
            if (chasing != null) animations.Add("Generic Creature - Chasing", chasing);
            // Combat
            if (attackA != null) animations.Add("Generic Creature - Attack A", attackA);
            if (attackB != null) animations.Add("Generic Creature - Attack B", attackB);
            if (attackC != null) animations.Add("Generic Creature - Attack C", attackC);
            if (attackD != null) animations.Add("Generic Creature - Attack D", attackD);
            if (takingDamage != null) animations.Add("Generic Creature - Taking Damage", takingDamage);
            if (dying != null) animations.Add("Generic Creature - Dying", dying);
            if (knockdown != null) animations.Add("Generic Creature - Posture Break", knockdown);
            if (parried != null) animations.Add("Generic Creature - Parried", parried);
            if (blocking != null) animations.Add("Generic Creature - Blocking", blocking);
            if (blockingReaction != null) animations.Add("Generic Creature - Blocking - Hit", blockingReaction);
            if (ambushIdle != null) animations.Add("Generic Creature - Ambush - Idle", ambushIdle);
            if (ambushExit != null) animations.Add("Generic Creature - Ambush - Exit", ambushExit);
            if (castSpell != null) animations.Add("Generic Creature - Cast Spell", castSpell);
            if (roar != null) animations.Add("Generic Creature - Roar", roar);
            if (taunt != null) animations.Add("Generic Creature - Taunt", taunt);
            return animations;
        }
    }
}
