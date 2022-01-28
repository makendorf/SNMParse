using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace SpeechTEST
{
    internal class Program
    {
        static void Main(string[] args)
        {
            OctetString community = new OctetString("public");
            AgentParameters param = new AgentParameters(community);
            string arr = "";
            string ip = "172.16.10.128";
            string OiD = "1.3.6.1.2.1.2.2.1.6.1";
            param.Version = SnmpVersion.Ver1;
            try
            {
                param.Version = SnmpVersion.Ver2;
                Console.WriteLine(ip);
                IpAddress agent = new IpAddress(ip);
                // Construct target
                UdpTarget target = new UdpTarget((IPAddress)agent, 161, 2000, 1);
                Pdu pdu = new Pdu(PduType.Get);
                pdu.VbList.Add(OiD); //MAC HP
                SnmpV2Packet result = (SnmpV2Packet)target.Request(pdu, param);
                if (result != null)
                {
                    if (result.Pdu.ErrorStatus != 0)
                    {
                    }
                    else
                    {
                        for (int j = 0; j < result.Pdu.VbCount; j++)
                        {
                            arr = result.Pdu.VbList[j].Value.ToString();
                        }
                    }
                }
                else
                {
                }
                target.Close();
            }
            catch { }
            Console.WriteLine(arr);
        }
    }
}
