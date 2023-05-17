using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMediator _mediator;

        public HomeController(ILogger<HomeController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Commands(CancellationToken cancellationToken)
        {
            if (HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name) == null)
            {
                return RedirectToAction("Signin", "Session");
            }

            GetAllCommandsQuery getAllCommandsQuery = new();
            GetAllCommandsQueryResult getAllCommandsQueryResult = await _mediator.Send(getAllCommandsQuery, cancellationToken);

            return View(getAllCommandsQueryResult.Commands);
        }

        public async Task<IActionResult> SocialMedias(CancellationToken cancellationToken)
        {
            if (HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name) == null)
            {
                return RedirectToAction("Signin", "Session");
            }

            GetAllSocialMediasQuery getAllSocialMediasQuery = new();
            GetAllSocialMediasQueryResult getAllCommandsQueryResult = await _mediator.Send(getAllSocialMediasQuery, cancellationToken);

            return View(getAllCommandsQueryResult.SocialMedias);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}