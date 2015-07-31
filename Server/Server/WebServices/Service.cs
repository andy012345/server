using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.IO;
using Orleans;
using Orleans.Runtime;


namespace Server
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        string TestGet(string s);

        [OperationContract]
        [WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        string TestPost(string s);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        System.ServiceModel.Channels.Message OrleanStats();
    }

    public class Service : IService
    {
        public string TestGet(string s) { return "Hi"; }
        public string TestPost(string s) { return "Hi2"; }
        public System.ServiceModel.Channels.Message OrleanStats()
        {
            if (Orleans.GrainClient.IsInitialized == false)
                return WebOperationContext.Current.CreateTextResponse("Error: Client not initialised", "text/plain", Encoding.UTF8);

            IManagementGrain systemManagement = GrainClient.GrainFactory.GetGrain<IManagementGrain>(RuntimeInterfaceConstants.SYSTEM_MANAGEMENT_ID);

            if (systemManagement == null)
                return WebOperationContext.Current.CreateTextResponse("Error: System management not found", "text/plain", Encoding.UTF8);

            var stats = systemManagement.GetSimpleGrainStatistics().Result;

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            Newtonsoft.Json.JsonTextWriter writer = new Newtonsoft.Json.JsonTextWriter(sw);
            writer.Formatting = Newtonsoft.Json.Formatting.Indented;

            writer.WriteStartObject();
            writer.WritePropertyName("stats");
            writer.WriteStartArray();

            foreach (var s in stats)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("activations");
                writer.WriteValue(s.ActivationCount);
                writer.WritePropertyName("address");
                writer.WriteValue(s.SiloAddress.ToString());
                writer.WritePropertyName("type");
                writer.WriteValue(s.GrainType);
                writer.WriteEndObject();

            }

            writer.WriteEndArray();
            writer.WriteEndObject();

            string ret = sb.ToString();
            return WebOperationContext.Current.CreateTextResponse(ret, "text/plain", Encoding.UTF8);
        }
    }
}

