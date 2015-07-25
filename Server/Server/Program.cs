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
                IAccountGrain test = factory.GetGrain<IAccountGrain>("Bob");

                test.CreateAccount("test", 2).Wait();

                Random rnd = new Random();

                for (int i = 0; i < 15; ++i)
                    test.AddQuestComplete((uint)rnd.Next(1, 1000));


                Console.WriteLine("Test");
            }

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
