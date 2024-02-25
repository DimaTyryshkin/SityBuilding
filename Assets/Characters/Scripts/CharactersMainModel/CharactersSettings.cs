using System;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu]
    public class CharactersSettings : ScriptableObject
    {
        [Header("Player")]
        public int playerMaxHp;
        public bool hpDebug;
        public bool isAttackDebug;
        public CharacterMotorSetting playerMotor;
        public CharacterSetting playerSettings;
            
        [Header("Zombie")]
        public int zombieMaxHp;
        public float zombieMinAttackDistance;
        public float zombieMaxAttackDistance; 
        public CharacterMotorSetting zombieMotor;
        public CharacterSetting zombieSettings;
    }

    [Serializable]
    public struct CharacterMotorSetting
    { 
        public float maxSpeed;
        public float maxSpeedInAttack;
        public float rotationSpeed;
        public float animationLerp;
    }
    
    [Serializable]
    public struct CharacterSetting
    {
        public float attackDistance;
    }
}