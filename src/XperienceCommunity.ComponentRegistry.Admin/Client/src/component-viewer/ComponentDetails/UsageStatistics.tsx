import React from 'react';

interface UsageStatisticsProps {
  totalPages: number;
  totalVariants: number;
  lastModified?: Date;
}

export const UsageStatistics: React.FC<UsageStatisticsProps> = ({
  totalPages,
  totalVariants,
  lastModified,
}) => {
  return (
    <div className="p-4 bg-white border border-slate-200 rounded-lg">
      <h3 className="text-sm font-semibold text-slate-900 mb-4">
        Usage Overview
      </h3>
      <div className="space-y-4">
        <div>
          <div className="text-2xl font-bold text-slate-900">{totalPages}</div>
          <p className="text-xs text-slate-600">Pages Using Component</p>
        </div>
        <div>
          <div className="text-2xl font-bold text-slate-900">
            {totalVariants}
          </div>
          <p className="text-xs text-slate-600">Language Variants</p>
        </div>
        {lastModified && (
          <div className="pt-2 border-t border-slate-200">
            <p className="text-xs text-slate-600">
              Last Modified: {lastModified.toLocaleString()}
            </p>
          </div>
        )}
      </div>
    </div>
  );
};
