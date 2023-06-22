using Microsoft.Win32;

namespace Loxifi.Services
{
    internal static class RegistryService
    {
        public static string GetRequiredValue(string keyName, string? valueName = "", object? defaultValue = null) => (string)Registry.GetValue(keyName, valueName, defaultValue) ?? throw new KeyNotFoundException("No registry key found at " + keyName);
    }
}