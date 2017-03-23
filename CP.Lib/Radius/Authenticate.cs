using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FP.Radius
{
     public class Authenticate
    {
        public async static Task Authentication(string[] args)
        {
            try
            {
                RadiusClient rc = new RadiusClient(args[0], args[1]);
                RadiusPacket authPacket = rc.Authenticate(args[2], args[3]);
                authPacket.SetAttribute(new VendorSpecificAttribute(10135, 1, UTF8Encoding.UTF8.GetBytes("Testing")));
                authPacket.SetAttribute(new VendorSpecificAttribute(10135, 2, new[] { (byte)7 }));

                RadiusPacket receivedPacket = await rc.SendAndReceivePacket(authPacket);
                if (receivedPacket == null) throw new Exception("Can't contact remote radius server !");

                switch (receivedPacket.PacketType)
                {
                    case RadiusCode.ACCESS_ACCEPT:
                        Console.WriteLine("Access-Accept");
                        foreach (var attr in receivedPacket.Attributes)
                            Console.WriteLine(attr.Type.ToString() + " = " + attr.Value);
                        break;
                    case RadiusCode.ACCESS_CHALLENGE:
                        Console.WriteLine("Access-Challenge");
                        break;
                    case RadiusCode.ACCESS_REJECT:
                        Console.WriteLine("Access-Reject");
                        if (!rc.VerifyAuthenticator(authPacket, receivedPacket))
                            Console.WriteLine("Authenticator check failed: Check your secret");
                        break;
                    default:
                        Console.WriteLine("Rejected");
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static void ShowUsage()
        {
            Console.WriteLine("Usage : ClientTest.exe hostname sharedsecret username password");
        }
    }
}
