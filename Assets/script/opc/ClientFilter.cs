using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opc.Ua;
using System.Text;

namespace OPCClientInterface
{
    /// <summary>
    /// 存储从服务器检索的类型声明功能库
    /// </summary>
    internal class TypeDeclaration
    {
        /// <summary>
        /// 节点ID
        /// </summary>
        public NodeId NodeId;
        /// <summary>
        /// 类型的实例声明的完全隐式列表。
        /// </summary>
        public List<InstanceDeclaration> Declarations;
    }

    /// <summary>
    /// 存储从服务器获取的实例声明。
    /// </summary>
    internal class InstanceDeclaration
    {
        /// <summary>
        /// 属性类型ID
        /// </summary>
        public NodeId RootTypeId;

        /// <summary>
        /// 实例声明的浏览路径。
        /// </summary>
        public QualifiedNameCollection BrowsePath;

        /// <summary>
        /// 浏览路径
        /// </summary>
        public string BrowsePathDisplayText;

        /// <summary>
        /// 实例声明的本地化路径。
        /// </summary>
        public string DisplayPath;

        /// <summary>
        /// 进程实例的节点ID
        /// </summary>
        public NodeId NodeId;

        /// <summary>
        /// 进程的节点类
        /// </summary>
        public NodeClass NodeClass;

        /// <summary>
        /// 实例进程的浏览名称
        /// </summary>
        public QualifiedName BrowseName;

        /// <summary>
        /// 实例进程的显示名称
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// 实例进程的描述
        /// </summary>
        public string Description;

        /// <summary>
        /// 实例声明的建模规则（即强制性或可选）。
        /// </summary>
        public NodeId ModellingRule;

        /// <summary>
        /// 实例进程的数据类型节点ID
        /// </summary>
        public NodeId DataType;

        /// <summary>
        /// 实际值
        /// </summary>
        public int ValueRank;

        /// <summary>
        /// 值类型
        /// </summary>
        public BuiltInType BuiltInType;

        /// <summary>
        /// 本地值类型
        /// </summary>
        public string DataTypeDisplayText;

        /// <summary>
        /// An instance declaration that has been overridden by the current instance.
        /// </summary>
        public InstanceDeclaration OverriddenDeclaration;
    }

    /// <summary>
    /// A field in a filter declaration.
    /// </summary>
    internal class FilterDeclarationField
    {
        /// <summary>
        /// Creates a new instance of a FilterDeclarationField.
        /// </summary>
        public FilterDeclarationField()
        {
            Selected = true;
            DisplayInList = false;
            FilterEnabled = false;
            FilterOperator = FilterOperator.Equals;
            FilterValue = Variant.Null;
            InstanceDeclaration = null;
        }

        /// <summary>
        /// Creates a new instance of a FilterDeclarationField.
        /// </summary>
        public FilterDeclarationField(InstanceDeclaration instanceDeclaration)
        {
            Selected = true;
            DisplayInList = false;
            FilterEnabled = false;
            FilterOperator = FilterOperator.Equals;
            FilterValue = Variant.Null;
            InstanceDeclaration = instanceDeclaration;
        }

        /// <summary>
        /// Creates a new instance of a FilterDeclarationField.
        /// </summary>
        public FilterDeclarationField(FilterDeclarationField field)
        {
            Selected = field.Selected;
            DisplayInList = field.DisplayInList;
            FilterEnabled = field.FilterEnabled;
            FilterOperator = field.FilterOperator;
            FilterValue = field.FilterValue;
            InstanceDeclaration = field.InstanceDeclaration;
        }

        /// <summary>
        /// Whether the field is returned as part of the event notification.
        /// </summary>
        public bool Selected;

        /// <summary>
        /// Whether the field is displayed in the list view.
        /// </summary>
        public bool DisplayInList;

        /// <summary>
        /// Whether the filter is enabled.
        /// </summary>
        public bool FilterEnabled;

        /// <summary>
        /// The filter operator to use in the where clause.
        /// </summary>
        public FilterOperator FilterOperator;

        /// <summary>
        /// The filter value to use in the where clause.
        /// </summary>
        public Variant FilterValue;

        /// <summary>
        /// The instance declaration associated with the field.
        /// </summary>
        public InstanceDeclaration InstanceDeclaration;
    }

    /// <summary>
    /// A declararion of an event filter.
    /// </summary>
    internal class FilterDeclaration
    {
        /// <summary>
        /// Creates a new instance of a FilterDeclaration.
        /// </summary>
        public FilterDeclaration()
        {
            EventTypeId = Opc.Ua.ObjectTypeIds.BaseEventType;
            Fields = new List<FilterDeclarationField>();
        }

        /// <summary>
        /// Creates a new instance of a FilterDeclaration.
        /// </summary>
        public FilterDeclaration(TypeDeclaration eventType, FilterDeclaration template)
        {
            EventTypeId = eventType.NodeId;
            Fields = new List<FilterDeclarationField>();

            foreach (InstanceDeclaration instanceDeclaration in eventType.Declarations)
            {
                if (instanceDeclaration.NodeClass == NodeClass.Method)
                {
                    continue;
                }

                if (NodeId.IsNull(instanceDeclaration.ModellingRule))
                {
                    continue;
                }

                FilterDeclarationField element = new FilterDeclarationField(instanceDeclaration);
                Fields.Add(element);

                // set reasonable defaults.
                if (template == null)
                {
                    if (instanceDeclaration.RootTypeId == Opc.Ua.ObjectTypeIds.BaseEventType && instanceDeclaration.BrowseName != Opc.Ua.BrowseNames.EventId)
                    {
                        element.DisplayInList = true;
                    }
                }

                // preserve filter settings.
                else
                {
                    foreach (FilterDeclarationField field in template.Fields)
                    {
                        if (field.InstanceDeclaration.BrowsePathDisplayText == element.InstanceDeclaration.BrowsePathDisplayText)
                        {
                            element.DisplayInList = field.DisplayInList;
                            element.FilterEnabled = field.FilterEnabled;
                            element.FilterOperator = field.FilterOperator;
                            element.FilterValue = field.FilterValue;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new instance of a FilterDeclaration.
        /// </summary>
        public FilterDeclaration(FilterDeclaration declaration)
        {
            EventTypeId = declaration.EventTypeId;
            Fields = new List<FilterDeclarationField>(declaration.Fields.Count);

            for (int ii = 0; ii < declaration.Fields.Count; ii++)
            {
                Fields.Add(new FilterDeclarationField(declaration.Fields[ii]));
            }
        }

        /// <summary>
        /// Returns the event filter defined by the filter declaration.
        /// </summary>
        public EventFilter GetFilter()
        {
            EventFilter filter = new EventFilter();
            filter.SelectClauses = GetSelectClause();
            filter.WhereClause = GetWhereClause();
            return filter;
        }

        /// <summary>
        /// Adds a simple field to the declaration.
        /// </summary>
        public void AddSimpleField(QualifiedName browseName, BuiltInType dataType, bool displayInList)
        {
            AddSimpleField(new QualifiedName[] { browseName }, NodeClass.Variable, dataType, ValueRanks.Scalar, displayInList);
        }

        /// <summary>
        /// Adds a simple field to the declaration.
        /// </summary>
        public void AddSimpleField(QualifiedName browseName, BuiltInType dataType, int valueRank, bool displayInList)
        {
            AddSimpleField(new QualifiedName[] { browseName }, NodeClass.Variable, dataType, valueRank, displayInList);
        }

        /// <summary>
        /// Adds a simple field to the declaration.
        /// </summary>
        public void AddSimpleField(QualifiedName[] browseNames, BuiltInType dataType, int valueRank, bool displayInList)
        {
            AddSimpleField(browseNames, NodeClass.Variable, dataType, valueRank, displayInList);
        }

        /// <summary>
        /// Adds a simple field to the declaration.
        /// </summary>
        public void AddSimpleField(QualifiedName[] browseNames, NodeClass nodeClass, BuiltInType dataType, int valueRank, bool displayInList)
        {
            FilterDeclarationField field = new FilterDeclarationField();

            field.DisplayInList = displayInList;
            field.InstanceDeclaration = new InstanceDeclaration();
            field.InstanceDeclaration.NodeClass = nodeClass;

            if (browseNames != null)
            {
                field.InstanceDeclaration.BrowseName = browseNames[browseNames.Length - 1];
                field.InstanceDeclaration.BrowsePath = new QualifiedNameCollection();

                StringBuilder path = new StringBuilder();

                for (int ii = 0; ii < browseNames.Length; ii++)
                {
                    if (path.Length > 0)
                    {
                        path.Append('/');
                    }

                    path.Append(browseNames[ii]);
                    field.InstanceDeclaration.BrowsePath.Add(browseNames[ii]);
                }

                field.InstanceDeclaration.BrowsePathDisplayText = path.ToString();
            }

            field.InstanceDeclaration.BuiltInType = dataType;
            field.InstanceDeclaration.DataType = (uint)dataType;
            field.InstanceDeclaration.ValueRank = valueRank;
            field.InstanceDeclaration.DataTypeDisplayText = dataType.ToString();

            if (valueRank >= 0)
            {
                field.InstanceDeclaration.DataTypeDisplayText += "[]";
            }

            field.InstanceDeclaration.DisplayName = field.InstanceDeclaration.BrowseName.Name;
            field.InstanceDeclaration.DisplayPath = field.InstanceDeclaration.BrowsePathDisplayText;
            field.InstanceDeclaration.RootTypeId = ObjectTypeIds.BaseEventType;
            Fields.Add(field);
        }

        /// <summary>
        /// Returns the select clause defined by the filter declaration.
        /// </summary>
        public SimpleAttributeOperandCollection GetSelectClause()
        {
            SimpleAttributeOperandCollection selectClause = new SimpleAttributeOperandCollection();

            SimpleAttributeOperand operand = new();
            operand.TypeDefinitionId = Opc.Ua.ObjectTypeIds.BaseEventType;
            operand.AttributeId = Attributes.NodeId;
            selectClause.Add(operand);

            foreach (FilterDeclarationField field in Fields)
            {
                if (field.Selected)
                {
                    operand = new SimpleAttributeOperand();
                    operand.TypeDefinitionId = field.InstanceDeclaration.RootTypeId;
                    operand.AttributeId = (field.InstanceDeclaration.NodeClass == NodeClass.Object) ? Attributes.NodeId : Attributes.Value;
                    operand.BrowsePath = field.InstanceDeclaration.BrowsePath;
                    selectClause.Add(operand);
                }
            }

            return selectClause;
        }

        /// <summary>
        /// Returns the where clause defined by the filter declaration.
        /// </summary>
        public ContentFilter GetWhereClause()
        {
            ContentFilter whereClause = new ContentFilter();
            ContentFilterElement element1 = whereClause.Push(FilterOperator.OfType, EventTypeId);

            EventFilter filter = new EventFilter();

            foreach (FilterDeclarationField field in Fields)
            {
                if (field.FilterEnabled)
                {
                    SimpleAttributeOperand operand1 = new SimpleAttributeOperand();
                    operand1.TypeDefinitionId = field.InstanceDeclaration.RootTypeId;
                    operand1.AttributeId = (field.InstanceDeclaration.NodeClass == NodeClass.Object) ? Attributes.NodeId : Attributes.Value;
                    operand1.BrowsePath = field.InstanceDeclaration.BrowsePath;

                    LiteralOperand operand2 = new LiteralOperand();
                    operand2.Value = field.FilterValue;

                    ContentFilterElement element2 = whereClause.Push(field.FilterOperator, operand1, operand2);
                    element1 = whereClause.Push(FilterOperator.And, element1, element2);
                }
            }

            return whereClause;
        }

        /// <summary>
        /// Returns the value for the specified browse name.
        /// </summary>
        public T GetValue<T>(QualifiedName browseName, VariantCollection fields, T defaultValue)
        {
            if (fields == null || fields.Count == 0)
            {
                return defaultValue;
            }

            if (browseName == null)
            {
                browseName = QualifiedName.Null;
            }

            for (int ii = 0; ii < this.Fields.Count; ii++)
            {
                if (this.Fields[ii].InstanceDeclaration.BrowseName == browseName)
                {
                    if (ii >= fields.Count + 1)
                    {
                        return defaultValue;
                    }

                    object value = fields[ii + 1].Value;

                    if (typeof(T).IsInstanceOfType(value))
                    {
                        return (T)value;
                    }

                    break;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// The type of event.
        /// </summary>
        public NodeId EventTypeId;

        /// <summary>
        /// The list of declarations for the fields.
        /// </summary>
        public List<FilterDeclarationField> Fields;
    }



}


