using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tracker.Common;
using Tracker.Common.Progress;

namespace Tracker.Web.Sandbox
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ProgressableTestService _progressableTestService;
        private readonly ProgressableParamsTestService _progressableParamsTestService;

        public TestController(ProgressableTestService progressableTestService,
            ProgressableParamsTestService progressableParamsTestService)
        {
            _progressableTestService = progressableTestService;
            _progressableParamsTestService = progressableParamsTestService;
        }

        [HttpPost("run-progressable-job")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RunProgressableJob(ProgressRm model)
        {
            await _progressableTestService.RunJob(model.SocketInfo, model.TaskId);
            return Ok();
        }

        [HttpPost("run-progressable-job-with-params")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RunProgressableJobWithParams(ProgressRm<TestJobParams> model)
        {
            await _progressableParamsTestService.RunJob(model.SocketInfo, model.Pars, model.TaskId);
            return Ok();
        }

        [HttpPost("run-unprogressable-job")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RunUnProgressableJob()
        {
            Thread.Sleep(5000);
            return Ok();
        }

        [HttpPost("run-unprogressable-job-with-params")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> RunUnProgressableJobWithParams(TestJobParams model)
        {
            Thread.Sleep(5000);
            return Ok();
        }
    }
}
