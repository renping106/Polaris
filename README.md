# Polaris.Abp

## Waht is Polaris

Polaris is based on ABP framework (https://docs.abp.io/en/abp/latest/Domain-Driven-Design) and adds some useful modules out of the box. Including: Plugin Management, Theme Management and Database provider selection on live. It is compatible with all ABP modules.

### Pre-requirements

* [.NET 8.0+ SDK](https://dotnet.microsoft.com/download/dotnet)
* [Node v18 or 20](https://nodejs.org/en)
* [ABP Cli latest](https://abp.io/get-started)

### Configurations

No migrations and connection string are needed before running the projects.

### Before running the application

#### Generating a Signing Certificate

In the production environment, you need to use a production signing certificate. ABP Framework sets up signing and encryption certificates in your application and expects an `openiddict.pfx` file in your application.

You need to generate a certificate, you can use the following command:

```bash
dotnet dev-certs https -v -ep openiddict.pfx -p d2d18edd-60f2-410b-8deb-d35be024b0a7
```

> `d2d18edd-60f2-410b-8deb-d35be024b0a7` is the password of the certificate, you can change it to any password you want.

It is recommended to use **two** RSA certificates, distinct from the certificate(s) used for HTTPS: one for encryption, one for signing.

For more information, please refer to: https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html#registering-a-certificate-recommended-for-production-ready-scenarios

> Also, see the [Configuring OpenIddict](https://docs.abp.io/en/abp/latest/Deployment/Configuring-OpenIddict#production-environment) documentation for more information.

#### Install Client-Side Libraries

Run the following command in the directory of Polaris root folder:

```bash
abp install-libs
```

> This command installs all NPM packages for MVC/Razor Pages and Blazor Server UIs and this command is already run by the ABP CLI, so most of the time you don't need to run this command manually.

### Solution structure

Polaris uses a single layer template of ABP framework to make it simple:

* `src\host`: The main application.
* `src\modules\`: Modules to support external features.
* `src\themes`: Theme modules.
* `src\samples`: You can use these packages to upload to test the plugin management.
* `test\`: Test projects.

### Features

* Dynamically load/enable/disable/remove plugin
* Dynamically migrate dbcontext for the plugin without migration assemblies
* Dynamically switch theme for tenant
* Setup on live for host/tenant
* TBD

### Deploying the application

* Publish the compiled dlls and exe to any Web Server that supports ASP.NET Core. 
* Browse the deployed Url and follow the setup page to init you website.

### Inspired by

* [OrchardCore](https://github.com/OrchardCMS/OrchardCore)
* [Oqtane](https://github.com/oqtane/oqtane.framework)

### Additional resources

Check ABP's office docs to see more details for development.

* [Web Application Development Tutorial](https://docs.abp.io/en/abp/latest/Tutorials/Part-1)
* [Application Startup Template Structure](https://docs.abp.io/en/abp/latest/Startup-Templates/Application)
* [ASP.NET Core MVC / Razor Pages: The Basic Theme](https://docs.abp.io/en/abp/latest/UI/AspNetCore/Basic-Theme)
