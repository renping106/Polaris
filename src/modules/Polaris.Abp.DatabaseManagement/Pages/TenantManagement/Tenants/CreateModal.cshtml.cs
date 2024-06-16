using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.ObjectExtending;
using Volo.Abp.TenantManagement;
using Volo.Abp.Validation;

namespace Polaris.Abp.DatabaseManagement.Pages.TenantManagement.Tenants;

public class CreateModalModel(ITenantAppService tenantAppService) : DatabaseManagementPageModel
{
    [BindProperty]
    public TenantInfoModel Tenant { get; set; } = new TenantInfoModel();

    protected ITenantAppService TenantAppService { get; } = tenantAppService;

    public virtual Task<IActionResult> OnGetAsync()
    {

        return Task.FromResult<IActionResult>(Page());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();

        var input = ObjectMapper.Map<TenantInfoModel, TenantCreateDto>(Tenant);
        await TenantAppService.CreateAsync(input);

        return NoContent();
    }

    public class TenantInfoModel : ExtensibleObject
    {
        [Required]
        [DynamicStringLength(typeof(TenantConsts), nameof(TenantConsts.MaxNameLength))]
        [Display(Name = "Tenant Name")]
        public string Name { get; set; } = string.Empty;

    }
}
