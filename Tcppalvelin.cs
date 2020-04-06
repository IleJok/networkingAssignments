using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace TCPpalvelin
{
    class Tcppalvelin
    {
        static void Main(string[] args)
        {

            Socket palvelin = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Loopback, 25000);

            palvelin.Bind(iPEndPoint);

            palvelin.Listen(5);

            Socket Asiakas = palvelin.Accept();
            // if this were a real server, this would be passed to thread

            IPEndPoint iPEndPoint2 = (IPEndPoint)Asiakas.RemoteEndPoint;

            Console.WriteLine("Yhteys osoitteesta : {0} portista {1}", iPEndPoint2.Address, iPEndPoint2.Port);

            NetworkStream networkStream = new NetworkStream(Asiakas);

            StreamReader streamReader = new StreamReader(networkStream);
            StreamWriter streamWriter = new StreamWriter(networkStream);

            String received = streamReader.ReadLine();
            Console.WriteLine("Saapunut viesti: " + received);
            streamWriter.WriteLine("Ilkan server kone;" + received + "\r\n");
            streamWriter.Flush();
            Console.ReadKey();
            streamWriter.Close();
            streamReader.Close();
            networkStream.Close();
            Asiakas.Close();
            Console.ReadKey();

            palvelin.Close();
        }

    }
}
