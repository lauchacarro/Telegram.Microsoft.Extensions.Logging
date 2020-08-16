using System;

using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.Logging.Telegram.Sample.Web.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly ILogger<SampleController> _logger;

        public SampleController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SampleController>();
        }

        [HttpGet]
        public IActionResult Run()
        {
            for (int i = 0; i < 30; i++)
            {
                _logger.LogInformation($"Test {i}");
            }

            try
            {
                throw new Exception("Throw Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal Server Error.");
            }

            return Ok();
        }
    }
}
