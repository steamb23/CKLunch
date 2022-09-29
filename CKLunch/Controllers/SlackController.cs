using Microsoft.AspNetCore.Mvc;

namespace CKLunch.Controllers;

[ApiController]
[Route("Slack")]
public class SlackController : ControllerBase
{
    [Route("Slash")]
    [HttpPost]
    public object Slash([FromForm(Name = "text")] string text)
    {
        return new
        {
            response_type = "in_channel",
            text = "테스트"
        };
    }
}

public class SlackResponse
{
}