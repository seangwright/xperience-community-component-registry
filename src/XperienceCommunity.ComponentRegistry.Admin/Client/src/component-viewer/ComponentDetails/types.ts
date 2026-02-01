export interface PageVariantDto {
  contentItemCommonDataId: number;
  languageName: string;
  lastModified?: string;
  configurationJson: string;
  configurationType: string;
  isPublished?: boolean;
}

export interface PageUsageDto {
  webPageItemId: number;
  contentItemId: number;
  pageName: string;
  pagePath: string;
  channelDisplayName: string;
  isPublished: boolean;
  createdAt: string;
  modifiedAt?: string;
  variants: PageVariantDto[];
}

export interface ComponentUsageDetailDto {
  componentIdentifier: string;
  componentType: string;
  totalPagesUsing: number;
  totalVariants: number;
  lastModified?: string;
  pages: PageUsageDto[];
}

export interface ComponentDto {
  identifier: string;
  name: string;
  description?: string;
  iconClass?: string;
  markedTypeName?: string;
}

export interface PageTemplateDto {
  identifier: string;
  name: string;
  description?: string;
  iconClass?: string;
  markedTypeName?: string;
  contentTypeNames: string[];
}
export interface EmailComponentDto {
  identifier: string;
  name: string;
  description?: string;
  iconClass?: string;
  markedTypeName?: string;
  propertiesTypeName?: string;
}

export interface EmailTemplateDto {
  identifier: string;
  name: string;
  description?: string;
  iconClass?: string;
  markedTypeName?: string;
  contentTypeNames: string[];
}

export interface EmailConfigurationVariantDto {
  contentItemCommonDataId: number;
  languageName: string;
  lastModified?: string;
  configurationJson: string;
  configurationType: string;
  isPublished?: boolean;
}

export interface EmailConfigurationUsageDto {
  emailConfigurationId: number;
  contentItemId: number;
  configurationName: string;
  configurationPurpose: string;
  channelDisplayName: string;
  createdAt: string;
  modifiedAt?: string;
  variants: EmailConfigurationVariantDto[];
}

export interface EmailConfigurationUsageDetailDto {
  componentIdentifier: string;
  componentType: string;
  totalEmailConfigurationsUsing: number;
  totalVariants: number;
  lastModified?: string;
  emailConfigurations: EmailConfigurationUsageDto[];
}
