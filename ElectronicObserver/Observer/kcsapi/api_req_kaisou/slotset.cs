using ElectronicObserver.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicObserver.Observer.kcsapi.api_req_kaisou
{

    public class slotset : APIBase
    {
        public override bool IsRequestSupported => true;
        public override bool IsResponseSupported => false;

        public override void OnRequestReceived(Dictionary<string, string> data)
        {
            Utility.DynamicDataReader.Instance.Get_Fit(data);

            base.OnRequestReceived(data);
        }

        public override string APIName => "api_req_kaisou/slotset";
    }

}