using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Web.Script.Serialization;
using System.IO;
using Orleans;
using Orleans.Runtime;
using System.Net.Http;
using System.ServiceModel.Channels;

namespace Server
{
    [ServiceContract]
    public interface IAccountService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/auth/{account}/{password}")]
        Message Auth(string account, string password);
    }

    public class AccountService : IAccountService
    {
        public Message Auth(string account, string password)
        {
            if (Orleans.GrainClient.IsInitialized == false)
                return WebService.JSONMessage(new { error = "Client not initialised" });

            if (account == null) //should never be hit provided no-one touches the templates
                return WebService.JSONMessage(new { error = "Account name must be provided" });
            if (password == null) //should never be hit provided no-one touches the templates
                return WebService.JSONMessage(new { error = "Password must be provided" });

            IAccount ac = Orleans.GrainClient.GrainFactory.GetGrain<IAccount>(account);

            if (ac == null)
                return WebService.JSONMessage(new { error = "Internal error: client returned null actor" });

            var result = ac.Authenticate(password).Result;

            var ret = new
            {
                account = account,
                auth_result = result,
                auth_result_enum = result.ToString()
            };

            return WebService.JSONMessage(ret);
        }
    }
}

