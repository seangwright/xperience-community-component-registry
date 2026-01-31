import React from 'react';

interface JsonViewerProps {
  json: string;
}

export const JsonViewer: React.FC<JsonViewerProps> = ({ json }) => {
  let parsed: unknown;

  try {
    parsed = JSON.parse(json);
  } catch {
    return (
      <div className="p-2 bg-red-50 border border-red-200 rounded text-red-700 text-sm">
        Invalid JSON configuration
      </div>
    );
  }

  return (
    <pre className="p-4 bg-slate-900 text-slate-100 rounded overflow-auto text-xs font-mono">
      {JSON.stringify(parsed, null, 2)}
    </pre>
  );
};
