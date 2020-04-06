using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPasiakas
{
    class Tcpasiakas
    {
        static void Main(string[] args)
        {
            // This program is done by Ilkka Jokela for tietoverkot course 1.B

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            s.Connect("localhost", 25000);
            Console.Write("Lähetä palvelimelle");
            String message = Console.ReadLine();
            message += "\r\n\r\n";
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            // Send message to server
            s.Send(buffer);

            String rec_string = "";
           

           
                byte[] rec = new byte[1024];
                int bytes_received = s.Receive(rec);
                Console.WriteLine("bytejä " + bytes_received.ToString());
                if (bytes_received > 0)
                {
                    rec_string = rec_string + Encoding.ASCII.GetString(rec, 0, bytes_received);
                    
                }
           

            string[] kallenmatiTahna = rec_string.Split(';');
            Console.WriteLine("Palvelin: " + kallenmatiTahna[0]);
            Console.WriteLine("Teksti: " + kallenmatiTahna[1]);

            // Stop the program before closing the connection
            Console.ReadKey();
            s.Close();
        }
    }
}
