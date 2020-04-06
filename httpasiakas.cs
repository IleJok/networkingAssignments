using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace HttpAsiakas
{
    class Httpasiakas
    {
        static void Main(string[] args)
        {
            // This program is done by Ilkka Jokela for tietoverkot course

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            s.Connect("localhost", 25000);

            String message = "GET / HTTP/1.1\r\nHost: localhost\r\n\r\n";

            byte[] buffer = Encoding.ASCII.GetBytes(message);
            // Send message to server
            s.Send(buffer);

            String page = "";           
            int count = 0;
            do
            {
                byte[] vs = new byte[1024];
 
                count = s.Receive(vs);

                page += Encoding.ASCII.GetString(vs, 0, count);
                


            } while (count > 0);

            // this to get the html page without http headers
            // substring cuts the string from the index given
            // in this case the index is the first occurence of <html>
            string pat = "<html>";
            int beg = page.IndexOf(pat);
            string finalpage = page.Substring(beg);
            Console.Write(finalpage);

            // Stop the program before closing the connection
            Console.ReadKey();
            s.Close();
        }
    }
}
