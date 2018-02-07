using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Best.VWPlatform.Common.Core.Rpc
{
    internal class RpcMethod
    {
        private readonly XElement _mElement;
        private RpcMethod(string pInterfaceName, string pFunctionName, IEnumerable<object> pParames)
        {
            _mElement = new XElement("CallRemoting");
            XElement interfaceElement = new XElement("Interface");
            interfaceElement.SetValue(pInterfaceName);
            _mElement.Add(interfaceElement);
            XElement methodElement = new XElement("Method");
            methodElement.SetValue(pFunctionName);
            _mElement.Add(methodElement);
            if (pParames != null)
            {
                XElement paramsElement = new XElement("Params");
                foreach (object param in pParames)
                {
                    XElement paramElement = new XElement("Param");
                    paramElement.SetAttributeValue("Type", param.GetType().ToString());
                    XCData xcd = new XCData(param.ToString());
                    paramElement.Add(xcd);
                    paramsElement.Add(paramElement);
                }
                _mElement.Add(paramsElement);
            }
        }

        public static string GetRpcMethodContext(string pInterfaceName, string pFunctionName, object[] pParames)
        {
            RpcMethod rpcMethod = new RpcMethod(pInterfaceName, pFunctionName, pParames);
            return rpcMethod._mElement.ToString();
        }
    }
}
