<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="0929d1a0-cbdc-4dd0-a5ce-0310752e5353" namespace="Orckestra.Composer.Caching" xmlSchemaNamespace="urn:Orckestra.Composer.Caching" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
    <enumeratedType name="ConfigurationCachePriority" namespace="Orckestra.Composer.Caching">
      <literals>
        <enumerationLiteral name="Normal" />
        <enumerationLiteral name="NotRemovable" />
      </literals>
    </enumeratedType>
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="CacheConfiguration" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="caching">
      <elementProperties>
        <elementProperty name="Profiles" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="profiles" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/CacheProfiles" />
          </type>
        </elementProperty>
        <elementProperty name="Categories" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="categories" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/CacheCategoryCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElementCollection name="CacheProfiles" xmlItemName="profile" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <attributeProperties>
        <attributeProperty name="Default" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="default" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <itemType>
        <configurationElementMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/CacheProfile" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="CacheProfile">
      <attributeProperties>
        <attributeProperty name="Duration" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="duration" isReadOnly="false" defaultValue="&quot;0:0:0&quot;">
          <type>
            <externalTypeMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/TimeSpan" />
          </type>
        </attributeProperty>
        <attributeProperty name="Priority" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="priority" isReadOnly="false">
          <type>
            <enumeratedTypeMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/ConfigurationCachePriority" />
          </type>
        </attributeProperty>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="ClientType" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="clientType" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="AcquiredLockTimeout" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="acquiredLockTimeout" isReadOnly="false" defaultValue="&quot;00:05:00&quot;">
          <type>
            <externalTypeMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/TimeSpan" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Settings" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="settings" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/ProfileSettings" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElement name="CacheCategory">
      <attributeProperties>
        <attributeProperty name="Name" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="name" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="ProfileName" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="profileName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="CacheCategoryCollection" xmlItemName="category" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/CacheCategory" />
      </itemType>
    </configurationElementCollection>
    <configurationElementCollection name="ProfileSettings" xmlItemName="profileSetting" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/ProfileSetting" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="ProfileSetting">
      <attributeProperties>
        <attributeProperty name="Key" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="key" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Value" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="value" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/0929d1a0-cbdc-4dd0-a5ce-0310752e5353/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>