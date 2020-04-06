using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SMTPasiakas
{
    class Smtpasiakas
    {
        static void Main(string[] args)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


            try
            {
                s.Connect("localhost", 25000);
            }
            catch (Exception ex)
            {
                Console.Write("Virhe: " + ex.Message);
                Console.ReadKey();
                throw;
            }

            NetworkStream ns = new NetworkStream(s);

            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);

            Boolean on = true;

            String messageFromServer = "";
            
            while (on)
            {
                messageFromServer = sr.ReadLine();
                Console.WriteLine(messageFromServer);
                String[] status = messageFromServer.Split(' ');

                switch (status[0])
                {
                    case "220":
                        Console.WriteLine("SMTP lisää itse protokollan mukaiset komennot kuten MAIL FROM ja HELO...");
                        String domain = "";
                        Console.Write("Anna domain tai QUIT: ");
                        domain = Console.ReadLine();
                        if (domain == "QUIT")
                        {
                            sw.WriteLine(domain);
                        }
                        else
                        {
                            domain = "HELO " + domain;
                            sw.WriteLine(domain);

                        }
                        break;
                    
                    case "221":
                        on = false;
                        Console.WriteLine("Lopeta asiakas painamalla jotakin nappia");
                        break;

                    case "250":
                        switch (status[1])
                        {
                            case "2.0.0":
                                sw.WriteLine("QUIT");
                                break;
                            case "2.1.0":
                                String receiver = "";
                                Console.Write("Vastaanottajan sähköpösti tai QUIT: ");
                                receiver  = Console.ReadLine();
                                if (receiver == "QUIT")
                                {
                                    sw.WriteLine(receiver);
                                } else
                                {
                                    receiver = "RCPT TO: " +receiver;
                                    sw.WriteLine(receiver);
                                }
                                sw.WriteLine(receiver);
                                break;
                            case "2.1.5":
                                sw.WriteLine("DATA");
                                break;
                            default:
                                String sender = "";
                                Console.Write("Vastaanottajan sähköpösti / tai QUIT: ");
                                sender = Console.ReadLine();
                                if (sender == "QUIT")
                                {
                                    sw.WriteLine(sender);
                                }
                                else
                                {
                                    sender = "MAIL FROM: " + sender;
                                    sw.WriteLine(sender);                    
                                }
                                break;
                        }
                        break;
                    case "354":
                        String message = "";
                        Console.Write("Kirjoita viestisi ilman erillistä pisteriviä tai QUIT: ");
                        message = Console.ReadLine();
                        if (message == "QUIT")
                        {
                            sw.WriteLine(message);
                        }
                        else
                        {
                            sw.WriteLine(message);
                            sw.WriteLine(".");
                        }
                        
                        break;
                    default:
                        Console.WriteLine("Virhe...");
                        sw.WriteLine("QUIT");
                        break;
                }
                sw.Flush();
            }
            Console.ReadKey();

            sw.Close();
            sr.Close();
            ns.Close();
            s.Close();


        }
    }
}
