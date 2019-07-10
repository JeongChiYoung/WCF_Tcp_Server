using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace WCF_Tcp_Server
{
    public partial class WCFTcpServerForm : Form
    {
        public ServiceHost serviceHost = null;
        static WCFTcpServerForm MainForm = null;
        int nTimer = 0;

        public WCFTcpServerForm()
        {
            InitializeComponent();

            MainForm = this;

            OpenServiceConfiguration();
        }

        public static void AddClient(IClientChannel client)
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.listClient.Items.Add($"info : client Add [{client.SessionId}]");
            }));
        }

        private void OpenServiceConfiguration()
        {
            //WCF host 생성//
            serviceHost = new ServiceHost(typeof(WCFTcpServer));

            serviceHost.Faulted += HostFaulted;
            serviceHost.Closing += HostClosing;
            serviceHost.Closed += HostClosed;
            serviceHost.Opened += HostOpened;
            serviceHost.Opening += HostOpening;

            //behavior 를 등록한다;
            ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
            behavior.HttpGetEnabled = true;
            //serviceHost.Description.Behaviors.Add(behavior);

            serviceHost.Open(/*new TimeSpan(0, 0, 0, 10)*/);
        }

        private void OpenService()
        {
            /* Tcp 바인딩을 선언한다. */
            NetTcpBinding tcpBinding = new NetTcpBinding();
            tcpBinding.Security.Mode = SecurityMode.None;
            tcpBinding.ReliableSession.Enabled = false;

            /* ServiceMetadataBehavior 를 선언하여 서비스 참조를 할수있도록 한다 */
            ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
            behavior.HttpGetEnabled = true;

            //WCF host 생성//
            serviceHost = new ServiceHost(
                        typeof(WCFTcpServer),
                        new Uri("http://150.1.13.166:1907/"),   // 서비스 참조 추가시 사용
                        new Uri("net.tcp://150.1.13.166/"));

            serviceHost.AddServiceEndpoint(typeof(IWCFTcpServer), tcpBinding, "WCFTcpServer");

            BasicHttpBinding httpBinding = new BasicHttpBinding();
            httpBinding.Security.Mode = BasicHttpSecurityMode.None;

            serviceHost.AddServiceEndpoint(typeof(IHelloWorld), httpBinding, "HelloService");

            serviceHost.Faulted += HostFaulted;
            serviceHost.Closing += HostClosing;
            serviceHost.Closed  += HostClosed;
            serviceHost.Opened  += HostOpened;
            serviceHost.Opening += HostOpening;

            //behavior 를 등록한다;
            serviceHost.Description.Behaviors.Add(behavior);
            serviceHost.Open(/*new TimeSpan(0, 0, 0, 10)*/);
        }

        public void HostFaulted(object sender, EventArgs e)
        {
            MainForm.listClient.Items.Add("change Host state : Faulted");
        }

        public void HostClosing(object sender, EventArgs e)
        {
            MainForm.listClient.Items.Add("change Host state : Closing");
        }

        public void HostClosed(object sender, EventArgs e)
        {
            MainForm.listClient.Items.Add("change Host state : Closed");
        }

        public void HostOpened(object sender, EventArgs e)
        {
            MainForm.listClient.Items.Add("change Host state : Opened");
        }

        public void HostOpening(object sender, EventArgs e)
        {
            MainForm.listClient.Items.Add("change Host state : Opening");
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            //WCFTcpServer.GetInstance().BrodCast(textBoxMessage.Text);
            WCFTcpServer.GetInstance().BrodCastMessage(WCFMessageKind.Message, textBoxMessage.Text);
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            switch (serviceHost.State)
            {
                case CommunicationState.Opened:
                case CommunicationState.Opening:
                case CommunicationState.Closing:
                    return;
            }

            OpenServiceConfiguration();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            switch (serviceHost.State)
            {
                case CommunicationState.Opened:
                    WCFTcpServer.GetInstance().BrodCastMessage(WCFMessageKind.State, CommunicationState.Closed.ToString());
                    serviceHost.Close(new TimeSpan(0, 0, 0, 0, 10));

                    timerPoll.Enabled = false;
                    return;
            }
        }

        private void WCFTcpServerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            WCFTcpServer.GetInstance().BrodCastMessage(WCFMessageKind.State, CommunicationState.Closed.ToString());
            serviceHost.Close(new TimeSpan(0, 0, 0, 0, 10));

            timerPoll.Enabled = false;
        }

        public static void StartTimer()
        {
            if (MainForm.timerPoll.Enabled == true)
            {
                return;
            }

            MainForm.nTimer = 0;
            MainForm.timerPoll.Enabled = true;
        }

        private void timerPoll_Tick(object sender, EventArgs e)
        {
            nTimer++;

            int nMin, nSec;

            nMin = nTimer / 60;
            nSec = (nTimer - (nMin * 60));
            labelTimer.Text = $"{nMin:00}:{nSec:00}";
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
