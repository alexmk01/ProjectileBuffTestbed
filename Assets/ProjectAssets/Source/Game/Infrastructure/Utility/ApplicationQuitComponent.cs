using UnityEngine;

namespace Game.Infrastructure.Utility
{
    public sealed class ApplicationQuitComponent : MonoBehaviour
    {
        public void Quit() => Application.Quit();
    }
}