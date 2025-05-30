using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class AIHumanoidAnimationOverrideHelper : MonoBehaviour
    {
        [Header("Locomotion")]
        public AnimationClip idle;
        public AnimationClip patrolling;
        public AnimationClip chasing;

        [Header("Combat")]
        public AnimationClip combatIdle;
        public AnimationClip takingDamage;
        public AnimationClip dying;
        public AnimationClip knockdown;
        public AnimationClip knockdownGetup;

        public AnimationClip throwProjectile;

        [Header("Block & Parry")]
        public AnimationClip parried;
        public AnimationClip parrying;
        public AnimationClip blocking;
        public AnimationClip blockingReaction;


        [Header("Ambush")]
        public AnimationClip ambushIdle;
        public AnimationClip ambushExit;

        public Dictionary<string, AnimationClip> GetClipOverrides()
        {
            Dictionary<string, AnimationClip> animations = new();

            // Locomotion
            if (idle != null) animations.Add("AI Humanoid - Idle", idle);
            if (patrolling != null) animations.Add("AI Humanoid - Patrolling", patrolling);
            if (chasing != null) animations.Add("AI Humanoid - Chasing", chasing);
            // Combat
            if (combatIdle != null) animations.Add("AI Humanoid - Combat Idle", combatIdle);
            if (takingDamage != null) animations.Add("AI Humanoid - Taking Damage", takingDamage);
            if (dying != null) animations.Add("AI Humanoid - Dying", dying);
            if (knockdown != null) animations.Add("AI Humanoid - Posture Break", knockdown);
            if (knockdownGetup != null) animations.Add("AI Humanoid - Posture Break - Exit", knockdownGetup);
            if (parried != null) animations.Add("Cacildes - Parried", parried);
            if (parrying != null) animations.Add("ARPG_Warrior_Parry", parrying);
            if (blocking != null) animations.Add("Cacildes - Block - Idle", blocking);
            if (blockingReaction != null) animations.Add("AI Humanoid - Block Hit", blockingReaction);
            if (ambushIdle != null) animations.Add("Getup01-Idle", ambushIdle);
            if (ambushExit != null) animations.Add("Frank_Sword2@Getup01", ambushExit);
            if (throwProjectile != null) animations.Add("AI Throw Projectile", throwProjectile);
            return animations;
        }
    }
}
