using System;

namespace SchoolAPI;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class ApiDocAttribute : Attribute
{
    public string Summary { get; }
    public string? Description { get; }

    public ApiDocAttribute(string summary, string? description = null)
    {
        Summary = summary;
        Description = description;
    }
}
