using UnityEngine;

namespace Game.Infrastructure.Rendering
{
    public static class RendererExtensions
    {
        public static void ScaleToFitArea(this Renderer renderer, float width, float height)
        {
            Bounds bounds = renderer.bounds;
            renderer.transform.localScale = new Vector3(width / bounds.size.x, height / bounds.size.y, 1f);
        }
        
        public static void FitArea(this Renderer renderer, float width, float height, Vector2 center)
        {
            renderer.ScaleToFitArea(width, height);
            Vector2 boundsCenter = renderer.bounds.center;
            Transform transform = renderer.transform;
            transform.position = center + boundsCenter - (Vector2)transform.position;
        }
    }
}