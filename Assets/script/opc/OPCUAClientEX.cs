using Opc.Ua;
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;


namespace OPCClientInterface
{
    public class OPCUAClientEX
    {
        OPCUAClient m_OPCUAClient = null;

        private List<string> subNodeIds = new List<string>();
        private bool isSingleValueSub = false;
        private string currentNodeID = string.Empty;

        public string CurrentNodeId
        {
            get { return currentNodeID; }
        }
        public OPCUAClientEX(OPCUAClient oPCUAClient)
        {
            m_OPCUAClient = oPCUAClient;
        }
        /// <summary>
        /// 获取图片键值
        /// </summary>
        /// <param name="oPCUAClient"></param>
        /// <param name="target"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        private string GetImageKeyFromDescription(ReferenceDescription target, NodeId sourceId)
        {
            if (target.NodeClass == NodeClass.Variable)
            {
                DataValue dataValue = m_OPCUAClient.ReadNode((NodeId)target.NodeId);

                if (dataValue.WrappedValue.TypeInfo != null)
                {
                    if (dataValue.WrappedValue.TypeInfo.ValueRank == ValueRanks.Scalar)
                    {
                        return "";
                    }
                    else if (dataValue.WrappedValue.TypeInfo.ValueRank == ValueRanks.OneDimension)
                    {
                        return "";
                    }
                    else if (dataValue.WrappedValue.TypeInfo.ValueRank == ValueRanks.TwoDimensions)
                    {
                        return "";
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
            else if (target.NodeClass == NodeClass.Object)
            {
                if (sourceId == ObjectIds.ObjectsFolder)
                {
                    return "";
                }
                else
                {
                    return "";
                }
            }
            else if (target.NodeClass == NodeClass.Method)
            {
                return "";
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 通过sourceId获取reference表,并填入树
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public List<OPCNode> GetBranch(string sourceID, bool IsFirst)
        {
            NodeId nodeId = new NodeId(sourceID);

            return GetBranch(nodeId, IsFirst);
        }
        /// <summary>
        /// 通过sourceId获取reference表,并填入树
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public List<OPCNode> GetBranch(object sourceID, bool IsFirst)
        {
            List<OPCNode> listNode = null;
            NodeId sourceId = null;
            #region 确定读取源文件ID
            if (IsFirst)
            {
                switch (sourceID)
                {
                    case SourceID.ObjectsFolder:
                        sourceId = ObjectIds.ObjectsFolder;
                        break;
                    case SourceID.DataTypesFolder:
                        sourceId = ObjectIds.DataTypesFolder;
                        break;
                    case SourceID.ObjectTypesFolder:
                        sourceId = ObjectIds.ObjectTypesFolder;
                        break;
                    case SourceID.TypesFolder:
                        sourceId = ObjectIds.TypesFolder;
                        break;
                    case SourceID.ViewsFolder:
                        sourceId = ObjectIds.ViewsFolder;
                        break;
                }
            }
            else
            {
                sourceId = (NodeId)sourceID;
            }

            #endregion
            try
            {
                // 从服务端获取所有分支
                ReferenceDescriptionCollection references = GetReferenceDescriptionCollection(sourceId);
                List<OPCNode> list = new List<OPCNode>();
                if (references != null)
                {
                    for (int ii = 0; ii < references.Count; ii++)
                    {
                        ReferenceDescription target = references[ii];
                        OPCNode child = new OPCNode(Utils.Format("{0}", target));

                        child.Tag = target;
                        string key = GetImageKeyFromDescription(target, sourceId);
                        child.ImageKey = key;
                        child.Name = target.DisplayName.ToString();
                        child.NodeId = target.NodeId;
                        list.Add(child);
                    }
                }
                return list;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 实时更新
        /// </summary>
        /// <param name="TagNodeID">当前对象ID</param>
        /// <param name="SubCallBack">回调函数</param>
        public async void UpdateInTime(string key, object TagNodeTag,
            Action<string, MonitoredItem, MonitoredItemNotificationEventArgs> SubCallBack)
        {
            subNodeIds = new List<string>();
            if (m_OPCUAClient != null)
            {
                m_OPCUAClient.RemoveSubscription(key);

                ReferenceDescriptionCollection references;
                try
                {
                    subNodeIds = await Task.Run(() =>
                    {
                        List<string> nodeids = new List<string>();
                        ReferenceDescription reference = TagNodeTag as ReferenceDescription;
                        NodeId sourceId = (NodeId)reference.NodeId;

                        references = GetReferenceDescriptionCollection(sourceId);
                        if (references?.Count > 0)
                        {
                            isSingleValueSub = false;
                            // 获取所有要订阅的子节点
                            for (int ii = 0; ii < references.Count; ii++)
                            {
                                ReferenceDescription target = references[ii];
                                nodeids.Add(((NodeId)target.NodeId).ToString());
                            }
                        }
                        else
                        {
                            isSingleValueSub = true;
                            // 子节点没有数据的情况
                            nodeids.Add(sourceId.ToString());
                        }

                        return nodeids;
                    });
                }
                catch (Exception exception)
                {
                    throw exception;
                }
                m_OPCUAClient.AddSubscription(key, subNodeIds.ToArray(), SubCallBack);
            }
        }
        /// <summary>
        /// 获取所有reference集合
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        private ReferenceDescriptionCollection GetReferenceDescriptionCollection(NodeId sourceId)
        {
            // 查找节点所有元素
            BrowseDescription nodeToBrowse1 = new BrowseDescription();

            nodeToBrowse1.NodeId = sourceId;
            nodeToBrowse1.BrowseDirection = BrowseDirection.Forward;
            nodeToBrowse1.ReferenceTypeId = ReferenceTypeIds.Aggregates;
            nodeToBrowse1.IncludeSubtypes = true;
            nodeToBrowse1.NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable | NodeClass.Method | NodeClass.ReferenceType | NodeClass.ObjectType | NodeClass.View | NodeClass.VariableType | NodeClass.DataType);
            nodeToBrowse1.ResultMask = (uint)BrowseResultMask.All;

            // 查找所有节点
            BrowseDescription nodeToBrowse2 = new BrowseDescription();

            nodeToBrowse2.NodeId = sourceId;
            nodeToBrowse2.BrowseDirection = BrowseDirection.Forward;
            nodeToBrowse2.ReferenceTypeId = ReferenceTypeIds.Organizes;
            nodeToBrowse2.IncludeSubtypes = true;
            nodeToBrowse2.NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable | NodeClass.Method | NodeClass.View | NodeClass.ReferenceType | NodeClass.ObjectType | NodeClass.VariableType | NodeClass.DataType);
            nodeToBrowse2.ResultMask = (uint)BrowseResultMask.All;

            BrowseDescriptionCollection nodesToBrowse = new BrowseDescriptionCollection();
            nodesToBrowse.Add(nodeToBrowse1);
            nodesToBrowse.Add(nodeToBrowse2);

            // 从服务端获取reference数据
            ReferenceDescriptionCollection references = FormUtils.Browse(m_OPCUAClient.Session, nodesToBrowse, false);
            return references;
        }

        /// <summary>
        /// 数据选中后操作
        /// </summary>
        public List<OPCNode> AfterNodeSelect(object SelectObj)
        {
            DataTable dataTable = new DataTable();
            NodeId currentNodeId = null;
            try
            {
                RemoveAllSubscript();
                // get the source for the node.
                ReferenceDescription reference = SelectObj as ReferenceDescription;

                if (reference == null || reference.NodeId.IsAbsolute)
                {
                    return null;
                }
                currentNodeId = (NodeId)reference.NodeId;
                // populate children.               

                var opcList=ShowMemberByList(SelectObj);
                currentNodeID = currentNodeId.ToString();
                return opcList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 节点参数开始编辑
        /// </summary>
        /// <param name="CurrentParamsType">当前参数值类型</param>
        /// <param name="CurrentParamsAccess">当前参数读写权限</param>
        public void BeginNodeEdit(object CurrentParamsType, object CurrentParamsAccess)
        {
            if (Enum.TryParse<BuiltInType>(CurrentParamsType.ToString(), true, out BuiltInType builtInType))
            {
                if (
                       builtInType == BuiltInType.Boolean ||
                       builtInType == BuiltInType.Byte ||
                       builtInType == BuiltInType.DateTime ||
                       builtInType == BuiltInType.Double ||
                       builtInType == BuiltInType.Float ||
                       builtInType == BuiltInType.Guid ||
                       builtInType == BuiltInType.Int16 ||
                       builtInType == BuiltInType.Int32 ||
                       builtInType == BuiltInType.Int64 ||
                       builtInType == BuiltInType.Integer ||
                       builtInType == BuiltInType.LocalizedText ||
                       builtInType == BuiltInType.SByte ||
                       builtInType == BuiltInType.String ||
                       builtInType == BuiltInType.UInt16 ||
                       builtInType == BuiltInType.UInt32 ||
                       builtInType == BuiltInType.UInt64 ||
                       builtInType == BuiltInType.UInteger
                       )
                {

                }
                else
                {
                    throw new Exception("Not support the Type of modify value!");
                }
            }
            else
            {
                throw new Exception("Not support the Type of modify value!");
            }
            if (!CurrentParamsAccess.ToString().Contains("Write"))
            {
                throw new Exception("Not support the access of modify value!");
            }
        }

        public void EndNodeEdit(object CurrentParamsType, object TagNodeID, object NewValue)
        {
            if (Enum.TryParse<BuiltInType>(CurrentParamsType.ToString(), true, out BuiltInType builtInType))
            {
                dynamic value = null;

                // 节点
                try
                {
                    value = GetValueFromString(NewValue.ToString(), builtInType);
                }
                catch
                {
                    throw new Exception("Invalid Input Value");
                }
                if (!m_OPCUAClient.WriteNode(TagNodeID.ToString(), value))
                {
                    throw new Exception("Failed to write value");
                }

            }
            else
            {
                throw new Exception("Invalid data type");
            }

        }
        public List<OPCNode> GetNodeChild(string NodeID)
        {
            List<OPCNode> treeNode = null;
            try
            {
                NodeId nodeId = new NodeId(NodeID);

                treeNode = GetBranch(nodeId, false);
                return treeNode;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public List<OPCNode> GetNodeChild(object NodeID)
        {
            List<OPCNode> treeNode = null;
            try
            {
                // get the source for the node.
                ReferenceDescription reference = NodeID as ReferenceDescription;

                if (reference == null || reference.NodeId.IsAbsolute)
                {
                    return null;
                }
                treeNode = GetBranch((NodeId)reference.NodeId, false);
                return treeNode;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private dynamic GetValueFromString(string value, BuiltInType builtInType)
        {
            switch (builtInType)
            {
                case BuiltInType.Boolean:
                    {
                        return bool.Parse(value);
                    }
                case BuiltInType.Byte:
                    {
                        return byte.Parse(value);
                    }
                case BuiltInType.DateTime:
                    {
                        return DateTime.Parse(value);
                    }
                case BuiltInType.Double:
                    {
                        return double.Parse(value);
                    }
                case BuiltInType.Float:
                    {
                        return float.Parse(value);
                    }
                case BuiltInType.Guid:
                    {
                        return Guid.Parse(value);
                    }
                case BuiltInType.Int16:
                    {
                        return short.Parse(value);
                    }
                case BuiltInType.Int32:
                    {
                        return int.Parse(value);
                    }
                case BuiltInType.Int64:
                    {
                        return long.Parse(value);
                    }
                case BuiltInType.Integer:
                    {
                        return int.Parse(value);
                    }
                case BuiltInType.LocalizedText:
                    {
                        return value;
                    }
                case BuiltInType.SByte:
                    {
                        return sbyte.Parse(value);
                    }
                case BuiltInType.String:
                    {
                        return value;
                    }
                case BuiltInType.UInt16:
                    {
                        return ushort.Parse(value);
                    }
                case BuiltInType.UInt32:
                    {
                        return uint.Parse(value);
                    }
                case BuiltInType.UInt64:
                    {
                        return ulong.Parse(value);
                    }
                case BuiltInType.UInteger:
                    {
                        return uint.Parse(value);
                    }
                default: throw new Exception("Not supported data type");
            }
        }

        public OPCNode GetData(string SourceID)
        {
            NodeId nodeId = new NodeId(SourceID);
            return GetData(nodeId);
        }
        public OPCNode GetData(object SourceID)
        {
            NodeId sourceId = SourceID as NodeId;
            DataValue[] dataValues = ReadOneNodeFiveAttributes(new List<NodeId>() { sourceId });

            int index = 0;
            OPCNode opcNode = null;

            for (int jj = 0; jj < dataValues.Length; jj += 5)
            {
                 opcNode = AddDataToList(dataValues, jj, index++, sourceId);
            }
            return opcNode;
        }
        /// <summary>
        /// 移除所有订阅消息
        /// </summary>
        public void RemoveAllSubscript()
        {
            m_OPCUAClient?.RemoveAllSubscription();
        }
        public List<OPCNode> ShowMemberByList(object SourceID)
        {
            List<OPCNode> opcList = new List<OPCNode>();

            // get the source for the node.
            ReferenceDescription reference = SourceID as ReferenceDescription;
            NodeId sourceId = (NodeId)reference.NodeId;
            int index = 0;

            try
            {

                ReferenceDescriptionCollection references = GetReferenceDescriptionCollection(sourceId);
                if (references?.Count > 0)
                {
                    // 获取所有要读取的子节点
                    List<NodeId> nodeIds = new List<NodeId>();
                    nodeIds.Add((NodeId)reference.NodeId);
                    for (int ii = 0; ii < references.Count; ii++)
                    {
                        ReferenceDescription target = references[ii];
                        nodeIds.Add((NodeId)target.NodeId);
                    }

                    // 获取所有的值
                    DataValue[] dataValues = ReadOneNodeFiveAttributes(nodeIds);

                    // 显示
                    for (int jj = 0; jj < dataValues.Length; jj += 5)
                    {
                        var opcNode = AddDataToList(dataValues, jj, index++, nodeIds[jj / 5]);
                        opcList.Add(opcNode);
                    }

                }
                else
                {
                    // 子节点没有数据的情况
                    try
                    {
                        DataValue dataValue = m_OPCUAClient.ReadNode(sourceId);

                        if (dataValue.WrappedValue.TypeInfo?.ValueRank == ValueRanks.OneDimension)
                        {
                            // 数组显示
                            var node = AddToListWithoutData(sourceId, out index);
                            opcList.Add(node);
                        }
                        else
                        {
                            // 显示单个数本身                       
                            var node = AddDataToList(ReadOneNodeFiveAttributes(new List<NodeId>() { sourceId }), 0, index++, sourceId);
                            opcList.Add(node);
                        }
                    }
                    catch (Exception exception)
                    {
                        throw exception;
                    }
                }
                return opcList;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
        public async Task<DataTable> ShowMember(object SourceID)
        {
            DataTable dataTable = new DataTable();
            string[] textDescription = new string[6] { "值ID", "参数名称", "参数值", "参数类型", "参数权限", "参数描述" };
            for (int i = 0; i < 6; i++)
            {
                dataTable.Columns.Add(textDescription[i]);
            }
            // get the source for the node.
            ReferenceDescription reference = SourceID as ReferenceDescription;
            NodeId sourceId = (NodeId)reference.NodeId;
            int index = 0;

            try
            {
                dataTable = await Task.Run(() =>
                {
                    ReferenceDescriptionCollection references = GetReferenceDescriptionCollection(sourceId);
                    if (references?.Count > 0)
                    {
                        // 获取所有要读取的子节点
                        List<NodeId> nodeIds = new List<NodeId>();
                        for (int ii = 0; ii < references.Count; ii++)
                        {
                            ReferenceDescription target = references[ii];
                            nodeIds.Add((NodeId)target.NodeId);
                        }

                        // 获取所有的值
                        DataValue[] dataValues = ReadOneNodeFiveAttributes(nodeIds);

                        // 显示
                        for (int jj = 0; jj < dataValues.Length; jj += 5)
                        {
                            AddDataTableNewRow(ref dataTable, dataValues, jj, index++, nodeIds[jj / 5]);
                        }

                    }
                    else
                    {
                        // 子节点没有数据的情况
                        try
                        {
                            DataValue dataValue = m_OPCUAClient.ReadNode(sourceId);

                            if (dataValue.WrappedValue.TypeInfo?.ValueRank == ValueRanks.OneDimension)
                            {
                                // 数组显示
                                AddDataTableArrayRow(ref dataTable, sourceId, out index);
                            }
                            else
                            {
                                // 显示单个数本身                       
                                AddDataTableNewRow(ref dataTable, ReadOneNodeFiveAttributes(new List<NodeId>() { sourceId }), 0, index++, sourceId);
                            }
                        }
                        catch (Exception exception)
                        {
                            throw exception;
                        }
                    }
                    return dataTable;
                });
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return dataTable;
        }
        /// <summary>
        /// 0:NodeClass  1:Value  2:AccessLevel  3:DisplayName  4:Description
        /// </summary>
        /// <param name="nodeIds"></param>
        /// <returns></returns>
        private DataValue[] ReadOneNodeFiveAttributes(List<NodeId> nodeIds)
        {
            ReadValueIdCollection nodesToRead = new ReadValueIdCollection();
            foreach (var nodeId in nodeIds)
            {
                NodeId sourceId = nodeId;
                nodesToRead.Add(new ReadValueId()
                {
                    NodeId = sourceId,
                    AttributeId = Attributes.NodeClass
                });
                nodesToRead.Add(new ReadValueId()
                {
                    NodeId = sourceId,
                    AttributeId = Attributes.Value
                });
                nodesToRead.Add(new ReadValueId()
                {
                    NodeId = sourceId,
                    AttributeId = Attributes.AccessLevel
                });
                nodesToRead.Add(new ReadValueId()
                {
                    NodeId = sourceId,
                    AttributeId = Attributes.DisplayName
                });
                nodesToRead.Add(new ReadValueId()
                {
                    NodeId = sourceId,
                    AttributeId = Attributes.Description
                });
            }

            // read all values.
            m_OPCUAClient.Session.Read(
                null,
                0,
                TimestampsToReturn.Neither,
                nodesToRead,
                out DataValueCollection results,
                out DiagnosticInfoCollection diagnosticInfos);

            ClientBase.ValidateResponse(results, nodesToRead);
            ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

            return results.ToArray();
        }
        /// <summary>
        /// 读取所有数据添加到表格
        /// </summary>
        /// <param name="dataGridView1"></param>
        /// <param name="dataValues"></param>
        /// <param name="startIndex"></param>
        /// <param name="index"></param>
        /// <param name="nodeId"></param>
        private OPCNode AddDataToList(DataValue[] dataValues, int startIndex, int index, NodeId nodeId)
        {

            if (dataValues[startIndex].WrappedValue.Value == null) return null;
            NodeClass nodeclass = (NodeClass)dataValues[startIndex].WrappedValue.Value;
            OPCNode opcNode = new OPCNode(nodeId.ToString());
            opcNode.NodeId = nodeId;
            opcNode.Name = dataValues[3 + startIndex].WrappedValue.Value?.ToString();
            opcNode.Description = dataValues[4 + startIndex].WrappedValue.Value?.ToString();
            opcNode.Auth = GetDiscriptionFromAccessLevel(dataValues[2 + startIndex]);

            if (nodeclass == NodeClass.Variable)
            {
                DataValue dataValue = dataValues[1 + startIndex];

                if (dataValue.WrappedValue.TypeInfo != null)
                {
                    opcNode.Type = dataValue.WrappedValue.TypeInfo.BuiltInType.ToString();

                    if (dataValue.WrappedValue.TypeInfo.ValueRank == ValueRanks.Scalar)
                    {
                        opcNode.Value = dataValue?.WrappedValue.Value;
                    }
                    else
                    {
                        opcNode.Value = dataValue?.Value?.GetType().ToString();
                    }
                }
                else
                {
                    opcNode.Value = dataValue.Value;
                    opcNode.Type = "null";
                }
            }
            else
            {
                opcNode.Type = nodeclass.ToString();
            }
            return opcNode;
        }
        /// <summary>
        /// 无数据的情况
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="index"></param>
        private OPCNode AddToListWithoutData(NodeId nodeId, out int index)
        {
            DataValue[] dataValues = ReadOneNodeFiveAttributes(new List<NodeId>() { nodeId });

            DataValue dataValue = dataValues[1];
            OPCNode opcNode = null;

            if (dataValue.WrappedValue.TypeInfo?.ValueRank == ValueRanks.OneDimension)
            {
                string access = GetDiscriptionFromAccessLevel(dataValues[2]);
                BuiltInType type = dataValue.WrappedValue.TypeInfo.BuiltInType;
                object des = dataValues[4].Value ?? "";
                object dis = dataValues[3].Value ?? type;

                Array array = dataValue.Value as Array;
                int i = 0;
                foreach (object obj in array)
                {

                    opcNode = new OPCNode("");
                    opcNode.Name = $"{dis} [{i++}]";
                    opcNode.Value = obj;
                    opcNode.Type = type.ToString();
                    opcNode.Auth = access;
                    opcNode.Description = des.ToString();
                }
                index = i;
            }
            else
            {
                index = 0;
            }
            return opcNode;
        }
        /// <summary>
        /// 读取所有数据添加到表格
        /// </summary>
        /// <param name="dataGridView1"></param>
        /// <param name="dataValues"></param>
        /// <param name="startIndex"></param>
        /// <param name="index"></param>
        /// <param name="nodeId"></param>
        private void AddDataTableNewRow(ref DataTable dataTable, DataValue[] dataValues, int startIndex, int index, NodeId nodeId)
        {

            while (index >= dataTable.Rows.Count)
            {
                dataTable.Rows.Add();
            }
            DataRow dataRow = dataTable.Rows[index];


            if (dataValues[startIndex].WrappedValue.Value == null) return;
            NodeClass nodeclass = (NodeClass)dataValues[startIndex].WrappedValue.Value;
            dataRow[0] = nodeId;
            dataRow[1] = dataValues[3 + startIndex].WrappedValue.Value;
            dataRow[5] = dataValues[4 + startIndex].WrappedValue.Value;
            dataRow[4] = GetDiscriptionFromAccessLevel(dataValues[2 + startIndex]);

            if (nodeclass == NodeClass.Object)
            {

                dataRow[2] = "";
                dataRow[3] = nodeclass.ToString();
            }
            else if (nodeclass == NodeClass.Method)
            {

                dataRow[2] = "";
                dataRow[3] = nodeclass.ToString();
            }
            else if (nodeclass == NodeClass.Variable)
            {
                DataValue dataValue = dataValues[1 + startIndex];

                if (dataValue.WrappedValue.TypeInfo != null)
                {
                    dataRow[3] = dataValue.WrappedValue.TypeInfo.BuiltInType;
                    // dgvr.Cells[3].Value = dataValue.Value.GetType().ToString();
                    if (dataValue.WrappedValue.TypeInfo.ValueRank == ValueRanks.Scalar)
                    {
                        dataRow[2] = dataValue?.WrappedValue.Value;

                    }
                    else if (dataValue.WrappedValue.TypeInfo.ValueRank == ValueRanks.OneDimension)
                    {
                        dataRow[2] = dataValue?.Value?.GetType().ToString();

                    }
                    else if (dataValue.WrappedValue.TypeInfo.ValueRank == ValueRanks.TwoDimensions)
                    {
                        dataRow[2] = dataValue?.Value?.GetType().ToString();

                    }
                    else
                    {
                        dataRow[2] = dataValue?.Value?.GetType().ToString();

                    }
                }
                else
                {
                    dataRow[2] = dataValue.Value;
                    dataRow[3] = "null";
                }
            }
            else
            {
                dataRow[2] = "";
                dataRow[3] = nodeclass.ToString();
            }
        }
        /// <summary>
        /// 无数据的情况
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="index"></param>
        private void AddDataTableArrayRow(ref DataTable dataTable, NodeId nodeId, out int index)
        {

            DateTime dateTimeStart = DateTime.Now;
            DataValue[] dataValues = ReadOneNodeFiveAttributes(new List<NodeId>() { nodeId });
            //label_time_spend.Text = (int)(DateTime.Now - dateTimeStart).TotalMilliseconds + " ms";

            DataValue dataValue = dataValues[1];

            if (dataValue.WrappedValue.TypeInfo?.ValueRank == ValueRanks.OneDimension)
            {
                string access = GetDiscriptionFromAccessLevel(dataValues[2]);
                BuiltInType type = dataValue.WrappedValue.TypeInfo.BuiltInType;
                object des = dataValues[4].Value ?? "";
                object dis = dataValues[3].Value ?? type;

                Array array = dataValue.Value as Array;
                int i = 0;
                foreach (object obj in array)
                {
                    while (i >= dataTable.Rows.Count)
                    {
                        dataTable.Rows.Add();
                    }
                    DataRow dataRow = dataTable.Rows[i];
                    //DataGridViewRow dgvr = dataGridView1.Rows[i];
                    //dataTable.
                    //dgvr.Tag = null;
                    dataRow[1] = "null";
                    dataRow[1] = $"{dis} [{i++}]";
                    dataRow[2] = obj;
                    dataRow[3] = type;
                    dataRow[4] = access;
                    dataRow[5] = des;
                }
                index = i;
            }
            else
            {
                index = 0;
            }
        }
        private string GetDiscriptionFromAccessLevel(DataValue value)
        {
            if (value.WrappedValue.Value != null)
            {
                switch ((byte)value.WrappedValue.Value)
                {
                    case 0: return "None";
                    case 1: return "CurrentRead";
                    case 2: return "CurrentWrite";
                    case 3: return "CurrentReadOrWrite";
                    case 4: return "HistoryRead";
                    case 5: return "CurrentRead,HistoryRead";
                    case 8: return "HistoryWrite";
                    case 12: return "HistoryReadOrWrite";
                    case 16: return "SemanticChange";
                    case 32: return "StatusWrite";
                    case 64: return "TimestampWrite";
                    default: return "None";
                }
            }
            else
            {
                return "null";
            }
        }
    }
}
