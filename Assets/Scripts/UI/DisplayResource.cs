using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class DisplayResource : MonoBehaviour
    {
        protected Image image;
        protected TMP_Text text;
        protected string valueToDisplay;

        protected virtual void Awake()
        {
            image = GetComponentInChildren<Image>();
            text = GetComponentInChildren<TMP_Text>();
        }

        protected virtual void UpdateText(int value)
        {
            valueToDisplay = value.ToString();
            text.text = valueToDisplay;
        }
    }
}
