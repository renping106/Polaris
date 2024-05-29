using Microsoft.AspNetCore.Mvc;
using Nerd.Abp.DatabaseManagement.Services.Dtos;
using Nerd.Abp.DatabaseManagement.Services.Interfaces;
using Volo.Abp.MultiTenancy;

namespace Nerd.Abp.DatabaseManagement.Pages.Setup;

public class InstallModel : DatabaseManagementPageModel
{
    [BindProperty]
    public SetupViewModel Config { get; set; } = new SetupViewModel();
    public List<DatabaseProviderDto> DatabaseProviders { get; set; } = new List<DatabaseProviderDto>();
    public string TenantName { get; set; } = string.Empty;

    private readonly ICurrentTenant _currentTenant;
    private readonly ISetupAppService _setupAppService;

    public InstallModel(ICurrentTenant currentTenant, ISetupAppService setupStatusAppService)
    {
        _currentTenant = currentTenant;
        _setupAppService = setupStatusAppService;
    }

    public async Task<IActionResult> OnGet([FromQuery(Name = "tenant")]string tenantId)
    {
        var (Valid, TenantId) = await CheckTenant(tenantId);

        using (_currentTenant.Change(TenantId))
        {
            if (!Valid)
            {
                return NotFound();
            }

            if (_setupAppService.IsInitialized(TenantId))
            {
                return NotFound();
            }

            DatabaseProviders = _setupAppService.GetSupportedDatabaseProviders().ToList();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync([FromQuery(Name = "tenant")] string tenantId)
    {
        var (Valid, TenantId) = await CheckTenant(tenantId);

        using (_currentTenant.Change(TenantId))
        {
            if (!Valid)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            //    var databaseProvider = _databaseProviderResolver.GetDatabaseProvider(Config.DatabaseProvider);
            //    try
            //    {
            //        var config = ObjectMapper.Map<SetupViewModel, InstallationConfigDto>(Config);
            //        var connectionCheck = false;

            //        if (_installationAppService.IsUseHostSetting(config))
            //        {
            //            connectionCheck = true;
            //        }
            //        else
            //        {
            //            var result = await databaseProvider.CheckConnectionString(Config.ConnectionString);
            //            connectionCheck = result.Connected;
            //        }

            //        if (connectionCheck)
            //        {
            //            var installError = await _installationAppService.Install(config);
            //            if (installError.Success)
            //            {
            //                return Redirect("/");
            //            }
            //            else
            //            {
            //                ModelState.AddModelError(string.Empty, installError.Message);
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        ModelState.AddModelError("Config.ConnectionString", ex.Message);
            //    }
            //}

            //DatabaseProviders = _installationAppService.GetSupportedDatabaseProviders().Items.ToList();
        }

        return Page();
    }

    private async Task<(bool Valid, Guid? TenantId)> CheckTenant(string tenantId)
    {
        Guid? tenantGuid = GetTenantId(tenantId);
        bool validTenantId = false;

        if (tenantGuid.HasValue)
        {
            try
            {
                //var tenant = await _tenantAppService.GetAsync(tenantGuid.Value);
                validTenantId = true;
                //TenantName = $"Tenant {tenant.Name}";
            }
            catch (Exception) { }
        }
        else if (tenantId == null)
        {
            TenantName = "Host";
            validTenantId = true;
        }

        return (validTenantId, tenantGuid);
    }

    private static Guid? GetTenantId(string tenantId)
    {
        Guid? tenantGuid = null;
        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            if (Guid.TryParse(tenantId, out var id))
            {
                tenantGuid = id;
            }
        }

        return tenantGuid;
    }
}


