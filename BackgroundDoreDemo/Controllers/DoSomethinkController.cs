using BackgroundDoreDemo.BackGrounProcess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundDoreDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DoSomethinkController : ControllerBase
    {
        private readonly ILogger<DoSomethinkController> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;

        private string _data { get; set; }
        public DoSomethinkController(ILogger<DoSomethinkController> logger, IBackgroundTaskQueue taskQueue)
        {
            _logger = logger;
            _taskQueue = taskQueue;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string data)
        {
            try
            {
                _data = data;
               _taskQueue.QueueBackgroundWorkItem(async token => {
                   bool complete = false;
                   while (!token.IsCancellationRequested && !complete)
                   {
                       for (int i = 0; i < _data.Length; i++)
                       {
                           await Task.Delay(TimeSpan.FromSeconds(2), token);
                           _logger.LogInformation($"Loop: {_data[i]}");
                       }
                       complete = true;

                   }
               });
                return Accepted("Task Accepted");
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "InvalidOperationException en Post");
                return StatusCode(StatusCodes.Status400BadRequest, e);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception en Post");
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }       
    }
}
