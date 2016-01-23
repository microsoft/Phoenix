--------------------------------------------------
-- Insert/Update/Delete script for table VmOs
--------------------------------------------------
 
 
SET NOCOUNT ON
--SET IDENTITY_INSERT VmOs ON
 
CREATE TABLE #WorkTable (
[VmOsId] [int] NOT NULL
, [Name] [nvarchar] (250) NULL
, [Description] [nvarchar] (500) NULL
, [OsFamily] [nvarchar] (1000) NULL
, [AzureImageName] [varchar] (max) NULL
, [IsCustomImage] [bit] NOT NULL
, [IsActive] [bit]  NOT NULL
, [CreatedOn] [datetime]  NOT NULL
, [CreatedBy] [nvarchar] (256) NOT NULL
, [LastUpdatedOn] [datetime]  NOT NULL
, [LastUpdatedBy] [varchar] (50) NOT NULL
, [AzureImagePublisher] [varchar](100) NULL
, [AzureImageOffer] [varchar](100) NULL
, [AzureWindowsOSVersion] [varchar](100) NULL
)
GO
 
DECLARE 
     @vInsertedRows INT
    , @vUpdatedRows INT
    , @vDeletedRows INT
    , @vNow         DATETIME
 
SELECT @vNow = GETDATE()
 
--------------------------------------------------
-- Populate base temp table. 
--------------------------------------------------

INSERT #WorkTable 
(VmOsId, Name, Description, OsFamily, AzureImageName, IsCustomImage, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy, AzureImagePublisher, AzureImageOffer, AzureWindowsOSVersion)
SELECT 7, N'4psa, voipnow, vnp360-single', N'NULL', N'NULL', N'NULL', 0, 1, N'2015-11-08 06:41:36.483' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.483' AS DateTime, N'CMP WAP Extension Installer', N'4psa', N'voipnow', N'vnp360-single'
UNION ALL
SELECT 8, N'4ward365, 4ward365, 4ward365_base', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.500' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.500' AS DateTime, N'CMP WAP Extension Installer', N'4ward365', N'4ward365', N'4ward365_base'
UNION ALL
SELECT 9, N'7isolutions, sapp-project-server-2015-05, sapp-project-server-2015-06', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.517' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.517' AS DateTime, N'CMP WAP Extension Installer', N'7isolutions', N'sapp-project-server-2015-05', N'sapp-project-server-2015-06'
UNION ALL
SELECT 10, N'a10networks, a10-vthunder-adc, vthunder_100mbps', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.517' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.517' AS DateTime, N'CMP WAP Extension Installer', N'a10networks', N'a10-vthunder-adc', N'vthunder_100mbps'
UNION ALL
SELECT 11, N'a10networks, a10-vthunder-adc, vthunder_10mbps', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.530' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.530' AS DateTime, N'CMP WAP Extension Installer', N'a10networks', N'a10-vthunder-adc', N'vthunder_10mbps'
UNION ALL
SELECT 12, N'a10networks, a10-vthunder-adc, vthunder_200mbps', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.530' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.530' AS DateTime, N'CMP WAP Extension Installer', N'a10networks', N'a10-vthunder-adc', N'vthunder_200mbps'
UNION ALL
SELECT 13, N'a10networks, a10-vthunder-adc, vthunder_500mbps', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.547' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.547' AS DateTime, N'CMP WAP Extension Installer', N'a10networks', N'a10-vthunder-adc', N'vthunder_500mbps'
UNION ALL
SELECT 14, N'a10networks, a10-vthunder-adc, vthunder_50mbps', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.563' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.563' AS DateTime, N'CMP WAP Extension Installer', N'a10networks', N'a10-vthunder-adc', N'vthunder_50mbps'
UNION ALL
SELECT 15, N'a10networks, a10-vthunder-adc, vthunder_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.563' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.563' AS DateTime, N'CMP WAP Extension Installer', N'a10networks', N'a10-vthunder-adc', N'vthunder_byol'
UNION ALL
SELECT 16, N'abiquo, abiquo-hybrid-cloud-34, abiquo-340-monolithic', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.577' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.577' AS DateTime, N'CMP WAP Extension Installer', N'abiquo', N'abiquo-hybrid-cloud-34', N'abiquo-340-monolithic'
UNION ALL
SELECT 17, N'active-navigation, an_discovery_center_azure_starter, an_disc_msaz_starter', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.593' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.593' AS DateTime, N'CMP WAP Extension Installer', N'active-navigation', N'an_discovery_center_azure_starter', N'an_disc_msaz_starter'
UNION ALL
SELECT 18, N'activeeon, activeeon-workload-scheduler, free', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.593' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.593' AS DateTime, N'CMP WAP Extension Installer', N'activeeon', N'activeeon-workload-scheduler', N'free'
UNION ALL
SELECT 19, N'adam-software, adam, adamsoftware', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.623' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.623' AS DateTime, N'CMP WAP Extension Installer', N'adam-software', N'adam', N'adamsoftware'
UNION ALL
SELECT 20, N'adobe, adobe_aem, adobeaem', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.623' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.623' AS DateTime, N'CMP WAP Extension Installer', N'adobe', N'adobe_aem', N'adobeaem'
UNION ALL
SELECT 21, N'adobe, adobe_campaign_test, adobe_camp_test', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.640' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.640' AS DateTime, N'CMP WAP Extension Installer', N'adobe', N'adobe_campaign_test', N'adobe_camp_test'
UNION ALL
SELECT 22, N'advantech, susiaccess30, std', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.657' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.657' AS DateTime, N'CMP WAP Extension Installer', N'advantech', N'susiaccess30', N'std'
UNION ALL
SELECT 23, N'advantech-webaccess, webaccess-8_0_1, 1500tags', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.657' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.657' AS DateTime, N'CMP WAP Extension Installer', N'advantech-webaccess', N'webaccess-8_0_1', N'1500tags'
UNION ALL
SELECT 24, N'advantech-webaccess, webaccess-8_0_1, ver8_0_0626', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.670' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.670' AS DateTime, N'CMP WAP Extension Installer', N'advantech-webaccess', N'webaccess-8_0_1', N'ver8_0_0626'
UNION ALL
SELECT 25, N'aerospike, aerospike-database-vm, aerospikedb001', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.687' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.687' AS DateTime, N'CMP WAP Extension Installer', N'aerospike', N'aerospike-database-vm', N'aerospikedb001'
UNION ALL
SELECT 26, N'aimsinnovation, aims-for-biztalk, aimsforbiztalkenterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.687' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.687' AS DateTime, N'CMP WAP Extension Installer', N'aimsinnovation', N'aims-for-biztalk', N'aimsforbiztalkenterprise'
UNION ALL
SELECT 27, N'aimsinnovation, aims-for-biztalk, aimsforbiztalkprofessional', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.703' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.703' AS DateTime, N'CMP WAP Extension Installer', N'aimsinnovation', N'aims-for-biztalk', N'aimsforbiztalkprofessional'
UNION ALL
SELECT 28, N'aiscaler-cache-control-ddos-and-url-rewriting-, aimobile-site-acceleration, byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.717' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.717' AS DateTime, N'CMP WAP Extension Installer', N'aiscaler-cache-control-ddos-and-url-rewriting-', N'aimobile-site-acceleration', N'byol'
UNION ALL
SELECT 29, N'aiscaler-cache-control-ddos-and-url-rewriting-, aimobile-site-acceleration, hourly', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.733' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.733' AS DateTime, N'CMP WAP Extension Installer', N'aiscaler-cache-control-ddos-and-url-rewriting-', N'aimobile-site-acceleration', N'hourly'
UNION ALL
SELECT 30, N'aiscaler-cache-control-ddos-and-url-rewriting-, aiprotect-ddos-firewall, byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.750' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.750' AS DateTime, N'CMP WAP Extension Installer', N'aiscaler-cache-control-ddos-and-url-rewriting-', N'aiprotect-ddos-firewall', N'byol'
UNION ALL
SELECT 31, N'aiscaler-cache-control-ddos-and-url-rewriting-, aiprotect-ddos-firewall, hourly', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.750' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.750' AS DateTime, N'CMP WAP Extension Installer', N'aiscaler-cache-control-ddos-and-url-rewriting-', N'aiprotect-ddos-firewall', N'hourly'
UNION ALL
SELECT 32, N'aiscaler-cache-control-ddos-and-url-rewriting-, aiscaler-traffic-manager-caching, byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.780' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.780' AS DateTime, N'CMP WAP Extension Installer', N'aiscaler-cache-control-ddos-and-url-rewriting-', N'aiscaler-traffic-manager-caching', N'byol'
UNION ALL
SELECT 33, N'aiscaler-cache-control-ddos-and-url-rewriting-, aiscaler-traffic-manager-caching, hourly', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.797' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.797' AS DateTime, N'CMP WAP Extension Installer', N'aiscaler-cache-control-ddos-and-url-rewriting-', N'aiscaler-traffic-manager-caching', N'hourly'
UNION ALL
SELECT 34, N'alachisoft, ncache-opensource, ncache-opensource', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.797' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.797' AS DateTime, N'CMP WAP Extension Installer', N'alachisoft', N'ncache-opensource', N'ncache-opensource'
UNION ALL
SELECT 35, N'alertlogic, alert-logic-tm, 20215000100-tmpbyol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.813' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.813' AS DateTime, N'CMP WAP Extension Installer', N'alertlogic', N'alert-logic-tm', N'20215000100-tmpbyol'
UNION ALL
SELECT 36, N'alertlogic, alert-logic-wsm, 20216000100-wsmpbyl', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.843' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.843' AS DateTime, N'CMP WAP Extension Installer', N'alertlogic', N'alert-logic-wsm', N'20216000100-wsmpbyl'
UNION ALL
SELECT 37, N'algebraix-data, algebraix-analytics, algebraixanalytics_std', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.843' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.843' AS DateTime, N'CMP WAP Extension Installer', N'algebraix-data', N'algebraix-analytics', N'algebraixanalytics_std'
UNION ALL
SELECT 38, N'algebraix-data, algebraix-analytics-enterprise, algebraixanalytics_ent', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.860' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.860' AS DateTime, N'CMP WAP Extension Installer', N'algebraix-data', N'algebraix-analytics-enterprise', N'algebraixanalytics_ent'
UNION ALL
SELECT 39, N'alldigital-brevity, alldigital-brevity-microsoft-media-services, brevity-uploader-with-microsoft-med', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.873' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.873' AS DateTime, N'CMP WAP Extension Installer', N'alldigital-brevity', N'alldigital-brevity-microsoft-media-services', N'brevity-uploader-with-microsoft-media-services'
UNION ALL
SELECT 40, N'alldigital-brevity, alldigital-brevity-uploader, brevity-uploader', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.873' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.873' AS DateTime, N'CMP WAP Extension Installer', N'alldigital-brevity', N'alldigital-brevity-uploader', N'brevity-uploader'
UNION ALL
SELECT 41, N'altiar, ecm, enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.890' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.890' AS DateTime, N'CMP WAP Extension Installer', N'altiar', N'ecm', N'enterprise'
UNION ALL
SELECT 42, N'appcitoinc, appcito-pep-marketplace, apcto-pep-azure', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.907' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.907' AS DateTime, N'CMP WAP Extension Installer', N'appcitoinc', N'appcito-pep-marketplace', N'apcto-pep-azure'
UNION ALL
SELECT 43, N'appex-networks, cloudexpress, standard-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.920' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.920' AS DateTime, N'CMP WAP Extension Installer', N'appex-networks', N'cloudexpress', N'standard-byol'
UNION ALL
SELECT 44, N'appistry, genomepilot, onebox-genome-pilot', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.920' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.920' AS DateTime, N'CMP WAP Extension Installer', N'appistry', N'genomepilot', N'onebox-genome-pilot'
UNION ALL
SELECT 45, N'apprenda, win2k12r2_dcenter_apprenda_5_5_exp_sql_2012_exp, apprenda_paas_5-5_express', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.937' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.937' AS DateTime, N'CMP WAP Extension Installer', N'apprenda', N'win2k12r2_dcenter_apprenda_5_5_exp_sql_2012_exp', N'apprenda_paas_5-5_express'
UNION ALL
SELECT 46, N'appveyorci, appveyor-ci, express', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.953' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.953' AS DateTime, N'CMP WAP Extension Installer', N'appveyorci', N'appveyor-ci', N'express'
UNION ALL
SELECT 47, N'appzero, appzero, appzero', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.953' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.953' AS DateTime, N'CMP WAP Extension Installer', N'appzero', N'appzero', N'appzero'
UNION ALL
SELECT 48, N'aranUNION ALLdb, aranUNION ALLdb, aranUNION ALLdb254', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.967' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.967' AS DateTime, N'CMP WAP Extension Installer', N'aranUNION ALLdb', N'aranUNION ALLdb', N'aranUNION ALLdb254'
UNION ALL
SELECT 49, N'aras, aras-innovator-plm-suite, aras_innovator_plm_suite', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:36.983' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:36.983' AS DateTime, N'CMP WAP Extension Installer', N'aras', N'aras-innovator-plm-suite', N'aras_innovator_plm_suite'
UNION ALL
SELECT 50, N'array_networks, array-networks-vapv, array-vapv-azure-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.017' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.017' AS DateTime, N'CMP WAP Extension Installer', N'array_networks', N'array-networks-vapv', N'array-vapv-azure-byol'
UNION ALL
SELECT 51, N'attunity_cloudbeam, cloudbeam-dw-byol, attunity-cloudbeam-for-microsoft-azure-dw-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.017' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.017' AS DateTime, N'CMP WAP Extension Installer', N'attunity_cloudbeam', N'cloudbeam-dw-byol', N'attunity-cloudbeam-for-microsoft-azure-dw-byol'
UNION ALL
SELECT 52, N'auriq-systems, essentia, data-viewer', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.030' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.030' AS DateTime, N'CMP WAP Extension Installer', N'auriq-systems', N'essentia', N'data-viewer'
UNION ALL
SELECT 53, N'avepoint, docave-for-office365, docavemgr_6_6_0', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.030' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.030' AS DateTime, N'CMP WAP Extension Installer', N'avepoint', N'docave-for-office365', N'docavemgr_6_6_0'
UNION ALL
SELECT 54, N'aviatrix-systems, aviatrix-cloud-services, av-csg-10-tunnels', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.047' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.047' AS DateTime, N'CMP WAP Extension Installer', N'aviatrix-systems', N'aviatrix-cloud-services', N'av-csg-10-tunnels'
UNION ALL
SELECT 55, N'aviatrix-systems, aviatrix-cloud-services, av-csg-25-tunnels', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.063' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.063' AS DateTime, N'CMP WAP Extension Installer', N'aviatrix-systems', N'aviatrix-cloud-services', N'av-csg-25-tunnels'
UNION ALL
SELECT 56, N'aviatrix-systems, aviatrix-cloud-services, av-csg-5-tunnels', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.063' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.063' AS DateTime, N'CMP WAP Extension Installer', N'aviatrix-systems', N'aviatrix-cloud-services', N'av-csg-5-tunnels'
UNION ALL
SELECT 57, N'aviatrix-systems, aviatrix-cloud-services, av-csg-50-tunnels', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.077' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.077' AS DateTime, N'CMP WAP Extension Installer', N'aviatrix-systems', N'aviatrix-cloud-services', N'av-csg-50-tunnels'
UNION ALL
SELECT 58, N'aviatrix-systems, aviatrix-cloud-services, av-csg-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.093' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.093' AS DateTime, N'CMP WAP Extension Installer', N'aviatrix-systems', N'aviatrix-cloud-services', N'av-csg-byol'
UNION ALL
SELECT 59, N'awingu, awingu, az0-000c-01c', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.110' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.110' AS DateTime, N'CMP WAP Extension Installer', N'awingu', N'awingu', N'az0-000c-01c'
UNION ALL
SELECT 60, N'azul, zulu-enterprise-ondemand-ub1404, azul-zulu-ub1404', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.110' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.110' AS DateTime, N'CMP WAP Extension Installer', N'azul', N'zulu-enterprise-ondemand-ub1404', N'azul-zulu-ub1404'
UNION ALL
SELECT 61, N'barracudanetworks, barracuda-message-archiver, byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.123' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.123' AS DateTime, N'CMP WAP Extension Installer', N'barracudanetworks', N'barracuda-message-archiver', N'byol'
UNION ALL
SELECT 62, N'barracudanetworks, barracuda-ng-firewall, byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.140' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.140' AS DateTime, N'CMP WAP Extension Installer', N'barracudanetworks', N'barracuda-ng-firewall', N'byol'
UNION ALL
SELECT 63, N'barracudanetworks, barracuda-ng-firewall, hourly', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.140' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.140' AS DateTime, N'CMP WAP Extension Installer', N'barracudanetworks', N'barracuda-ng-firewall', N'hourly'
UNION ALL
SELECT 64, N'barracudanetworks, barracuda-spam-firewall, byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.157' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.157' AS DateTime, N'CMP WAP Extension Installer', N'barracudanetworks', N'barracuda-spam-firewall', N'byol'
UNION ALL
SELECT 65, N'barracudanetworks, barracuda-spam-firewall, hourly', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.157' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.157' AS DateTime, N'CMP WAP Extension Installer', N'barracudanetworks', N'barracuda-spam-firewall', N'hourly'
UNION ALL
SELECT 66, N'barracudanetworks, waf, byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.170' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.170' AS DateTime, N'CMP WAP Extension Installer', N'barracudanetworks', N'waf', N'byol'
UNION ALL
SELECT 67, N'barracudanetworks, waf, hourly', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.187' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.187' AS DateTime, N'CMP WAP Extension Installer', N'barracudanetworks', N'waf', N'hourly'
UNION ALL
SELECT 68, N'basho, riak-2-0-1, rs201-o', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.187' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.187' AS DateTime, N'CMP WAP Extension Installer', N'basho', N'riak-2-0-1', N'rs201-o'
UNION ALL
SELECT 69, N'Bitnami, DreamFactory, 1.6', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.203' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.203' AS DateTime, N'CMP WAP Extension Installer', N'Bitnami', N'DreamFactory', N'1.6'
UNION ALL
SELECT 70, N'Bitnami, DreamFactory, 1.7', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.203' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.203' AS DateTime, N'CMP WAP Extension Installer', N'Bitnami', N'DreamFactory', N'1.7'
UNION ALL
SELECT 71, N'Bitnami, eXo-Platform, Express-4', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.217' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.217' AS DateTime, N'CMP WAP Extension Installer', N'Bitnami', N'eXo-Platform', N'Express-4'
UNION ALL
SELECT 72, N'Bitnami, exoplatformenterprise, 4-2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.233' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.233' AS DateTime, N'CMP WAP Extension Installer', N'Bitnami', N'exoplatformenterprise', N'4-2'
UNION ALL
SELECT 73, N'Bitnami, openedx, cypress', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.233' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.233' AS DateTime, N'CMP WAP Extension Installer', N'Bitnami', N'openedx', N'cypress'
UNION ALL
SELECT 74, N'Bitnami, redmine, 3', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.250' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.250' AS DateTime, N'CMP WAP Extension Installer', N'Bitnami', N'redmine', N'3'
UNION ALL
SELECT 75, N'Bitnami, wordpress, 4-2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.280' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.280' AS DateTime, N'CMP WAP Extension Installer', N'Bitnami', N'wordpress', N'4-2'
UNION ALL
SELECT 76, N'bizagi, bizagibpms, bizagi106', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.280' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.280' AS DateTime, N'CMP WAP Extension Installer', N'bizagi', N'bizagibpms', N'bizagi106'
UNION ALL
SELECT 77, N'bluetalon, bluetalon, bluetalon', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.297' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.297' AS DateTime, N'CMP WAP Extension Installer', N'bluetalon', N'bluetalon', N'bluetalon'
UNION ALL
SELECT 78, N'boundlessgeo, opengeosuite, opengeosuite_postgis_tomcat', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.317' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.317' AS DateTime, N'CMP WAP Extension Installer', N'boundlessgeo', N'opengeosuite', N'opengeosuite_postgis_tomcat'
UNION ALL
SELECT 79, N'boxless, boxless, boxless', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.317' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.317' AS DateTime, N'CMP WAP Extension Installer', N'boxless', N'boxless', N'boxless'
UNION ALL
SELECT 80, N'brocade_communications, brocade-virtual-traffic-manager, stm_dev_64_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.330' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.330' AS DateTime, N'CMP WAP Extension Installer', N'brocade_communications', N'brocade-virtual-traffic-manager', N'stm_dev_64_byol'
UNION ALL
SELECT 81, N'bryte, bryteflow-cdc-free-trial, bryteflowcdc', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.347' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.347' AS DateTime, N'CMP WAP Extension Installer', N'bryte', N'bryteflow-cdc-free-trial', N'bryteflowcdc'
UNION ALL
SELECT 82, N'bssw, bluestripe-factfinder, factfinder-win-ms-4-azure', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.360' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.360' AS DateTime, N'CMP WAP Extension Installer', N'bssw', N'bluestripe-factfinder', N'factfinder-win-ms-4-azure'
UNION ALL
SELECT 83, N'buddhalabs, sles_12_pci, sles12-pci', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.393' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.393' AS DateTime, N'CMP WAP Extension Installer', N'buddhalabs', N'sles_12_pci', N'sles12-pci'
UNION ALL
SELECT 84, N'bwappengine, boardwalk, boardwalk', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.393' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.393' AS DateTime, N'CMP WAP Extension Installer', N'bwappengine', N'boardwalk', N'boardwalk'
UNION ALL
SELECT 85, N'Canonical, Ubuntu15.04Snappy, 15.04-Snappy', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.407' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.407' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'Ubuntu15.04Snappy', N'15.04-Snappy'
UNION ALL
SELECT 86, N'Canonical, Ubuntu15.04SnappyDocker, 15.04-SnappyDocker', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.423' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.423' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'Ubuntu15.04SnappyDocker', N'15.04-SnappyDocker'
UNION ALL
SELECT 87, N'Canonical, UbunturollingSnappy, 99.99-Snappy', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.440' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.440' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbunturollingSnappy', N'99.99-Snappy'
UNION ALL
SELECT 88, N'Canonical, UbuntuServer, 12.04.2-LTS', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.440' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.440' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'12.04.2-LTS'
UNION ALL
SELECT 89, N'Canonical, UbuntuServer, 12.04.3-LTS', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.457' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.457' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'12.04.3-LTS'
UNION ALL
SELECT 90, N'Canonical, UbuntuServer, 12.04.4-LTS', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.470' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.470' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'12.04.4-LTS'
UNION ALL
SELECT 91, N'Canonical, UbuntuServer, 12.04.5-DAILY-LTS', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.503' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.503' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'12.04.5-DAILY-LTS'
UNION ALL
SELECT 92, N'Canonical, UbuntuServer, 12.04.5-LTS', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.557' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.557' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'12.04.5-LTS'
UNION ALL
SELECT 93, N'Canonical, UbuntuServer, 12.10', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.570' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.570' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'12.10'
UNION ALL
SELECT 94, N'Canonical, UbuntuServer, 14.04-beta', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.603' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.603' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'14.04-beta'
UNION ALL
SELECT 95, N'Canonical, UbuntuServer, 14.04.0-LTS', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.643' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.643' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'14.04.0-LTS'
UNION ALL
SELECT 96, N'Canonical, UbuntuServer, 14.04.1-LTS', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.660' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.660' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'14.04.1-LTS'
UNION ALL
SELECT 97, N'Canonical, UbuntuServer, 14.04.2-LTS', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.677' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.677' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'14.04.2-LTS'
UNION ALL
SELECT 98, N'Canonical, UbuntuServer, 14.04.3-DAILY-LTS', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.690' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.690' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'14.04.3-DAILY-LTS'
UNION ALL
SELECT 99, N'Canonical, UbuntuServer, 14.04.3-LTS', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.717' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.717' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'14.04.3-LTS'
UNION ALL
SELECT 100, N'Canonical, UbuntuServer, 14.10', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.723' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.723' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'14.10'
UNION ALL
SELECT 101, N'Canonical, UbuntuServer, 14.10-beta', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.753' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.753' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'14.10-beta'
UNION ALL
SELECT 102, N'Canonical, UbuntuServer, 14.10-DAILY', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.753' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.753' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'14.10-DAILY'
UNION ALL
SELECT 103, N'Canonical, UbuntuServer, 15.04', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.770' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.770' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'15.04'
UNION ALL
SELECT 104, N'Canonical, UbuntuServer, 15.04-beta', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.787' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.787' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'15.04-beta'
UNION ALL
SELECT 105, N'Canonical, UbuntuServer, 15.04-DAILY', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.787' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.787' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'15.04-DAILY'
UNION ALL
SELECT 106, N'Canonical, UbuntuServer, 15.10', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.800' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.800' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'15.10'
UNION ALL
SELECT 107, N'Canonical, UbuntuServer, 15.10-alpha', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.817' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.817' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'15.10-alpha'
UNION ALL
SELECT 108, N'Canonical, UbuntuServer, 15.10-beta', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.817' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.817' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'15.10-beta'
UNION ALL
SELECT 109, N'Canonical, UbuntuServer, 15.10-DAILY', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.833' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.833' AS DateTime, N'CMP WAP Extension Installer', N'Canonical', N'UbuntuServer', N'15.10-DAILY'
UNION ALL
SELECT 110, N'catechnologies, ca-service-virtualization-8-1, casvondemand81', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.847' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.847' AS DateTime, N'CMP WAP Extension Installer', N'catechnologies', N'ca-service-virtualization-8-1', N'casvondemand81'
UNION ALL
SELECT 111, N'cautelalabs, log_management, cautela_labs_log_management_and_soc', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.847' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.847' AS DateTime, N'CMP WAP Extension Installer', N'cautelalabs', N'log_management', N'cautela_labs_log_management_and_soc'
UNION ALL
SELECT 112, N'cds, cds-data-migration-solution-for-legacy-to-cloud, dms-9000msaz', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.870' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.870' AS DateTime, N'CMP WAP Extension Installer', N'cds', N'cds-data-migration-solution-for-legacy-to-cloud', N'dms-9000msaz'
UNION ALL
SELECT 113, N'certivox, sso-test, sso', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.880' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.880' AS DateTime, N'CMP WAP Extension Installer', N'certivox', N'sso-test', N'sso'
UNION ALL
SELECT 114, N'checkpoint, check-point-r77-10, r7710-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.897' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.897' AS DateTime, N'CMP WAP Extension Installer', N'checkpoint', N'check-point-r77-10', N'r7710-byol'
UNION ALL
SELECT 115, N'checkpoint, check-point-r77-10, r7710-ngtp-payg', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.897' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.897' AS DateTime, N'CMP WAP Extension Installer', N'checkpoint', N'check-point-r77-10', N'r7710-ngtp-payg'
UNION ALL
SELECT 116, N'checkpointsystems, oatsystems-oatxpress-82-base, oatsystems-oatxpress-82-sku', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.910' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.910' AS DateTime, N'CMP WAP Extension Installer', N'checkpointsystems', N'oatsystems-oatxpress-82-base', N'oatsystems-oatxpress-82-sku'
UNION ALL
SELECT 117, N'chef-software, chef-server, azure_marketplace_100', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.927' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.927' AS DateTime, N'CMP WAP Extension Installer', N'chef-software', N'chef-server', N'azure_marketplace_100'
UNION ALL
SELECT 118, N'chef-software, chef-server, azure_marketplace_150', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.927' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.927' AS DateTime, N'CMP WAP Extension Installer', N'chef-software', N'chef-server', N'azure_marketplace_150'
UNION ALL
SELECT 119, N'chef-software, chef-server, azure_marketplace_200', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.943' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.943' AS DateTime, N'CMP WAP Extension Installer', N'chef-software', N'chef-server', N'azure_marketplace_200'
UNION ALL
SELECT 120, N'chef-software, chef-server, azure_marketplace_25', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.957' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.957' AS DateTime, N'CMP WAP Extension Installer', N'chef-software', N'chef-server', N'azure_marketplace_25'
UNION ALL
SELECT 121, N'chef-software, chef-server, azure_marketplace_250', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.973' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.973' AS DateTime, N'CMP WAP Extension Installer', N'chef-software', N'chef-server', N'azure_marketplace_250'
UNION ALL
SELECT 122, N'chef-software, chef-server, azure_marketplace_50', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.973' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.973' AS DateTime, N'CMP WAP Extension Installer', N'chef-software', N'chef-server', N'azure_marketplace_50'
UNION ALL
SELECT 123, N'chef-software, chef-server, chefbyol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:37.990' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:37.990' AS DateTime, N'CMP WAP Extension Installer', N'chef-software', N'chef-server', N'chefbyol'
UNION ALL
SELECT 124, N'circleci, circleci-enterprise, circleci-builder-base', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.007' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.007' AS DateTime, N'CMP WAP Extension Installer', N'circleci', N'circleci-enterprise', N'circleci-builder-base'
UNION ALL
SELECT 125, N'circleci, circleci-enterprise, circleci-enterprise-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.020' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.020' AS DateTime, N'CMP WAP Extension Installer', N'circleci', N'circleci-enterprise', N'circleci-enterprise-byol'
UNION ALL
SELECT 126, N'cires21, c21l-enc, c21l-enc-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.037' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.037' AS DateTime, N'CMP WAP Extension Installer', N'cires21', N'c21l-enc', N'c21l-enc-byol'
UNION ALL
SELECT 127, N'cires21, c21l-mos, c21l-mos-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.037' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.037' AS DateTime, N'CMP WAP Extension Installer', N'cires21', N'c21l-mos', N'c21l-mos-byol'
UNION ALL
SELECT 128, N'cisco, cisco-csr-1000v, csr-azure-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.053' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.053' AS DateTime, N'CMP WAP Extension Installer', N'cisco', N'cisco-csr-1000v', N'csr-azure-byol'
UNION ALL
SELECT 129, N'clickberry, clickberry-encoder, clickberry-encoder-core', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.067' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.067' AS DateTime, N'CMP WAP Extension Installer', N'clickberry', N'clickberry-encoder', N'clickberry-encoder-core'
UNION ALL
SELECT 130, N'cloudbees-enterprise-jenkins, cloudbees-jenkins-enterprise, cloudbees-jenkins-enterprise-14-11', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.083' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.083' AS DateTime, N'CMP WAP Extension Installer', N'cloudbees-enterprise-jenkins', N'cloudbees-jenkins-enterprise', N'cloudbees-jenkins-enterprise-14-11'
UNION ALL
SELECT 131, N'cloudbees-enterprise-jenkins, cloudbees-jenkins-operations-center, cloudbees-jenkins-operations-cent', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.083' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.083' AS DateTime, N'CMP WAP Extension Installer', N'cloudbees-enterprise-jenkins', N'cloudbees-jenkins-operations-center', N'cloudbees-jenkins-operations-center-14-11'
UNION ALL
SELECT 132, N'cloudboost, cloudboost, cloudboost-enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.100' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.100' AS DateTime, N'CMP WAP Extension Installer', N'cloudboost', N'cloudboost', N'cloudboost-enterprise'
UNION ALL
SELECT 133, N'cloudera, cloudera-centos-6, cloudera-centos-6', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.117' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.117' AS DateTime, N'CMP WAP Extension Installer', N'cloudera', N'cloudera-centos-6', N'cloudera-centos-6'
UNION ALL
SELECT 134, N'cloudlink, cloudlink-securevm, cloudlink-securevm-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.130' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.130' AS DateTime, N'CMP WAP Extension Installer', N'cloudlink', N'cloudlink-securevm', N'cloudlink-securevm-byol'
UNION ALL
SELECT 135, N'cloudsoft, cloudsoft-amp, amp-in-azure', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.130' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.130' AS DateTime, N'CMP WAP Extension Installer', N'cloudsoft', N'cloudsoft-amp', N'amp-in-azure'
UNION ALL
SELECT 136, N'cloudsoft, cloudsoft-amp, cloudsoft-amp', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.147' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.147' AS DateTime, N'CMP WAP Extension Installer', N'cloudsoft', N'cloudsoft-amp', N'cloudsoft-amp'
UNION ALL
SELECT 137, N'clustrix, clustrixdb, clxnode_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.160' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.160' AS DateTime, N'CMP WAP Extension Installer', N'clustrix', N'clustrixdb', N'clxnode_byol'
UNION ALL
SELECT 138, N'codelathe, codelathe-filecloud-win2012r2, filecloud_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.160' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.160' AS DateTime, N'CMP WAP Extension Installer', N'codelathe', N'codelathe-filecloud-win2012r2', N'filecloud_byol'
UNION ALL
SELECT 139, N'cohesive, cohesiveft-vns3-for-azure, cohesive-vns3-free', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.177' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.177' AS DateTime, N'CMP WAP Extension Installer', N'cohesive', N'cohesiveft-vns3-for-azure', N'cohesive-vns3-free'
UNION ALL
SELECT 140, N'cohesive, cohesiveft-vns3-for-azure, cohesive-vns3-lite', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.193' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.193' AS DateTime, N'CMP WAP Extension Installer', N'cohesive', N'cohesiveft-vns3-for-azure', N'cohesive-vns3-lite'
UNION ALL
SELECT 141, N'commvault, commvault, csmav10', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.207' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.207' AS DateTime, N'CMP WAP Extension Installer', N'commvault', N'commvault', N'csmav10'
UNION ALL
SELECT 142, N'cordis, myfidoc, myfidoc', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.207' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.207' AS DateTime, N'CMP WAP Extension Installer', N'cordis', N'myfidoc', N'myfidoc'
UNION ALL
SELECT 143, N'corent-technology-pvt, surpaas-analyzer-marketplace, corent_surpaas_analyzer', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.223' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.223' AS DateTime, N'CMP WAP Extension Installer', N'corent-technology-pvt', N'surpaas-analyzer-marketplace', N'corent_surpaas_analyzer'
UNION ALL
SELECT 144, N'CoreOS, CoreOS, Alpha', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.240' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.240' AS DateTime, N'CMP WAP Extension Installer', N'CoreOS', N'CoreOS', N'Alpha'
UNION ALL
SELECT 145, N'CoreOS, CoreOS, Beta', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.257' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.257' AS DateTime, N'CMP WAP Extension Installer', N'CoreOS', N'CoreOS', N'Beta'
UNION ALL
SELECT 146, N'CoreOS, CoreOS, Stable', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.257' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.257' AS DateTime, N'CMP WAP Extension Installer', N'CoreOS', N'CoreOS', N'Stable'
UNION ALL
SELECT 147, N'cortical-io, cortical-io-retinaservice-eng-gen, cortical_io_api_eng_gen', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.270' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.270' AS DateTime, N'CMP WAP Extension Installer', N'cortical-io', N'cortical-io-retinaservice-eng-gen', N'cortical_io_api_eng_gen'
UNION ALL
SELECT 148, N'couchbase, couchbase-server-30-community, cbs_30_ce', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.287' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.287' AS DateTime, N'CMP WAP Extension Installer', N'couchbase', N'couchbase-server-30-community', N'cbs_30_ce'
UNION ALL
SELECT 149, N'couchbase, couchbase-server-30-enterprise, cbs_30_ee_UNION ALLld', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.287' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.287' AS DateTime, N'CMP WAP Extension Installer', N'couchbase', N'couchbase-server-30-enterprise', N'cbs_30_ee_UNION ALLld'
UNION ALL
SELECT 150, N'couchbase, couchbase-server-30-enterprise, cbs_30_ee_silver', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.303' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.303' AS DateTime, N'CMP WAP Extension Installer', N'couchbase', N'couchbase-server-30-enterprise', N'cbs_30_ee_silver'
UNION ALL
SELECT 151, N'couchbase, couchbase-server-40-community, cbs_40_ce', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.317' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.317' AS DateTime, N'CMP WAP Extension Installer', N'couchbase', N'couchbase-server-40-community', N'cbs_40_ce'
UNION ALL
SELECT 152, N'couchbase, couchbase-server-40-enterprise, cbs_40_ee_UNION ALLld', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.333' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.333' AS DateTime, N'CMP WAP Extension Installer', N'couchbase', N'couchbase-server-40-enterprise', N'cbs_40_ee_UNION ALLld'
UNION ALL
SELECT 153, N'couchbase, couchbase-server-40-enterprise, cbs_40_ee_silver', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.333' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.333' AS DateTime, N'CMP WAP Extension Installer', N'couchbase', N'couchbase-server-40-enterprise', N'cbs_40_ee_silver'
UNION ALL
SELECT 154, N'credativ, Debian, 7-DAILY', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.350' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.350' AS DateTime, N'CMP WAP Extension Installer', N'credativ', N'Debian', N'7-DAILY'
UNION ALL
SELECT 155, N'credativ, Debian, 8-DAILY', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.367' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.367' AS DateTime, N'CMP WAP Extension Installer', N'credativ', N'Debian', N'8-DAILY'
UNION ALL
SELECT 156, N'credativ, Debian, 9-DAILY', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.380' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.380' AS DateTime, N'CMP WAP Extension Installer', N'credativ', N'Debian', N'9-DAILY'
UNION ALL
SELECT 157, N'dataart, devicehive, devicehivestandard', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.380' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.380' AS DateTime, N'CMP WAP Extension Installer', N'dataart', N'devicehive', N'devicehivestandard'
UNION ALL
SELECT 158, N'datale, datale-red, dcredvault01', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.397' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.397' AS DateTime, N'CMP WAP Extension Installer', N'datale', N'datale-red', N'dcredvault01'
UNION ALL
SELECT 159, N'dataexpeditioninc, expedat, expedat-windows-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.413' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.413' AS DateTime, N'CMP WAP Extension Installer', N'dataexpeditioninc', N'expedat', N'expedat-windows-byol'
UNION ALL
SELECT 160, N'datalayer, datalayer-notebook, spark-hadoop-r-python', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.413' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.413' AS DateTime, N'CMP WAP Extension Installer', N'datalayer', N'datalayer-notebook', N'spark-hadoop-r-python'
UNION ALL
SELECT 161, N'dataliberation, instant-intelligence-top-line-reporter, dl_tlr_01', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.430' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.430' AS DateTime, N'CMP WAP Extension Installer', N'dataliberation', N'instant-intelligence-top-line-reporter', N'dl_tlr_01'
UNION ALL
SELECT 162, N'datastax, datastax, enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.443' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.443' AS DateTime, N'CMP WAP Extension Installer', N'datastax', N'datastax', N'enterprise'
UNION ALL
SELECT 163, N'datastax, datastax-enterprise-non-production-use-only, dse-nonproduction-use-single-datacenter-20cor', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.443' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.443' AS DateTime, N'CMP WAP Extension Installer', N'datastax', N'datastax-enterprise-non-production-use-only', N'dse-nonproduction-use-single-datacenter-20core-max'
UNION ALL
SELECT 164, N'datastax, datastax-enterprise-non-production-use-only, sandbox_single-node', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.460' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.460' AS DateTime, N'CMP WAP Extension Installer', N'datastax', N'datastax-enterprise-non-production-use-only', N'sandbox_single-node'
UNION ALL
SELECT 165, N'datasunrise, datasunrise-database-security-suite, datasunrise_database_security_suite', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.477' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.477' AS DateTime, N'CMP WAP Extension Installer', N'datasunrise', N'datasunrise-database-security-suite', N'datasunrise_database_security_suite'
UNION ALL
SELECT 166, N'defacto_global_, defacto_modeler_w2012_sql2014_ssas2014_sampleapp01, defacto_business_modeler_byol_0', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.477' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.477' AS DateTime, N'CMP WAP Extension Installer', N'defacto_global_', N'defacto_modeler_w2012_sql2014_ssas2014_sampleapp01', N'defacto_business_modeler_byol_01'
UNION ALL
SELECT 167, N'dell_software, appassure-replication-target-for-azure, appassure_replication_target_for_azure', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.497' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.497' AS DateTime, N'CMP WAP Extension Installer', N'dell_software', N'appassure-replication-target-for-azure', N'appassure_replication_target_for_azure'
UNION ALL
SELECT 168, N'dell_software, changebase-6, changebasebyol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.497' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.497' AS DateTime, N'CMP WAP Extension Installer', N'dell_software', N'changebase-6', N'changebasebyol'
UNION ALL
SELECT 169, N'dell_software, migration-manager-for-enterprise-social, mmes', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.510' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.510' AS DateTime, N'CMP WAP Extension Installer', N'dell_software', N'migration-manager-for-enterprise-social', N'mmes'
UNION ALL
SELECT 170, N'dell_software, migration_suite_sharepoint, migration_suite_sharepoint_win2008r2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.527' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.527' AS DateTime, N'CMP WAP Extension Installer', N'dell_software', N'migration_suite_sharepoint', N'migration_suite_sharepoint_win2008r2'
UNION ALL
SELECT 171, N'dell_software, site_admin_for_sharepoint, site_admin_for_sharepoint_w2k8r2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.527' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.527' AS DateTime, N'CMP WAP Extension Installer', N'dell_software', N'site_admin_for_sharepoint', N'site_admin_for_sharepoint_w2k8r2'
UNION ALL
SELECT 172, N'dell_software, spotlight-on-sqlserver-enterprise, spotlight-on-sql-server-enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.543' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.543' AS DateTime, N'CMP WAP Extension Installer', N'dell_software', N'spotlight-on-sqlserver-enterprise', N'spotlight-on-sql-server-enterprise'
UNION ALL
SELECT 173, N'dell_software, statistica-data-miner, dell-statistica-data-miner-rental', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.560' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.560' AS DateTime, N'CMP WAP Extension Installer', N'dell_software', N'statistica-data-miner', N'dell-statistica-data-miner-rental'
UNION ALL
SELECT 174, N'dell_software, toad-intelligence-central, toad_intelligence_central_24', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.577' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.577' AS DateTime, N'CMP WAP Extension Installer', N'dell_software', N'toad-intelligence-central', N'toad_intelligence_central_24'
UNION ALL
SELECT 175, N'dell_software, uccs, uccs', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.577' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.577' AS DateTime, N'CMP WAP Extension Installer', N'dell_software', N'uccs', N'uccs'
UNION ALL
SELECT 176, N'dell_software, vworkspace-azure-trial, wyse_vworkspace_broker', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.590' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.590' AS DateTime, N'CMP WAP Extension Installer', N'dell_software', N'vworkspace-azure-trial', N'wyse_vworkspace_broker'
UNION ALL
SELECT 177, N'derdack, enterprisealert, enterprisealert-2015-datacenter-10users', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.607' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.607' AS DateTime, N'CMP WAP Extension Installer', N'derdack', N'enterprisealert', N'enterprisealert-2015-datacenter-10users'
UNION ALL
SELECT 178, N'derdack, enterprisealert, enterprisealert-2015-datacenter-15users', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.623' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.623' AS DateTime, N'CMP WAP Extension Installer', N'derdack', N'enterprisealert', N'enterprisealert-2015-datacenter-15users'
UNION ALL
SELECT 179, N'derdack, enterprisealert, enterprisealert-2015-datacenter-20users', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.637' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.637' AS DateTime, N'CMP WAP Extension Installer', N'derdack', N'enterprisealert', N'enterprisealert-2015-datacenter-20users'
UNION ALL
SELECT 180, N'derdack, enterprisealert, enterprisealert-2015-datacenter-25users', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.637' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.637' AS DateTime, N'CMP WAP Extension Installer', N'derdack', N'enterprisealert', N'enterprisealert-2015-datacenter-25users'
UNION ALL
SELECT 181, N'derdack, enterprisealert, enterprisealert-2015-datacenter-50users', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.653' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.653' AS DateTime, N'CMP WAP Extension Installer', N'derdack', N'enterprisealert', N'enterprisealert-2015-datacenter-50users'
UNION ALL
SELECT 182, N'dgsecure, dgsecure, dgsecure_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.670' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.670' AS DateTime, N'CMP WAP Extension Installer', N'dgsecure', N'dgsecure', N'dgsecure_byol'
UNION ALL
SELECT 183, N'docker, docker-subscription-for-azure, dse-subem1y-000001', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.687' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.687' AS DateTime, N'CMP WAP Extension Installer', N'docker', N'docker-subscription-for-azure', N'dse-subem1y-000001'
UNION ALL
SELECT 184, N'dolbydeveloper, dolby-encoding-azure-freetool, dolby_encoder_free', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.700' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.700' AS DateTime, N'CMP WAP Extension Installer', N'dolbydeveloper', N'dolby-encoding-azure-freetool', N'dolby_encoder_free'
UNION ALL
SELECT 185, N'donovapub, domino2UNION ALL, domino2UNION ALL1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.717' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.717' AS DateTime, N'CMP WAP Extension Installer', N'donovapub', N'domino2UNION ALL', N'domino2UNION ALL1'
UNION ALL
SELECT 186, N'donovapub, domino2UNION ALL-byol, domino2UNION ALL-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.733' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.733' AS DateTime, N'CMP WAP Extension Installer', N'donovapub', N'domino2UNION ALL-byol', N'domino2UNION ALL-byol'
UNION ALL
SELECT 187, N'drone, drone, free', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.747' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.747' AS DateTime, N'CMP WAP Extension Installer', N'drone', N'drone', N'free'
UNION ALL
SELECT 188, N'dundas, dundas-bi, dbi1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.763' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.763' AS DateTime, N'CMP WAP Extension Installer', N'dundas', N'dundas-bi', N'dbi1'
UNION ALL
SELECT 189, N'dundas, dundas-bi, dbi2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.763' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.763' AS DateTime, N'CMP WAP Extension Installer', N'dundas', N'dundas-bi', N'dbi2'
UNION ALL
SELECT 190, N'easyterritory, easyterritory, easyterritory_enterprise_en', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.787' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.787' AS DateTime, N'CMP WAP Extension Installer', N'easyterritory', N'easyterritory', N'easyterritory_enterprise_en'
UNION ALL
SELECT 191, N'egress, egress-switch-gateway, shsg-azu-0001', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.787' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.787' AS DateTime, N'CMP WAP Extension Installer', N'egress', N'egress-switch-gateway', N'shsg-azu-0001'
UNION ALL
SELECT 192, N'elastacloud, brisk, briskengine', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.797' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.797' AS DateTime, N'CMP WAP Extension Installer', N'elastacloud', N'brisk', N'briskengine'
UNION ALL
SELECT 193, N'elasticbox, elasticbox-enterprise, byol-single-instance', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.813' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.813' AS DateTime, N'CMP WAP Extension Installer', N'elasticbox', N'elasticbox-enterprise', N'byol-single-instance'
UNION ALL
SELECT 194, N'elfiqnetworks, cloud-connector, cloud-connector-azure', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.830' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.830' AS DateTime, N'CMP WAP Extension Installer', N'elfiqnetworks', N'cloud-connector', N'cloud-connector-azure'
UNION ALL
SELECT 195, N'eloquera, eloqueradb, eloqueradb', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.830' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.830' AS DateTime, N'CMP WAP Extension Installer', N'eloquera', N'eloqueradb', N'eloqueradb'
UNION ALL
SELECT 196, N'eperi, eperi-gateway-for-cloud-apps, egfca', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.847' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.847' AS DateTime, N'CMP WAP Extension Installer', N'eperi', N'eperi-gateway-for-cloud-apps', N'egfca'
UNION ALL
SELECT 197, N'equilibrium, mediarich-all-media-server, 404115az', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.847' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.847' AS DateTime, N'CMP WAP Extension Installer', N'equilibrium', N'mediarich-all-media-server', N'404115az'
UNION ALL
SELECT 198, N'equilibrium, mediarich-hot-folder, 402000az', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.860' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.860' AS DateTime, N'CMP WAP Extension Installer', N'equilibrium', N'mediarich-hot-folder', N'402000az'
UNION ALL
SELECT 199, N'esri, arcgis-for-server, cloud', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.877' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.877' AS DateTime, N'CMP WAP Extension Installer', N'esri', N'arcgis-for-server', N'cloud'
UNION ALL
SELECT 200, N'eurotech, ec-smallbiz-2015, everyware_cloud_lite', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.893' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.893' AS DateTime, N'CMP WAP Extension Installer', N'eurotech', N'ec-smallbiz-2015', N'everyware_cloud_lite'
UNION ALL
SELECT 201, N'exasol, exasolution-analytic-database, exasolution_database', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.907' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.907' AS DateTime, N'CMP WAP Extension Installer', N'exasol', N'exasolution-analytic-database', N'exasolution_database'
UNION ALL
SELECT 202, N'exit-games, photon-server-vm, photon-vm', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.907' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.907' AS DateTime, N'CMP WAP Extension Installer', N'exit-games', N'photon-server-vm', N'photon-vm'
UNION ALL
SELECT 203, N'expertime, magento-pimcore-as, vm_magento_pimcore_v0', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.923' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.923' AS DateTime, N'CMP WAP Extension Installer', N'expertime', N'magento-pimcore-as', N'vm_magento_pimcore_v0'
UNION ALL
SELECT 204, N'f5-networks, f5-big-ip, f5-bigip-virtual-edition-best-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.940' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.940' AS DateTime, N'CMP WAP Extension Installer', N'f5-networks', N'f5-big-ip', N'f5-bigip-virtual-edition-best-byol'
UNION ALL
SELECT 205, N'f5-networks, f5-big-ip, f5-bigip-virtual-edition-better-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.940' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.940' AS DateTime, N'CMP WAP Extension Installer', N'f5-networks', N'f5-big-ip', N'f5-bigip-virtual-edition-better-byol'
UNION ALL
SELECT 206, N'f5-networks, f5-big-ip, f5-bigip-virtual-edition-UNION ALLod-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.957' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.957' AS DateTime, N'CMP WAP Extension Installer', N'f5-networks', N'f5-big-ip', N'f5-bigip-virtual-edition-UNION ALLod-byol'
UNION ALL
SELECT 207, N'f5-networks, f5-waf-evaluation, bigip-waf-byol-preview', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.970' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.970' AS DateTime, N'CMP WAP Extension Installer', N'f5-networks', N'f5-waf-evaluation', N'bigip-waf-byol-preview'
UNION ALL
SELECT 208, N'filebridge, azure, 10tb', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.970' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.970' AS DateTime, N'CMP WAP Extension Installer', N'filebridge', N'azure', N'10tb'
UNION ALL
SELECT 209, N'filebridge, azure, 20tb', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:38.987' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:38.987' AS DateTime, N'CMP WAP Extension Installer', N'filebridge', N'azure', N'20tb'
UNION ALL
SELECT 210, N'filebridge, azure, 50tb', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.017' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.017' AS DateTime, N'CMP WAP Extension Installer', N'filebridge', N'azure', N'50tb'
UNION ALL
SELECT 211, N'firehost, firehost_armor, byol_centos6', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.017' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.017' AS DateTime, N'CMP WAP Extension Installer', N'firehost', N'firehost_armor', N'byol_centos6'
UNION ALL
SELECT 212, N'firehost, firehost_armor_ubuntu, byol_ubuntu', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.050' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.050' AS DateTime, N'CMP WAP Extension Installer', N'firehost', N'firehost_armor_ubuntu', N'byol_ubuntu'
UNION ALL
SELECT 213, N'firehost, firehost_armor_windows, byol_windows2012r2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.063' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.063' AS DateTime, N'CMP WAP Extension Installer', N'firehost', N'firehost_armor_windows', N'byol_windows2012r2'
UNION ALL
SELECT 214, N'flexerasoftware, flexera-software-licensing, software-monetization', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.063' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.063' AS DateTime, N'CMP WAP Extension Installer', N'flexerasoftware', N'flexera-software-licensing', N'software-monetization'
UNION ALL
SELECT 215, N'foghorn-systems, foghorn-edge-device-manager, foghorn-edm', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.080' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.080' AS DateTime, N'CMP WAP Extension Installer', N'foghorn-systems', N'foghorn-edge-device-manager', N'foghorn-edm'
UNION ALL
SELECT 216, N'fortinet, fortinet_fortigate-vm_v5, fortinet_fg-vm', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.097' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.097' AS DateTime, N'CMP WAP Extension Installer', N'fortinet', N'fortinet_fortigate-vm_v5', N'fortinet_fg-vm'
UNION ALL
SELECT 217, N'gemalto-safenet, safenet-protectv, 100_protectv_clients', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.097' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.097' AS DateTime, N'CMP WAP Extension Installer', N'gemalto-safenet', N'safenet-protectv', N'100_protectv_clients'
UNION ALL
SELECT 218, N'gemalto-safenet, safenet-protectv, 200_protectv_clients', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.110' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.110' AS DateTime, N'CMP WAP Extension Installer', N'gemalto-safenet', N'safenet-protectv', N'200_protectv_clients'
UNION ALL
SELECT 219, N'gemalto-safenet, safenet-protectv, 50_protectv_clients', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.127' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.127' AS DateTime, N'CMP WAP Extension Installer', N'gemalto-safenet', N'safenet-protectv', N'50_protectv_clients'
UNION ALL
SELECT 220, N'GitHub, GitHub-Enterprise, GitHub-Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.127' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.127' AS DateTime, N'CMP WAP Extension Installer', N'GitHub', N'GitHub-Enterprise', N'GitHub-Enterprise'
UNION ALL
SELECT 221, N'greensql, greensql-database-security, greensql_database_security_azure', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.150' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.150' AS DateTime, N'CMP WAP Extension Installer', N'greensql', N'greensql-database-security', N'greensql_database_security_azure'
UNION ALL
SELECT 222, N'haivision, media-gateway, mi-mg-azu-1-1-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.150' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.150' AS DateTime, N'CMP WAP Extension Installer', N'haivision', N'media-gateway', N'mi-mg-azu-1-1-byol'
UNION ALL
SELECT 223, N'halobicloud, halo-paas2, halopaas02', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.167' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.167' AS DateTime, N'CMP WAP Extension Installer', N'halobicloud', N'halo-paas2', N'halopaas02'
UNION ALL
SELECT 224, N'hanu, hanu-insight, enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.167' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.167' AS DateTime, N'CMP WAP Extension Installer', N'hanu', N'hanu-insight', N'enterprise'
UNION ALL
SELECT 225, N'hanu, hanu-insight, enterprise-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.180' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.180' AS DateTime, N'CMP WAP Extension Installer', N'hanu', N'hanu-insight', N'enterprise-byol'
UNION ALL
SELECT 226, N'hanu, hanu-insight, standard', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.197' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.197' AS DateTime, N'CMP WAP Extension Installer', N'hanu', N'hanu-insight', N'standard'
UNION ALL
SELECT 227, N'hanu, hanu-insight, standard-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.210' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.210' AS DateTime, N'CMP WAP Extension Installer', N'hanu', N'hanu-insight', N'standard-byol'
UNION ALL
SELECT 228, N'hewlett-packard, hp-loadrunner, lr_12_02_ga_2012r2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.210' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.210' AS DateTime, N'CMP WAP Extension Installer', N'hewlett-packard', N'hp-loadrunner', N'lr_12_02_ga_2012r2'
UNION ALL
SELECT 229, N'hewlett-packard, hp-quality-center, hp_qc_community_edition', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.227' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.227' AS DateTime, N'CMP WAP Extension Installer', N'hewlett-packard', N'hp-quality-center', N'hp_qc_community_edition'
UNION ALL
SELECT 230, N'hortonworks, hortonworks, dataplatform', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.243' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.243' AS DateTime, N'CMP WAP Extension Installer', N'hortonworks', N'hortonworks', N'dataplatform'
UNION ALL
SELECT 231, N'hortonworks, hortonworks-sandbox, sandbox22', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.257' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.257' AS DateTime, N'CMP WAP Extension Installer', N'hortonworks', N'hortonworks-sandbox', N'sandbox22'
UNION ALL
SELECT 232, N'iaansys, iaansys-magento, iaansys-magento-ubuntu', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.257' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.257' AS DateTime, N'CMP WAP Extension Installer', N'iaansys', N'iaansys-magento', N'iaansys-magento-ubuntu'
UNION ALL
SELECT 233, N'iamcloud, university-for-life, university-for-life', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.273' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.273' AS DateTime, N'CMP WAP Extension Installer', N'iamcloud', N'university-for-life', N'university-for-life'
UNION ALL
SELECT 234, N'imc, imc-process-guidance-suite, imc_pgs41_l', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.290' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.290' AS DateTime, N'CMP WAP Extension Installer', N'imc', N'imc-process-guidance-suite', N'imc_pgs41_l'
UNION ALL
SELECT 235, N'imc, imc-process-guidance-suite, imc_pgs41_m', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.307' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.307' AS DateTime, N'CMP WAP Extension Installer', N'imc', N'imc-process-guidance-suite', N'imc_pgs41_m'
UNION ALL
SELECT 236, N'imc, imc-process-guidance-suite, imc_pgs41_s', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.323' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.323' AS DateTime, N'CMP WAP Extension Installer', N'imc', N'imc-process-guidance-suite', N'imc_pgs41_s'
UNION ALL
SELECT 237, N'imc, imc-process-guidance-suite, imc_pgs41_xs', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.323' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.323' AS DateTime, N'CMP WAP Extension Installer', N'imc', N'imc-process-guidance-suite', N'imc_pgs41_xs'
UNION ALL
SELECT 238, N'infolibrarian, infolibrarian-metadata-management-server, infolibrarian-metadata-windows-64-bit-hourl', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.337' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.337' AS DateTime, N'CMP WAP Extension Installer', N'infolibrarian', N'infolibrarian-metadata-management-server', N'infolibrarian-metadata-windows-64-bit-hourly'
UNION ALL
SELECT 239, N'informatica-cloud, informatica-cloud, informatica_cloud_secure_agent_32_bit', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.353' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.353' AS DateTime, N'CMP WAP Extension Installer', N'informatica-cloud', N'informatica-cloud', N'informatica_cloud_secure_agent_32_bit'
UNION ALL
SELECT 240, N'informatica-cloud, informatica-cloud, informatica_cloud_secure_agent_32_bit_linux', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.370' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.370' AS DateTime, N'CMP WAP Extension Installer', N'informatica-cloud', N'informatica-cloud', N'informatica_cloud_secure_agent_32_bit_linux'
UNION ALL
SELECT 241, N'infostrat, portvue, infostratportvuev1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.370' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.370' AS DateTime, N'CMP WAP Extension Installer', N'infostrat', N'portvue', N'infostratportvuev1'
UNION ALL
SELECT 242, N'intel, lustre-cloud-edition-cv-image, eval-lustre-2-7', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.387' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.387' AS DateTime, N'CMP WAP Extension Installer', N'intel', N'lustre-cloud-edition-cv-image', N'eval-lustre-2-7'
UNION ALL
SELECT 243, N'intelligent-plant-ltd, intelligent-plant-gestalt, gestalt_suite', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.400' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.400' AS DateTime, N'CMP WAP Extension Installer', N'intelligent-plant-ltd', N'intelligent-plant-gestalt', N'gestalt_suite'
UNION ALL
SELECT 244, N'iquest, meeting-rooms, enterprise-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.400' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.400' AS DateTime, N'CMP WAP Extension Installer', N'iquest', N'meeting-rooms', N'enterprise-byol'
UNION ALL
SELECT 245, N'itelios, magento2-on-zendserver, magento2-0-74-0-beta14-demo', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.417' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.417' AS DateTime, N'CMP WAP Extension Installer', N'itelios', N'magento2-on-zendserver', N'magento2-0-74-0-beta14-demo'
UNION ALL
SELECT 246, N'jedox, jedox-for-azure, jedox_5_1_sr5_lin_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.447' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.447' AS DateTime, N'CMP WAP Extension Installer', N'jedox', N'jedox-for-azure', N'jedox_5_1_sr5_lin_byol'
UNION ALL
SELECT 247, N'jedox, jedox-for-azure, jedox_6_0_lin_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.463' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.463' AS DateTime, N'CMP WAP Extension Installer', N'jedox', N'jedox-for-azure', N'jedox_6_0_lin_byol'
UNION ALL
SELECT 248, N'jelastic, jelastic-hybrid-paas-standard, jelastic_azure_hybrid_basic', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.463' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.463' AS DateTime, N'CMP WAP Extension Installer', N'jelastic', N'jelastic-hybrid-paas-standard', N'jelastic_azure_hybrid_basic'
UNION ALL
SELECT 249, N'jetnexus, jetnexus-application-load-balancer, jnxalbx-azure-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.477' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.477' AS DateTime, N'CMP WAP Extension Installer', N'jetnexus', N'jetnexus-application-load-balancer', N'jnxalbx-azure-byol'
UNION ALL
SELECT 250, N'jetnexus, jetnexus-application-load-balancer, jnxalbx-azure-va-10g', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.477' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.477' AS DateTime, N'CMP WAP Extension Installer', N'jetnexus', N'jetnexus-application-load-balancer', N'jnxalbx-azure-va-10g'
UNION ALL
SELECT 251, N'jetnexus, jetnexus-application-load-balancer, jnxalbx-azure-va-1g', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.493' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.493' AS DateTime, N'CMP WAP Extension Installer', N'jetnexus', N'jetnexus-application-load-balancer', N'jnxalbx-azure-va-1g'
UNION ALL
SELECT 252, N'jetnexus, jetnexus-application-load-balancer, jnxalbx-azure-va-3g', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.510' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.510' AS DateTime, N'CMP WAP Extension Installer', N'jetnexus', N'jetnexus-application-load-balancer', N'jnxalbx-azure-va-3g'
UNION ALL
SELECT 253, N'jetnexus, jetnexus-application-load-balancer, jnxalbx-azure-va-500m', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.510' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.510' AS DateTime, N'CMP WAP Extension Installer', N'jetnexus', N'jetnexus-application-load-balancer', N'jnxalbx-azure-va-500m'
UNION ALL
SELECT 254, N'jetnexus, jetnexus-application-load-balancer, jnxalbx-azure-va-5g', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.527' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.527' AS DateTime, N'CMP WAP Extension Installer', N'jetnexus', N'jetnexus-application-load-balancer', N'jnxalbx-azure-va-5g'
UNION ALL
SELECT 255, N'jfrog, artifactory, artifactory-3-7-0', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.540' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.540' AS DateTime, N'CMP WAP Extension Installer', N'jfrog', N'artifactory', N'artifactory-3-7-0'
UNION ALL
SELECT 256, N'jfrog, artifactory, artifactory-3-8-0', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.540' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.540' AS DateTime, N'CMP WAP Extension Installer', N'jfrog', N'artifactory', N'artifactory-3-8-0'
UNION ALL
SELECT 257, N'jfrog, artifactory, artifactory-3-9-2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.557' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.557' AS DateTime, N'CMP WAP Extension Installer', N'jfrog', N'artifactory', N'artifactory-3-9-2'
UNION ALL
SELECT 258, N'jfrog, artifactory, jfrog-artifactory-4-2-0', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.573' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.573' AS DateTime, N'CMP WAP Extension Installer', N'jfrog', N'artifactory', N'jfrog-artifactory-4-2-0'
UNION ALL
SELECT 259, N'kaazing, kaazing-kwic, activedirectory-free', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.573' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.573' AS DateTime, N'CMP WAP Extension Installer', N'kaazing', N'kaazing-kwic', N'activedirectory-free'
UNION ALL
SELECT 260, N'kaazing, kaazing-kwic, activedirectory-silver-support', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.587' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.587' AS DateTime, N'CMP WAP Extension Installer', N'kaazing', N'kaazing-kwic', N'activedirectory-silver-support'
UNION ALL
SELECT 261, N'kaazing, kaazing-vpa, activedirectory-free', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.603' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.603' AS DateTime, N'CMP WAP Extension Installer', N'kaazing', N'kaazing-vpa', N'activedirectory-free'
UNION ALL
SELECT 262, N'kaazing, kaazing-vpa, activedirectory-silver-support', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.603' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.603' AS DateTime, N'CMP WAP Extension Installer', N'kaazing', N'kaazing-vpa', N'activedirectory-silver-support'
UNION ALL
SELECT 263, N'kaspersky_lab, kaspersky_secure_mail_gateway, ksg', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.627' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.627' AS DateTime, N'CMP WAP Extension Installer', N'kaspersky_lab', N'kaspersky_secure_mail_gateway', N'ksg'
UNION ALL
SELECT 264, N'kaspersky_lab, kav_for_lfs, kav_for_lfs', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.637' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.637' AS DateTime, N'CMP WAP Extension Installer', N'kaspersky_lab', N'kav_for_lfs', N'kav_for_lfs'
UNION ALL
SELECT 265, N'kaspersky_lab, kav_for_wsee, kav_for_wsee', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.637' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.637' AS DateTime, N'CMP WAP Extension Installer', N'kaspersky_lab', N'kav_for_wsee', N'kav_for_wsee'
UNION ALL
SELECT 266, N'kemptech, vlm-azure, basic-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.653' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.653' AS DateTime, N'CMP WAP Extension Installer', N'kemptech', N'vlm-azure', N'basic-byol'
UNION ALL
SELECT 267, N'kemptech, vlm-azure, vlm-10g-hrl', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.667' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.667' AS DateTime, N'CMP WAP Extension Installer', N'kemptech', N'vlm-azure', N'vlm-10g-hrl'
UNION ALL
SELECT 268, N'kemptech, vlm-azure, vlm-200-hrl', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.667' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.667' AS DateTime, N'CMP WAP Extension Installer', N'kemptech', N'vlm-azure', N'vlm-200-hrl'
UNION ALL
SELECT 269, N'kemptech, vlm-azure, vlm-2000-hrl', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.683' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.683' AS DateTime, N'CMP WAP Extension Installer', N'kemptech', N'vlm-azure', N'vlm-2000-hrl'
UNION ALL
SELECT 270, N'kemptech, vlm-azure, vlm-5000-hrl', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.703' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.703' AS DateTime, N'CMP WAP Extension Installer', N'kemptech', N'vlm-azure', N'vlm-5000-hrl'
UNION ALL
SELECT 271, N'le, logentries-datahub, azuredh', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.717' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.717' AS DateTime, N'CMP WAP Extension Installer', N'le', N'logentries-datahub', N'azuredh'
UNION ALL
SELECT 272, N'lieberlieber, lieberlieber-web-collaborator, llweb4ea01', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.730' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.730' AS DateTime, N'CMP WAP Extension Installer', N'lieberlieber', N'lieberlieber-web-collaborator', N'llweb4ea01'
UNION ALL
SELECT 273, N'liebsoft, enterprise_random_password_manager, erpm', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.747' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.747' AS DateTime, N'CMP WAP Extension Installer', N'liebsoft', N'enterprise_random_password_manager', N'erpm'
UNION ALL
SELECT 274, N'literatu, literatu, lit-az-vir', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.747' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.747' AS DateTime, N'CMP WAP Extension Installer', N'literatu', N'literatu', N'lit-az-vir'
UNION ALL
SELECT 275, N'loadbalancer, loadbalancer-org-load-balancer-for-azure, loadbalancer_org_azure', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.763' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.763' AS DateTime, N'CMP WAP Extension Installer', N'loadbalancer', N'loadbalancer-org-load-balancer-for-azure', N'loadbalancer_org_azure'
UNION ALL
SELECT 276, N'logi-analytics, logi-info-11, logi-info-v11-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.777' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.777' AS DateTime, N'CMP WAP Extension Installer', N'logi-analytics', N'logi-info-11', N'logi-info-v11-byol'
UNION ALL
SELECT 277, N'logi-analytics, logi-vision-1, logi-vision-hourly', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.793' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.793' AS DateTime, N'CMP WAP Extension Installer', N'logi-analytics', N'logi-vision-1', N'logi-vision-hourly'
UNION ALL
SELECT 278, N'logi-analytics, logi-vision-1_4-byol, logi-vision-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.810' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.810' AS DateTime, N'CMP WAP Extension Installer', N'logi-analytics', N'logi-vision-1_4-byol', N'logi-vision-byol'
UNION ALL
SELECT 279, N'loginpeople, digital-dna-authentication-server, enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.810' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.810' AS DateTime, N'CMP WAP Extension Installer', N'loginpeople', N'digital-dna-authentication-server', N'enterprise'
UNION ALL
SELECT 280, N'loginpeople, digital-dna-authentication-server, enterprise-plus', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.833' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.833' AS DateTime, N'CMP WAP Extension Installer', N'loginpeople', N'digital-dna-authentication-server', N'enterprise-plus'
UNION ALL
SELECT 281, N'loginpeople, digital-dna-authentication-server, small-business', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.843' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.843' AS DateTime, N'CMP WAP Extension Installer', N'loginpeople', N'digital-dna-authentication-server', N'small-business'
UNION ALL
SELECT 282, N'loginpeople, digital-dna-authentication-server, small-business-plus', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.843' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.843' AS DateTime, N'CMP WAP Extension Installer', N'loginpeople', N'digital-dna-authentication-server', N'small-business-plus'
UNION ALL
SELECT 283, N'logtrust, logtrust-log-management, lmrelay', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.860' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.860' AS DateTime, N'CMP WAP Extension Installer', N'logtrust', N'logtrust-log-management', N'lmrelay'
UNION ALL
SELECT 284, N'looker, looker-analytics-platform, looker-analytics-platform-sqldb-10users', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.877' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.877' AS DateTime, N'CMP WAP Extension Installer', N'looker', N'looker-analytics-platform', N'looker-analytics-platform-sqldb-10users'
UNION ALL
SELECT 285, N'looker, looker-analytics-platform-326, looker-analytics-platform-sqldb-10users', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.890' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.890' AS DateTime, N'CMP WAP Extension Installer', N'looker', N'looker-analytics-platform-326', N'looker-analytics-platform-sqldb-10users'
UNION ALL
SELECT 286, N'looker, looker-analytics-platform-326, looker-analytics-platform-sqldb-25users', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.907' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.907' AS DateTime, N'CMP WAP Extension Installer', N'looker', N'looker-analytics-platform-326', N'looker-analytics-platform-sqldb-25users'
UNION ALL
SELECT 287, N'magelia, magelia-webstore-professional, magelia-webstore-professional-edition', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.923' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.923' AS DateTime, N'CMP WAP Extension Installer', N'magelia', N'magelia-webstore-professional', N'magelia-webstore-professional-edition'
UNION ALL
SELECT 288, N'manageengine, passwordmanagerpro-windows, 14116_2s', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.923' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.923' AS DateTime, N'CMP WAP Extension Installer', N'manageengine', N'passwordmanagerpro-windows', N'14116_2s'
UNION ALL
SELECT 289, N'manageengine, servicedeskplus, sdp-50tech', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.937' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.937' AS DateTime, N'CMP WAP Extension Installer', N'manageengine', N'servicedeskplus', N'sdp-50tech'
UNION ALL
SELECT 290, N'massiveanalytic-, oscarap, oscarap', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.953' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.953' AS DateTime, N'CMP WAP Extension Installer', N'massiveanalytic-', N'oscarap', N'oscarap'
UNION ALL
SELECT 291, N'meanio, meanio-050-vm, meanio-050-vm', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.967' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.967' AS DateTime, N'CMP WAP Extension Installer', N'meanio', N'meanio-050-vm', N'meanio-050-vm'
UNION ALL
SELECT 292, N'memsql, memsql-community-single-vm, memsql-community-single-vm', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.983' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.983' AS DateTime, N'CMP WAP Extension Installer', N'memsql', N'memsql-community-single-vm', N'memsql-community-single-vm'
UNION ALL
SELECT 293, N'memsql, memsql-enterprise-single-vm, memsql-enterprise-single-vm', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:39.983' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:39.983' AS DateTime, N'CMP WAP Extension Installer', N'memsql', N'memsql-enterprise-single-vm', N'memsql-enterprise-single-vm'
UNION ALL
SELECT 294, N'mentalnotes, mental-notes-vm, mn01', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.000' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.000' AS DateTime, N'CMP WAP Extension Installer', N'mentalnotes', N'mental-notes-vm', N'mn01'
UNION ALL
SELECT 295, N'mentalnotes, testmentalnotes, mn001', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.017' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.017' AS DateTime, N'CMP WAP Extension Installer', N'mentalnotes', N'testmentalnotes', N'mn001'
UNION ALL
SELECT 296, N'metavistech, metavis-office365-suite, mv-office365-ste-azure-1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.017' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.017' AS DateTime, N'CMP WAP Extension Installer', N'metavistech', N'metavis-office365-suite', N'mv-office365-ste-azure-1'
UNION ALL
SELECT 297, N'mfiles, standard, byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.030' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.030' AS DateTime, N'CMP WAP Extension Installer', N'mfiles', N'standard', N'byol'
UNION ALL
SELECT 298, N'mfiles, standard, sku1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.047' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.047' AS DateTime, N'CMP WAP Extension Installer', N'mfiles', N'standard', N'sku1'
UNION ALL
SELECT 299, N'mfiles, standard, sku2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.047' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.047' AS DateTime, N'CMP WAP Extension Installer', N'mfiles', N'standard', N'sku2'
UNION ALL
SELECT 300, N'mfiles, standard, sku3', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.063' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.063' AS DateTime, N'CMP WAP Extension Installer', N'mfiles', N'standard', N'sku3'
UNION ALL
SELECT 301, N'Microsoft, IBM, IBM_DB2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.077' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.077' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'IBM', N'IBM_DB2'
UNION ALL
SELECT 302, N'Microsoft, IBM, IBM_MQ', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.093' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.093' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'IBM', N'IBM_MQ'
UNION ALL
SELECT 303, N'Microsoft, IBM, IBM_MQ8', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.093' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.093' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'IBM', N'IBM_MQ8'
UNION ALL
SELECT 304, N'Microsoft, IBM, IBM_WAS', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.110' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.110' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'IBM', N'IBM_WAS'
UNION ALL
SELECT 305, N'Microsoft, JDK, JDK_6', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.127' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.127' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'JDK', N'JDK_6'
UNION ALL
SELECT 306, N'Microsoft, JDK, JDK_7', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.127' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.127' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'JDK', N'JDK_7'
UNION ALL
SELECT 307, N'Microsoft, JDK, JDK_8', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.140' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.140' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'JDK', N'JDK_8'
UNION ALL
SELECT 308, N'Microsoft, Oracle_Database_11g_R2, Oracle_Database_11g_R2_Enterprise_Edition', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.157' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.157' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'Oracle_Database_11g_R2', N'Oracle_Database_11g_R2_Enterprise_Edition'
UNION ALL
SELECT 309, N'Microsoft, Oracle_Database_11g_R2, Oracle_WebLogic_Server_11g_R2_Standard_Edition', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.173' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.173' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'Oracle_Database_11g_R2', N'Oracle_WebLogic_Server_11g_R2_Standard_Edition'
UNION ALL
SELECT 310, N'Microsoft, Oracle_Database_11g_R2_and_WebLogic_Server_11g, Oracle_Database_11gR2_and_WebLogic_Server', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.173' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.173' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'Oracle_Database_11g_R2_and_WebLogic_Server_11g', N'Oracle_Database_11gR2_and_WebLogic_Server_11g_Enterprise_Edition'
UNION ALL
SELECT 311, N'Microsoft, Oracle_Database_11g_R2_and_WebLogic_Server_11g, Oracle_Database_11g_R2_and_WebLogic_Serve', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.187' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.187' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'Oracle_Database_11g_R2_and_WebLogic_Server_11g', N'Oracle_Database_11g_R2_and_WebLogic_Server_11g_EE'
UNION ALL
SELECT 312, N'Microsoft, Oracle_Database_11g_R2_and_WebLogic_Server_11g, Oracle_Database_11g_R2_and_WebLogic_Serve', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.203' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.203' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'Oracle_Database_11g_R2_and_WebLogic_Server_11g', N'Oracle_Database_11g_R2_and_WebLogic_Server_11g_Standard_Edition'
UNION ALL
SELECT 313, N'Microsoft, Oracle_Database_12c, Oracle_Database_12c_Enterprise_Edition', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.203' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.203' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'Oracle_Database_12c', N'Oracle_Database_12c_Enterprise_Edition'
UNION ALL
SELECT 314, N'Microsoft, Oracle_Database_12c, Oracle_Database_12c_Standard_Edition', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.217' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.217' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'Oracle_Database_12c', N'Oracle_Database_12c_Standard_Edition'
UNION ALL
SELECT 315, N'Microsoft, Oracle_Database_12c_and_WebLogic_Server_12c, Oracle_Database_12c_and_WebLogic_Server_12c_', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.233' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.233' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'Oracle_Database_12c_and_WebLogic_Server_12c', N'Oracle_Database_12c_and_WebLogic_Server_12c_Enterprise_Edition'
UNION ALL
SELECT 316, N'Microsoft, Oracle_Database_12c_and_WebLogic_Server_12c, Oracle_Database_12c_and_WebLogic_Server_12c_', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.233' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.233' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'Oracle_Database_12c_and_WebLogic_Server_12c', N'Oracle_Database_12c_and_WebLogic_Server_12c_Standard_Edition'
UNION ALL
SELECT 317, N'Microsoft, Oracle_WebLogic_Server_11g, Oracle_WebLogic_Server_11g_Enterprise_Edition', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.250' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.250' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'Oracle_WebLogic_Server_11g', N'Oracle_WebLogic_Server_11g_Enterprise_Edition'
UNION ALL
SELECT 318, N'Microsoft, Oracle_WebLogic_Server_11g, Oracle_WebLogic_Server_11g_R2_Standard_Edition', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.267' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.267' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'Oracle_WebLogic_Server_11g', N'Oracle_WebLogic_Server_11g_R2_Standard_Edition'
UNION ALL
SELECT 319, N'Microsoft, Oracle_WebLogic_Server_12c, Oracle_WebLogic_Server_12c_Enterprise_Edition', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.280' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.280' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'Oracle_WebLogic_Server_12c', N'Oracle_WebLogic_Server_12c_Enterprise_Edition'
UNION ALL
SELECT 320, N'Microsoft, Oracle_WebLogic_Server_12c, Oracle_WebLogic_Server_12c_Standard_Edition', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.280' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.280' AS DateTime, N'CMP WAP Extension Installer', N'Microsoft', N'Oracle_WebLogic_Server_12c', N'Oracle_WebLogic_Server_12c_Standard_Edition'
UNION ALL
SELECT 321, N'microsoft-ads, standard-data-science-vm, standard-data-science-vm', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.297' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.297' AS DateTime, N'CMP WAP Extension Installer', N'microsoft-ads', N'standard-data-science-vm', N'standard-data-science-vm'
UNION ALL
SELECT 322, N'MicrosoftAzureSiteRecovery, Configuration-Server-Non-VPN, Windows-2012-R2-Datacenter', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.313' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.313' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftAzureSiteRecovery', N'Configuration-Server-Non-VPN', N'Windows-2012-R2-Datacenter'
UNION ALL
SELECT 323, N'MicrosoftAzureSiteRecovery, Configuration-Server-VPN, Windows-2012-R2-Datacenter', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.327' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.327' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftAzureSiteRecovery', N'Configuration-Server-VPN', N'Windows-2012-R2-Datacenter'
UNION ALL
SELECT 324, N'MicrosoftAzureSiteRecovery, Master-Target-Server, Windows-2012-R2-Datacenter', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.343' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.343' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftAzureSiteRecovery', N'Master-Target-Server', N'Windows-2012-R2-Datacenter'
UNION ALL
SELECT 325, N'MicrosoftAzureSiteRecovery, Process-Server, Windows-2012-R2-Datacenter', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.390' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.390' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftAzureSiteRecovery', N'Process-Server', N'Windows-2012-R2-Datacenter'
UNION ALL
SELECT 326, N'MicrosoftBizTalkServer, BizTalk-Server, 2013-Developer', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.413' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.413' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftBizTalkServer', N'BizTalk-Server', N'2013-Developer'
UNION ALL
SELECT 327, N'MicrosoftBizTalkServer, BizTalk-Server, 2013-R2-Developer', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.423' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.423' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftBizTalkServer', N'BizTalk-Server', N'2013-R2-Developer'
UNION ALL
SELECT 328, N'MicrosoftBizTalkServer, BizTalk-Server, 2013-R2-Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.423' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.423' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftBizTalkServer', N'BizTalk-Server', N'2013-R2-Enterprise'
UNION ALL
SELECT 329, N'MicrosoftBizTalkServer, BizTalk-Server, 2013-R2-Standard', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.440' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.440' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftBizTalkServer', N'BizTalk-Server', N'2013-R2-Standard'
UNION ALL
SELECT 330, N'MicrosoftDynamicsAX, Dynamics, Pre-Req-AX6-ARA', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.453' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.453' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsAX', N'Dynamics', N'Pre-Req-AX6-ARA'
UNION ALL
SELECT 331, N'MicrosoftDynamicsAX, Dynamics, Pre-Req-AX7-AOS', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.470' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.470' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsAX', N'Dynamics', N'Pre-Req-AX7-AOS'
UNION ALL
SELECT 332, N'MicrosoftDynamicsAX, Dynamics, Pre-Req-AX7-BI', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.493' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.493' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsAX', N'Dynamics', N'Pre-Req-AX7-BI'
UNION ALL
SELECT 333, N'MicrosoftDynamicsAX, Dynamics, Pre-Req-AX7-OneBox', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.503' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.503' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsAX', N'Dynamics', N'Pre-Req-AX7-OneBox'
UNION ALL
SELECT 334, N'MicrosoftDynamicsAX, Dynamics, Pre-Req-AX7-Onebox-VSENT', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.503' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.503' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsAX', N'Dynamics', N'Pre-Req-AX7-Onebox-VSENT'
UNION ALL
SELECT 335, N'MicrosoftDynamicsAX, DynamicsAX, Pre-Req-2012-R3-AOS-Test', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.517' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.517' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsAX', N'DynamicsAX', N'Pre-Req-2012-R3-AOS-Test'
UNION ALL
SELECT 336, N'MicrosoftDynamicsAX, DynamicsAX, Pre-Req-2012-R3-AXOneBox', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.533' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.533' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsAX', N'DynamicsAX', N'Pre-Req-2012-R3-AXOneBox'
UNION ALL
SELECT 337, N'MicrosoftDynamicsAX, DynamicsAX, Pre-Req-2012-R3-Client-Test', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.550' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.550' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsAX', N'DynamicsAX', N'Pre-Req-2012-R3-Client-Test'
UNION ALL
SELECT 338, N'MicrosoftDynamicsAX, DynamicsAX, Pre-Req-2012-R3-DatabaseServer', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.550' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.550' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsAX', N'DynamicsAX', N'Pre-Req-2012-R3-DatabaseServer'
UNION ALL
SELECT 339, N'MicrosoftDynamicsAX, DynamicsAX, Pre-Req-2012-R3-EnterprisePortal', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.563' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.563' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsAX', N'DynamicsAX', N'Pre-Req-2012-R3-EnterprisePortal'
UNION ALL
SELECT 340, N'MicrosoftDynamicsAX, DynamicsAX, Pre-Req-2012-R3-RetailE-Commerce', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.580' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.580' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsAX', N'DynamicsAX', N'Pre-Req-2012-R3-RetailE-Commerce'
UNION ALL
SELECT 341, N'MicrosoftDynamicsAX, DynamicsAX, Pre-Req-2012-R3-RetailEssentials', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.597' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.597' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsAX', N'DynamicsAX', N'Pre-Req-2012-R3-RetailEssentials'
UNION ALL
SELECT 342, N'MicrosoftDynamicsGP, Dynamics-GP, 2013-Developer', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.610' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.610' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsGP', N'Dynamics-GP', N'2013-Developer'
UNION ALL
SELECT 343, N'MicrosoftDynamicsGP, Dynamics-GP, 2013-R2-Developer', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.623' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.623' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsGP', N'Dynamics-GP', N'2013-R2-Developer'
UNION ALL
SELECT 344, N'MicrosoftDynamicsGP, Dynamics-GP, 2015-Developer', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.633' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.633' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsGP', N'Dynamics-GP', N'2015-Developer'
UNION ALL
SELECT 345, N'MicrosoftDynamicsNAV, DynamicsNAV, 2015', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.633' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.633' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsNAV', N'DynamicsNAV', N'2015'
UNION ALL
SELECT 346, N'MicrosoftDynamicsNAV, DynamicsNAV, 2016', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.650' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.650' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftDynamicsNAV', N'DynamicsNAV', N'2016'
UNION ALL
SELECT 347, N'MicrosoftHybridCloudStorage, NA, 1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.663' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.663' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftHybridCloudStorage', N'NA', N'1'
UNION ALL
SELECT 348, N'MicrosoftHybridCloudStorage, StorSimple, StorSimple-8000-Series-Release', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.663' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.663' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftHybridCloudStorage', N'StorSimple', N'StorSimple-8000-Series-Release'
UNION ALL
SELECT 349, N'MicrosoftHybridCloudStorage, StorSimple, StorSimple-8000-Series-Update-0.3', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.680' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.680' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftHybridCloudStorage', N'StorSimple', N'StorSimple-8000-Series-Update-0.3'
UNION ALL
SELECT 350, N'MicrosoftHybridCloudStorage, TestingPrivateBakedVHDPremiumStorage, 2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.700' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.700' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftHybridCloudStorage', N'TestingPrivateBakedVHDPremiumStorage', N'2'
UNION ALL
SELECT 351, N'MicrosoftSharePoint, MicrosoftSharePointServer, 2013', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.713' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.713' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSharePoint', N'MicrosoftSharePointServer', N'2013'
UNION ALL
SELECT 352, N'MicrosoftSharePoint, MicrosoftSharePointServer, 2016', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.713' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.713' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSharePoint', N'MicrosoftSharePointServer', N'2016'
UNION ALL
SELECT 353, N'MicrosoftSQLServer, SQL2008R2SP3-WS2008R2SP1, Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.727' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.727' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2008R2SP3-WS2008R2SP1', N'Enterprise'
UNION ALL
SELECT 354, N'MicrosoftSQLServer, SQL2008R2SP3-WS2008R2SP1, Standard', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.743' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.743' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2008R2SP3-WS2008R2SP1', N'Standard'
UNION ALL
SELECT 355, N'MicrosoftSQLServer, SQL2008R2SP3-WS2008R2SP1, Web', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.757' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.757' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2008R2SP3-WS2008R2SP1', N'Web'
UNION ALL
SELECT 356, N'MicrosoftSQLServer, SQL2012SP2-WS2012, Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.757' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.757' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2012SP2-WS2012', N'Enterprise'
UNION ALL
SELECT 357, N'MicrosoftSQLServer, SQL2012SP2-WS2012, Enterprise-Optimized-for-DW', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.780' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.780' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2012SP2-WS2012', N'Enterprise-Optimized-for-DW'
UNION ALL
SELECT 358, N'MicrosoftSQLServer, SQL2012SP2-WS2012, Enterprise-Optimized-for-OLTP', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.790' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.790' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2012SP2-WS2012', N'Enterprise-Optimized-for-OLTP'
UNION ALL
SELECT 359, N'MicrosoftSQLServer, SQL2012SP2-WS2012, Standard', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.790' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.790' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2012SP2-WS2012', N'Standard'
UNION ALL
SELECT 360, N'MicrosoftSQLServer, SQL2012SP2-WS2012, Web', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.807' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.807' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2012SP2-WS2012', N'Web'
UNION ALL
SELECT 361, N'MicrosoftSQLServer, SQL2012SP2-WS2012R2, Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.820' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.820' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2012SP2-WS2012R2', N'Enterprise'
UNION ALL
SELECT 362, N'MicrosoftSQLServer, SQL2012SP2-WS2012R2, Enterprise-Optimized-for-DW', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.820' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.820' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2012SP2-WS2012R2', N'Enterprise-Optimized-for-DW'
UNION ALL
SELECT 363, N'MicrosoftSQLServer, SQL2012SP2-WS2012R2, Enterprise-Optimized-for-OLTP', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.853' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.853' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2012SP2-WS2012R2', N'Enterprise-Optimized-for-OLTP'
UNION ALL
SELECT 364, N'MicrosoftSQLServer, SQL2012SP2-WS2012R2, Standard', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.867' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.867' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2012SP2-WS2012R2', N'Standard'
UNION ALL
SELECT 365, N'MicrosoftSQLServer, SQL2012SP2-WS2012R2, Web', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.883' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.883' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2012SP2-WS2012R2', N'Web'
UNION ALL
SELECT 366, N'MicrosoftSQLServer, SQL2014-WS2012R2, Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.883' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.883' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2014-WS2012R2', N'Enterprise'
UNION ALL
SELECT 367, N'MicrosoftSQLServer, SQL2014-WS2012R2, Enterprise-Optimized', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.900' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.900' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2014-WS2012R2', N'Enterprise-Optimized'
UNION ALL
SELECT 368, N'MicrosoftSQLServer, SQL2014-WS2012R2, Enterprise-Optimized-for-DW', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.917' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.917' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2014-WS2012R2', N'Enterprise-Optimized-for-DW'
UNION ALL
SELECT 369, N'MicrosoftSQLServer, SQL2014-WS2012R2, Enterprise-Optimized-for-OLTP', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.930' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.930' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2014-WS2012R2', N'Enterprise-Optimized-for-OLTP'
UNION ALL
SELECT 370, N'MicrosoftSQLServer, SQL2014-WS2012R2, Standard', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.930' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.930' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2014-WS2012R2', N'Standard'
UNION ALL
SELECT 371, N'MicrosoftSQLServer, SQL2014-WS2012R2, Web', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:40.947' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:40.947' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2014-WS2012R2', N'Web'
UNION ALL
SELECT 372, N'MicrosoftSQLServer, SQL2014SP1-WS2012R2, Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.167' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.167' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2014SP1-WS2012R2', N'Enterprise'
UNION ALL
SELECT 373, N'MicrosoftSQLServer, SQL2014SP1-WS2012R2, Enterprise-Optimized-for-DW', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.167' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.167' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2014SP1-WS2012R2', N'Enterprise-Optimized-for-DW'
UNION ALL
SELECT 374, N'MicrosoftSQLServer, SQL2014SP1-WS2012R2, Enterprise-Optimized-for-OLTP', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.180' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.180' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2014SP1-WS2012R2', N'Enterprise-Optimized-for-OLTP'
UNION ALL
SELECT 375, N'MicrosoftSQLServer, SQL2014SP1-WS2012R2, Standard', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.197' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.197' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2014SP1-WS2012R2', N'Standard'
UNION ALL
SELECT 376, N'MicrosoftSQLServer, SQL2014SP1-WS2012R2, Web', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.197' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.197' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2014SP1-WS2012R2', N'Web'
UNION ALL
SELECT 377, N'MicrosoftSQLServer, SQL2016CTP2.1-WS2012R2, Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.213' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.213' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2016CTP2.1-WS2012R2', N'Enterprise'
UNION ALL
SELECT 378, N'MicrosoftSQLServer, SQL2016CTP2.2-WS2012R2, Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.227' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.227' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2016CTP2.2-WS2012R2', N'Enterprise'
UNION ALL
SELECT 379, N'MicrosoftSQLServer, SQL2016CTP2.3-WS2012R2, Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.227' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.227' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2016CTP2.3-WS2012R2', N'Enterprise'
UNION ALL
SELECT 380, N'MicrosoftSQLServer, SQL2016CTP2.4-WS2012R2, Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.243' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.243' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2016CTP2.4-WS2012R2', N'Enterprise'
UNION ALL
SELECT 381, N'MicrosoftSQLServer, SQL2016CTP3-WS2012R2, Evaluation', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.260' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.260' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftSQLServer', N'SQL2016CTP3-WS2012R2', N'Evaluation'
UNION ALL
SELECT 382, N'MicrosoftVisualStudio, TeamFoundationServer, 2013-Update4', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.277' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.277' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'TeamFoundationServer', N'2013-Update4'
UNION ALL
SELECT 383, N'MicrosoftVisualStudio, VisualStudio, 2013-Community-Update-4-ws2012-az25-ntvs10', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.290' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.290' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2013-Community-Update-4-ws2012-az25-ntvs10'
UNION ALL
SELECT 384, N'MicrosoftVisualStudio, VisualStudio, 2013-Community-Update-4-ws2012-az26', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.307' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.307' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2013-Community-Update-4-ws2012-az26'
UNION ALL
SELECT 385, N'MicrosoftVisualStudio, VisualStudio, 2013-Community-Update-4-ws2012-az26-cor31', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.307' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.307' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2013-Community-Update-4-ws2012-az26-cor31'
UNION ALL
SELECT 386, N'MicrosoftVisualStudio, VisualStudio, 2013-Premium-Update-4-win81', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.323' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.323' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2013-Premium-Update-4-win81'
UNION ALL
SELECT 387, N'MicrosoftVisualStudio, VisualStudio, 2013-Premium-Update-4-win81n-az26', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.337' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.337' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2013-Premium-Update-4-win81n-az26'
UNION ALL
SELECT 388, N'MicrosoftVisualStudio, VisualStudio, 2013-Premium-Update-4-ws2012-az26', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.350' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.350' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2013-Premium-Update-4-ws2012-az26'
UNION ALL
SELECT 389, N'MicrosoftVisualStudio, VisualStudio, 2013-Professional-Update-4-ws2012-az26', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.367' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.367' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2013-Professional-Update-4-ws2012-az26'
UNION ALL
SELECT 390, N'MicrosoftVisualStudio, VisualStudio, 2013-Ultimate-Update-4-win81', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.383' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.383' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2013-Ultimate-Update-4-win81'
UNION ALL
SELECT 391, N'MicrosoftVisualStudio, VisualStudio, 2013-Ultimate-Update-4-win81n-az26', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.397' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.397' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2013-Ultimate-Update-4-win81n-az26'
UNION ALL
SELECT 392, N'MicrosoftVisualStudio, VisualStudio, 2013-Ultimate-Update-4-ws2012-az26', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.407' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.407' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2013-Ultimate-Update-4-ws2012-az26'
UNION ALL
SELECT 393, N'MicrosoftVisualStudio, VisualStudio, 2015-Community-RC', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.420' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.420' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2015-Community-RC'
UNION ALL
SELECT 394, N'MicrosoftVisualStudio, VisualStudio, 2015-Enterprise-RC', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.437' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.437' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2015-Enterprise-RC'
UNION ALL
SELECT 395, N'MicrosoftVisualStudio, VisualStudio, 2015-Enterprise-Win10Tools', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.437' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.437' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2015-Enterprise-Win10Tools'
UNION ALL
SELECT 396, N'MicrosoftVisualStudio, VisualStudio, 2015-Professional-RC', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.450' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.450' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'2015-Professional-RC'
UNION ALL
SELECT 397, N'MicrosoftVisualStudio, VisualStudio, CoreCLR', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.483' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.483' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'CoreCLR'
UNION ALL
SELECT 398, N'MicrosoftVisualStudio, VisualStudio, VS-2013-Community-VSU5-AzureSDK-2.7-Win8.1-N-x64', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.483' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.483' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2013-Community-VSU5-AzureSDK-2.7-Win8.1-N-x64'
UNION ALL
SELECT 399, N'MicrosoftVisualStudio, VisualStudio, VS-2013-Community-VSU5-AzureSDK-2.7-WS2012R2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.497' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.497' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2013-Community-VSU5-AzureSDK-2.7-WS2012R2'
UNION ALL
SELECT 400, N'MicrosoftVisualStudio, VisualStudio, VS-2013-Community-VSU5-Cordova-CTP3.2-AzureSDK-2.7-WS2012R2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.513' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.513' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2013-Community-VSU5-Cordova-CTP3.2-AzureSDK-2.7-WS2012R2'
UNION ALL
SELECT 401, N'MicrosoftVisualStudio, VisualStudio, VS-2013-Premium-VSU5-AzureSDK-2.7-SQL-WS2012R2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.530' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.530' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2013-Premium-VSU5-AzureSDK-2.7-SQL-WS2012R2'
UNION ALL
SELECT 402, N'MicrosoftVisualStudio, VisualStudio, VS-2013-Premium-VSU5-AzureSDK-2.7-Win8.1-N-x64', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.543' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.543' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2013-Premium-VSU5-AzureSDK-2.7-Win8.1-N-x64'
UNION ALL
SELECT 403, N'MicrosoftVisualStudio, VisualStudio, VS-2013-Premium-VSU5-AzureSDK-2.7-WS2012R2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.560' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.560' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2013-Premium-VSU5-AzureSDK-2.7-WS2012R2'
UNION ALL
SELECT 404, N'MicrosoftVisualStudio, VisualStudio, VS-2013-Ultimate-VSU5-AzureSDK-2.7-SQL-WS2012R2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.577' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.577' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2013-Ultimate-VSU5-AzureSDK-2.7-SQL-WS2012R2'
UNION ALL
SELECT 405, N'MicrosoftVisualStudio, VisualStudio, VS-2013-Ultimate-VSU5-AzureSDK-2.7-Win8.1-N-x64', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.590' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.590' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2013-Ultimate-VSU5-AzureSDK-2.7-Win8.1-N-x64'
UNION ALL
SELECT 406, N'MicrosoftVisualStudio, VisualStudio, VS-2013-Ultimate-VSU5-AzureSDK-2.7-WS2012R2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.607' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.607' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2013-Ultimate-VSU5-AzureSDK-2.7-WS2012R2'
UNION ALL
SELECT 407, N'MicrosoftVisualStudio, VisualStudio, VS-2015-Community-AzureSDK-2.7-Cordova-Win8.1-N-x64', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.780' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.780' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2015-Community-AzureSDK-2.7-Cordova-Win8.1-N-x64'
UNION ALL
SELECT 408, N'MicrosoftVisualStudio, VisualStudio, VS-2015-Community-AzureSDK-2.7-W10T-Win10-N', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.793' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.793' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2015-Community-AzureSDK-2.7-W10T-Win10-N'
UNION ALL
SELECT 409, N'MicrosoftVisualStudio, VisualStudio, VS-2015-Community-AzureSDK-2.7-WS2012R2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.793' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.793' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2015-Community-AzureSDK-2.7-WS2012R2'
UNION ALL
SELECT 410, N'MicrosoftVisualStudio, VisualStudio, VS-2015-Enterprise-AzureSDK-2.7-Cordova-Win8.1-N-x64', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.810' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.810' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2015-Enterprise-AzureSDK-2.7-Cordova-Win8.1-N-x64'
UNION ALL
SELECT 411, N'MicrosoftVisualStudio, VisualStudio, VS-2015-Enterprise-AzureSDK-2.7-W10T-Win10-N', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.827' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.827' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2015-Enterprise-AzureSDK-2.7-W10T-Win10-N'
UNION ALL
SELECT 412, N'MicrosoftVisualStudio, VisualStudio, VS-2015-Enterprise-AzureSDK-2.7-WS2012R2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.840' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.840' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2015-Enterprise-AzureSDK-2.7-WS2012R2'
UNION ALL
SELECT 413, N'MicrosoftVisualStudio, VisualStudio, VS-2015-Professional-AzureSDK-2.7-Cordova-Win8.1-N-x64', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.857' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.857' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2015-Professional-AzureSDK-2.7-Cordova-Win8.1-N-x64'
UNION ALL
SELECT 414, N'MicrosoftVisualStudio, VisualStudio, VS-2015-Professional-AzureSDK-2.7-W10T-Win10-N', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.857' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.857' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'VisualStudio', N'VS-2015-Professional-AzureSDK-2.7-W10T-Win10-N'
UNION ALL
SELECT 415, N'MicrosoftVisualStudio, Windows, 10-Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.873' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.873' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'Windows', N'10-Enterprise'
UNION ALL
SELECT 416, N'MicrosoftVisualStudio, Windows, 10-Enterprise-N', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.887' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.887' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'Windows', N'10-Enterprise-N'
UNION ALL
SELECT 417, N'MicrosoftVisualStudio, Windows, 7.0-Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.887' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.887' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'Windows', N'7.0-Enterprise'
UNION ALL
SELECT 418, N'MicrosoftVisualStudio, Windows, 7.0-Enterprise-N', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.903' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.903' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'Windows', N'7.0-Enterprise-N'
UNION ALL
SELECT 419, N'MicrosoftVisualStudio, Windows, 8.1-Enterprise', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.920' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.920' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'Windows', N'8.1-Enterprise'
UNION ALL
SELECT 420, N'MicrosoftVisualStudio, Windows, 8.1-Enterprise-N', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.920' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.920' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftVisualStudio', N'Windows', N'8.1-Enterprise-N'
UNION ALL
SELECT 421, N'MicrosoftWindowsServer, WindowsServer, 2008-R2-SP1', N'NULL', N'NULL', N'NULL', 0, 1, N'2015-11-08 06:41:41.937' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.937' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'2008-R2-SP1'
UNION ALL
SELECT 422, N'MicrosoftWindowsServer, WindowsServer, 2012-Datacenter', N'NULL', N'NULL', N'NULL', 0, 1, N'2015-11-08 06:41:41.950' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.950' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'2012-Datacenter'
UNION ALL
SELECT 423, N'MicrosoftWindowsServer, WindowsServer, 2012-R2-Datacenter', N'NULL', N'NULL', N'NULL', 0, 1, N'2015-11-08 06:41:41.967' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.967' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'2012-R2-Datacenter'
UNION ALL
SELECT 424, N'MicrosoftWindowsServer, WindowsServer, 2016-Technical-Preview-3-with-Containers', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.967' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.967' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'2016-Technical-Preview-3-with-Containers'
UNION ALL
SELECT 425, N'MicrosoftWindowsServer, WindowsServer, 2016-Technical-Preview-4-Nano-Server', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.983' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.983' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'2016-Technical-Preview-4-Nano-Server'
UNION ALL
SELECT 426, N'MicrosoftWindowsServer, WindowsServer, 2016-Technical-Preview-Nano-Server', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.983' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.983' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'2016-Technical-Preview-Nano-Server'
UNION ALL
SELECT 427, N'MicrosoftWindowsServer, WindowsServer, Windows-Server-Technical-Preview', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:41.997' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:41.997' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServer', N'WindowsServer', N'Windows-Server-Technical-Preview'
UNION ALL
SELECT 428, N'MicrosoftWindowsServerEssentials, WindowsServerEssentials, WindowsServerEssentials', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.013' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.013' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServerEssentials', N'WindowsServerEssentials', N'WindowsServerEssentials'
UNION ALL
SELECT 429, N'MicrosoftWindowsServerHPCPack, WindowsServerHPCPack, 2012R2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.013' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.013' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServerHPCPack', N'WindowsServerHPCPack', N'2012R2'
UNION ALL
SELECT 430, N'MicrosoftWindowsServerHPCPack, WindowsServerHPCPack, 2012R2CN', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.037' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.037' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServerHPCPack', N'WindowsServerHPCPack', N'2012R2CN'
UNION ALL
SELECT 431, N'MicrosoftWindowsServerHPCPack, WindowsServerHPCPack, 2012R2CNExcel', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.037' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.037' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServerHPCPack', N'WindowsServerHPCPack', N'2012R2CNExcel'
UNION ALL
SELECT 432, N'MicrosoftWindowsServerHPCPack, WindowsServerHPCPack, TechnicalPreview', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.050' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.050' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServerHPCPack', N'WindowsServerHPCPack', N'TechnicalPreview'
UNION ALL
SELECT 433, N'MicrosoftWindowsServerRemoteDesktop, WindowServer, RDSH-Office13P', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.063' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.063' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServerRemoteDesktop', N'WindowServer', N'RDSH-Office13P'
UNION ALL
SELECT 434, N'MicrosoftWindowsServerRemoteDesktop, WindowServer, RDSH-Office365P', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.063' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.063' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServerRemoteDesktop', N'WindowServer', N'RDSH-Office365P'
UNION ALL
SELECT 435, N'MicrosoftWindowsServerRemoteDesktop, WindowServer, Remote-Desktop-Session-Host', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.080' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.080' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServerRemoteDesktop', N'WindowServer', N'Remote-Desktop-Session-Host'
UNION ALL
SELECT 436, N'MicrosoftWindowsServerRemoteDesktop, WindowsServer, RDSH-Office13P', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.080' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.080' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServerRemoteDesktop', N'WindowsServer', N'RDSH-Office13P'
UNION ALL
SELECT 437, N'MicrosoftWindowsServerRemoteDesktop, WindowsServer, RDSH-Office365P', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.097' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.097' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServerRemoteDesktop', N'WindowsServer', N'RDSH-Office365P'
UNION ALL
SELECT 438, N'MicrosoftWindowsServerRemoteDesktop, WindowsServer, Remote-Desktop-Session-Host', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.110' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.110' AS DateTime, N'CMP WAP Extension Installer', N'MicrosoftWindowsServerRemoteDesktop', N'WindowsServer', N'Remote-Desktop-Session-Host'
UNION ALL
SELECT 439, N'midvision, websphere-application-server-be, midvision-ibm_was_base_edition-7_0_0', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.127' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.127' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-be', N'midvision-ibm_was_base_edition-7_0_0'
UNION ALL
SELECT 440, N'midvision, websphere-application-server-be, midvision-ibm_was_base_edition-7_0_0_29', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.143' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.143' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-be', N'midvision-ibm_was_base_edition-7_0_0_29'
UNION ALL
SELECT 441, N'midvision, websphere-application-server-be, midvision-ibm_was_base_edition-7_0_0_37', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.157' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.157' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-be', N'midvision-ibm_was_base_edition-7_0_0_37'
UNION ALL
SELECT 442, N'midvision, websphere-application-server-be, midvision-ibm_was_base_edition-8_0_0', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.157' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.157' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-be', N'midvision-ibm_was_base_edition-8_0_0'
UNION ALL
SELECT 443, N'midvision, websphere-application-server-be, midvision-ibm_was_base_edition-8_0_0_10', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.173' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.173' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-be', N'midvision-ibm_was_base_edition-8_0_0_10'
UNION ALL
SELECT 444, N'midvision, websphere-application-server-be, midvision-ibm_was_base_edition-8_0_0_7', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.173' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.173' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-be', N'midvision-ibm_was_base_edition-8_0_0_7'
UNION ALL
SELECT 445, N'midvision, websphere-application-server-be, midvision-ibm_was_base_edition-8_5_0', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.190' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.190' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-be', N'midvision-ibm_was_base_edition-8_5_0'
UNION ALL
SELECT 446, N'midvision, websphere-application-server-be, midvision-ibm_was_base_edition-8_5_0_2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.207' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.207' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-be', N'midvision-ibm_was_base_edition-8_5_0_2'
UNION ALL
SELECT 447, N'midvision, websphere-application-server-be, midvision-ibm_was_base_edition-8_5_5', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.207' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.207' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-be', N'midvision-ibm_was_base_edition-8_5_5'
UNION ALL
SELECT 448, N'midvision, websphere-application-server-be, midvision-ibm_was_base_edition-8_5_5_1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.220' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.220' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-be', N'midvision-ibm_was_base_edition-8_5_5_1'
UNION ALL
SELECT 449, N'midvision, websphere-application-server-be, midvision-ibm_was_base_edition-8_5_5_6', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.237' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.237' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-be', N'midvision-ibm_was_base_edition-8_5_5_6'
UNION ALL
SELECT 450, N'midvision, websphere-application-server-lp, midvision-ibm_was_liberty_profile-8_5_5_1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.253' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.253' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-lp', N'midvision-ibm_was_liberty_profile-8_5_5_1'
UNION ALL
SELECT 451, N'midvision, websphere-application-server-lp, midvision-ibm_was_liberty_profile-8_5_5_6', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.253' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.253' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-lp', N'midvision-ibm_was_liberty_profile-8_5_5_6'
UNION ALL
SELECT 452, N'midvision, websphere-application-server-nde, midvision-ibm_was_nd_edition-7_0_0_29', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.267' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.267' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-nde', N'midvision-ibm_was_nd_edition-7_0_0_29'
UNION ALL
SELECT 453, N'midvision, websphere-application-server-nde, midvision-ibm_was_nd_edition-7_0_0_37', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.283' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.283' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-nde', N'midvision-ibm_was_nd_edition-7_0_0_37'
UNION ALL
SELECT 454, N'midvision, websphere-application-server-nde, midvision-ibm_was_nd_edition-8_0_0_10', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.283' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.283' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-nde', N'midvision-ibm_was_nd_edition-8_0_0_10'
UNION ALL
SELECT 455, N'midvision, websphere-application-server-nde, midvision-ibm_was_nd_edition-8_0_0_7', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.300' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.300' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-nde', N'midvision-ibm_was_nd_edition-8_0_0_7'
UNION ALL
SELECT 456, N'midvision, websphere-application-server-nde, midvision-ibm_was_nd_edition-8_5_0_2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.313' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.313' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-nde', N'midvision-ibm_was_nd_edition-8_5_0_2'
UNION ALL
SELECT 457, N'midvision, websphere-application-server-nde, midvision-ibm_was_nd_edition-8_5_5_1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.330' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.330' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-nde', N'midvision-ibm_was_nd_edition-8_5_5_1'
UNION ALL
SELECT 458, N'midvision, websphere-application-server-nde, midvision-ibm_was_nd_edition-8_5_5_6', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.353' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.353' AS DateTime, N'CMP WAP Extension Installer', N'midvision', N'websphere-application-server-nde', N'midvision-ibm_was_nd_edition-8_5_5_6'
UNION ALL
SELECT 459, N'miracl_linux, asianux-server-4-sp4, axs4sp4licazu', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.363' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.363' AS DateTime, N'CMP WAP Extension Installer', N'miracl_linux', N'asianux-server-4-sp4', N'axs4sp4licazu'
UNION ALL
SELECT 460, N'miracl_linux, asianux-server-7, axs7licazu', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.363' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.363' AS DateTime, N'CMP WAP Extension Installer', N'miracl_linux', N'asianux-server-7', N'axs7licazu'
UNION ALL
SELECT 461, N'mokxa-technologies, joget-enterprise, joget-ee-v414', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.380' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.380' AS DateTime, N'CMP WAP Extension Installer', N'mokxa-technologies', N'joget-enterprise', N'joget-ee-v414'
UNION ALL
SELECT 462, N'moviemasher, moviemasher, moviemasher', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.397' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.397' AS DateTime, N'CMP WAP Extension Installer', N'moviemasher', N'moviemasher', N'moviemasher'
UNION ALL
SELECT 463, N'msopentech, ibm-db2-10-5, db2-10-5-0-4-aws', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.397' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.397' AS DateTime, N'CMP WAP Extension Installer', N'msopentech', N'ibm-db2-10-5', N'db2-10-5-0-4-aws'
UNION ALL
SELECT 464, N'msopentech, mq-v8-0, mq-v8-0-0-1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.413' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.413' AS DateTime, N'CMP WAP Extension Installer', N'msopentech', N'mq-v8-0', N'mq-v8-0-0-1'
UNION ALL
SELECT 465, N'msopentech, oracle-db-11g, db-11g-ee-all', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.430' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.430' AS DateTime, N'CMP WAP Extension Installer', N'msopentech', N'oracle-db-11g', N'db-11g-ee-all'
UNION ALL
SELECT 466, N'msopentech, oracle-db-11g, db-11g-ee-popular', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.430' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.430' AS DateTime, N'CMP WAP Extension Installer', N'msopentech', N'oracle-db-11g', N'db-11g-ee-popular'
UNION ALL
SELECT 467, N'msopentech, oracle-db-12c, db-12c-ee-all', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.447' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.447' AS DateTime, N'CMP WAP Extension Installer', N'msopentech', N'oracle-db-12c', N'db-12c-ee-all'
UNION ALL
SELECT 468, N'msopentech, oracle-db-12c, db-12c-ee-popular', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.460' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.460' AS DateTime, N'CMP WAP Extension Installer', N'msopentech', N'oracle-db-12c', N'db-12c-ee-popular'
UNION ALL
SELECT 469, N'msopentech, was-8-5, was-8-5-5-3', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.460' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.460' AS DateTime, N'CMP WAP Extension Installer', N'msopentech', N'was-8-5', N'was-8-5-5-3'
UNION ALL
SELECT 470, N'mvp-systems, jamsscheduler-single-instance, byol_x-num-servers_x-num-jobs', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.477' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.477' AS DateTime, N'CMP WAP Extension Installer', N'mvp-systems', N'jamsscheduler-single-instance', N'byol_x-num-servers_x-num-jobs'
UNION ALL
SELECT 471, N'mxhero, mail2cloud, mxhm2c-10000pk-gdss-x', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.493' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.493' AS DateTime, N'CMP WAP Extension Installer', N'mxhero', N'mail2cloud', N'mxhm2c-10000pk-gdss-x'
UNION ALL
SELECT 472, N'mxhero, mail2cloud, mxhm2c-1000pk-gdss-x', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.507' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.507' AS DateTime, N'CMP WAP Extension Installer', N'mxhero', N'mail2cloud', N'mxhm2c-1000pk-gdss-x'
UNION ALL
SELECT 473, N'mxhero, mail2cloud, mxhm2c-100pk-gdss-x', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.507' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.507' AS DateTime, N'CMP WAP Extension Installer', N'mxhero', N'mail2cloud', N'mxhm2c-100pk-gdss-x'
UNION ALL
SELECT 474, N'mxhero, mail2cloud, mxhm2c-2500pk-gdss-x', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.523' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.523' AS DateTime, N'CMP WAP Extension Installer', N'mxhero', N'mail2cloud', N'mxhm2c-2500pk-gdss-x'
UNION ALL
SELECT 475, N'mxhero, mail2cloud, mxhm2c-250pk-gdss-x', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.540' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.540' AS DateTime, N'CMP WAP Extension Installer', N'mxhero', N'mail2cloud', N'mxhm2c-250pk-gdss-x'
UNION ALL
SELECT 476, N'mxhero, mail2cloud, mxhm2c-500pk-gdss-x', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.553' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.553' AS DateTime, N'CMP WAP Extension Installer', N'mxhero', N'mail2cloud', N'mxhm2c-500pk-gdss-x'
UNION ALL
SELECT 477, N'ncbi, ncbi-free-2-2-31, free', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.553' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.553' AS DateTime, N'CMP WAP Extension Installer', N'ncbi', N'ncbi-free-2-2-31', N'free'
UNION ALL
SELECT 478, N'netapp, netapp-altavault-cloud-integrated-storage-solution, ava-c4', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.570' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.570' AS DateTime, N'CMP WAP Extension Installer', N'netapp', N'netapp-altavault-cloud-integrated-storage-solution', N'ava-c4'
UNION ALL
SELECT 479, N'new-signature, cloud-management-portal, igvmv1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.587' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.587' AS DateTime, N'CMP WAP Extension Installer', N'new-signature', N'cloud-management-portal', N'igvmv1'
UNION ALL
SELECT 480, N'nexus, nexus-chameleon-9, nexuschameleon', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.600' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.600' AS DateTime, N'CMP WAP Extension Installer', N'nexus', N'nexus-chameleon-9', N'nexuschameleon'
UNION ALL
SELECT 481, N'nginxinc, nginx-plus-v1, nginx-plus', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.617' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.617' AS DateTime, N'CMP WAP Extension Installer', N'nginxinc', N'nginx-plus-v1', N'nginx-plus'
UNION ALL
SELECT 482, N'nicepeopleatwork, youzana, youzana', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.633' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.633' AS DateTime, N'CMP WAP Extension Installer', N'nicepeopleatwork', N'youzana', N'youzana'
UNION ALL
SELECT 483, N'nodejsapi, node-js-api, professional', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.633' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.633' AS DateTime, N'CMP WAP Extension Installer', N'nodejsapi', N'node-js-api', N'professional'
UNION ALL
SELECT 484, N'nodejsapi, node-js-api, standard', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.647' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.647' AS DateTime, N'CMP WAP Extension Installer', N'nodejsapi', N'node-js-api', N'standard'
UNION ALL
SELECT 485, N'nodejsapi, node-js-api, starter', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.663' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.663' AS DateTime, N'CMP WAP Extension Installer', N'nodejsapi', N'node-js-api', N'starter'
UNION ALL
SELECT 486, N'nuxeo, nuxeo-6-lts, nuxeo-6-lts', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.680' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.680' AS DateTime, N'CMP WAP Extension Installer', N'nuxeo', N'nuxeo-6-lts', N'nuxeo-6-lts'
UNION ALL
SELECT 487, N'officeclipsuite, officeclipsuite, officeclipsuite', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.680' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.680' AS DateTime, N'CMP WAP Extension Installer', N'officeclipsuite', N'officeclipsuite', N'officeclipsuite'
UNION ALL
SELECT 488, N'op5, op5-monitor, op5monitor', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.697' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.697' AS DateTime, N'CMP WAP Extension Installer', N'op5', N'op5-monitor', N'op5monitor'
UNION ALL
SELECT 489, N'opencell, meveo, meveo', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.710' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.710' AS DateTime, N'CMP WAP Extension Installer', N'opencell', N'meveo', N'meveo'
UNION ALL
SELECT 490, N'opencell, meveo403sp2, meveo', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.727' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.727' AS DateTime, N'CMP WAP Extension Installer', N'opencell', N'meveo403sp2', N'meveo'
UNION ALL
SELECT 491, N'OpenLogic, CentOS, 6.5', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.743' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.743' AS DateTime, N'CMP WAP Extension Installer', N'OpenLogic', N'CentOS', N'6.5'
UNION ALL
SELECT 492, N'OpenLogic, CentOS, 6.6', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.743' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.743' AS DateTime, N'CMP WAP Extension Installer', N'OpenLogic', N'CentOS', N'6.6'
UNION ALL
SELECT 493, N'OpenLogic, CentOS, 6.7', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.757' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.757' AS DateTime, N'CMP WAP Extension Installer', N'OpenLogic', N'CentOS', N'6.7'
UNION ALL
SELECT 494, N'OpenLogic, CentOS, 7.0', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.790' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.790' AS DateTime, N'CMP WAP Extension Installer', N'OpenLogic', N'CentOS', N'7.0'
UNION ALL
SELECT 495, N'OpenLogic, CentOS, 7.1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.820' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.820' AS DateTime, N'CMP WAP Extension Installer', N'OpenLogic', N'CentOS', N'7.1'
UNION ALL
SELECT 496, N'openmeap, openmeap, openmeap-1_5-windows', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.820' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.820' AS DateTime, N'CMP WAP Extension Installer', N'openmeap', N'openmeap', N'openmeap-1_5-windows'
UNION ALL
SELECT 497, N'opennebulasystems, opennebula-sandbox, opennebula-sandbox', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.837' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.837' AS DateTime, N'CMP WAP Extension Installer', N'opennebulasystems', N'opennebula-sandbox', N'opennebula-sandbox'
UNION ALL
SELECT 498, N'opentext, opentext_content-server_105, ot-test-cs105', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.850' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.850' AS DateTime, N'CMP WAP Extension Installer', N'opentext', N'opentext_content-server_105', N'ot-test-cs105'
UNION ALL
SELECT 499, N'Oracle, c290a6b031d841e09f2da759bbabe71f__Oracle-Linux-6-12-2014, OL64', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.867' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.867' AS DateTime, N'CMP WAP Extension Installer', N'Oracle', N'c290a6b031d841e09f2da759bbabe71f__Oracle-Linux-6-12-2014', N'OL64'
UNION ALL
SELECT 500, N'Oracle, Oracle-Linux-7, OL70', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.867' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.867' AS DateTime, N'CMP WAP Extension Installer', N'Oracle', N'Oracle-Linux-7', N'OL70'
UNION ALL
SELECT 501, N'Oracle, Oracle-WebLogic-Server, Oracle-WebLogic-Server', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.883' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.883' AS DateTime, N'CMP WAP Extension Installer', N'Oracle', N'Oracle-WebLogic-Server', N'Oracle-WebLogic-Server'
UNION ALL
SELECT 502, N'orientdb, orientdb-community-edition, orientdb-community-edition-2_0_10', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.897' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.897' AS DateTime, N'CMP WAP Extension Installer', N'orientdb', N'orientdb-community-edition', N'orientdb-community-edition-2_0_10'
UNION ALL
SELECT 503, N'outsystems, outsystems, outsystems_azure_intro', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.913' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.913' AS DateTime, N'CMP WAP Extension Installer', N'outsystems', N'outsystems', N'outsystems_azure_intro'
UNION ALL
SELECT 504, N'pointmatter, pointmatter-csvhub, pointmatter-csvhub', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.913' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.913' AS DateTime, N'CMP WAP Extension Installer', N'pointmatter', N'pointmatter-csvhub', N'pointmatter-csvhub'
UNION ALL
SELECT 505, N'predictionio, predictionio, community', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.930' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.930' AS DateTime, N'CMP WAP Extension Installer', N'predictionio', N'predictionio', N'community'
UNION ALL
SELECT 506, N'predixion, predixion-insight-2015-02-feb, r-only', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.950' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.950' AS DateTime, N'CMP WAP Extension Installer', N'predixion', N'predixion-insight-2015-02-feb', N'r-only'
UNION ALL
SELECT 507, N'prestashop, prestashop16-lamp, prestashopdev', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.967' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.967' AS DateTime, N'CMP WAP Extension Installer', N'prestashop', N'prestashop16-lamp', N'prestashopdev'
UNION ALL
SELECT 508, N'primestream, xchange_media_cloud_10_users, xchange_media_cloud_10_users', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.967' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.967' AS DateTime, N'CMP WAP Extension Installer', N'primestream', N'xchange_media_cloud_10_users', N'xchange_media_cloud_10_users'
UNION ALL
SELECT 509, N'profisee, maestro-base-server, maestrosibase', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.983' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.983' AS DateTime, N'CMP WAP Extension Installer', N'profisee', N'maestro-base-server', N'maestrosibase'
UNION ALL
SELECT 510, N'ptv_group, ptv_xserver, win_northamerica_one_eighteen', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:42.997' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:42.997' AS DateTime, N'CMP WAP Extension Installer', N'ptv_group', N'ptv_xserver', N'win_northamerica_one_eighteen'
UNION ALL
SELECT 511, N'ptv_group, ptv_xserver, win_southamerica_one_eighteen', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.013' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.013' AS DateTime, N'CMP WAP Extension Installer', N'ptv_group', N'ptv_xserver', N'win_southamerica_one_eighteen'
UNION ALL
SELECT 512, N'PuppetLabs, PuppetEnterprise, 3.2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.030' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.030' AS DateTime, N'CMP WAP Extension Installer', N'PuppetLabs', N'PuppetEnterprise', N'3.2'
UNION ALL
SELECT 513, N'PuppetLabs, PuppetEnterprise, 3.7', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.043' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.043' AS DateTime, N'CMP WAP Extension Installer', N'PuppetLabs', N'PuppetEnterprise', N'3.7'
UNION ALL
SELECT 514, N'pxlag_swiss, pxl-portal-marketplace-edition, pxlportalmarketplaceedition2015', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.060' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.060' AS DateTime, N'CMP WAP Extension Installer', N'pxlag_swiss', N'pxl-portal-marketplace-edition', N'pxlportalmarketplaceedition2015'
UNION ALL
SELECT 515, N'rancher, rancheros, os', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.060' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.060' AS DateTime, N'CMP WAP Extension Installer', N'rancher', N'rancheros', N'os'
UNION ALL
SELECT 516, N'redpoint-global, redpoint-interaction, rpi', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.077' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.077' AS DateTime, N'CMP WAP Extension Installer', N'redpoint-global', N'redpoint-interaction', N'rpi'
UNION ALL
SELECT 517, N'redpoint-global, redpoint-rpdm, rpdm', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.090' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.090' AS DateTime, N'CMP WAP Extension Installer', N'redpoint-global', N'redpoint-rpdm', N'rpdm'
UNION ALL
SELECT 518, N'remotelearner, fully-supported-moodle, version27', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.090' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.090' AS DateTime, N'CMP WAP Extension Installer', N'remotelearner', N'fully-supported-moodle', N'version27'
UNION ALL
SELECT 519, N'revolution-analytics, revolution-r-enterprise, rre74-centos65', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.107' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.107' AS DateTime, N'CMP WAP Extension Installer', N'revolution-analytics', N'revolution-r-enterprise', N'rre74-centos65'
UNION ALL
SELECT 520, N'revolution-analytics, revolution-r-enterprise, rre74-win2012r2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.123' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.123' AS DateTime, N'CMP WAP Extension Installer', N'revolution-analytics', N'revolution-r-enterprise', N'rre74-win2012r2'
UNION ALL
SELECT 521, N'RightScaleLinux, RightImage-CentOS, 6', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.123' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.123' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleLinux', N'RightImage-CentOS', N'6'
UNION ALL
SELECT 522, N'RightScaleLinux, RightImage-CentOS, 7', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.137' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.137' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleLinux', N'RightImage-CentOS', N'7'
UNION ALL
SELECT 523, N'RightScaleLinux, RightImage-Ubuntu, 12.04', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.153' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.153' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleLinux', N'RightImage-Ubuntu', N'12.04'
UNION ALL
SELECT 524, N'RightScaleLinux, RightImage-Ubuntu, 14.04', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.170' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.170' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleLinux', N'RightImage-Ubuntu', N'14.04'
UNION ALL
SELECT 525, N'RightScaleWindowsServer, RightImage-WindowsServer, 2008-R2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.170' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.170' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2008-R2'
UNION ALL
SELECT 526, N'RightScaleWindowsServer, RightImage-WindowsServer, 2008-R2-SP1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.190' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.190' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2008-R2-SP1'
UNION ALL
SELECT 527, N'RightScaleWindowsServer, RightImage-WindowsServer, 2008-R2-SP1-with-IIS-7', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.203' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.203' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2008-R2-SP1-with-IIS-7'
UNION ALL
SELECT 528, N'RightScaleWindowsServer, RightImage-WindowsServer, 2008-R2-SP1-with-SQL-2012', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.217' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.217' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2008-R2-SP1-with-SQL-2012'
UNION ALL
SELECT 529, N'RightScaleWindowsServer, RightImage-WindowsServer, 2008-R2-with-IIS-7', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.280' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.280' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2008-R2-with-IIS-7'
UNION ALL
SELECT 530, N'RightScaleWindowsServer, RightImage-WindowsServer, 2008-R2-with-SQL-2008-R2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.297' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.297' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2008-R2-with-SQL-2008-R2'
UNION ALL
SELECT 531, N'RightScaleWindowsServer, RightImage-WindowsServer, 2008-R2-with-SQL-2008-R2-Ent', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.297' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.297' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2008-R2-with-SQL-2008-R2-Ent'
UNION ALL
SELECT 532, N'RightScaleWindowsServer, RightImage-WindowsServer, 2008-R2-with-SQL-2012', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.310' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.310' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2008-R2-with-SQL-2012'
UNION ALL
SELECT 533, N'RightScaleWindowsServer, RightImage-WindowsServer, 2008-R2-with-SQL-2012-Ent', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.327' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.327' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2008-R2-with-SQL-2012-Ent'
UNION ALL
SELECT 534, N'RightScaleWindowsServer, RightImage-WindowsServer, 2012-Datacenter', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.343' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.343' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2012-Datacenter'
UNION ALL
SELECT 535, N'RightScaleWindowsServer, RightImage-WindowsServer, 2012-Datacenter-with-IIS-8', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.357' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.357' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2012-Datacenter-with-IIS-8'
UNION ALL
SELECT 536, N'RightScaleWindowsServer, RightImage-WindowsServer, 2012-Datacenter-with-SQL-2012', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.373' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.373' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2012-Datacenter-with-SQL-2012'
UNION ALL
SELECT 537, N'RightScaleWindowsServer, RightImage-WindowsServer, 2012-Datacenter-with-SQL-2012-Ent', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.390' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.390' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2012-Datacenter-with-SQL-2012-Ent'
UNION ALL
SELECT 538, N'RightScaleWindowsServer, RightImage-WindowsServer, 2012-R2-Datacenter', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.390' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.390' AS DateTime, N'CMP WAP Extension Installer', N'RightScaleWindowsServer', N'RightImage-WindowsServer', N'2012-R2-Datacenter'
UNION ALL
SELECT 539, N'riverbed, riverbed-steelcentral-appinternals, ai_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.407' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.407' AS DateTime, N'CMP WAP Extension Installer', N'riverbed', N'riverbed-steelcentral-appinternals', N'ai_byol'
UNION ALL
SELECT 540, N'riverbed, steelapp_traffic_manager, stm_dev_64_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.420' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.420' AS DateTime, N'CMP WAP Extension Installer', N'riverbed', N'steelapp_traffic_manager', N'stm_dev_64_byol'
UNION ALL
SELECT 541, N'RiverbedTechnology, steelapp_traffic_manager, stm_dev_64_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.437' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.437' AS DateTime, N'CMP WAP Extension Installer', N'RiverbedTechnology', N'steelapp_traffic_manager', N'stm_dev_64_byol'
UNION ALL
SELECT 542, N'rocketsoftware, rocket-discover, rocketdiscoverlinux', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.453' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.453' AS DateTime, N'CMP WAP Extension Installer', N'rocketsoftware', N'rocket-discover', N'rocketdiscoverlinux'
UNION ALL
SELECT 543, N'saltstack, centos65saltstackenterprise, sse-01', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.467' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.467' AS DateTime, N'CMP WAP Extension Installer', N'saltstack', N'centos65saltstackenterprise', N'sse-01'
UNION ALL
SELECT 544, N'sap, ase, ase_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.467' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.467' AS DateTime, N'CMP WAP Extension Installer', N'sap', N'ase', N'ase_byol'
UNION ALL
SELECT 545, N'sap, ase, ase_hourly', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.483' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.483' AS DateTime, N'CMP WAP Extension Installer', N'sap', N'ase', N'ase_hourly'
UNION ALL
SELECT 546, N'scalearc, scalearc-v3-5-1, byol-351', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.507' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.507' AS DateTime, N'CMP WAP Extension Installer', N'scalearc', N'scalearc-v3-5-1', N'byol-351'
UNION ALL
SELECT 547, N'scalearc, scalearc_mysql-server, byol_mysql', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.517' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.517' AS DateTime, N'CMP WAP Extension Installer', N'scalearc', N'scalearc_mysql-server', N'byol_mysql'
UNION ALL
SELECT 548, N'scalearc, scalearc_sql_server, byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.580' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.580' AS DateTime, N'CMP WAP Extension Installer', N'scalearc', N'scalearc_sql_server', N'byol'
UNION ALL
SELECT 549, N'seagate, backup, 014-141-001', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.597' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.597' AS DateTime, N'CMP WAP Extension Installer', N'seagate', N'backup', N'014-141-001'
UNION ALL
SELECT 550, N'searchblox, searchblox_server_v82, searchblox_win_version82', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.610' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.610' AS DateTime, N'CMP WAP Extension Installer', N'searchblox', N'searchblox_server_v82', N'searchblox_win_version82'
UNION ALL
SELECT 551, N'servoy, servoy-v7, servoy-7-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.627' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.627' AS DateTime, N'CMP WAP Extension Installer', N'servoy', N'servoy-v7', N'servoy-7-byol'
UNION ALL
SELECT 552, N'sharefile, netscaler-vpx-bring, netscalervpxbyol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.643' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.643' AS DateTime, N'CMP WAP Extension Installer', N'sharefile', N'netscaler-vpx-bring', N'netscalervpxbyol'
UNION ALL
SELECT 553, N'sharefile, sharefile-storagezones-controller, sharefile-storagezones-controller', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.940' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.940' AS DateTime, N'CMP WAP Extension Installer', N'sharefile', N'sharefile-storagezones-controller', N'sharefile-storagezones-controller'
UNION ALL
SELECT 554, N'shavlik, shavlik-protect-azure-standard, shavlik_protect_azure_standard_commercial', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.953' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.953' AS DateTime, N'CMP WAP Extension Installer', N'shavlik', N'shavlik-protect-azure-standard', N'shavlik_protect_azure_standard_commercial'
UNION ALL
SELECT 555, N'sightapps, sightapps, dtap-sightapps', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.970' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.970' AS DateTime, N'CMP WAP Extension Installer', N'sightapps', N'sightapps', N'dtap-sightapps'
UNION ALL
SELECT 556, N'sinefa, sinefa-probe, sf-va-msa', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:43.987' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:43.987' AS DateTime, N'CMP WAP Extension Installer', N'sinefa', N'sinefa-probe', N'sf-va-msa'
UNION ALL
SELECT 557, N'sios_datakeeper, sios-datakeeper-8, bring_your_own_license', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.000' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.000' AS DateTime, N'CMP WAP Extension Installer', N'sios_datakeeper', N'sios-datakeeper-8', N'bring_your_own_license'
UNION ALL
SELECT 558, N'sisense, sisense_byol, sisense_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.000' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.000' AS DateTime, N'CMP WAP Extension Installer', N'sisense', N'sisense_byol', N'sisense_byol'
UNION ALL
SELECT 559, N'snip2code, snip2codeprivateinstance, s2c-company', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.017' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.017' AS DateTime, N'CMP WAP Extension Installer', N'snip2code', N'snip2codeprivateinstance', N's2c-company'
UNION ALL
SELECT 560, N'snip2code, snip2codeprivateinstance, s2c-starter', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.033' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.033' AS DateTime, N'CMP WAP Extension Installer', N'snip2code', N'snip2codeprivateinstance', N's2c-starter'
UNION ALL
SELECT 561, N'snip2code, snip2codeprivateinstance, s2c-team', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.047' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.047' AS DateTime, N'CMP WAP Extension Installer', N'snip2code', N'snip2codeprivateinstance', N's2c-team'
UNION ALL
SELECT 562, N'softnas, softnas-cloud, express_subscription', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.063' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.063' AS DateTime, N'CMP WAP Extension Installer', N'softnas', N'softnas-cloud', N'express_subscription'
UNION ALL
SELECT 563, N'softnas, softnas-cloud, standard_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.080' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.080' AS DateTime, N'CMP WAP Extension Installer', N'softnas', N'softnas-cloud', N'standard_byol'
UNION ALL
SELECT 564, N'softnas, softnas-cloud, standard_subscription', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.097' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.097' AS DateTime, N'CMP WAP Extension Installer', N'softnas', N'softnas-cloud', N'standard_subscription'
UNION ALL
SELECT 565, N'soha, soha-cloud, soha_cloud_basic_plan', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.110' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.110' AS DateTime, N'CMP WAP Extension Installer', N'soha', N'soha-cloud', N'soha_cloud_basic_plan'
UNION ALL
SELECT 566, N'soha, soha-cloud, soha_cloud_free_plan', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.110' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.110' AS DateTime, N'CMP WAP Extension Installer', N'soha', N'soha-cloud', N'soha_cloud_free_plan'
UNION ALL
SELECT 567, N'soha, soha-cloud, soha_cloud_premium_plan', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.127' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.127' AS DateTime, N'CMP WAP Extension Installer', N'soha', N'soha-cloud', N'soha_cloud_premium_plan'
UNION ALL
SELECT 568, N'soha, soha-cloud, soha_cloud_standard_plan', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.143' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.143' AS DateTime, N'CMP WAP Extension Installer', N'soha', N'soha-cloud', N'soha_cloud_standard_plan'
UNION ALL
SELECT 569, N'solanolabs, solano-ci-private-beta, 20150603', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.157' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.157' AS DateTime, N'CMP WAP Extension Installer', N'solanolabs', N'solano-ci-private-beta', N'20150603'
UNION ALL
SELECT 570, N'spacecurve, spacecurve-quickstart, sc-qs', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.157' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.157' AS DateTime, N'CMP WAP Extension Installer', N'spacecurve', N'spacecurve-quickstart', N'sc-qs'
UNION ALL
SELECT 571, N'spaUNION ALLbi, spaUNION ALLbi, spaUNION ALLbi_ubuntu_14_04_lts', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.173' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.173' AS DateTime, N'CMP WAP Extension Installer', N'spaUNION ALLbi', N'spaUNION ALLbi', N'spaUNION ALLbi_ubuntu_14_04_lts'
UNION ALL
SELECT 572, N'sphere3d, snapcloud-standard, snapcloud-standard', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.207' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.207' AS DateTime, N'CMP WAP Extension Installer', N'sphere3d', N'snapcloud-standard', N'snapcloud-standard'
UNION ALL
SELECT 573, N'stackato-platform-as-a-service, activestate-stackato, stackato36', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.220' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.220' AS DateTime, N'CMP WAP Extension Installer', N'stackato-platform-as-a-service', N'activestate-stackato', N'stackato36'
UNION ALL
SELECT 574, N'stackstorm, stackstorm-2015-1, scu-1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.237' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.237' AS DateTime, N'CMP WAP Extension Installer', N'stackstorm', N'stackstorm-2015-1', N'scu-1'
UNION ALL
SELECT 575, N'starwind, starwindvirtualsan, starwindbyol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.250' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.250' AS DateTime, N'CMP WAP Extension Installer', N'starwind', N'starwindvirtualsan', N'starwindbyol'
UNION ALL
SELECT 576, N'starwind, starwindvirtualsan, starwindperhour', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.250' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.250' AS DateTime, N'CMP WAP Extension Installer', N'starwind', N'starwindvirtualsan', N'starwindperhour'
UNION ALL
SELECT 577, N'starwind, starwindvtl, starwindvtl', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.267' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.267' AS DateTime, N'CMP WAP Extension Installer', N'starwind', N'starwindvtl', N'starwindvtl'
UNION ALL
SELECT 578, N'steelhive, steelhive_carbon, steelhive_carbon_10_users', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.283' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.283' AS DateTime, N'CMP WAP Extension Installer', N'steelhive', N'steelhive_carbon', N'steelhive_carbon_10_users'
UNION ALL
SELECT 579, N'steelhive, steelhive_carbon, steelhive_carbon_15plus_users', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.297' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.297' AS DateTime, N'CMP WAP Extension Installer', N'steelhive', N'steelhive_carbon', N'steelhive_carbon_15plus_users'
UNION ALL
SELECT 580, N'steelhive, steelhive_carbon, steelhive_carbon_5_users', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.313' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.313' AS DateTime, N'CMP WAP Extension Installer', N'steelhive', N'steelhive_carbon', N'steelhive_carbon_5_users'
UNION ALL
SELECT 581, N'stormshield, stormshield-network-security-for-cloud, byol-single-instance', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.313' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.313' AS DateTime, N'CMP WAP Extension Installer', N'stormshield', N'stormshield-network-security-for-cloud', N'byol-single-instance'
UNION ALL
SELECT 582, N'sunview-software, sunview_changegear_change_management, sunview_changegear_change_management_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.330' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.330' AS DateTime, N'CMP WAP Extension Installer', N'sunview-software', N'sunview_changegear_change_management', N'sunview_changegear_change_management_byol'
UNION ALL
SELECT 583, N'SUSE, Infra-SMT, 11-SP3', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.347' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.347' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'Infra-SMT', N'11-SP3'
UNION ALL
SELECT 584, N'SUSE, Infrastructure, SMT', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.367' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.367' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'Infrastructure', N'SMT'
UNION ALL
SELECT 585, N'SUSE, openSUSE, 13.1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.367' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.367' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'openSUSE', N'13.1'
UNION ALL
SELECT 586, N'SUSE, openSUSE, 13.2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.380' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.380' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'openSUSE', N'13.2'
UNION ALL
SELECT 587, N'SUSE, SLES, 11-SP3', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.397' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.397' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'SLES', N'11-SP3'
UNION ALL
SELECT 588, N'SUSE, SLES, 11-SP4', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.413' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.413' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'SLES', N'11-SP4'
UNION ALL
SELECT 589, N'SUSE, SLES, 12', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.413' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.413' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'SLES', N'12'
UNION ALL
SELECT 590, N'SUSE, SLES-HPC, 12', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.427' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.427' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'SLES-HPC', N'12'
UNION ALL
SELECT 591, N'SUSE, SLES-HPC-Priority, 12', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.443' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.443' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'SLES-HPC-Priority', N'12'
UNION ALL
SELECT 592, N'SUSE, SLES-Priority, 11-SP3', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.443' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.443' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'SLES-Priority', N'11-SP3'
UNION ALL
SELECT 593, N'SUSE, SLES-Priority, 11-SP4', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.460' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.460' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'SLES-Priority', N'11-SP4'
UNION ALL
SELECT 594, N'SUSE, SLES-Priority, 12', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.477' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.477' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'SLES-Priority', N'12'
UNION ALL
SELECT 595, N'SUSE, SLES-SAPCAL, 11-SP3', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.490' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.490' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'SLES-SAPCAL', N'11-SP3'
UNION ALL
SELECT 596, N'SUSE, SLES-SAPCAL, 11-SP4', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.490' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.490' AS DateTime, N'CMP WAP Extension Installer', N'SUSE', N'SLES-SAPCAL', N'11-SP4'
UNION ALL
SELECT 597, N'tactic, tactic-workflow-v001, tactic-workflow-v001', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.507' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.507' AS DateTime, N'CMP WAP Extension Installer', N'tactic', N'tactic-workflow-v001', N'tactic-workflow-v001'
UNION ALL
SELECT 598, N'talon, cloudfast-for-azure-files, talon_cloudfast_core', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.523' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.523' AS DateTime, N'CMP WAP Extension Installer', N'talon', N'cloudfast-for-azure-files', N'talon_cloudfast_core'
UNION ALL
SELECT 599, N'targit, targit-decision-suite, targit-2014-byol-sql2014std', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.523' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.523' AS DateTime, N'CMP WAP Extension Installer', N'targit', N'targit-decision-suite', N'targit-2014-byol-sql2014std'
UNION ALL
SELECT 600, N'tavendo, crossbar_on_azure_ubuntu1404, free', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.537' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.537' AS DateTime, N'CMP WAP Extension Installer', N'tavendo', N'crossbar_on_azure_ubuntu1404', N'free'
UNION ALL
SELECT 601, N'techdivision, appserver-io-pe, appserver-io-pe', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.553' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.553' AS DateTime, N'CMP WAP Extension Installer', N'techdivision', N'appserver-io-pe', N'appserver-io-pe'
UNION ALL
SELECT 602, N'telepat, free, free', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.570' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.570' AS DateTime, N'CMP WAP Extension Installer', N'telepat', N'free', N'free'
UNION ALL
SELECT 603, N'tenable, tenable-nessus-professional, byol-single-instance', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.570' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.570' AS DateTime, N'CMP WAP Extension Installer', N'tenable', N'tenable-nessus-professional', N'byol-single-instance'
UNION ALL
SELECT 604, N'tentity, websql-server, websql-server-x64-hourly', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.587' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.587' AS DateTime, N'CMP WAP Extension Installer', N'tentity', N'websql-server', N'websql-server-x64-hourly'
UNION ALL
SELECT 605, N'thinkboxsoftware, deadline-single-vm, deadline-single-vm-linux', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.600' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.600' AS DateTime, N'CMP WAP Extension Installer', N'thinkboxsoftware', N'deadline-single-vm', N'deadline-single-vm-linux'
UNION ALL
SELECT 606, N'thinkboxsoftware, deadline-single-vm, deadline-single-vm-windows', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.617' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.617' AS DateTime, N'CMP WAP Extension Installer', N'thinkboxsoftware', N'deadline-single-vm', N'deadline-single-vm-windows'
UNION ALL
SELECT 607, N'thinkboxsoftware, deadline7-2, deadline-repository-7-2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.617' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.617' AS DateTime, N'CMP WAP Extension Installer', N'thinkboxsoftware', N'deadline7-2', N'deadline-repository-7-2'
UNION ALL
SELECT 608, N'thinkboxsoftware, deadline7-2, deadline-slave-7-2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.633' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.633' AS DateTime, N'CMP WAP Extension Installer', N'thinkboxsoftware', N'deadline7-2', N'deadline-slave-7-2'
UNION ALL
SELECT 609, N'topdesk, topdesk-demonstration, demonstration_account', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.647' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.647' AS DateTime, N'CMP WAP Extension Installer', N'topdesk', N'topdesk-demonstration', N'demonstration_account'
UNION ALL
SELECT 610, N'topdesk, topdesk-itsm-software, 100_operator_license', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.647' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.647' AS DateTime, N'CMP WAP Extension Installer', N'topdesk', N'topdesk-itsm-software', N'100_operator_license'
UNION ALL
SELECT 611, N'topdesk, topdesk-itsm-software, 10_operator_license', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.663' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.663' AS DateTime, N'CMP WAP Extension Installer', N'topdesk', N'topdesk-itsm-software', N'10_operator_license'
UNION ALL
SELECT 612, N'topdesk, topdesk-itsm-software, 50_operator_license', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.680' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.680' AS DateTime, N'CMP WAP Extension Installer', N'topdesk', N'topdesk-itsm-software', N'50_operator_license'
UNION ALL
SELECT 613, N'topdesk, topdesk-itsm-software, 5_operator_license', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.693' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.693' AS DateTime, N'CMP WAP Extension Installer', N'topdesk', N'topdesk-itsm-software', N'5_operator_license'
UNION ALL
SELECT 614, N'topdesk, topdesk_byol, topdesk_service_management_byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.693' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.693' AS DateTime, N'CMP WAP Extension Installer', N'topdesk', N'topdesk_byol', N'topdesk_service_management_byol'
UNION ALL
SELECT 615, N'torusware, speedus-lite-ubuntu, speedus_lite_free-ubuntu', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.710' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.710' AS DateTime, N'CMP WAP Extension Installer', N'torusware', N'speedus-lite-ubuntu', N'speedus_lite_free-ubuntu'
UNION ALL
SELECT 616, N'transvault, sprint_3_0, sprint_3_0', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.727' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.727' AS DateTime, N'CMP WAP Extension Installer', N'transvault', N'sprint_3_0', N'sprint_3_0'
UNION ALL
SELECT 617, N'trendmicro, interscan-messaging-security-va, imsva90', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.727' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.727' AS DateTime, N'CMP WAP Extension Installer', N'trendmicro', N'interscan-messaging-security-va', N'imsva90'
UNION ALL
SELECT 618, N'trendmicro, wfbs-90sp1-std-adv, wfbs90sp1advanced', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.740' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.740' AS DateTime, N'CMP WAP Extension Installer', N'trendmicro', N'wfbs-90sp1-std-adv', N'wfbs90sp1advanced'
UNION ALL
SELECT 619, N'trendmicro, wfbs-90sp1-std-adv, wfbs90sp1standard', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.757' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.757' AS DateTime, N'CMP WAP Extension Installer', N'trendmicro', N'wfbs-90sp1-std-adv', N'wfbs90sp1standard'
UNION ALL
SELECT 620, N'tsa-public-service, ckan-server, basepackage', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.773' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.773' AS DateTime, N'CMP WAP Extension Installer', N'tsa-public-service', N'ckan-server', N'basepackage'
UNION ALL
SELECT 621, N'typesafe, typesafe-reactive-maps-demo, typesafe-reactive-maps-demo', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.773' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.773' AS DateTime, N'CMP WAP Extension Installer', N'typesafe', N'typesafe-reactive-maps-demo', N'typesafe-reactive-maps-demo'
UNION ALL
SELECT 622, N'ubercloud, openfoam-v2dot3-centos-v6, openfoam-23-centos6', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.787' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.787' AS DateTime, N'CMP WAP Extension Installer', N'ubercloud', N'openfoam-v2dot3-centos-v6', N'openfoam-23-centos6'
UNION ALL
SELECT 623, N'ubercloud, openfoam-v2dot3-centos-v6, trial', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.803' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.803' AS DateTime, N'CMP WAP Extension Installer', N'ubercloud', N'openfoam-v2dot3-centos-v6', N'trial'
UNION ALL
SELECT 624, N'usp, unified-streaming-vod-standard, usp-vod', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.820' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.820' AS DateTime, N'CMP WAP Extension Installer', N'usp', N'unified-streaming-vod-standard', N'usp-vod'
UNION ALL
SELECT 625, N'vbot, vbot, vbotcms001', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.820' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.820' AS DateTime, N'CMP WAP Extension Installer', N'vbot', N'vbot', N'vbotcms001'
UNION ALL
SELECT 626, N'veeam, veeamcloudconnect, veeambackup', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.850' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.850' AS DateTime, N'CMP WAP Extension Installer', N'veeam', N'veeamcloudconnect', N'veeambackup'
UNION ALL
SELECT 627, N'vidispine, vidispine-content-management, developer', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.850' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.850' AS DateTime, N'CMP WAP Extension Installer', N'vidispine', N'vidispine-content-management', N'developer'
UNION ALL
SELECT 628, N'vidispine, vidispine-content-management, midsized_team', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.867' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.867' AS DateTime, N'CMP WAP Extension Installer', N'vidispine', N'vidispine-content-management', N'midsized_team'
UNION ALL
SELECT 629, N'vidizmo, enterprisetube-video-streaming-premium-portal, vdzmo-azr-ent-prm-1000', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.883' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.883' AS DateTime, N'CMP WAP Extension Installer', N'vidizmo', N'enterprisetube-video-streaming-premium-portal', N'vdzmo-azr-ent-prm-1000'
UNION ALL
SELECT 630, N'virtualworks, viaworks, viaworks', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.883' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.883' AS DateTime, N'CMP WAP Extension Installer', N'virtualworks', N'viaworks', N'viaworks'
UNION ALL
SELECT 631, N'virtualworks, viaworks-online-2_5-beta, viaworks', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.897' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.897' AS DateTime, N'CMP WAP Extension Installer', N'virtualworks', N'viaworks-online-2_5-beta', N'viaworks'
UNION ALL
SELECT 632, N'vision_solutions, double_take_dr, double_take_dr_recovery_target_2008r2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.913' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.913' AS DateTime, N'CMP WAP Extension Installer', N'vision_solutions', N'double_take_dr', N'double_take_dr_recovery_target_2008r2'
UNION ALL
SELECT 633, N'vision_solutions, double_take_dr, double_take_dr_recovery_target_2012', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.913' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.913' AS DateTime, N'CMP WAP Extension Installer', N'vision_solutions', N'double_take_dr', N'double_take_dr_recovery_target_2012'
UNION ALL
SELECT 634, N'vision_solutions, double_take_dr, double_take_dr_recovery_target_2012r2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.930' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.930' AS DateTime, N'CMP WAP Extension Installer', N'vision_solutions', N'double_take_dr', N'double_take_dr_recovery_target_2012r2'
UNION ALL
SELECT 635, N'vision_solutions, double_take_dr, double_take_dr_repository', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.943' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.943' AS DateTime, N'CMP WAP Extension Installer', N'vision_solutions', N'double_take_dr', N'double_take_dr_repository'
UNION ALL
SELECT 636, N'vision_solutions, double_take_move, double_take_move_target_2008r2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.960' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.960' AS DateTime, N'CMP WAP Extension Installer', N'vision_solutions', N'double_take_move', N'double_take_move_target_2008r2'
UNION ALL
SELECT 637, N'vision_solutions, double_take_move, double_take_move_target_2012', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.977' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.977' AS DateTime, N'CMP WAP Extension Installer', N'vision_solutions', N'double_take_move', N'double_take_move_target_2012'
UNION ALL
SELECT 638, N'vision_solutions, double_take_move, double_take_move_target_2012r2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:44.990' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:44.990' AS DateTime, N'CMP WAP Extension Installer', N'vision_solutions', N'double_take_move', N'double_take_move_target_2012r2'
UNION ALL
SELECT 639, N'vmturbo, vmturbo64-opsmgr-5_3, vmturbo64-opsmgr-5-3', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.007' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.007' AS DateTime, N'CMP WAP Extension Installer', N'vmturbo', N'vmturbo64-opsmgr-5_3', N'vmturbo64-opsmgr-5-3'
UNION ALL
SELECT 640, N'vte, slashdb, slashdb-azure', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.007' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.007' AS DateTime, N'CMP WAP Extension Installer', N'vte', N'slashdb', N'slashdb-azure'
UNION ALL
SELECT 641, N'vte, slashdb, slashdb-unlimited', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.023' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.023' AS DateTime, N'CMP WAP Extension Installer', N'vte', N'slashdb', N'slashdb-unlimited'
UNION ALL
SELECT 642, N'waratek, waratek-locker, tomcat7', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.037' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.037' AS DateTime, N'CMP WAP Extension Installer', N'waratek', N'waratek-locker', N'tomcat7'
UNION ALL
SELECT 643, N'watchfulsoftware, rightswatch-single-instance, rightswatch-subscritpion-license-ae-1-100', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.053' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.053' AS DateTime, N'CMP WAP Extension Installer', N'watchfulsoftware', N'rightswatch-single-instance', N'rightswatch-subscritpion-license-ae-1-100'
UNION ALL
SELECT 644, N'websense-apmailpe, ap-data-email-gateway, tdeg', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.070' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.070' AS DateTime, N'CMP WAP Extension Installer', N'websense-apmailpe', N'ap-data-email-gateway', N'tdeg'
UNION ALL
SELECT 645, N'wmspanel, nimble-streamer-centos, nimble-streamer', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.070' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.070' AS DateTime, N'CMP WAP Extension Installer', N'wmspanel', N'nimble-streamer-centos', N'nimble-streamer'
UNION ALL
SELECT 646, N'wmspanel, nimble-streamer-ubuntu, nimble-streamer', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.087' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.087' AS DateTime, N'CMP WAP Extension Installer', N'wmspanel', N'nimble-streamer-ubuntu', N'nimble-streamer'
UNION ALL
SELECT 647, N'wmspanel, nimble-streamer-windows, nimble-streamer', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.100' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.100' AS DateTime, N'CMP WAP Extension Installer', N'wmspanel', N'nimble-streamer-windows', N'nimble-streamer'
UNION ALL
SELECT 648, N'workshare-technology, ws-prot-byol-protsvr-offer, ws-prot-byol-protsv', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.117' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.117' AS DateTime, N'CMP WAP Extension Installer', N'workshare-technology', N'ws-prot-byol-protsvr-offer', N'ws-prot-byol-protsv'
UNION ALL
SELECT 649, N'wowza, wowzastreamingengine, std-linux-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.133' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.133' AS DateTime, N'CMP WAP Extension Installer', N'wowza', N'wowzastreamingengine', N'std-linux-byol'
UNION ALL
SELECT 650, N'wowza, wowzastreamingengine, std-linux-byol-4-2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.133' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.133' AS DateTime, N'CMP WAP Extension Installer', N'wowza', N'wowzastreamingengine', N'std-linux-byol-4-2'
UNION ALL
SELECT 651, N'wowza, wowzastreamingengine, std-linux-byol-4-3', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.147' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.147' AS DateTime, N'CMP WAP Extension Installer', N'wowza', N'wowzastreamingengine', N'std-linux-byol-4-3'
UNION ALL
SELECT 652, N'wowza, wowzastreamingengine, std-windows-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.163' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.163' AS DateTime, N'CMP WAP Extension Installer', N'wowza', N'wowzastreamingengine', N'std-windows-byol'
UNION ALL
SELECT 653, N'wowza, wowzastreamingengine, std-windows-byol-4-2', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.180' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.180' AS DateTime, N'CMP WAP Extension Installer', N'wowza', N'wowzastreamingengine', N'std-windows-byol-4-2'
UNION ALL
SELECT 654, N'xebialabs, xebialabs-xl-deploy, xl-deploy-linux', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.180' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.180' AS DateTime, N'CMP WAP Extension Installer', N'xebialabs', N'xebialabs-xl-deploy', N'xl-deploy-linux'
UNION ALL
SELECT 655, N'xfinityinc, d3view-v5, d3view-v5-hosted', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.193' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.193' AS DateTime, N'CMP WAP Extension Installer', N'xfinityinc', N'd3view-v5', N'd3view-v5-hosted'
UNION ALL
SELECT 656, N'xmpro, xmpro-evaluation, bpm', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.210' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.210' AS DateTime, N'CMP WAP Extension Installer', N'xmpro', N'xmpro-evaluation', N'bpm'
UNION ALL
SELECT 657, N'xtremedata, dbx, dbx15001mnt', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.227' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.227' AS DateTime, N'CMP WAP Extension Installer', N'xtremedata', N'dbx', N'dbx15001mnt'
UNION ALL
SELECT 658, N'xtremedata, dbx, dbx2014001', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.240' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.240' AS DateTime, N'CMP WAP Extension Installer', N'xtremedata', N'dbx', N'dbx2014001'
UNION ALL
SELECT 659, N'yellowfin, yellowfin-for-azure-byol, yellowfinforazurebyol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.257' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.257' AS DateTime, N'CMP WAP Extension Installer', N'yellowfin', N'yellowfin-for-azure-byol', N'yellowfinforazurebyol'
UNION ALL
SELECT 660, N'your-shop-online, herefordshire-enterprise-platform-drupal-7, herefordshire-1', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.273' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.273' AS DateTime, N'CMP WAP Extension Installer', N'your-shop-online', N'herefordshire-enterprise-platform-drupal-7', N'herefordshire-1'
UNION ALL
SELECT 661, N'your-shop-online, xenofile, xenofile-basic', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.287' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.287' AS DateTime, N'CMP WAP Extension Installer', N'your-shop-online', N'xenofile', N'xenofile-basic'
UNION ALL
SELECT 662, N'zementis, adapa-decision-engine, adapa-decision-engine', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.287' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.287' AS DateTime, N'CMP WAP Extension Installer', N'zementis', N'adapa-decision-engine', N'adapa-decision-engine'
UNION ALL
SELECT 663, N'zementis, adapa-scoring-enging-4-1, zementis-adapa-4-1-azure', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.303' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.303' AS DateTime, N'CMP WAP Extension Installer', N'zementis', N'adapa-scoring-enging-4-1', N'zementis-adapa-4-1-azure'
UNION ALL
SELECT 664, N'zend, php-zend-server, zs-d-00-u-php5_6', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.337' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.337' AS DateTime, N'CMP WAP Extension Installer', N'zend', N'php-zend-server', N'zs-d-00-u-php5_6'
UNION ALL
SELECT 665, N'zend, php-zend-server, zs-e-00-u-php5_6', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.350' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.350' AS DateTime, N'CMP WAP Extension Installer', N'zend', N'php-zend-server', N'zs-e-00-u-php5_6'
UNION ALL
SELECT 666, N'zend, php-zend-server, zs-p-00-u-php5_6', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.350' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.350' AS DateTime, N'CMP WAP Extension Installer', N'zend', N'php-zend-server', N'zs-p-00-u-php5_6'
UNION ALL
SELECT 667, N'zoomdata, zoomdata-server, server-byol', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.367' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.367' AS DateTime, N'CMP WAP Extension Installer', N'zoomdata', N'zoomdata-server', N'server-byol'
UNION ALL
SELECT 668, N'zoomdata, zoomdata-server, server-production', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.383' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.383' AS DateTime, N'CMP WAP Extension Installer', N'zoomdata', N'zoomdata-server', N'server-production'
UNION ALL
SELECT 669, N'zoomdata, zoomdata-server, server-trial', N'NULL', N'NULL', N'NULL', 0, 0, N'2015-11-08 06:41:45.383' AS DateTime, N'CMP WAP Extension Installer', N'2015-11-08 06:41:45.383' AS DateTime, N'CMP WAP Extension Installer', N'zoomdata', N'zoomdata-server', N'server-trial'
 
-----------------------------------------------------
-- Begin script transaction
-----------------------------------------------------
 
BEGIN TRAN
 
 
--------------------------------------------------
-- UPDATE existing data. 
--------------------------------------------------
-- Dev Note - Update will not update ' to NULL OR 0 to NULL
UPDATE VmOs
SET   Name = source.Name
        , Description = source.Description
        , OsFamily = source.OsFamily
        , AzureImageName = source.AzureImageName
        , IsCustomImage = source.IsCustomImage
        , IsActive = source.IsActive
        , CreatedOn = source.CreatedOn
        , CreatedBy = source.CreatedBy
        , LastUpdatedOn = source.LastUpdatedOn
        , LastUpdatedBy = source.LastUpdatedBy
		, AzureImagePublisher = source.AzureImagePublisher
		, AzureImageOffer = source.AzureImageOffer
		, AzureWindowsOSVersion = source.AzureWindowsOSVersion

FROM #WorkTable source
    JOIN VmOs target
    ON      source.Name = target.Name
    AND ( ISNULL(source.Name , '') <>  ISNULL(target.Name , '')
    OR  ISNULL(source.Description , '') <>  ISNULL(target.Description , '')
    OR  ISNULL(source.OsFamily , '') <>  ISNULL(target.OsFamily , '')
    OR  ISNULL(source.AzureImageName , '') <>  ISNULL(target.AzureImageName , '')
    OR  ISNULL(source.IsCustomImage , 0) <>  ISNULL(target.IsCustomImage , 0)
    OR  ISNULL(source.IsActive , 0) <>  ISNULL(target.IsActive , 0)
    OR  ISNULL(source.CreatedOn , '') <>  ISNULL(target.CreatedOn , '')
    OR  ISNULL(source.CreatedBy , '') <>  ISNULL(target.CreatedBy , '')
    OR  ISNULL(source.LastUpdatedOn , '') <>  ISNULL(target.LastUpdatedOn , '')
    OR  ISNULL(source.LastUpdatedBy , '') <>  ISNULL(target.LastUpdatedBy , '')
    OR  ISNULL(source.AzureImagePublisher , '') <>  ISNULL(target.AzureImagePublisher , '')
    OR  ISNULL(source.AzureImageOffer , '') <>  ISNULL(target.AzureImageOffer , '')
    OR  ISNULL(source.AzureWindowsOSVersion , '') <>  ISNULL(target.AzureWindowsOSVersion , '')
    )
 
SELECT @vUpdatedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Insert new data. 
--------------------------------------------------
INSERT VmOs
    (Name, Description, OsFamily, AzureImageName, IsCustomImage, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy, AzureImagePublisher, AzureImageOffer, AzureWindowsOSVersion)
SELECT
    Name, Description, OsFamily, AzureImageName, IsCustomImage, IsActive, CreatedOn, CreatedBy, LastUpdatedOn, LastUpdatedBy, AzureImagePublisher, AzureImageOffer, AzureWindowsOSVersion
FROM #WorkTable source
WHERE NOT EXISTS(SELECT * 
FROM VmOs target
WHERE 
    source.VmOsId = target.VmOsId

)
 
SELECT @vInsertedRows = @@ROWCOUNT
 
--------------------------------------------------
-- Delete non matching data. 
--------------------------------------------------
DELETE VmOs
FROM VmOs target
WHERE NOT EXISTS (
    SELECT * 
    FROM #WorkTable source 
    WHERE     source.Name = target.Name

)
 
SELECT @vDeletedRows = @@ROWCOUNT
 
 
GOTO SuccessfulExit 
 
FailureExit:
    ROLLBACK
SET IDENTITY_INSERT VmOs OFF
    RETURN
 
SuccessfulExit:
    PRINT 'Data for VmOs modified. Inserted: ' + CONVERT(VARCHAR(10), @vInsertedRows) + ' rows. Updated: ' + CONVERT(VARCHAR(10), @vUpdatedRows) + ' rows. Deleted: ' + CONVERT(VARCHAR(10), ISNULL(@vDeletedRows, 0)) + ' rows'
    COMMIT
SET IDENTITY_INSERT VmOs OFF
 
--------------------------------------------------
-- Drop temp table 
--------------------------------------------------
GO
DROP TABLE #WorkTable
GO
