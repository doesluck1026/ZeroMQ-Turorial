using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NETMQ_Tutorial
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int UpdateFrequency = 30;   /// Hz
        private double UpdatePeriod;

        private Thread UI_UpdateThread;
        private bool IsUpdateEnabled = false;
        public MainWindow()
        {
            InitializeComponent();
            UpdatePeriod = 1.0 / UpdateFrequency;
        }

        private void Btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            if(!Main.IsSubscriberEnabled)
            {
                string ip = Txt_IP.Text;
                if(!string.IsNullOrEmpty(ip))
                {
                    Main.StartReceiving(ip);
                }
                Btn_Connect.Content = "Disconnect";
                StartUIThread();
            }
            else
            {
                Main.StopReceiving();
                Btn_Connect.Content = "Connect";
                StopUIThread();
            }
        }

        private void Btn_Share_Click(object sender, RoutedEventArgs e)
        {
            if (!Main.IsPublisherEnabled)
            {
                Main.StartSharing();
                Btn_Share.Content = "Stop";
                StartUIThread();
            }
            else
            {
                Main.StopSharing();
                Btn_Share.Content = "Share";
                StopUIThread();
            }
        }

        private void StartUIThread()
        {
            UI_UpdateThread = new Thread(UIUpdateCoreFunction);
            UI_UpdateThread.Start();
            IsUpdateEnabled = true;
        }
        private void StopUIThread()
        {
            try
            {
                IsUpdateEnabled = false;
                if(UI_UpdateThread!=null)
                {
                    if (UI_UpdateThread.IsAlive)
                        UI_UpdateThread.Abort();
                    UI_UpdateThread = null;
                }
            }
            catch
            {
               Debug.WriteLine("Failed to Stop Update Thread");
            }
        }
        private void UIUpdateCoreFunction()
        {
            Stopwatch watch = Stopwatch.StartNew();
            while(IsUpdateEnabled)
            {
                if(Main.IsNewDataReceived)
                {
                    var image = Main.GetReceivedImage();
                    if (image != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                           ImageBox.Source = ImageProcessing.ToBitmapSource(image);
                        });
                    }
                    else
                        Debug.WriteLine("received image was null");
                }
                while (watch.Elapsed.TotalSeconds < UpdatePeriod)
                    Thread.Sleep(1);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopUIThread();
            Main.StopSharing();
            Main.StopReceiving();
        }
    }
}
