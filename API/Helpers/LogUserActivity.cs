using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
namespace API.Helpers;
public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next(); //รอให้ next ทำงานเสร็จก่อน
        var user = resultContext.HttpContext.User;
        if (user is null) return;
        if (user.Identity is not null && !user.Identity.IsAuthenticated) return;

        //var username = user.GetUsername();
        var userId = user.GetUserId();
        if (userId is null) return;

        var repository = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        //var userRepository = await repository.GetUserByUserNameAsync(username);
        var userRepository = await repository.GetUserByIdAsync((int)userId);
        if (userRepository is null) return;

        userRepository.LastActive = DateTime.UtcNow;
        await repository.SaveAllAsync();
    }
}