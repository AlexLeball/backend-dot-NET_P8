using Microsoft.AspNetCore.Mvc;
using TourGuide.Models;
using TourGuide.Services.Interfaces;

namespace TourGuide.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;

    public UserController(ITourGuideService tourGuideService)
    {
        _tourGuideService = tourGuideService;
    }

    [HttpGet("{userName}")]
    public ActionResult<UserDto> GetUser(string userName)
    {
        var user = _tourGuideService.GetUser(userName);

        if (user == null)
            return NotFound();

        return Ok(user.ToDto());
    }

    [HttpGet]
    public ActionResult<List<UserDto>> GetAllUsers()
    {
        var users = _tourGuideService
            .GetAllUsers()
            .Select(u => u.ToDto())
            .ToList();

        return Ok(users);
    }
}