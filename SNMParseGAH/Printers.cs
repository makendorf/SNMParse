using Canteen;
using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SNMParseGAH
{
    public class Printers
    {
        Timer timer; 
        SpeechSynthesizer synth = new SpeechSynthesizer();

       
        // Запуск процесса записи
        public void Start()
        {
            //timer = new Timer(3600000);
            timer = new Timer(1000);
            timer.Elapsed += this.ParseNetworkStart;
            timer.Start();
            synth.SetOutputToDefaultAudioDevice();
            var voices = synth.GetInstalledVoices(new System.Globalization.CultureInfo("ru-RU"));
            synth.SelectVoice(voices[0].VoiceInfo.Name);
        }
        // Завершение записи
        public void Stop()
        {
            timer.Stop();
        }
        public void Log(object exc)
        {
            SQL sql = new SQL();
            sql.SetSqlParameters(new List<System.Data.SqlClient.SqlParameter>
            {
                new System.Data.SqlClient.SqlParameter("@date", DateTime.Now),
                new System.Data.SqlClient.SqlParameter("@log", exc.ToString())
            });
            sql.ExecuteNonQuery("insert into Log (date, log) values (@date, @log)");
        }
        // Собственно запись
        public void ParseNetworkStart(Object source, ElapsedEventArgs e)
        {
            timer.Interval = 3600000;
            Log("Старт проверки");
            synth.SpeakAsync("Старт службы");
            OctetString community = new OctetString("public");
            AgentParameters param = new AgentParameters(community)
            {
                Version = SnmpVersion.Ver1
            };
            SQL sql = new SQL();
            string ip = "";
            for (int i = 0; i <= 255; i++)
            {
                try
                {
                    dynamic[] arrSNMP = {"","","","","","" };
                    ip = $"172.16.10.{i}";
                    IpAddress agent = new IpAddress(ip);
                    UdpTarget target = new UdpTarget((IPAddress)agent, 161, 2000, 1);
                    Pdu pdu = new Pdu(PduType.Get);
                   
                    if(SeachPrinter(ip, "1.3.6.1.2.1.43.5.1.1.16.1") != string.Empty) //Имя
                    {
                        pdu.VbList.Add("1.3.6.1.2.1.43.5.1.1.16.1");
                    }
                    else
                    {
                        pdu.VbList.Add("1.3.6.1.2.1.25.3.2.1.3.1");
                    }
                    pdu.VbList.Add("1.3.6.1.2.1.43.11.1.1.9.1.1"); //Максимальный уровень тонера
                    pdu.VbList.Add("1.3.6.1.2.1.43.11.1.1.8.1.1"); //Текущий уровень тонера
                    pdu.VbList.Add("1.3.6.1.2.1.43.10.2.1.4.1.1"); //Счетчик страниц
                    if (SeachPrinter(ip, "1.3.6.1.2.1.2.2.1.6.2") != string.Empty)//MAK
                    {
                        pdu.VbList.Add("1.3.6.1.2.1.2.2.1.6.2");
                    }
                    else
                    {
                        pdu.VbList.Add("1.3.6.1.2.1.2.2.1.6.1");
                    }
                    pdu.VbList.Add("1.3.6.1.2.1.1.6.0");//Расположение

                    SnmpV1Packet result = (SnmpV1Packet)target.Request(pdu, param);
                    if (result != null)
                    {
                        if (result.Pdu.ErrorStatus != 0)
                        {
                        }
                        else
                        {
                            

                            for (int j = 0; j < result.Pdu.VbCount; j++)
                            {
                                try
                                {
                                    if(Convert.ToInt32(result.Pdu.VbList[j].Value) <= 0)
                                    {
                                        arrSNMP[j] = 0;
                                    }
                                    else
                                    {
                                        arrSNMP[j] = result.Pdu.VbList[j].Value;
                                    }
                                }
                                catch 
                                {
                                    arrSNMP[j] = result.Pdu.VbList[j].Value;
                                }
                            }
                           
                            
                            //if(Convert.ToDouble(arrSNMP[1] * 100 / arrSNMP[2]) < 0)
                            //{
                            //    arrSNMP[1] = SeachPrinter(ip, "1.3.6.1.2.1.43.11.1.1.7.1.1");
                            //}
                            //else
                            //{
                            //    arrSNMP[1] = Convert.ToDouble(arrSNMP[1] * 100 / arrSNMP[2]);
                            //}
                            sql.SetSqlParameters(new List<System.Data.SqlClient.SqlParameter>
                                {
                                    new System.Data.SqlClient.SqlParameter("@name", Convert.ToString(arrSNMP[0])),
                                    new System.Data.SqlClient.SqlParameter("@toner", Convert.ToDouble(arrSNMP[1] * 100 / arrSNMP[2])),
                                    new System.Data.SqlClient.SqlParameter("@count", Convert.ToInt32(arrSNMP[3])),
                                    new System.Data.SqlClient.SqlParameter("@ip", ip),
                                    new System.Data.SqlClient.SqlParameter("@mac", Convert.ToString(arrSNMP[4])),
                                    new System.Data.SqlClient.SqlParameter("@location", Convert.ToString(arrSNMP[5]))
                                });

                            sql.ExecuteNonQuery("insert into PrintersCount "+
                                                "( " +
                                                "    [name], toner, print_count, ip, mac, location " +
                                                ") " +
                                                "values " +
                                                "( " +
                                                    "@name, @toner, @count, @ip, @mac, @location " +
                                                "); ");

                        }
                    }
                    else
                    {
                    }
                    target.Close();
                }
                catch(Exception exc)
                {
                    //Log($"{ip}: {exc.Message}");
                }
            }
        }
        private static string SeachPrinter(string ip, string OiD = "")
        {
            OctetString community = new OctetString("public");
            AgentParameters param = new AgentParameters(community);
            string arr = "";
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
            return arr;
        }
    }
}
