using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WCF_Tcp_Server
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    class WCFTcpServer : IWCFTcpServer, IDisposable
    {
        private static WCFTcpServer Instance = null;

        public static WCFTcpServer GetInstance()
        {
            if (Instance == null)
            {
                Instance = new WCFTcpServer();
            }
            return Instance;
        }

        ////클라이언트들을 모을 배열이다.
        private WCFTcpServer()
        {
            Instance = this;
        }

        //접속
        //클라이언트들이 접속되자 마자 실행하게될 메소드
        //접속한 클라이언트 정보를 받아 클라이언트 배열에 넣는다.&#
        public void StartService()
        {
            OperationContext ctx = OperationContext.Current;
            IWCFTcpClientCallback client = ctx.GetCallbackChannel<IWCFTcpClientCallback>();
            Common.clients.Add(client);

            //클라이언트 들은 IClientChannel로 타입캐스팅을 하여
            //각종 정보를 얻는다
            IClientChannel channel = client as IClientChannel;
            WCFTcpServerForm.AddClient(channel);
        }

        public void StopService()
        {
            OperationContext ctx = OperationContext.Current;
            IWCFTcpClientCallback client = ctx.GetCallbackChannel<IWCFTcpClientCallback>();

            for (int i = 0; i < Common.clients.Count; i++)
            {
                if (client == Common.clients[i])
                {
                    Common.clients.RemoveAt(i);
                    break;
                }
            }
        }

        private void BrodCastMessage(WCFMessageKind messageKind, object data)
        {
            foreach (IWCFTcpClientCallback client in Common.clients)
            {
                //클라이언트 들은 IClientChannel로 타입캐스팅을 하여
                //상태 체크를 한다. 그중 접속이 꺼지거나,
                //전송을 할수 없는 상황을 체크하여 활성 클라이언트만 골라낼수 있다
                // cf)channel.State
                client.SetDataToClient(messageKind, data);
            }
        }

        //서버가 클라이언트들에게 전송
        public void brodcast(string message)
        {
            BrodCastMessage(WCFMessageKind.Message, message);

            //foreach (IWCFClientCallback client in Common.clients)
            //{
            //    //클라이언트 들은 IClientChannel로 타입캐스팅을 하여
            //    //상태 체크를 한다. 그중 접속이 꺼지거나,
            //    //전송을 할수 없는 상황을 체크하여 활성 클라이언트만 골라낼수 있다
            //    // cf)channel.State
            //    IClientChannel channel = client as IClientChannel;

            //    client.SetDataToClient(WCFMessageKind.Message, message);
            //}
        }

        public void Dispose()
        {
            BrodCastMessage(WCFMessageKind.State, CommunicationState.Closed);
        }

        public void SetDataToServer(string someStr)
        {
            Console.WriteLine(someStr);
        }
    }
}
