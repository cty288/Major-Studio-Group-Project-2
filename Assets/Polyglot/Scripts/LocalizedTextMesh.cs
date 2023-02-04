
#if UNITY_5 || UNITY_2017_1_OR_NEWER
using JetBrains.Annotations;
#endif
using TMPro;
using UnityEngine;

namespace Polyglot
{
    [AddComponentMenu("Mesh/Localized TextMesh")]
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedTextMesh : LocalizedTextComponent<TMP_Text>, ILocalize
    {

       


       
        protected override void SetText(TMP_Text component, string value) {
            if (component == null)
            {
                Debug.LogWarning("Missing Text Component on " + gameObject, gameObject);
                return;
            }
            component.text = value;
        }

        protected override void UpdateAlignment(TMP_Text component, LanguageDirection direction) {
            
        }

      

        private bool IsOppositeDirection(TextAlignmentOptions alignment, LanguageDirection direction)
        {
            return (direction == LanguageDirection.LeftToRight && IsAlignmentRight(alignment)) || (direction == LanguageDirection.RightToLeft && IsAlignmentLeft(alignment));
        }

        private bool IsAlignmentRight(TextAlignmentOptions alignment)
        {
            return alignment == TextAlignmentOptions.Right;
        }
        private bool IsAlignmentLeft(TextAlignmentOptions alignment)
        {
            return alignment == TextAlignmentOptions.Left;
        }
    }
}