using System;
using System.Web.Mvc;
using StorageLogic.Exception;

namespace StorageUI.Filters
{
    /// <summary>
    /// Атрибут для отлова исключений типа StorageLogicBaseException и возвращения
    /// спец.ответа со статусом 200. Работает только для JsonResult-методов
    /// </summary>
    public class LogicExceptionFilter : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            // If custom errors are disabled, we need to let the normal ASP.NET exception handler
            // execute so that the user can see useful debugging information.
            if (filterContext.ExceptionHandled)// || !filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            Exception exception = filterContext.Exception;

            if (exception is StorageLogicBaseException)
            {
                string actionName = filterContext.RouteData.Values["action"].ToString();
                Type controllerType = filterContext.Controller.GetType();
                var method = controllerType.GetMethod(actionName);

                var returnType = method.ReturnType;
                
                //If the action that generated the exception returns JSON
                if (returnType == typeof (JsonResult))
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            Error = exception.Message
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };

                    filterContext.ExceptionHandled = true;
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 200;

                    // Certain versions of IIS will sometimes use their own error page when
                    // they detect a server error. Setting this property indicates that we
                    // want it to try to render ASP.NET MVC's error page instead.
                    filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                }                
            }
        }
    }
}