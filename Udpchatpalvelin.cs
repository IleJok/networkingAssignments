using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDPChatpalvelin
{
    class Udpchatpalvelin
    {
        static void Main(string[] args)
        {
            Socket s = null;
            int portNumber = 9999;
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Loopback, portNumber);
            List<EndPoint> asiakkaat = new List<EndPoint>();

            try
            {
                s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                s.Bind(iPEnd);
            
            }
            catch (Exception ex)
            {
                Console.WriteLine("Virhe..." + ex.Message);
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Odotetaan asiakasta...");
       
            while (!Console.KeyAvailable)
            {
                byte[] buf = new byte[256];
                IPEndPoint asiakas = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remote = (EndPoint)asiakas;
                int received = s.ReceiveFrom(buf, ref remote);

                String rec_string = Encoding.ASCII.GetString(buf, 0, received);
                char[] delim = { ';' };
                String[] viestit = rec_string.Split(delim, 2);
                if (viestit.Length < 2)
                {
                    Console.WriteLine("Virhe...");
                }
                else
                {
                    if (!asiakkaat.Contains(remote))
                    {
                        asiakkaat.Add(remote);
                        Console.WriteLine("Uusi asiakas: [{0}:{1}]", 
                            ((IPEndPoint)remote).Address, ((IPEndPoint)remote).Port);
                    }
                    string viesti = viestit[0] + ": " + viestit[1];
                    Console.WriteLine(viesti);
                    foreach (var item in asiakkaat)
                    {
                        s.SendTo(Encoding.ASCII.GetBytes(rec_string), item);
                    }
                }
            }

            Console.ReadKey();
            s.Close();

        }
    }
}
