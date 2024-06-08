using System;
using System.Collections.Generic;
using System.Text;
using Nerd.BookStore.Localization;
using Volo.Abp.Application.Services;

namespace Nerd.BookStore;

/* Inherit your application services from this class.
 */
public abstract class BookStoreAppService : ApplicationService
{
    protected BookStoreAppService()
    {
        LocalizationResource = typeof(BookStoreResource);
    }
}
