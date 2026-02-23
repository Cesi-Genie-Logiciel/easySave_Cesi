using Microsoft.AspNetCore.Mvc;
using LogCentralizer.Models;
using LogCentralizer.Repositories;

namespace LogCentralizer.Controllers
{
    /// <summary>
    /// Contrôleur API pour recevoir les logs des clients (P4).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly ILogRepository _logRepository;

        public LogController(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        [HttpPost]
        public IActionResult PostLog([FromBody] LogRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ClientId))
                return BadRequest("ClientId is required");

            _logRepository.AppendLog(request.ClientId, request.Entry);
            return Ok();
        }

        [HttpGet("{date}")]
        public IActionResult GetLogs(string date)
        {
            var logs = _logRepository.GetLogs(date);
            return Ok(logs);
        }

        [HttpHead("/api/health")]
        [HttpGet("/api/health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy" });
        }
    }
}
