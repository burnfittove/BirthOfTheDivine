using System;
using Events;

namespace UI
{
    public class DisplayBlood : DisplayResource
    {
        private void Start()
        {
            GameEventManager.Instance.uiEvents.UpdateBlood += UpdateText;
        }
    }
}