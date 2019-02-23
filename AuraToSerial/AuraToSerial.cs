using AuraSDKWrapper;
using NLog;
using System;
using System.IO.Ports;
using System.Timers;

namespace AuraToSerial
{
    public class AuraToSerial
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string _portNo;
        private readonly Timer _timer;
        private SerialPort _port;
        private AuraSDK _auraSdk;

        public AuraToSerial(string portNo, TimeSpan interval)
        {
            _portNo = portNo;
            _timer = new Timer(interval.TotalMilliseconds) { AutoReset = true };
            _timer.Elapsed += Tick;
        }

        public void Start()
        {
            _auraSdk = new AuraSDK();
            var success = _auraSdk.DetectAuraDevices();

            if (success != 0 || _auraSdk.MBControllersCount < 0)
            {
                Logger.Error("Invalid Aura setup");
                return;
            }

            _port = new SerialPort(_portNo);
            _port.Open();
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _timer.Dispose();
            _port.Close();
            _port.Dispose();
        }

        private void Tick(object sender, ElapsedEventArgs e)
        {
            var current = _auraSdk.GetMBLedColor(0);

            if (current.Length < 3)
            {
                Logger.Warn("Not enough data");
                return;
            }

            var bytes = new byte[3] { current[0], current[1], current[2] };
            _port.Write(bytes, 0, 3);
        }
    }
}
