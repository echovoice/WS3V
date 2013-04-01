<?xml version="1.0" encoding="utf-8"?>
<serviceModel xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" name="Azure_Cloud_Demos" generation="1" functional="0" release="0" Id="3a34d9fa-63f6-4954-ac8a-e6bfd2744410" dslVersion="1.2.0.0" xmlns="http://schemas.microsoft.com/dsltools/RDSM">
  <groups>
    <group name="Azure_Cloud_DemosGroup" generation="1" functional="0" release="0">
      <componentports>
        <inPort name="Demo:Endpoint1" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/LB:Demo:Endpoint1" />
          </inToChannel>
        </inPort>
        <inPort name="Demo:Endpoint2" protocol="tcp">
          <inToChannel>
            <lBChannelMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/LB:Demo:Endpoint2" />
          </inToChannel>
        </inPort>
        <inPort name="Demo:Web" protocol="http">
          <inToChannel>
            <lBChannelMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/LB:Demo:Web" />
          </inToChannel>
        </inPort>
      </componentports>
      <settings>
        <aCS name="DemoInstances" defaultValue="[1,1,1]">
          <maps>
            <mapMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/MapDemoInstances" />
          </maps>
        </aCS>
      </settings>
      <channels>
        <lBChannel name="LB:Demo:Endpoint1">
          <toPorts>
            <inPortMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/Demo/Endpoint1" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:Demo:Endpoint2">
          <toPorts>
            <inPortMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/Demo/Endpoint2" />
          </toPorts>
        </lBChannel>
        <lBChannel name="LB:Demo:Web">
          <toPorts>
            <inPortMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/Demo/Web" />
          </toPorts>
        </lBChannel>
      </channels>
      <maps>
        <map name="MapDemoInstances" kind="Identity">
          <setting>
            <sCSPolicyIDMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/DemoInstances" />
          </setting>
        </map>
      </maps>
      <components>
        <groupHascomponents>
          <role name="Demo" generation="1" functional="0" release="0" software="C:\Users\Mike\Dropbox\New Project Echovoice\GitHub\WS3V\Azure Cloud Demos\Azure Cloud Demos\csx\Release\roles\Demo" entryPoint="base\x64\WaHostBootstrapper.exe" parameters="base\x64\WaIISHost.exe " memIndex="768" hostingEnvironment="frontendadmin" hostingEnvironmentVersion="2">
            <componentports>
              <inPort name="Endpoint1" protocol="tcp" portRanges="8181" />
              <inPort name="Endpoint2" protocol="tcp" portRanges="8182" />
              <inPort name="Web" protocol="http" portRanges="80" />
            </componentports>
            <settings>
              <aCS name="__ModelData" defaultValue="&lt;m role=&quot;Demo&quot; xmlns=&quot;urn:azure:m:v1&quot;&gt;&lt;r name=&quot;Demo&quot;&gt;&lt;e name=&quot;Endpoint1&quot; /&gt;&lt;e name=&quot;Endpoint2&quot; /&gt;&lt;e name=&quot;Web&quot; /&gt;&lt;/r&gt;&lt;/m&gt;" />
            </settings>
            <resourcereferences>
              <resourceReference name="DiagnosticStore" defaultAmount="[4096,4096,4096]" defaultSticky="true" kind="Directory" />
              <resourceReference name="EventStore" defaultAmount="[1000,1000,1000]" defaultSticky="false" kind="LogStore" />
            </resourcereferences>
          </role>
          <sCSPolicy>
            <sCSPolicyIDMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/DemoInstances" />
            <sCSPolicyUpdateDomainMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/DemoUpgradeDomains" />
            <sCSPolicyFaultDomainMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/DemoFaultDomains" />
          </sCSPolicy>
        </groupHascomponents>
      </components>
      <sCSPolicy>
        <sCSPolicyUpdateDomain name="DemoUpgradeDomains" defaultPolicy="[5,5,5]" />
        <sCSPolicyFaultDomain name="DemoFaultDomains" defaultPolicy="[2,2,2]" />
        <sCSPolicyID name="DemoInstances" defaultPolicy="[1,1,1]" />
      </sCSPolicy>
    </group>
  </groups>
  <implements>
    <implementation Id="0705f7a9-359b-427e-a5dd-d6f50547395e" ref="Microsoft.RedDog.Contract\ServiceContract\Azure_Cloud_DemosContract@ServiceDefinition">
      <interfacereferences>
        <interfaceReference Id="049b452a-1792-453c-8119-517dda1a874a" ref="Microsoft.RedDog.Contract\Interface\Demo:Endpoint1@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/Demo:Endpoint1" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="dfd8e2ec-ce42-45c7-855b-eee7f0eb53b2" ref="Microsoft.RedDog.Contract\Interface\Demo:Endpoint2@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/Demo:Endpoint2" />
          </inPort>
        </interfaceReference>
        <interfaceReference Id="ead50674-c822-47cd-8c84-dd54b63f7ef7" ref="Microsoft.RedDog.Contract\Interface\Demo:Web@ServiceDefinition">
          <inPort>
            <inPortMoniker name="/Azure_Cloud_Demos/Azure_Cloud_DemosGroup/Demo:Web" />
          </inPort>
        </interfaceReference>
      </interfacereferences>
    </implementation>
  </implements>
</serviceModel>