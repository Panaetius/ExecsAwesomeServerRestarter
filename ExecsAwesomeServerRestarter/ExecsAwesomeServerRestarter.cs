using System;
using System.Net;
using System.Configuration;

using Arma2Net.AddInProxy;
using BattleNET;

namespace ExecsAwesomeServerRestarter
{
    [AddIn("ExecsAwesomeServerRestarter")]
    public class ExecsAwesomeServerRestarter
    {
        private BattlEyeClient Login()
        {
            var credentials = new BattlEyeLoginCredentials(
                    Dns.GetHostAddresses(ConfigurationManager.AppSettings["host"])[0],
                    int.Parse(ConfigurationManager.AppSettings["port"]),
                    ConfigurationManager.AppSettings["password"]);

            var b = new BattlEyeClient(credentials);
            b.BattlEyeMessageReceived += BattlEyeMessageReceived;
            b.BattlEyeConnected += BattlEyeConnected;
            b.BattlEyeDisconnected += BattlEyeDisconnected;
            b.ReconnectOnPacketLoss = true;
            b.Connect();

            return b;
        }

        public bool ShutdownServer()
        {
            try
            {
                var b = this.Login();

                b.SendCommand("#shutdown");

                b.Disconnect();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return false;
            }
        }

        public bool LockServer()
        {
            try
            {
                var b = this.Login();

                b.SendCommand("#lock");

                b.Disconnect();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                return false;
            }
        }

        private static void BattlEyeConnected(BattlEyeConnectEventArgs args)
        {
            Console.WriteLine(args.Message);
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
