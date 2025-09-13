using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AuthorizeRoleAttribute : ActionFilterAttribute
{
    private readonly string[] _roles;

    public AuthorizeRoleAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var tipoUtilizadores = context.HttpContext.Session.GetString("TipoUtilizadores");

        if (string.IsNullOrEmpty(tipoUtilizadores) || !_roles.Contains(tipoUtilizadores))
        {
            context.Result = new RedirectToActionResult("acessoNegado", "Account", null);
        }
    }
}
