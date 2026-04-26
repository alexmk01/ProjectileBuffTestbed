using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Infrastructure.Localization
{
    public static class LocalizationExtensions
    {
        public static async UniTaskVoid SetLocalizedTextAsync<T>(this T component, string table, string key, Action<T, string> setter)
        {
            if (LocalizationSettings.HasSettings)
            {
                AsyncOperationHandle<string> asyncOperation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(table, key);

                await asyncOperation;

                if (asyncOperation.IsDone && asyncOperation.Result != null)
                {
                    setter.Invoke(component, asyncOperation.Result);
                    return;
                }
            }
            
            setter.Invoke(component, key);
        }

        public static void SetLocalizedText<T>(this T component, string table, string key, Action<T, string> setter)
        {
            component.SetLocalizedTextAsync(table, key, setter).Forget();
        }
    }
}