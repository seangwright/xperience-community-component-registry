import React, { useState } from 'react';
import { ChevronDown } from 'lucide-react';
import { LanguageVariantList } from './LanguageVariantList';
import { PageUsageDto } from './types';

interface PageListItemProps {
  page: PageUsageDto;
}

export const PageListItem: React.FC<PageListItemProps> = ({ page }) => {
  const [variantsExpanded, setVariantsExpanded] = useState(false);

  const modifiedDate = page.modifiedAt
    ? new Date(page.modifiedAt).toLocaleDateString()
    : new Date(page.createdAt).toLocaleDateString();

  return (
    <div className="border border-slate-200 rounded-lg overflow-hidden mb-3">
      <button
        onClick={() => setVariantsExpanded(!variantsExpanded)}
        className="w-full flex items-center gap-3 p-4 hover:bg-slate-50 transition-colors text-left"
      >
        <ChevronDown
          size={18}
          className={`text-slate-600 transition-transform flex-shrink-0 ${
            variantsExpanded ? 'rotate-180' : ''
          }`}
        />

        <div className="flex-1">
          <p className="text-sm font-medium text-slate-900">{page.pageName}</p>
          <p className="text-xs text-slate-600">
            {page.channelDisplayName} • {page.pagePath} • {page.variants.length}{' '}
            variant
            {page.variants.length !== 1 ? 's' : ''} • Modified: {modifiedDate}
          </p>
        </div>
      </button>

      {variantsExpanded && (
        <div className="p-4 bg-slate-50 border-t border-slate-200">
          <p className="text-xs font-semibold text-slate-700 mb-3">
            Language Variants:
          </p>
          <LanguageVariantList variants={page.variants} />
        </div>
      )}
    </div>
  );
};
