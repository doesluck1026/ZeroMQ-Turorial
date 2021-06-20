using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Main
{
    #region Parameters

    private static string Topic = "Screen";
    private static int CommunicationFrequency = 60; /// Hz
    public static int Port = 4145;

    #endregion

    #region Public Variables

    public static string MyIP;
    public static double CommunicationPeriod;
    public static bool IsNewDataReceived
    {
        get
        {
            lock (Lck_IsSubDataReceived)
                return _isSubDataReceived;
        }
        private set
        {
            lock (Lck_IsSubDataReceived)
                _isSubDataReceived = value;
        }
    }
    public static bool IsPublisherEnabled
    {
        get
        {
            lock (Lck_IsPublisherEnabled)
                return _isPublisherEnabled;
        }
        private set
        {
            lock (Lck_IsPublisherEnabled)
                _isPublisherEnabled = value;
        }
    }
    public static bool IsSubscriberEnabled
    {
        get
        {
            lock (Lck_IsSubscriberEnabled)
                return _isSubscriberEnabled;
        }
        private set
        {
            lock (Lck_IsSubscriberEnabled)
                _isSubscriberEnabled = value;
        }
    }
    #endregion

    #region Private Variables

    private static MQPublisher Publisher;
    private static MQSubscriber Subscriber;
    private static Thread PublisherThread;

    private static bool _isSubDataReceived = false;
    private static bool _isPublisherEnabled = false;
    private static bool _isSubscriberEnabled = false;
    private static byte[] SubReceivedData;

    #endregion

    #region Lock Objects
    private static object Lck_IsSubDataReceived = new object();
    private static object Lck_SubReceivedData = new object();
    private static object Lck_IsPublisherEnabled = new object();
    private static object Lck_IsSubscriberEnabled = new object();

    #endregion

    #region Publisher Functions

    /// <summary>
    /// Initializes a MQ Publisher with defined topic at given port and starts publisher thread. Also Starts Screen Capturer
    /// </summary>
    public static void StartSharing()
    {
        CommunicationPeriod = 1.0 / CommunicationFrequency;
        MyIP = GetDeviceIP();
        Publisher = new MQPublisher(Topic, MyIP, Port);
        IsPublisherEnabled = true;
        PublisherThread = new Thread(PublisherCoreFcn);
        PublisherThread.Start();
    }

    /// <summary>
    /// Stops ScreenCapturer and publisher. and cleans everthing related.
    /// </summary>
    public static void StopSharing()
    {
        IsPublisherEnabled = false;
        try
        {
            if (PublisherThread != null)
            {
                if(PublisherThread.IsAlive)
                    PublisherThread.Abort();
                PublisherThread = null;
            }
            if(Publisher!=null)
                Publisher.Stop();
            Publisher = null;
        }
        catch
        {
            Debug.WriteLine("Failed to Stop Publisher");
        }
    }
    /// <summary>
    /// This will be used by publisher thread as long as it is activated. Data will be prepared and published here in this function.
    /// </summary>
    private static void PublisherCoreFcn()
    {
        Stopwatch watch = Stopwatch.StartNew();
        while (IsPublisherEnabled)
        {
            Bitmap image = ImageProcessing.GetScreenShot();
            byte[] data = ImageProcessing.ImageToByteArray(image);
            if (data != null)
                Publisher.Publish(data);
            while (watch.Elapsed.TotalSeconds <= CommunicationPeriod)
                Thread.Sleep(1);
            watch.Restart();
        }
        Debug.WriteLine("While loop in Publisheer Core Fcn is broken");
    }

    #endregion

    #region Subscriber Functions
    public static void StartReceiving(string ip)
    {
        Subscriber = new MQSubscriber(Topic, ip, Port);
        Subscriber.OnDataReceived += Subscriber_OnDataReceived;
        IsSubscriberEnabled = true;
    }
    public static void StopReceiving()
    {
        if (Subscriber != null)
        {
            Subscriber.OnDataReceived -= Subscriber_OnDataReceived;
            Subscriber.Stop();
            Subscriber = null;
        }
        IsSubscriberEnabled = false;
    }
    /// <summary>
    /// This function will be called when a data is received by subscriber
    /// </summary>
    /// <param name="data"></param>
    private static void Subscriber_OnDataReceived(byte[] data)
    {
        if (data != null)
        {
            lock (Lck_SubReceivedData)
            {
                SubReceivedData = data;
            }
            IsNewDataReceived = true;
        }
        else
        {
            Debug.WriteLine("image data was null!");
        }
    }


    #endregion

    /// <summary>
    /// Gets current device's ip4 address.
    /// </summary>
    /// <returns>ip as string</returns>
    public static string GetDeviceIP()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        string localAddr = "";

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localAddr = ip.ToString();
            }
        }
        return localAddr;
    }

    public static Bitmap GetReceivedImage()
    {
        byte[] data;
        lock (Lck_SubReceivedData)
        {
            if (SubReceivedData == null)
                return null;
            data = new byte[SubReceivedData.Length];
            SubReceivedData.CopyTo(data, 0);
        }
        IsNewDataReceived = false;
        return ImageProcessing.ImageFromByteArray(data);
    }
}
