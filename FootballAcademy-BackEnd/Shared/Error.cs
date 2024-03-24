using Newtonsoft.Json;

namespace FootballAcademy_BackEnd.Shared;

public class Error
{
    public string Code { get; private set; }
    public string Message { get; private set; }

    public Error(string code, object message)
    {

        if (message is FluentValidation.Results.ValidationResult validationResult)
        {
            var errorDictionary = new Dictionary<string, string>();

            foreach (var error in validationResult.Errors)
            {
                errorDictionary[error.PropertyName] = error.ErrorMessage;
            }

            Message = JsonConvert.SerializeObject(errorDictionary);
            Code = "422";
        }
        else
        {
            Message = message.ToString();
            Code = code;
        }
    }

    public static Error BadRequest(string Message)
    {
        return new Error(StatusCodes.Status400BadRequest.ToString(), Message);
    }

    public static Error ValidationError(object Message)
    {
        return new Error(StatusCodes.Status422UnprocessableEntity.ToString(), Message);
    }


    public static readonly Error None = new Error(string.Empty, string.Empty);

    public static readonly Error NullValue = new Error("Error.NullValue", "The specified result value is null.");

    public static readonly Error NotFound = new Error("404", "A Requested Item Was Not Found");

    public static readonly Error InvalidRequest = new Error("Invalid Request", "Invlaid Body Type");

    public static readonly Error ConditionNotMet = new Error("Error.ConditionNotMet", "The specified condition was not met.");
}
