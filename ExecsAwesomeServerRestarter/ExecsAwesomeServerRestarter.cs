using System;
using System.IO;
using System.Net;
using System.Threading;

using Arma2Net.AddInProxy;
using BattleNET;


namespace ExecsAwesomeServerRestarter
{
    [AddIn("ExecsAwesomeServerRestarter")]
    public class ExecsAwesomeServerRestarter : MethodAddIn
    {
        private static Action Callback;

        private static BattlEyeClient Client;

        private static EventWaitHandle done = new EventWaitHandle(false, EventResetMode.AutoReset);

        private void Login(string host, string port, string password)
        {
            var credentials = new BattlEyeLoginCredentials(
                    Dns.GetHostAddresses(host)[0],
                    int.Parse(port),
                    password);

            Client = new BattlEyeClient(credentials);

            Client.BattlEyeMessageReceived += BattlEyeMessageReceived;
            Client.BattlEyeConnected += BattlEyeConnected;
            Client.BattlEyeDisconnected += BattlEyeDisconnected;
            Client.ReconnectOnPacketLoss = true;
            Client.Connect();

        }

        public string ShutdownServer(string host, string port, string password)
        {
            try
            {
                Callback = () =>
                    {
                        Client.SendCommand("#shutdown");

                        Client.Disconnect();
                    };

                this.Login(host, port, password);

                done.WaitOne();

                return "true";
            }
            catch (Exception e)
            {
                File.WriteAllText(@"debug.log", e.Message + "\r\n\r\n" + e.StackTrace);

                return "false";
            }
        }

        public string LockServer(string host, string port, string password)
        {
            try
            {
                Callback = () =>
                    {
                        Client.SendCommand("#lock");

                        Client.Disconnect();
                    };

                this.Login(host, port, password);

                done.WaitOne();

                return "true";
            }
            catch (Exception e)
            {
                File.WriteAllText(@"debug.log", e.Message + "\r\n\r\n" + e.StackTrace);

                return "false";
            }
        }

        private static void BattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            if (Callback != null)
            {
                Callback.Invoke();

                Client.Disconnect();

                Callback = null;
                Client = null;

                done.Set();
            }
        }

        private static void BattlEyeDisconnected(BattlEyeDisconnectEventArgs args)
        {
            Console.WriteLine(args.Message);
        }

        private static void BattlEyeMessageReceived(BattlEyeMessageEventArgs args)
        {
            Console.WriteLine(args.Message);
        }
    }
}
