# Phoenix

Microsoft Phoenix is a solution for [Windows Azure Pack (WAP)] {http://aka.ms/azurepack} that enables public Azure IaaS provisioning and management via Azure Resource Manager. The extension and resource provider enable a single pane of glass user experience for public and private cloud deployments through WAP.

## Installation & Usage

Follow instructions in the [Admin Guide] {doc/AdminGuide.docx} to install the binaries.

## Using a Azure Subscription with Phoenix

Follow the intructions in the [CSP Subscription Guide] {doc/CSPSubGuide.docx} to prepare a subscription for use with Phoenix.

## Project Information

Phoenix consists of 4 basic components:

* Admin Extension: These are the UI components for the WAP Admin Site.
* Tenant Extension: These are the UI components for the WAP Tenant Site.
* CMP WAP Extension: This is the API site the UI components interact with.
* CMP Service: This is a Windows Service that orchestrates provisioning in ARM.

## Compiling the Project

In order to compile the project, you will need the following components:

* Microsoft.NET 4.5.2 Framework
* Web Platform Installer 5 or later
* Azure SDK 2.5
* Wix Toolset
* DLL's from WAP installation (UR8 or above)

For detailed instructions, see the [Admin Guide] {doc/AdminGuide.docx}

## License

[MIT](LICENSE)