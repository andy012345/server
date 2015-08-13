using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            if (System.IO.File.Exists("Config-Server.xml"))
            {
                Console.WriteLine("Starting orleans server...");

                AppDomain hostDomain = AppDomain.CreateDomain("OrleansHost", null, new AppDomainSetup
                {
                    AppDomainInitializer = InitSilo,
                    AppDomainInitializerArguments = args,
                });
            }

            if (System.IO.File.Exists("Config-Client.xml"))
            {
                Console.WriteLine("Starting orleans client...");
                Orleans.GrainClient.Initialize("Config-Client.xml");

                if (!Orleans.GrainClient.IsInitialized)
                    throw new Exception("Could not initialise orleans client");

                var factory = Orleans.GrainClient.GrainFactory;


                //woo test code
                IAccountGrain test = factory.GetGrain<IAccountGrain>("TESTACCOUNT");

                test.CreateAccount("test").Wait();

                Random rnd = new Random();

                test.SetPassword("Test");


                Console.WriteLine("Test");

                IPlayer test1 = factory.GetGrain<IPlayer>(0);
                IUnit test2 = factory.GetGrain<IUnit>(0);
                IObject test3 = factory.GetGrain<IObject>(0);

                test1.GetPrimaryKeyLong();

                List<string> testres = new List<string>();

                testres.Add(test1.VirtualCall().Result);
                testres.Add(test2.VirtualCall().Result);
                testres.Add(test3.VirtualCall().Result);
                testres.Add(test1.PlayerCall().Result);
                testres.Add(test1.UnitCall().Result);
                testres.Add(test1.ObjectCall().Result);
                testres.Add(test2.UnitCall().Result);
                testres.Add(test2.ObjectCall().Result);
                testres.Add(test3.ObjectCall().Result);

                foreach (var line in testres)
                    Console.WriteLine("{0}", line);

                var manager = factory.GetGrain<IDataStoreManager>(0);
                manager.GetPlayerCreateInfo(0, 0);
            }

            WebService.Run();
            AuthServer.Main.Run();
            RealmServer.Main.Run();

            Console.ReadLine();
        }

        static void InitSilo(string[] args)
        {
            hostWrapper = new OrleansHostWrapper(args);

            if (!hostWrapper.Run())
            {
                Console.Error.WriteLine("Failed to initialize Orleans silo");
            }
        }

        static void ShutdownSilo()
        {
            if (hostWrapper != null)
            {
                hostWrapper.Dispose();
                GC.SuppressFinalize(hostWrapper);
            }
        }

        private static OrleansHostWrapper hostWrapper;
    }
}
