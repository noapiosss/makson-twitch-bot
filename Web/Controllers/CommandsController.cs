using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Database;
using Contracts.Http;
using Domain.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [Route("api")]
    public class PostController : BaseController
    {
        private readonly IMediator _mediator;

        public PostController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("addCommand")]
        public Task<IActionResult> AddCommandEndpoint([FromBody] AddCommandRequest request, CancellationToken cancellationToken)
        {
            return SafeExecute(async () =>
            {
                if (HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name) == null)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Code = ErrorCode.Unauthorized,
                        Message = "current request require authorization"
                    });
                }

                _ = Enum.TryParse(request.CommandType, out CommandType commandType);
                Command command = new()
                {
                    CommandName = request.CommandName,
                    CommandType = commandType,
                    CommandOutput = request.CommandOutput
                };

                AddCommand addCommand = new() { Command = command };
                AddCommandResult addCommandResult = await _mediator.Send(addCommand, cancellationToken);

                if (!addCommandResult.CommandIsAdded)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Code = ErrorCode.AlreadyExists,
                        Message = "command already exists"
                    });
                }

                AddCommandResponse response = new() { CommandIsAdded = addCommandResult.CommandIsAdded };

                return Ok(response);
            }, cancellationToken);
        }

        [HttpDelete("deleteCommand")]
        public Task<IActionResult> DeleteCommandEndpoint([FromBody] DeleteCommandRequest request, CancellationToken cancellationToken)
        {
            return SafeExecute(async () =>
            {
                if (HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name) == null)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Code = ErrorCode.Unauthorized,
                        Message = "current request require authorization"
                    });
                }

                DeleteCommand deleteCommand = new() { CommandName = request.CommandName };
                DeleteCommandResult deleteCommandResult = await _mediator.Send(deleteCommand, cancellationToken);

                if (!deleteCommandResult.CommandIsDeleted)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Code = ErrorCode.NotFound,
                        Message = "command not found"
                    });
                }

                DeleteCommandResponse response = new() { DeleteIsSuccessful = deleteCommandResult.CommandIsDeleted };

                return Ok(response);
            }, cancellationToken);
        }
    }
}