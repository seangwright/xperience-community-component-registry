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
