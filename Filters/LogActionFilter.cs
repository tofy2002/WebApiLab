using Microsoft.AspNetCore.Mvc.Filters;

namespace Lab3.Filters
{
    public class LogActionFilter:IActionFilter
    {
        private readonly ILogger<LogActionFilter> _logger;
        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            var method = context.HttpContext.Request.Method;
            var path = context.HttpContext.Request.Path;

            _logger.LogInformation(
                "[ACTION EXECUTING] Controller: {Controller} | Action: {Action} | Method: {Method} | Path: {Path} | Arguments: {@Arguments}",
                controllerName,
                actionName,
                method,
                path,
                context.ActionArguments
            );
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
           var controllerName = context.RouteData.Values["controller"];
            var actionName = context.RouteData.Values["action"];
            var method = context.HttpContext.Request.Method;
            var path = context.HttpContext.Request.Path;
            var resultType = context.Result?.GetType().Name ?? "No Result";
            _logger.LogInformation(
                "[ACTION EXECUTED] Controller: {Controller} | Action: {Action} | Method: {Method} | Path: {Path} | Result Type: {ResultType}",
                controllerName,
                actionName,
                method,
                path,
                resultType
            );
        }
    }
}
