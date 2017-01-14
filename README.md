# Sitecore Dependency Config Reader

By default Sitecore patches config files in the `/App_Config/Include` folder alphabetically, 
which means if you need to make sure your file is patched after a specific default Sitecore config,
then you need to make sure it is sorted after. This is commonly done by prepending one or more z's, e.g. `z.MySettings.config`.

I was wondering if it would be a better solution to be able to specify dependencies on other configs instead.

## Requirements

This config reader require Sitecore 8.1.160302 (8.1 Update-2) or higher.

This is where they moved the logic from `Sitecore.Configuration.Factory` to `Sitecore.Configuration.ConfigReader` and made
the class methods `virtual` so it's possible to override parts of the logic.

## Installation

Just install the NuGet package and it will transform your `Web.config` to replace the default Sitecore `ConfigReader` with this one.

If your `Web.config` is not in the same project, you will have to manually replace it.

```xml
<!-- Web.config -->
<configuration>
  <configSections>
    <!-- Replace the type of this with 'Additio.Configuration.DependencyConfigReader, Additio.Configuration' -->
    <section name="sitecore" type="Sitecore.Configuration.ConfigReader, Sitecore.Kernel" />
    <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, Sitecore.Logging" />
  </configSections>

  <!-- ... -->
</configuration
```

## Usage

You add dependencies for your config by adding a `dependencies` attribute to the root element `configuration` with the name
of the file you are depending on - with or without the `.config` at the end.

```xml
<!-- You don't have to include '.config' (but you can) -->
<configuration dependencies="Sitecore.Analytics">
<configuration dependencies="Sitecore.Analytics.config">

<!-- Wildcard matching (supports '?' and '*') -->
<configuration dependencies="Sitecore.*">

<!-- For files in sub folders specify the relative path (relative to '/App_Config/Include') -->
<configuration dependencies="CES/Sitecore.CES">

<!-- You can specify multiple dependencies by separating them by either ',', ';' or '|' -->
<configuration dependencies="Sitecore.Speak.*, Sitecore.Publishing.*">

```