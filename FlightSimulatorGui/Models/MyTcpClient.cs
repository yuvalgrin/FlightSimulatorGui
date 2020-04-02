﻿using System;
using System.IO;
using System.Net.Sockets;
using FlightSimulatorGui.Model;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Threading;

public class MyTcpClient
{
    public MyTcpClient()
    {
    }

    private static bool runClient = true;
    private static TcpClient client = null;
    private readonly object clientLock = new object();
    public static AutoResetEvent m = new AutoResetEvent(false);
    public static bool threadAlreadyRunning = true;



    public NetworkStream initializeConnection(string ip, string port)
    {
        Int32 connectionPort; string server;
        if (ip == null || port == null)
        {
            connectionPort = int.Parse(ConfigurationSettings.AppSettings["ServerPort"]);
            server = ConfigurationSettings.AppSettings["ServerIP"];
        } else
        {
            try
            {
                connectionPort = int.Parse(port);
                server = ip;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        try
        {
            TcpClient tcpClient = new TcpClient(server, connectionPort);
            NetworkStream stream = tcpClient.GetStream();
            MyTcpClient.client = tcpClient;
            return stream;
        }
        catch (Exception e)
        {
            return null;
        }
    }


    //create a tcp server with the default port and ip
    public void createAndRunClient(NetworkStream stream)
    {
        
        
        try
        {
            Byte[] data = null;
            // Get a client stream for reading and writing.
            //  Stream stream = client.GetStream();
            if (!runClient)
                throw new Exception("runclient problem");
            while (runClient)
            {
                // Translate the passed message into ASCII and store it as a Byte array.
                Command c = FlightSimulatorModel.get().getCommandsQueue().Dequeue();
                data = System.Text.Encoding.ASCII.GetBytes(c.execute());
                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Thread.Sleep(40);
                stream.ReadTimeout = 200;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                responseData = responseData.Substring(0, responseData.Length - 1);
                FlightSimulatorModel.get().updateValueMap(c.path(), responseData);
            }
            // Close everything
            stream.Close();
            //MyTcpClient.client.Close();
        }
        catch (IOException e)
        {
            stream.Close();
            //MyTcpClient.client.Close();
            threadAlreadyRunning = false;
            FlightSimulatorModel.get().throwNewError("Connection to the server was lost\r\n Please insert IP and Port in the connection tab");
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        finally
        {
            runClient = true;
            m.Set();
        }
        
    }

    //the function that will be run in a thread
    public static void killClient()
    {
        MyTcpClient.runClient = false;
    }
    public static void sClient()
    {
        MyTcpClient.runClient = true;
    }

    public static bool getRunning()
    {
        return MyTcpClient.runClient;
    }

}
