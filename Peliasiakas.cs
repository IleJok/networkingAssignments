using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Peliasiakas
{
    class Peliasiakas
    {
        static void Main(string[] args)
        {
            Socket palvelin = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, 9999);

            EndPoint poe = ep;
            palvelin.ReceiveTimeout = 1000000;
            Console.WriteLine("Tervetuloa kahden hengen arvauspeliin, jossa arvataan numeroa!");
            Console.Write("Anna nimesi peliin: ");
            string nimi = Console.ReadLine();
            Laheta(palvelin, poe, "JOIN " + nimi);

            Boolean on = true;
            string tila = "JOIN";
            string vastustaja = "";
            int luku = 0;
            while (on)
            {

                string[] palvelinVastaus = Vastaanota(palvelin);
                if (palvelinVastaus.Length == 0)
                {
                    tila = "CLOSED";
                }
                
                switch (tila)
                {
                    case "JOIN":
                        switch (palvelinVastaus[0])
                        {
                            case "ACK":
                                switch (palvelinVastaus[1])
                                {
                                    case "201":
                                        Console.WriteLine("Liitytty, odotetaan toista pelaajaa...");
                                        break;
                                    case "202":
                                        Console.WriteLine("Vastustajasi on {0}", palvelinVastaus[2]);
                                        vastustaja = palvelinVastaus[2];
                                        Console.Write("Arvaa numero 1-10: ");
                                        
                                        try
                                        {
                                            luku = int.Parse(Console.ReadLine());
                                        }
                                        catch {
                                            Console.WriteLine("Arvauksesi ei ollut numero, sinulle arvottiin 0 vastaukseksi");
                                            luku = 0;
                                        }
                                        Laheta(palvelin, poe, "DATA " + luku.ToString());
                                        tila = "GAME";
                                        break;
                                    case "203":
                                        Console.WriteLine("Vastustaja {0} saa aloittaa", palvelinVastaus[2]);
                                        vastustaja = palvelinVastaus[2];
                                        tila = "GAME";
                                        break;
                                    case "402":
                                        Console.WriteLine("EI OLE VUOROSI");
                                        break;
                                    case "403":
                                        Console.WriteLine("ACK VIESTI VIRHEELLINEN");
                                        break;
                                    default:
                                        Console.WriteLine("Virhe " + palvelinVastaus[1]);                               
                                        break;
                                }
                                break;                  
                            default:
                                Console.WriteLine("Virhe " + palvelinVastaus[0]);
                                break;
                        }
                        break;
                    case "GAME":
                        switch (palvelinVastaus[0])
                        {
                            case "ACK":
                                switch (palvelinVastaus[1])
                                {
                                    case "300":
                                        Console.WriteLine("Vastaus hyväksytty, mutta se ei ollut oikein!");
                                        break;
                                    case "402":
                                        Console.WriteLine("Väärä vuoro, ole kohtelias ja odota vuoroasi");
                                        break;
                                    case "407":
                                        Console.WriteLine("Muista käyttää numeroita, tämä ei ole hirsipuu");
                                        break;
                                    default:
                                        Console.WriteLine("Virhe " + palvelinVastaus[1]);
                                        break;
                                }
                                break;
                            case "DATA":
                                Console.WriteLine("Vastustajasi {0} arvasi numeron {1}, joka oli väärin", vastustaja, palvelinVastaus[1]);
                                Boolean jatketaan = true;
                                while (jatketaan)
                                {
                                    Console.Write("Kuittaa arvaus vastaanotetuksi kirjaimella k tai lopeta peli e:llä: ");
                                    string kuittaus = Console.ReadLine();
                                    if (kuittaus == "e")
                                    {
                                        Laheta(palvelin, poe, "QUIT 500");
                                        jatketaan = false;
                                    }
                                    else if (kuittaus == "k")
                                    {
                                        Laheta(palvelin, poe, "ACK 300 DATA OK");
                                        Console.Write("Arvaa numero 1-100: ");
                                        
                                        try
                                        {
                                            luku = int.Parse(Console.ReadLine());
                                        }
                                        catch
                                        {
                                            Console.WriteLine("Arvauksesi ei ollut numero, sinulle arvottiin 0 vastaukseksi");
                                            luku = 0;
                                        }
                                        Laheta(palvelin, poe, "DATA " + luku.ToString());
                                        jatketaan = false;
                                    }
                                    
                                }
                                break;
                            case "QUIT":
                                
                                switch (palvelinVastaus[1])
                                {
                                    case "501":
                                        Console.WriteLine("Arvauksesi {0} oli oikein! Voitit pelin!", luku);                                      
                                        Laheta(palvelin, poe, "ACK 500");
                                        on = false;
                                        break;
                                    case "502":
                                        Console.WriteLine("Vastustajasi arvasi oikein luvun " + palvelinVastaus[2]);
                                        Laheta(palvelin, poe, "ACK 500");
                                        on = false;
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            default:
                                Console.WriteLine("Virhe " + palvelinVastaus[0]);
                                break;
                        }
                        break;
                    case "CLOSED":
                        on = false;
                        break;
                    default:
                        on = false;
                        break;
                }

            }
            palvelin.Close();
        }

        static void Laheta(Socket s, EndPoint endPoint, string message) {
            s.SendTo(Encoding.ASCII.GetBytes(message), endPoint);
        }

        static string[] Vastaanota(Socket s) {
            
            try
            {
                IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
                EndPoint palvelinep = remote;

                byte[] buf = new byte[256];
                int received = s.ReceiveFrom(buf, ref palvelinep);

                string rec_string = Encoding.ASCII.GetString(buf, 0, received);
                char[] delim = { ' ' };
                string[] viestit = rec_string.Split(delim, 3);
                return viestit;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Palvelin ei ole vielä käynnissä, käynnistä se ensin!");
                return new string[0];
            }
            
        }
    }
}
