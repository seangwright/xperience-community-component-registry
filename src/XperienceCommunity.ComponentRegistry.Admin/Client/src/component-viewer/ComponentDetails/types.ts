export interface PageVariantDto {
  contentItemCommonDataId: number;
  languageName: string;
  lastModified?: string;
  configurationJson: string;
  configurationType: string;
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
