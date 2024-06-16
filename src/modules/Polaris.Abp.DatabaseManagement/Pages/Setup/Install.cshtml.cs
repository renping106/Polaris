using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Polaris.Abp.DatabaseManagement.Services.Dtos;
using Polaris.Abp.DatabaseManagement.Services.Interfaces;

namespace Polaris.Abp.DatabaseManagement.Pages.Setup;

public class InstallModel : DatabaseManagementPageModel
{
    [BindProperty]
    public SetupViewModel Config { get; set; } = new SetupViewModel();
    public List<DatabaseProviderDto> DatabaseProviders { get; set; }
    public List<SelectListItem> TimeZoneItems { get; set; } = [];
    public bool ShowUseHostSetting { get; set; } = true;

    private readonly ISetupAppService _setupAppService;

    public InstallModel(ISetupAppService setupStatusAppService)
    {
        _setupAppService = setupStatusAppService;
        DatabaseProviders = [.. _setupAppService.GetSupportedDatabaseProviders()];
        GetTimezoneItems();
    }

    public async Task<IActionResult> OnGet([FromQuery(Name = "tenant")] Guid? tenantId)
    {
        if (_setupAppService.IsInitialized(tenantId))
        {
            return NotFound();
        }

        if (tenantId.HasValue)
        {
            var flag = await _setupAppService.TenantExistsAsync(tenantId.Value);
            if (!flag)
            {
                return NotFound();
            }
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

    private void GetTimezoneItems()
    {
        TimeZoneItems = [];
        var timezones = _setupAppService.GetTimeZonesAsync().Result;
        TimeZoneItems.AddRange(timezones.Select(x => new SelectListItem(x.Name, x.Value)).ToList());
    }
}


