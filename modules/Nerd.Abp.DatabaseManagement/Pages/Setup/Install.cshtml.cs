using Microsoft.AspNetCore.Mvc;
using Nerd.Abp.DatabaseManagement.Services.Dtos;
using Nerd.Abp.DatabaseManagement.Services.Interfaces;
using Volo.Abp.MultiTenancy;

namespace Nerd.Abp.DatabaseManagement.Pages.Setup;

public class InstallModel : DatabaseManagementPageModel
{
    [BindProperty]
    public SetupViewModel Config { get; set; } = new SetupViewModel();
    public List<DatabaseProviderDto> DatabaseProviders { get; set; }

    private readonly ICurrentTenant _currentTenant;
    private readonly ISetupAppService _setupAppService;

    public InstallModel(ICurrentTenant currentTenant, ISetupAppService setupStatusAppService)
    {
        _currentTenant = currentTenant;
        _setupAppService = setupStatusAppService;
        DatabaseProviders = _setupAppService.GetSupportedDatabaseProviders().ToList();
    }

    public IActionResult OnGet([FromQuery(Name = "tenant")] string? tenantId)
    {
        var tenantGuid = GetTenantId(tenantId);

        using (_currentTenant.Change(tenantGuid))
        {
            if (_setupAppService.IsInitialized(tenantGuid))
            {
                return NotFound();
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync([FromQuery(Name = "tenant")] string? tenantId)
    {
        var tenantGuid = GetTenantId(tenantId);

        using (_currentTenant.Change(tenantGuid))
        {
            if (_setupAppService.IsInitialized(tenantGuid))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var setupInput = ObjectMapper.Map<SetupViewModel, SetupInputDto>(Config);
                    await _setupAppService.InstallAsync(setupInput);
                    return Redirect("/");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
        }

        return Page();
    }

    private static Guid? GetTenantId(string? tenantId)
    {
        Guid? tenantGuid = null;
        if (!string.IsNullOrWhiteSpace(tenantId) && Guid.TryParse(tenantId, out var id))
        {
            tenantGuid = id;
        }

        return tenantGuid;
    }
}


