using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AluguelRoupa.Extensions;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class ValidateObj: Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        IEnumerable<ValidationResult> results = [];

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is IValidatableObject validatable)
            {
                var validationContext = new ValidationContext(validatable);
                results = [..results, ..validatable.Validate(validationContext)];
            }
        }


        if (results.Any())
        {
            context.Result = new BadRequestObjectResult(RenderValidationErrors([.. results]));
            return;
        }

        await next();
    }

    private static string RenderValidationErrors(List<ValidationResult> errors)
    {
        var html = "<ul class='validation-errors'>";

        foreach (var error in errors)
            html += $"<li class='error'>{error.ErrorMessage}</li>";

        html += "</ul>";

        return html;
    }
}