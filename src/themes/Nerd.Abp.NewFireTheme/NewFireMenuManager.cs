using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.SimpleStateChecking;
using Volo.Abp.UI.Navigation;

namespace Nerd.Abp.NewFireTheme
{
    [Dependency(ReplaceServices = true)]
    public class NewFireMenuManager : MenuManager
    {
        private static readonly List<string> _icons = new List<string>()
        {
            "crop",
            "compass",
            "fax",
            "flag",
            "folder",
            "flag",
            "envelope",
            "edit",
            "database",
            "hashtag"
        };

        public NewFireMenuManager(
            IOptions<AbpNavigationOptions> options,
            IServiceScopeFactory serviceScopeFactory,
            ISimpleStateCheckerManager<ApplicationMenuItem> simpleStateCheckerManager)
            : base(options, serviceScopeFactory, simpleStateCheckerManager)
        {
        }

        protected override async Task<ApplicationMenu> GetAsync(params string[] menuNames)
        {
            var result = await base.GetAsync(menuNames);

            FillIcons(result);

            return result;
        }

        private void FillIcons(ApplicationMenu menu)
        {
            var items = new List<ApplicationMenuItem>();
            GetAllMenuItems(menu, items);

            var index = 0;
            foreach (var menuItem in items)
            {
                if (menuItem.Icon == null)
                {
                    menuItem.Icon = $"fa fa-{_icons[index]}";
                    index++;
                }

                if (index > _icons.Count)
                {
                    index = 0;
                }
            }
        }
    }
}
