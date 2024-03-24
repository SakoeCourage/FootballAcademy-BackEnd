using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FootballAcademy_BackEnd.Shared
{
    public class ResponseAnotator
    {

        
    }

public class ResponseAnnotatorAttribute : Attribute
{
    public Type ResponseType { get; }
    public int StatusCode { get; }

    public ResponseAnnotatorAttribute(Type responseType, int statusCode)
    {
        ResponseType = responseType;
        StatusCode = statusCode;
    }
}

public class ResponseAnnotatorOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var responseAnnotations = context.MethodInfo.GetCustomAttributes(typeof(ResponseAnnotatorAttribute), false);
        foreach (ResponseAnnotatorAttribute annotation in responseAnnotations)
        {
            operation.Responses.Add(annotation.StatusCode.ToString(), new OpenApiResponse { Description = annotation.ResponseType.Name });
        }
    }
}

}