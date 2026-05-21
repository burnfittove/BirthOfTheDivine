using System;
using Events;

namespace UI
{
    public class DisplayBones : DisplayResource
    {
        private void Start()
        {
            GameEventManager.Instance.uiEvents.UpdateBones += UpdateText;
        }
    }
}