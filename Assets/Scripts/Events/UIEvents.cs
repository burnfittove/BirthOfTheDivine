using System;

namespace Events
{
    public class UIEvents
    {
        #region Health

        // Update health UI
        public event Action<int> UpdateHealth;
        public void OnUpdateHealth(int amount)
        {
            UpdateHealth?.Invoke(amount);
        }
        
        #endregion


        #region Blood

        // Update blood UI
        public event Action<int> UpdateBlood;
        public void OnUpdateBlood(int amount)
        {
            UpdateBlood?.Invoke(amount);
        }

        #endregion


        #region Bones

        // Update bone UI
        public event Action<int> UpdateBones;
        public void OnUpdateBones(int amount)
        {
            UpdateBones?.Invoke(amount);
        }

        #endregion
    }
}
