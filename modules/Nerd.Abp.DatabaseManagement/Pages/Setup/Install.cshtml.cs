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
    public bool ShowUseHostSetting { get; set; } = true;

    private readonly ICurrentTenant _currentTenant;
    private readonly ISetupAppService _setupAppService;

    public InstallModel(ICurrentTenant currentTenant, ISetupAppService setupStatusAppService)
    {
        _currentTenant = currentTenant;
        _setupAppService = setupStatusAppService;
        DatabaseProviders = _setupAppService.GetSupportedDatabaseProviders().ToList();
    }

    public IActionResult OnGet([FromQuery(Name = "tenant")] Guid? tenantId)
    {
        if (_setupAppService.IsInitialized(tenantId))
        {
            return NotFound();
        }

        if (!tenantId.HasValue)
        {
            DatabaseProviders = DatabaseProviders.Where(p => p.HasConnectionString).ToList();
            Config.UseHostSetting = false;
            ShowUseHostSetting = false;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync([FromQuery(Name = "tenant")] Guid? tenantId)
    {
        if (_setupAppService.IsInitialized(tenantId))
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var setupInput = ObjectMapper.Map<SetupViewModel, SetupInputDto>(Config);
                await _setupAppService.InstallAsync(setupInput, tenantId);
                return Redirect("/");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        }

        if (!tenantId.HasValue)
        {
            Config.UseHostSetting = false;
            ShowUseHostSetting = false;
        }

        return Page();
    }
}


