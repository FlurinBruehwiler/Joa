using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Joa;

public static class JsonUtilities
{
    private class PopulateTypeInfoResolver : IJsonTypeInfoResolver
    {
        private bool _isRootResolved;

        private readonly object _rootObject;
        private readonly Type _rootObjectType;
        private readonly IJsonTypeInfoResolver _jsonTypeInfoResolver;

        public PopulateTypeInfoResolver(object rootObject, IJsonTypeInfoResolver jsonTypeInfoResolver)
        {
            _rootObject = rootObject;
            _rootObjectType = rootObject.GetType();
            _jsonTypeInfoResolver = jsonTypeInfoResolver;
        }

        public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
        {
            var typeInfo = _jsonTypeInfoResolver.GetTypeInfo(type, options);
            if (typeInfo != null && type == _rootObjectType)
            {
                typeInfo.CreateObject = () =>
                {
                    if (_isRootResolved)
                        return Activator.CreateInstance(type)!;

                    _isRootResolved = true;
                    return _rootObject;
                };
            }

            return typeInfo;
        }
    }

    public static void PopulateObject(JsonElement json, object obj, JsonSerializerOptions? options = null)
    {
        var modifiedOptions = options != null
            ? new JsonSerializerOptions(options)
            {
                TypeInfoResolver =
                    new PopulateTypeInfoResolver(obj, options.TypeInfoResolver ?? new DefaultJsonTypeInfoResolver()),
            }
            : new JsonSerializerOptions
            {
                TypeInfoResolver = new PopulateTypeInfoResolver(obj, new DefaultJsonTypeInfoResolver()),
            };

        json.Deserialize(obj.GetType(), modifiedOptions);
    }
}