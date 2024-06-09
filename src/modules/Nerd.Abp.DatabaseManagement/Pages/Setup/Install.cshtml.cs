using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nerd.Abp.DatabaseManagement.Services.Dtos;
using Nerd.Abp.DatabaseManagement.Services.Interfaces;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace Nerd.Abp.DatabaseManagement.Pages.Setup;

public class InstallModel : DatabaseManagementPageModel
{
    [BindProperty]
    public SetupViewModel Config { get; set; } = new SetupViewModel();
    public List<DatabaseProviderDto> DatabaseProviders { get; set; }
    public List<SelectListItem> TimeZoneItems { get; set; }
    public bool ShowUseHostSetting { get; set; } = true;

    private readonly ISetupAppService _setupAppService;

    public InstallModel(ISetupAppService setupStatusAppService)
    {
        _setupAppService = setupStatusAppService;
        DatabaseProviders = _setupAppService.GetSupportedDatabaseProviders().ToList();
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
        TimeZoneItems = new List<SelectListItem>();
        var timezones = _setupAppService.GetTimezonesAsync().Result;
        TimeZoneItems.AddRange(timezones.Select(x => new SelectListItem(x.Name, x.Value)).ToList());
    }
}


