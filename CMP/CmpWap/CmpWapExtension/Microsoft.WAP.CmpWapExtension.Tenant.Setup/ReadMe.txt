Readme for Microsoft.WindowsAzurePack.CmpWapExtension.Setup.wixproj

PRE-REQUISITES:

This project uses the Windows Installer XML (WiX) toolset 3.6.3303.0.
Download the WiX Toolset v3.6 from http://wix.codeplex.com/ and install.
Direct link to download WiX36.exe: http://wix.codeplex.com/downloads/get/482065


"%ProgramFiles(x86)%\WiX Toolset v3.6\bin\heat.exe" dir "..\AdminExtension"  -dr AdminSiteContentExtDir  -gg -scom -sfrag -srd -sreg -suid -var "var.AdminExtensionRoot"  -out "AdminSite.temp.wxi"
"%ProgramFiles(x86)%\WiX Toolset v3.6\bin\heat.exe" dir "..\TenantExtension" -dr TenantSiteContentExtDir -gg -scom -sfrag -srd -sreg -suid -var "var.TenantExtensionRoot" -out "TenantSite.temp.wxi"
"%ProgramFiles(x86)%\WiX Toolset v3.6\bin\heat.exe" dir "..\Api"             -dr WEBSITEDIR              -gg -scom -sfrag -srd -sreg -suid -var "var.WebSiteRoot"         -out "WebSiteContent.temp.wxi"

