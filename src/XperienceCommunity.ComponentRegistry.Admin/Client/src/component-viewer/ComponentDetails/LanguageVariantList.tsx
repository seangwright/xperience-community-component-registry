import React from 'react';
import { LanguageVariantItem } from './LanguageVariantItem';

interface PageVariantDto {
  contentItemCommonDataId: number;
  languageName: string;
  lastModified?: string;
  configurationJson: string;
  configurationType: string;
}

interface LanguageVariantListProps {
  variants: PageVariantDto[];
}

export const LanguageVariantList: React.FC<LanguageVariantListProps> = ({
  variants,
}) => {
  if (variants.length === 0) {
    return (
      <p className="text-sm text-slate-500 py-2">No language variants found</p>
    );
  }

  return (
    <div className="space-y-3">
      {variants.map((variant) => (
        <LanguageVariantItem
          key={variant.contentItemCommonDataId}
          variant={variant}
        />
      ))}
    </div>
  );
};
