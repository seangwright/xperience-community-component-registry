import React from 'react';
import { PageList } from './PageList';
import { UsageStatistics } from './UsageStatistics';

interface PageVariantDto {
  contentItemCommonDataId: number;
  languageName: string;
  lastModified?: string;
  configurationJson: string;
  configurationType: string;
}

interface PageUsageDto {
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

interface ComponentUsageDetailDto {
  componentIdentifier: string;
  componentType: string;
  totalPagesUsing: number;
  totalVariants: number;
  lastModified?: string;
  pages: PageUsageDto[];
}

interface ComponentDetailsPanelProps {
  data: ComponentUsageDetailDto;
}

export const ComponentDetailsPanel: React.FC<ComponentDetailsPanelProps> = ({
  data,
}) => {
  const lastModified = data.lastModified
    ? new Date(data.lastModified)
    : undefined;

  return (
    <div className="p-4 bg-slate-50 rounded-lg border border-slate-200">
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        {/* Statistics Column */}
        <div>
          <UsageStatistics
            totalPages={data.totalPagesUsing}
            totalVariants={data.totalVariants}
            lastModified={lastModified}
          />
        </div>

        {/* Pages List Column */}
        <div className="md:col-span-3">
          <PageList pages={data.pages} />
        </div>
      </div>
    </div>
  );
};
