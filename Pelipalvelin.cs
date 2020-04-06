using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Pelipalvelin
{
    class Pelipalvelin
    {
        static string Erotin = " ";
        static void Main(string[] args)
        {
            Socket palvelin;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, 9999);
            try
            {
                palvelin = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
                palvelin.Bind(endpoint);    
            }
            catch (Exception ex)
            {

                Console.WriteLine("Virhe..." + ex.Message);
                Console.ReadKey();
                return;
            }

            string state = "WAIT";
            Console.WriteLine("Palvelin käynnistetty");
            bool on = true;
            int vuoro = -1;
            int pelaajat = 0;
            int quit_ack = 0;
            int luku = -1;
            int number;
            EndPoint[] Pelaaja = new EndPoint[2];
            string[] nimet = new string[2];

            while (on)
            {
                IPEndPoint asiakas = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remote = (EndPoint)(asiakas);
                string[] kehys = Vastaanota(palvelin, ref remote);                
                switch (state)
                {
                    case "WAIT":
                        switch (kehys[0])
                        {
                            case "JOIN":
                                Pelaaja[pelaajat] = remote;
                                nimet[pelaajat] = kehys[1];
                                pelaajat++;
                                if (pelaajat == 1)
                                {
                                    Laheta(palvelin, Pelaaja[0], "ACK 201 JOIN OK");
                                    Console.WriteLine(kehys[1] + " Liittyi peliin, odotetaan toista pelaajaa");
                                }
                                else if (pelaajat == 2)
                                {
                                    // arvotaan aloittaja
                                    // arvotaan oikea luku
                                    Console.WriteLine(kehys[1] + " Liittyi peliin, peli voi alkaa!");
                                    Random rand = new Random();
                                    int aloittaja = rand.Next(0, 1);
                                    vuoro = aloittaja;
                                    Console.WriteLine("VUORO ON " + nimet[vuoro]);
                                    Console.WriteLine("Aloittaja on " + nimet[aloittaja]);
                                    luku = rand.Next(0, 10);
                                    Console.WriteLine("OIKEA VASTAUS: " + luku);
                                    Laheta(palvelin, Pelaaja[aloittaja], "ACK 202 " + nimet[Flip(aloittaja)]); // Vastustajan nimi, siksi nimessä Flip
                                    Console.WriteLine("Pelaaja {0} aloittaa", nimet[aloittaja]);
                                    Laheta(palvelin, Pelaaja[Flip(aloittaja)], "ACK 203 " + nimet[aloittaja]);
                                    state = "GAME";
                                }
                                else
                                {
                                    Console.WriteLine("Liian monta haluaa kerralla pelata :)");
                                    state = "END";
                                }
                                break;
                            default:
                                break;
                        }
                        break;
                    case "GAME":
                        switch (kehys[0])
                        {
                            case "DATA":
                                if (Pelaaja[vuoro].Equals(remote) && int.TryParse(kehys[1], out number))
                                {
                                    if (int.Parse(kehys[1]) == luku)
                                    {
                                       Laheta(palvelin, Pelaaja[Flip(vuoro)], "QUIT 502 " + luku);
                                       Laheta(palvelin, Pelaaja[vuoro], "QUIT 501"); // Oikein arvanneelle
                                        state = "END"; 
                                    }
                                    else
                                    {
                                        Console.WriteLine("Pelaaja {0} arvasi luvun {1}, joka ei ollut oikein!", nimet[vuoro], kehys[1]);
                                        Laheta(palvelin, Pelaaja[vuoro], "ACK 300 DATA OK");
                                        Laheta(palvelin, Pelaaja[Flip(vuoro)], "DATA " + kehys[1]);
                                        vuoro = Flip(vuoro);
                                        state = "WAIT_ACK";

                                    }

                                }
                                else if (Pelaaja[vuoro].Equals(remote))
                                {
                                    Laheta(palvelin, Pelaaja[vuoro], "ACK 407 Arvaus ei ollut numero");
                                }
                                else
                                {
                                    Laheta(palvelin, remote, "ACK 402 Väärä Vuoro");

                                }
                                break;
                            default:
                                if (Pelaaja[vuoro].Equals(remote))
                                {
                                    Laheta(palvelin, remote, "ACK 404 Väärä kehysrakenne");
                                }
                                else
                                {
                                    Laheta(palvelin, remote, "ACK 402 Väärä vuoro ja kehysrakenne, opettele säännöt");

                                }

                                break;
                        }
                        break;

                    case "WAIT_ACK":
                        switch (kehys[0])
                        {
                            case "ACK":
                                if (Pelaaja[vuoro].Equals(remote) && kehys[1] == "300")
                                {
                                    state = "GAME";
                                }
                                else if (Pelaaja[vuoro].Equals(remote))
                                {
                                    Laheta(palvelin, remote, "ACK 403 ACK Viesti virheellinen");
                                }
                                else
                                {
                                    Laheta(palvelin, remote, "ACK 402 Väärä vuoro");
                                }
                                break;
                            default:
                                if (Pelaaja[vuoro].Equals(remote)) { 
                                    Laheta(palvelin, remote, "ACK 404 Väärä kehysrakenne");
                                }
                                else
                                {
                                    Laheta(palvelin, remote, "ACK 402 Väärä vuoro ja kehysrakenne, opettele säännöt");                             
                                }
                                break;
                        }
                        break;

                    case "END":
                        switch (kehys[0])
                        {
                            case "ACK":
                                if (kehys[1] == "500")
                                {
                                    if (Pelaaja[vuoro].Equals(remote) || Pelaaja[Flip(vuoro)].Equals(remote))
                                    {
                                        quit_ack++;
                                        if (quit_ack == Pelaaja.Length)
                                        {
                                            on = false;
                                        }
                                    }
                                }
                                
                                break;
                            default:
                                Console.WriteLine("Väärä lopetusviesti");
                                Laheta(palvelin, remote, "ACK 400 Väärä lopetusviesti");
                                break;
                        }
                        break;

                    default:
                        Console.WriteLine("Virheitä tulee mutta tulkoon...");
                        break;
                }

            }
            palvelin.Close();
        }

        static string[] Vastaanota(Socket s, ref EndPoint endPoint)
        {
            byte[] buf = new byte[256];
            int received = s.ReceiveFrom(buf, ref endPoint);

            string rec_string = Encoding.ASCII.GetString(buf, 0, received);
            char[] delim = { ' ' };
            string[] viestit = rec_string.Split(delim, 3);
            if (viestit.Length < 2)
            {
                Console.WriteLine("Virhe...");
                return new string[0];
            }

            return viestit;
        }

        static void Laheta(Socket s, EndPoint endPoint, string message)
        {
            s.SendTo(Encoding.ASCII.GetBytes(message), endPoint);
        }

        static int Flip(int i)
        {
            return 1 - i;
        }
    }
}
