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

        public WCFTcpServerForm()
        {
            InitializeComponent();

            MainForm = this;

            OpenService();
        }

        public static void AddClient(IClientChannel client)
        {
            MainForm.Invoke(new MethodInvoker(delegate ()
            {
                MainForm.listClient.Items.Add($"info : client Add [{client.SessionId}]");
            }));
        }

        private void OpenService()
        {
            /* Tcp 바인딩을 선언한다. */
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            binding.ReliableSession.Enabled = false;

            /* ServiceMetadataBehavior 를 선언하여 서비스 참조를 할수있도록 한다 */
            ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
            behavior.HttpGetEnabled = true;

            //WCF host 생성//
            serviceHost = new ServiceHost(
                        typeof(WCFTcpServer),
                        new Uri("http://150.1.13.166:1907/WCFTcpServer"),   // 서비스 참조 추가시 사용
                        new Uri("net.tcp://150.1.13.166/WCFTcpServer"));

            serviceHost.AddServiceEndpoint(typeof(IWCFTcpServer), binding, "");

            //behavior 를 등록한다;
            serviceHost.Description.Behaviors.Add(behavior);
            serviceHost.Open();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            WCFTcpServer.GetInstance().brodcast(textBoxMessage.Text);
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

            OpenService();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            switch (serviceHost.State)
            {
                case CommunicationState.Opened:
                    serviceHost.Close(new TimeSpan(0, 0, 0, 0, 10));
                    return;
            }
        }

        private void WCFTcpServerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            serviceHost.Close(new TimeSpan(0, 0, 0, 0, 10));
        }
    }
}
