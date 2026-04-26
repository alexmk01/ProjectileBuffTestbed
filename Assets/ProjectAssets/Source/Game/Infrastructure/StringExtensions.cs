
namespace Game.Infrastructure
{
    public static class StringExtensions
    {
        public static string FormatAmount(this float amount) => $"{amount:+0.#;-0.#;0}";
    }
}