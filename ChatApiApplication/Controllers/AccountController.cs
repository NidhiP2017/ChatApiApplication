using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }


    [HttpGet("LoginWithGoogle")]
    public IActionResult LoginWithGoogle()
    {
        string redirectUrl = Url.Action(nameof(GoogleResponse), "Account");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("GoogleResponse")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (!result.Succeeded)
            return BadRequest();

        var externalUser = result.Principal;
        var userEmail = externalUser.FindFirst(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        var user = await _signInManager.UserManager.FindByEmailAsync(userEmail);

        if (user == null)
        {
            user = new IdentityUser { UserName = userEmail, Email = userEmail };
            await _signInManager.UserManager.CreateAsync(user);

            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                throw new InvalidOperationException("Error retrieving external login information.");
            }

            var claim = result.Principal.Identities.First().FindFirst(ClaimTypes.NameIdentifier);
            var loginInfo = new UserLoginInfo(externalLoginInfo.LoginProvider, claim.Value, externalLoginInfo.ProviderDisplayName);

            await _signInManager.UserManager.AddLoginAsync(user, loginInfo);
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return Ok("Logged in with Google");
    }
}