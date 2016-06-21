# Azure Pack Connector

Azure Pack Connector (codename Phoenix) is a solution for [Azure Pack] (http://aka.ms/azurepack) that enables public Azure IaaS provisioning and management via Azure Resource Manager. The extension and resource provider enable a single pane of glass user experience for public and private cloud deployments through Azure Pack.

## Installation & Usage

Follow instructions in the [Admin Guide] (doc/AdminGuide.docx) to install the binaries.

## Using a Azure Subscription with Azure Pack Connector

Follow the intructions in the [CSP Subscription Guide] (doc/CSPSubGuide.docx) to prepare a subscription for use with Azure Pack Connector.

## Project Information

Azure Pack Connector consists of 4 basic components:

* Admin Extension: These are the UI components for the Azure Pack Admin Site.
* Tenant Extension: These are the UI components for the Azure Pack Tenant Site.
* CMP WAP Extension: This is the API site the UI components interact with.
* CMP Service: This is a Windows Service that orchestrates provisioning in ARM.

## Compiling the Project

In order to compile the project, you will need the following components:

* Microsoft.NET 4.5.2 Framework
* Web Platform Installer 5 or later
* Azure SDK 2.5
* Wix Toolset
* DLL's from Azure Pack installation (UR8 or above)

For detailed instructions, see the [Admin Guide] (doc/AdminGuide.docx)

## License

[MIT](LICENSE)


This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
