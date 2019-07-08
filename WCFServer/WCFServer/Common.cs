using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCF_Tcp_Server
{
    public enum WCFMessageKind
    {
        State,
        Message,
        InsertClient,
        RemoveClient,
    }

    public class Common
    {
        //클라이언트들을 모을 배열이다.
        public static List<IWCFTcpClientCallback> clients = new List<IWCFTcpClientCallback>();
    }
}
