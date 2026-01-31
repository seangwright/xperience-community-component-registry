import React, { useState } from 'react';
import { ChevronDown } from 'lucide-react';
import { JsonViewer } from './JsonViewer';
import { PageVariantDto } from './types';

interface LanguageVariantItemProps {
  variant: PageVariantDto;
}

export const LanguageVariantItem: React.FC<LanguageVariantItemProps> = ({
  variant,
}) => {
  const [expanded, setExpanded] = useState(false);

  const lastModified = variant.lastModified
    ? new Date(variant.lastModified).toLocaleDateString()
    : 'Unknown';

  const statusColor = variant.isPublished
    ? 'text-green-600 bg-green-50'
    : 'text-slate-500 bg-slate-50';
  const statusLabel = variant.isPublished ? 'Published' : 'Draft';

  return (
    <div className="mb-3 border border-slate-200 rounded overflow-hidden">
      <button
        onClick={() => setExpanded(!expanded)}
        className="w-full flex items-center gap-2 p-3 hover:bg-slate-50 transition-colors text-left"
      >
        <ChevronDown
          size={16}
          className={`text-slate-600 transition-transform ${
            expanded ? 'rotate-180' : ''
          }`}
        />
        <div className="flex-1">
          <p className="text-sm font-medium text-slate-900">
            {variant.languageName}
          </p>
          <p className="text-xs text-slate-600">Modified: {lastModified}</p>
        </div>
        <span
          className={`text-xs font-medium px-2 py-1 rounded ${statusColor}`}
        >
          {statusLabel}
        </span>
      </button>

      {expanded && (
        <div className="p-3 bg-slate-50 border-t border-slate-200">
          <p className="text-xs text-slate-600 mb-2 font-semibold">
            Configuration:
          </p>
          <JsonViewer json={variant.configurationJson} />
        </div>
      )}
    </div>
  );
};
