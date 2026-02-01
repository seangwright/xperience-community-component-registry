import React, { useState } from 'react';
import { usePageCommand } from '@kentico/xperience-admin-base';
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from './ui/card';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from './ui/table';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { ChevronDown, Loader } from 'lucide-react';

interface FormComponentDto {
  identifier: string;
  name: string;
  description?: string;
  iconClass?: string;
  markedTypeName?: string;
}

interface FormSectionDto {
  identifier: string;
  name: string;
  description?: string;
  iconClass?: string;
  markedTypeName?: string;
}

interface FormClassUsageDto {
  classId: number;
  classDisplayName: string;
  className: string;
  classXmlSchema: string;
  classFormDefinition: string;
  classTableName: string;
}

interface FormBuilderFormUsageDto {
  formID: number;
  formName: string;
  formDisplayName: string;
  formBuilderLayout: string;
}

interface FormComponentUsageDetailDto {
  componentIdentifier: string;
  componentType: string;
  totalFormClassesUsing: number;
  totalFormBuilderFormsUsing: number;
  lastModified?: string;
  formClasses: FormClassUsageDto[];
  formBuilderForms: FormBuilderFormUsageDto[];
}

interface FormBuilderComponentViewerClientProperties {
  formComponents: FormComponentDto[];
  formSections: FormSectionDto[];
}

// Table row component for form builder components
const FormComponentTableRow: React.FC<{
  component: FormComponentDto | FormSectionDto;
  componentType: 'component' | 'section';
}> = ({ component, componentType }) => {
  const [expanded, setExpanded] = useState(false);
  const [usageData, setUsageData] =
    useState<FormComponentUsageDetailDto | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const { execute: getFormBuilderComponentUsage } = usePageCommand<
    FormComponentUsageDetailDto,
    { componentIdentifier: string }
  >('GetFormBuilderComponentUsageAsync', {
    after: (response) => {
      if (response) setUsageData(response);
      setIsLoading(false);
    },
  });

  const { execute: getFormBuilderSectionUsage } = usePageCommand<
    FormComponentUsageDetailDto,
    { componentIdentifier: string }
  >('GetFormBuilderSectionUsageAsync', {
    after: (response) => {
      if (response) setUsageData(response);
      setIsLoading(false);
    },
  });

  const handleExpandClick = async () => {
    if (!expanded && !usageData) {
      setIsLoading(true);
      try {
        const params = { componentIdentifier: component.identifier };
        if (componentType === 'section') {
          await getFormBuilderSectionUsage(params);
        } else {
          await getFormBuilderComponentUsage(params);
        }
      } catch {
        setIsLoading(false);
        // Handle error silently
      }
    }
    setExpanded(!expanded);
  };

  return (
    <>
      <TableRow>
        <TableCell className="w-10">
          <button
            onClick={handleExpandClick}
            disabled={isLoading}
            className="p-1 hover:bg-slate-100 rounded transition-colors disabled:opacity-50"
          >
            {isLoading ? (
              <Loader size={16} className="text-slate-600 animate-spin" />
            ) : (
              <ChevronDown
                size={16}
                className={`text-slate-600 transition-transform ${
                  expanded ? '-rotate-180' : ''
                }`}
              />
            )}
          </button>
        </TableCell>
        <TableCell>
          <code className="px-2 py-1 bg-slate-100 rounded text-xs font-mono text-slate-700">
            {component.identifier}
          </code>
        </TableCell>
        <TableCell className="font-semibold text-slate-900">
          {component.name}
        </TableCell>
        <TableCell className="text-slate-600 max-w-md">
          {component.description || (
            <span className="text-slate-400 italic">No description</span>
          )}
        </TableCell>
        <TableCell>
          {component.iconClass ? (
            <code className="px-2 py-1 bg-blue-50 rounded text-xs font-mono text-blue-700">
              {component.iconClass}
            </code>
          ) : (
            <span className="text-slate-400">—</span>
          )}
        </TableCell>
        <TableCell>
          {component.markedTypeName ? (
            <div
              className="max-w-xs overflow-x-auto overflow-y-hidden"
              title={component.markedTypeName}
            >
              <code className="px-2 py-1 bg-purple-50 rounded text-xs font-mono text-purple-700 whitespace-nowrap">
                {component.markedTypeName}
              </code>
            </div>
          ) : (
            <span className="text-slate-400">—</span>
          )}
        </TableCell>
      </TableRow>

      {expanded && (
        <TableRow>
          <TableCell colSpan={6} className="p-0">
            <div className="p-4 bg-slate-50 border-t">
              <div className="bg-white p-4 rounded border border-slate-200">
                <h4 className="font-semibold text-slate-900 mb-4">Details</h4>

                {/* Component info section */}
                <div className="mb-6 pb-6 border-b">
                  <h5 className="text-sm font-medium text-slate-700 mb-3">
                    Component Information
                  </h5>
                  <dl className="space-y-2 text-sm">
                    <div>
                      <dt className="font-medium text-slate-700">Identifier</dt>
                      <dd className="text-slate-600 font-mono">
                        {component.identifier}
                      </dd>
                    </div>
                    <div>
                      <dt className="font-medium text-slate-700">Name</dt>
                      <dd className="text-slate-600">{component.name}</dd>
                    </div>
                    {component.description && (
                      <div>
                        <dt className="font-medium text-slate-700">
                          Description
                        </dt>
                        <dd className="text-slate-600">
                          {component.description}
                        </dd>
                      </div>
                    )}
                    {component.markedTypeName && (
                      <div>
                        <dt className="font-medium text-slate-700">
                          Component Type
                        </dt>
                        <dd className="text-slate-600 font-mono text-xs break-all">
                          {component.markedTypeName}
                        </dd>
                      </div>
                    )}
                  </dl>
                </div>

                {/* Usage section */}
                <div>
                  <h5 className="text-sm font-medium text-slate-700 mb-4">
                    Component Usage
                  </h5>
                  {usageData ? (
                    <div className="space-y-6">
                      {usageData.componentType === 'Component' ? (
                        // Show form classes only for components
                        <div>
                          <h6 className="text-sm font-medium text-slate-600 mb-3">
                            Legacy Form Classes
                          </h6>
                          <div className="space-y-3">
                            <div className="grid grid-cols-2 gap-4 text-sm">
                              <div className="bg-blue-50 p-3 rounded">
                                <div className="text-xs text-blue-700 font-medium">
                                  Total Form Classes
                                </div>
                                <div className="text-xl font-bold text-blue-900">
                                  {usageData.totalFormClassesUsing}
                                </div>
                              </div>
                              <div className="bg-slate-100 p-3 rounded">
                                <div className="text-xs text-slate-700 font-medium">
                                  Last Updated
                                </div>
                                <div className="text-sm text-slate-600">
                                  {usageData.lastModified
                                    ? new Date(
                                        usageData.lastModified,
                                      ).toLocaleDateString()
                                    : 'N/A'}
                                </div>
                              </div>
                            </div>

                            {usageData.formClasses.length > 0 ? (
                              <div className="mt-4">
                                <div className="text-xs font-medium text-slate-700 mb-2">
                                  Form Classes:
                                </div>
                                <div className="space-y-2 max-h-64 overflow-y-auto">
                                  {usageData.formClasses.map((formClass) => (
                                    <div
                                      key={formClass.classId}
                                      className="p-3 bg-slate-50 rounded border border-slate-200 text-xs"
                                    >
                                      <div className="font-medium text-slate-900 mb-1">
                                        {formClass.classDisplayName}
                                      </div>
                                      <div className="font-mono text-slate-600 text-xs mb-1">
                                        {formClass.className}
                                      </div>
                                      <div className="text-slate-500 text-xs">
                                        Table: {formClass.classTableName}
                                      </div>
                                    </div>
                                  ))}
                                </div>
                              </div>
                            ) : (
                              <div className="p-3 bg-yellow-50 rounded text-sm text-yellow-700">
                                No legacy form classes use this component
                              </div>
                            )}
                          </div>
                        </div>
                      ) : (
                        // Show form builder forms only for sections
                        <div>
                          <h6 className="text-sm font-medium text-slate-600 mb-3">
                            Form Builder Forms
                          </h6>
                          <div className="space-y-3">
                            <div className="grid grid-cols-2 gap-4 text-sm">
                              <div className="bg-green-50 p-3 rounded">
                                <div className="text-xs text-green-700 font-medium">
                                  Total Form Builder Forms
                                </div>
                                <div className="text-xl font-bold text-green-900">
                                  {usageData.totalFormBuilderFormsUsing}
                                </div>
                              </div>
                              <div className="bg-slate-100 p-3 rounded">
                                <div className="text-xs text-slate-700 font-medium">
                                  Last Updated
                                </div>
                                <div className="text-sm text-slate-600">
                                  {usageData.lastModified
                                    ? new Date(
                                        usageData.lastModified,
                                      ).toLocaleDateString()
                                    : 'N/A'}
                                </div>
                              </div>
                            </div>

                            {usageData.formBuilderForms.length > 0 ? (
                              <div className="mt-4">
                                <div className="text-xs font-medium text-slate-700 mb-2">
                                  Forms:
                                </div>
                                <div className="space-y-2 max-h-64 overflow-y-auto">
                                  {usageData.formBuilderForms.map((form) => (
                                    <div
                                      key={form.formID}
                                      className="p-3 bg-slate-50 rounded border border-slate-200 text-xs"
                                    >
                                      <div className="font-medium text-slate-900 mb-1">
                                        {form.formDisplayName}
                                      </div>
                                      <div className="font-mono text-slate-600 text-xs">
                                        {form.formName}
                                      </div>
                                    </div>
                                  ))}
                                </div>
                              </div>
                            ) : (
                              <div className="p-3 bg-yellow-50 rounded text-sm text-yellow-700">
                                No form builder forms use this section
                              </div>
                            )}
                          </div>
                        </div>
                      )}
                    </div>
                  ) : (
                    <div className="text-slate-500 italic">
                      Loading usage information...
                    </div>
                  )}
                </div>
              </div>
            </div>
          </TableCell>
        </TableRow>
      )}
    </>
  );
};

export const FormBuilderComponentViewerTemplate = (
  props: FormBuilderComponentViewerClientProperties,
) => {
  const totalComponents =
    props.formComponents.length + props.formSections.length;

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 to-slate-100 p-8">
      <div className="max-w-7xl mx-auto space-y-8">
        {/* Header */}
        <div className="space-y-2">
          <h1 className="text-4xl font-bold tracking-tight !text-slate-900">
            Form Builder Components
          </h1>
          <p className="text-lg !text-slate-600">
            Browse and explore all registered form builder components in the
            system
          </p>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <Card className="bg-gradient-to-br from-blue-50 to-blue-100 border-blue-200">
            <CardHeader className="pb-2">
              <CardTitle className="text-sm font-medium !text-blue-700">
                Total Components
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-bold !text-blue-900">
                {totalComponents}
              </div>
            </CardContent>
          </Card>

          <Card className="bg-gradient-to-br from-purple-50 to-purple-100 border-purple-200">
            <CardHeader className="pb-2">
              <CardTitle className="text-sm font-medium !text-purple-700">
                Form Components
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-bold !text-purple-900">
                {props.formComponents.length}
              </div>
            </CardContent>
          </Card>

          <Card className="bg-gradient-to-br from-green-50 to-green-100 border-green-200">
            <CardHeader className="pb-2">
              <CardTitle className="text-sm font-medium !text-green-700">
                Form Sections
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-bold !text-green-900">
                {props.formSections.length}
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Tabs */}
        <Tabs defaultValue="components" className="w-full">
          <TabsList className="grid w-full max-w-md grid-cols-2">
            <TabsTrigger
              value="components"
              className="!text-slate-700 data-[state=active]:!text-slate-900"
            >
              Components ({props.formComponents.length})
            </TabsTrigger>
            <TabsTrigger
              value="sections"
              className="!text-slate-700 data-[state=active]:!text-slate-900"
            >
              Sections ({props.formSections.length})
            </TabsTrigger>
          </TabsList>

          <TabsContent value="components" className="space-y-4">
            <Card className="shadow-lg">
              <CardHeader className="bg-gradient-to-r from-purple-50 to-blue-50">
                <CardTitle className="text-2xl !text-slate-900">
                  Form Component Types
                </CardTitle>
                <CardDescription className="text-base !text-slate-600">
                  Reusable components for building forms
                </CardDescription>
              </CardHeader>
              <CardContent className="pt-6">
                {props.formComponents.length > 0 ? (
                  <div className="rounded-lg border">
                    <Table>
                      <TableHeader>
                        <TableRow className="bg-slate-50">
                          <TableHead className="w-10"></TableHead>
                          <TableHead className="font-semibold !text-slate-700">
                            Identifier
                          </TableHead>
                          <TableHead className="font-semibold !text-slate-700">
                            Name
                          </TableHead>
                          <TableHead className="font-semibold !text-slate-700">
                            Description
                          </TableHead>
                          <TableHead className="font-semibold !text-slate-700">
                            Icon
                          </TableHead>
                          <TableHead className="font-semibold !text-slate-700">
                            Component Type
                          </TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {props.formComponents.map((component) => (
                          <FormComponentTableRow
                            key={component.identifier}
                            component={component}
                            componentType="component"
                          />
                        ))}
                      </TableBody>
                    </Table>
                  </div>
                ) : (
                  <div className="text-center py-12 text-slate-500">
                    <p className="text-lg">No form components registered</p>
                  </div>
                )}
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="sections" className="space-y-4">
            <Card className="shadow-lg">
              <CardHeader className="bg-gradient-to-r from-green-50 to-teal-50">
                <CardTitle className="text-2xl !text-slate-900">
                  Form Section Types
                </CardTitle>
                <CardDescription className="text-base !text-slate-600">
                  Layout sections for organizing form components
                </CardDescription>
              </CardHeader>
              <CardContent className="pt-6">
                {props.formSections.length > 0 ? (
                  <div className="rounded-lg border">
                    <Table>
                      <TableHeader>
                        <TableRow className="bg-slate-50">
                          <TableHead className="w-10"></TableHead>
                          <TableHead className="font-semibold !text-slate-700">
                            Identifier
                          </TableHead>
                          <TableHead className="font-semibold !text-slate-700">
                            Name
                          </TableHead>
                          <TableHead className="font-semibold !text-slate-700">
                            Description
                          </TableHead>
                          <TableHead className="font-semibold !text-slate-700">
                            Icon
                          </TableHead>
                          <TableHead className="font-semibold !text-slate-700">
                            Component Type
                          </TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {props.formSections.map((section) => (
                          <FormComponentTableRow
                            key={section.identifier}
                            component={section}
                            componentType="section"
                          />
                        ))}
                      </TableBody>
                    </Table>
                  </div>
                ) : (
                  <div className="text-center py-12 text-slate-500">
                    <p className="text-lg">No form sections registered</p>
                  </div>
                )}
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </div>
    </div>
  );
};
