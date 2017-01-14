
# DependencyConfigReader

For Sitecore to use the dependency config reader you have to change the type of 'configuration/configSections/section[name=sitecore]'
in web.config to 'Additio.Configuration.DepencdencyConfigReader, Additio.Configuration'.

This package should have already done this for you, if it was installed in the same project as your web.config - otherwise you have to do it manually.



<configuration>
  <configSections>
    <!-- Change the type of this to 'Additio.Configuration.DepencdencyConfigReader, Additio.Configuration' -->
    <section name="sitecore" type="Sitecore.Configuration.ConfigReader, Sitecore.Kernel" />
  </configSections>

  <!-- ... -->
</configuration