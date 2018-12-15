namespace WavesBot.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using Telegram.Bot.Types;

    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly StateSynchronizer stateSynchronizer;

        public BotController(StateSynchronizer stateSynchronizer)
        {
            this.stateSynchronizer = stateSynchronizer;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            await stateSynchronizer.ResolveState(update);
           
            return Ok();
        }
    }
}