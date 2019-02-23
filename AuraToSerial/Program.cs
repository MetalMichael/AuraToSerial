using NLog;
using System;
using Topshelf;

namespace AuraToSerial
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static string portNo = "COM4";
        private static int DelayMs = 10;

        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>
            {
                x.Service<AuraToSerial>(s =>
                {
                    s.ConstructUsing(name => new AuraToSerial(portNo, TimeSpan.FromMilliseconds(DelayMs)));
                    s.WhenStarted(a2s => a2s.Start());
                    s.WhenStopped(a2s => a2s.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Convert the current first Motherboard LED colour to a Serial Message");
                x.SetDisplayName("AuraToSerial");
                x.SetServiceName("AuraToSerial");
                x.OnException(ex => Logger.Error("AuraToSerial TopShelf encountered an error"));
            });
        }
    }
}
