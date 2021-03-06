using NetMQ;
using NetMQ.Sockets;

/// <summary>
/// This Class is used to publish data under a topic using zero MQ.
/// You only need to provide ip, port and topic in order to start easy transfer.
/// After Starting Publisher, Prepare your data and call the Publish function in a thread, timer or whenever you want.
/// If a subscriber comes and subscribes, it will receive your data. And if you close publisher and restart it again,
/// Subacriber will again start receiving your data without doing anything special.
/// 
/// And don't forget to pray the Lords of Communication for giving this beautiful protocol to us...
/// </summary>
class MQPublisher
{
    /// <summary>
    ///  Topic name to be published with data.
    /// </summary>
    public string Topic { get; private set; }

    /// <summary>
    /// Your IP Address.
    /// </summary>
    public string IP { get; private set; }

    /// <summary>
    /// The Port You Want to Publish in.
    /// </summary>
    public int Port {get; private set;}


    /// <summary>
    /// High Water Mark of Publisher Socket.
    /// Default is Set to 1000.
    /// This Parameters determines how many memeber can be in a queue if data 
    /// cant be received by subscriber.
    /// Setting this value too high can result memory overflow problems
    /// </summary>
    public int HWM { get; private set; }

    /// <summary>
    /// Publisher Object
    /// </summary>
    private PublisherSocket Publisher;

    /// <summary>
    /// Initializes an MQ Publisher Object.
    /// </summary>
    /// <param name="topic">Topic Name of communication</param>
    /// <param name="ip">Your IP Address</param>
    /// <param name="port">The port You want to publish in</param>
    /// <param name="hwm">High Water Mark of Publisher Socket</param>
    public MQPublisher(string topic, string ip, int port, int hwm = 1000)
    {
        this.Topic = topic;
        this.IP = ip;
        this.Port = port;
        this.HWM = hwm;
        Publisher = new PublisherSocket();
        Publisher.Options.SendHighWatermark = HWM;
        Publisher.Bind("tcp://" + IP + ":" + Port.ToString());
    }

    /// <summary>
    /// This Function will Publish the data you gave in.
    /// </summary>
    /// <param name="data">a bye array where your data is stored</param>
    public void Publish(byte[] data)
    {
        if (Publisher != null)
            Publisher.SendMoreFrame(Topic).SendFrame(data);
    }

    /// <summary>
    /// Stops MQ Publisher once and for all
    /// </summary>
    public void Stop()
    {
        if (Publisher != null)
        {
            Publisher.Unbind("tcp://" + IP + ":" + Port.ToString());
            Publisher.Close();
            Publisher.Dispose();
            Publisher = null;
        }
    }
}

