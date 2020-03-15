namespace CodeGolf.Integration.Test.Tooling
{
    using System.Text.Json;
    using Dahomey.Json;

    public static class JsonOptions
    {
        public static JsonSerializerOptions Options => new JsonSerializerOptions { PropertyNameCaseInsensitive = true }.SetupExtensions();
    }
}
