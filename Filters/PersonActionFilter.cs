using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Lab3.Filters
{
    public class PersonActionFilter:IActionFilter
    {
        private readonly ILogger<PersonActionFilter> _logger;
        
        public PersonActionFilter(ILogger<PersonActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var search = context.ActionArguments.ContainsKey("search") ? context.ActionArguments["search"] as string : null;
            if (string.IsNullOrEmpty(search))
            {
                _logger.LogWarning(
                    "[ACTION EXECUTING] Action: {Action} | Validation failed: search parameter is required",
                    context.RouteData.Values["action"]
                );
                context.Result = new BadRequestObjectResult(new { StatusCode = 400, Message = "Search parameter is required" });
                return;
            }
            else
            {
                _logger.LogInformation(
                "[ACTION EXECUTING] Action: {Action} | Search input: {Search}",
                context.RouteData.Values["action"],
                search
            );

            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation(
             "[ACTION EXECUTED] Action: {Action} | StatusCode: {StatusCode} | Completed",
             context.RouteData.Values["action"],
             context.HttpContext.Response.StatusCode
         );
        }
    }
}
