using Microsoft.AspNetCore.Mvc;

namespace MyPersonalizedTodos.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
    }
}
