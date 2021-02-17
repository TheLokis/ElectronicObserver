using ElectronicObserver.Utility;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ElectronicObserver.Data
{
    public class NodeData
    {
        public static string GetNodeName(int world, int map, int node)
        {
            if (ExternalDataReader.Instance.GetData(TranslateType.NodeData)["World " + world + "-" + map] == null)
            {
                return node.ToString();
            }

            return ExternalDataReader.Instance.GetData(TranslateType.NodeData)["World " + world + "-" + map].SelectToken(node.ToString())[1].ToString();
        }


        public static List<int> GetSameNodeList(int world, int map, int node)
        {
            List<int> nodeList = new List<int>();

            var targetMap = ExternalDataReader.Instance.GetData(TranslateType.NodeData)["World " + world + "-" + map];
            if (targetMap == null)
            {
                nodeList.Add(node);
                return nodeList;
            }

            if (targetMap.SelectToken(node.ToString()) == null)
            {
                nodeList.Add(node);
                return nodeList;
            }

            var targetNode = targetMap.SelectToken(node.ToString())[1];
            var nodes = targetMap.ToList<JToken>();

            foreach (var nod in nodes)
            {
                var jProperty = nod.ToObject<JProperty>();
                if (targetNode.Equals(jProperty.Value[1]))
                    nodeList.Add(Convert.ToInt32(jProperty.Name));
            }

            return nodeList;
        }
    }
}
