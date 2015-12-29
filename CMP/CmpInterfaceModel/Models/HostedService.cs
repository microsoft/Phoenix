using System.Collections.Generic;

namespace CmpInterfaceModel.Models
{
    public class ExtendedProperty
    {
        public string Name {set; get;}
        public string Value { set; get; }
    }

    public class HostedService
    {
        public string ServiceName {set; get;}
        public string Label {set; get;}
        public string Description {set; get;}
        public string Location {set; get;}
        public string AffinityGroup {set; get;}
        public List<ExtendedProperty> ExtendedProperties {set; get;}
    }

    /*
    <?xml version="1.0" encoding="utf-8"?>
        <CreateHostedService xmlns="http://schemas.microsoft.com/windowsazure">
          <ServiceName>service-name</ServiceName>
          <Label>base64-encoded-service-label</Label>
          <Description>description</Description>
          <Location>location</Location>
          <AffinityGroup>affinity-group</AffinityGroup>
          <ExtendedProperties>
            <ExtendedProperty>
              <Name>property-name</Name>
              <Value>property-value</Value>
            </ExtendedProperty>
          </ExtendedProperties>
        </CreateHostedService>
    */
}
