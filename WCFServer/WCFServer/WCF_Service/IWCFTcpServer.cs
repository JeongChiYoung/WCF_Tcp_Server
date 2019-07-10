using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WCF_Tcp_Server
{
    //콜백할 인터페이스 이다. 곧 Client 가 된다..
    [ServiceContract]
    public interface IWCFTcpClientCallback
    {
        [OperationContract]
        void SetDataToClient(WCFMessageKind kind, object data);
    }

    //WCF host에서 구현할  인터페이스 이다.
    //ServiceContract 에 콜백 인터페이스를 등록 하였다.
    [ServiceContract(CallbackContract = typeof(IWCFTcpClientCallback))]
    interface IWCFTcpServer
    {
        [OperationContract]
        void StartService();

        [OperationContract]
        void StopService();

        [OperationContract]
        void SetDataToServer(string someStr);
    }

    [ServiceContract]
    public interface IHelloWorldCallback
    {
        [OperationContract]
        void SetData(string messgae);
    }

    //[ServiceContract(CallbackContract = typeof(IHelloWorldCallback))]
    [ServiceContract]
    public interface IHelloWorld// : IHelloWorldCallback
    {
        [OperationContract]
        string SayHello();

        [OperationContract]
        void Join();
    }

    //public class HeloWorldWCFService : IHelloWorld
    //{
    //    public string SayHello()
    //    {
    //        return "Heelo WCF World!";
    //    }
    //}
}
