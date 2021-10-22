using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TasksShared
{
    public class SerialPortIO8Manipulate
    {

        public static string Locate_serialPortIO8Name()
        {/* 
            Locate the correct COM port used for communicating with the DLP-IO8 and return serialPortIO8 Name 

            Returns:
                string serialPortIO8 Name, if located; "", false
            
             */

            string[] portNames = SerialPort.GetPortNames();
            string serialPortIO8_Name = "";
            foreach (string portName in portNames)
            {
                SerialPort serialPort = new SerialPort();
                try
                {
                    serialPort.PortName = portName;
                    serialPort.BaudRate = 115200;
                    serialPort.Open();

                    for (int i = 0; i < 5; i++)
                    {
                        // channel 1 Ping command of the DLP-IO8, return 0 or 1
                        serialPort.WriteLine("Z");

                        // Read exist Analog in from serialPort
                        string str_Read = serialPort.ReadExisting();

                        if (str_Read.Contains("V"))
                        {//
                            serialPortIO8_Name = portName;
                            serialPort.Close();
                            return serialPortIO8_Name;
                        }
                        Thread.Sleep(30);
                    }
                    serialPort.Close();
                }
                catch (Exception ex)
                {
                    if (serialPort.IsOpen)
                        serialPort.Close();
                    MessageBox.Show(ex.Message, "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return serialPortIO8_Name;
        }


        public static void Open_serialPortIO8(SerialPort serialPort_IO8, string portName, int baudRate)
        {
            try
            {
                serialPort_IO8.PortName = portName;
                serialPort_IO8.BaudRate = baudRate;
                serialPort_IO8.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static string Convert2_IO8EventCmd_Bit5to8(string EventCode)
        {/*
            Generate IO8 Event Command based on EventCode using bit 5-8
            E.g. "0000" -> "TYUI", "1111" -> "5678", "1010" -> "5Y7I"
            */

            string cmdHigh5 = "5";
            string cmdLow5 = "T";
            string cmdHigh6 = "6";
            string cmdLow6 = "Y";
            string cmdHigh7 = "7";
            string cmdLow7 = "U";
            string cmdHigh8 = "8";
            string cmdLow8 = "I";

            string IO8EventCmd = cmdLow5 + cmdLow6 + cmdLow7 + cmdLow8;
            if (EventCode[0] == '1')
                IO8EventCmd = IO8EventCmd.Remove(0, 1).Insert(0, cmdHigh5);
            if (EventCode[1] == '1')
                IO8EventCmd = IO8EventCmd.Remove(1, 1).Insert(1, cmdHigh6);
            if (EventCode[2] == '1')
                IO8EventCmd = IO8EventCmd.Remove(2, 1).Insert(2, cmdHigh7);
            if (EventCode[3] == '1')
                IO8EventCmd = IO8EventCmd.Remove(3, 1).Insert(3, cmdHigh8);

            return IO8EventCmd;
        }
    }
}
