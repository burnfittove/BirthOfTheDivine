using System;
using Events;
using UnityEngine;

namespace Managers
{
    public class ResourceManager : MonoBehaviour
    {
        [SerializeField] private int blood;
        [SerializeField] private int bones;
        
        private void OnEnable()
        {
            GameEventManager.Instance.resourceEvents.RewardBlood += AddBlood;
            GameEventManager.Instance.resourceEvents.RewardBones += AddBones;
            GameEventManager.Instance.resourceEvents.TakeBlood += TakeBlood;
            GameEventManager.Instance.resourceEvents.TakeBones += TakeBones;
            GameEventManager.Instance.resourceEvents.GetBlood += GetBlood;
            GameEventManager.Instance.resourceEvents.GetBones += GetBones;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.resourceEvents.RewardBlood -= AddBlood;
            GameEventManager.Instance.resourceEvents.RewardBones -= AddBones;
            GameEventManager.Instance.resourceEvents.TakeBlood -= TakeBlood;
            GameEventManager.Instance.resourceEvents.TakeBones -= TakeBones;
            GameEventManager.Instance.resourceEvents.GetBlood -= GetBlood;
            GameEventManager.Instance.resourceEvents.GetBones -= GetBones;
        }

        #region Reward

        private void AddBlood(int amount)
        {
            blood += amount;
            GameEventManager.Instance.uiEvents.OnUpdateBlood(blood);
        }
    
        private void AddBones(int amount)
        {
            bones += amount;
            GameEventManager.Instance.uiEvents.OnUpdateBones(bones);
        }

        #endregion


        #region Lose

        private void TakeBlood(int amount)
        {
            blood -= amount;
        }
    
        private void TakeBones(int amount)
        {
            bones -= amount;
        }

        #endregion
        

        #region Retrieve

        private int GetBlood()
        {
            return blood;
        }

        private int GetBones()
        {
            return bones;
        }

        #endregion
        
    }
}