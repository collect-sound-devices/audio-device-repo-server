using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeviceRepoAspNetCore.Pages;

public class PrivacyModel(ILogger<PrivacyModel> logger) : PageModel
{
    // ReSharper disable once NotAccessedField.Local
    // ReSharper disable once UnusedMember.Local
    private readonly ILogger<PrivacyModel> _logger = logger;

    public void OnGet()
    {
    }
}

