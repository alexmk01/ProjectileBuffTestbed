using TMPro;
using UnityEngine;

namespace Game.Infrastructure.UI
{
    public static class UIExtensions
    {
        public static bool TryTransformScreenToLocalPosition(this RectTransform rectTransform, Camera camera, Vector3 worldPosition, out Vector2 localPosition)
        {
            Vector3 screenPosition = camera.WorldToScreenPoint(worldPosition);
            
            if (screenPosition.z >= 0f)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPosition, null, out localPosition);
                return true;
            }
            
            localPosition = default;
            return false;
        }

        public static GameObject InstantiateMessageView(this GameObject prefab, Vector2 localPosition, string text, Transform parent, float lifetime)
        {
            if (prefab == null || lifetime <= 0f)
            {
                return null;
            }
            
            GameObject messageObject = Object.Instantiate(prefab, parent);
            ((RectTransform)messageObject.transform).anchoredPosition = localPosition;
            messageObject.GetComponentInChildren<TMP_Text>().text = text;
            Object.Destroy(messageObject, lifetime);
            return messageObject;
        }
    }
}