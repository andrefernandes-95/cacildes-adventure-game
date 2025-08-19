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

        [Header("One Hand Attacks")]
        public AnimationClip oh_LightAttack1;
        public AnimationClip oh_LightAttack2;
        public AnimationClip oh_LightAttack3;
        public AnimationClip oh_Left_LightAttack1;
        public AnimationClip oh_Left_LightAttack2;
        public AnimationClip oh_Left_LightAttack3;
        public AnimationClip oh_HeavyAttack1;
        public AnimationClip oh_HeavyAttack2;

        [Header("Two Hand Attacks")]
        public AnimationClip th_LightAttack1;
        public AnimationClip th_LightAttack2;
        public AnimationClip th_LightAttack3;
        public AnimationClip th_HeavyAttack1;
        public AnimationClip th_HeavyAttack2;

        CharacterBaseManager characterBaseManager;

        private void Awake()
        {
            if (characterBaseManager == null)
            {
                characterBaseManager = GetComponent<CharacterBaseManager>();
            }
        }

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

            //Attacks

            if (characterBaseManager != null)
            {
                if (characterBaseManager.characterBaseWeaponsManager.IsTwoHanding())
                {
                    if (th_LightAttack1 != null) animations.Add("Cacildes - Light Attack - 1", th_LightAttack1);
                    if (th_LightAttack2 != null) animations.Add("Cacildes - Light Attack - 2", th_LightAttack2);
                    if (th_LightAttack3 != null) animations.Add("Cacildes - Light Attack - 3", th_LightAttack3);
                    if (th_HeavyAttack1 != null) animations.Add("Cacildes - Heavy Attack - 1", th_HeavyAttack1);
                    if (th_HeavyAttack2 != null) animations.Add("Cacildes - Heavy Attack - 2", th_HeavyAttack2);
                }
                else
                {
                    if (oh_LightAttack1 != null) animations.Add("Cacildes - Light Attack - 1", oh_LightAttack1);
                    if (oh_LightAttack2 != null) animations.Add("Cacildes - Light Attack - 2", oh_LightAttack2);
                    if (oh_LightAttack3 != null) animations.Add("Cacildes - Light Attack - 3", oh_LightAttack3);
                    if (oh_HeavyAttack1 != null) animations.Add("Cacildes - Heavy Attack - 1", oh_HeavyAttack1);
                    if (oh_HeavyAttack2 != null) animations.Add("Cacildes - Heavy Attack - 2", oh_HeavyAttack2);
                    if (oh_Left_LightAttack1 != null) animations.Add("Cacildes - Left Light Attack - 1", oh_Left_LightAttack1);
                    if (oh_Left_LightAttack2 != null) animations.Add("Cacildes - Left Light Attack - 2", oh_Left_LightAttack2);
                }
            }

            return animations;
        }
    }
}
