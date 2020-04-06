using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDPChatasiakas
{
    class Udpchatasiakas
    {
        static void Main(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, 
                SocketType.Dgram, ProtocolType.Udp);

            int portNumber = 9999;

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Loopback, portNumber);
            byte[] buf = new byte[256];

            EndPoint end = iPEndPoint;
            s.ReceiveTimeout = 1000;
            string message;

            Boolean on = true;

            do
            {
                Console.Write("Kirjoita viesti tai q(uit): ");
                message = Console.ReadLine();
                if (message.Equals("q") || message.Equals("quit"))
                {
                    on = false;
                }
                else
                {
                    s.SendTo(Encoding.ASCII.GetBytes(message), end);

                    while (!Console.KeyAvailable)
                    {
                        IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                        EndPoint palvelinep = remote;
                        string viesti = "";


                        try
                        {
                            s.ReceiveTimeout = 2000;
                            int received = s.ReceiveFrom(buf, ref palvelinep);

                            String rec_string = Encoding.ASCII.GetString(buf, 0, received);
                            char[] delim = { ';' };
                            String[] viestit = rec_string.Split(delim, 2);
                            if (viestit.Length < 2)
                            {
                                Console.WriteLine("Virhe...");
                            }
                            else
                            {
                                viesti = viestit[0] + ": " + viestit[1];
                            }

                        }
                        catch 
                        {

                           
                        }
                        if (viesti != "")
                        {
                            Console.WriteLine(viesti);
                        }
                        
                    }
                }

            } while (on);
            s.Close();
        }
    }
}
