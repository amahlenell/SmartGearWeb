using Microsoft.AspNetCore.Mvc.Filters;

namespace SmartGearWeb.Filters
{
    // Logs the controller/action name before and after it runs.
    // Applied to ProductController actions to demonstrate an action filter.
    public class LogActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<LogActionFilterAttribute>>();

            var controller = context.ActionDescriptor.RouteValues["controller"];
            var action = context.ActionDescriptor.RouteValues["action"];

            logger.LogInformation("Action starting: {Controller}.{Action}", controller, action);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILogger<LogActionFilterAttribute>>();

            var controller = context.ActionDescriptor.RouteValues["controller"];
            var action = context.ActionDescriptor.RouteValues["action"];

            logger.LogInformation("Action finished: {Controller}.{Action}", controller, action);
        }
    }
}
