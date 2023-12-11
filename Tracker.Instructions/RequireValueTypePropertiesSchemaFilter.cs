using System.Reflection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tracker.Instructions;

public sealed class RequireValueTypePropertiesSchemaFilter : ISchemaFilter
{
    private readonly CamelCasePropertyNamesContractResolver? _camelCaseContractResolver;

    public RequireValueTypePropertiesSchemaFilter(bool camelCasePropertyNames)
    {
        _camelCaseContractResolver = camelCasePropertyNames ? new CamelCasePropertyNamesContractResolver() : null;
    }

    private string PropertyName(PropertyInfo property)
    {
        return _camelCaseContractResolver?.GetResolvedPropertyName(property.Name) ?? property.Name;
    }

    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        foreach (var property in context.Type.GetProperties())
        {
            var schemaPropertyName = PropertyName(property);
            // This check ensures that properties that are not in the schema are not added as required.
            // This includes properties marked with [IgnoreDataMember] or [JsonIgnore] (should not be present in schema or required).
            if (model.Properties?.ContainsKey(schemaPropertyName) == true)
            {

                model.Required.Add(schemaPropertyName);
            }
        }
    }
}
