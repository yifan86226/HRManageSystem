using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Best.VWPlatform.Common.Net
{
    public static class SocketErrorMessage
    {
        private static readonly Dictionary<SocketError, string> MSocketErrorList = InitializeSocketErrorMessage();
        private static Dictionary<SocketError, string> InitializeSocketErrorMessage()
        {
            Dictionary<SocketError, string> errorMsg = new Dictionary<SocketError, string>();
            errorMsg[SocketError.AccessDenied] = "已试图通过被其访问权限禁止的方式访问 Socket。";
            errorMsg[SocketError.Success] = "Socket 操作成功。";
            errorMsg[SocketError.SocketError] = "发生了未指定的 Socket 错误。";
            errorMsg[SocketError.Interrupted] = "已取消阻止 Socket 调用的操作。";
            errorMsg[SocketError.Fault] = "基础套接字提供程序检测到无效的指针地址。";
            errorMsg[SocketError.InvalidArgument] = "给 Socket 成员提供了一个无效参数。";
            errorMsg[SocketError.TooManyOpenSockets] = "基础套接字提供程序中打开的套接字太多。";
            errorMsg[SocketError.WouldBlock] = "对非阻止性套接字的操作不能立即完成。";
            errorMsg[SocketError.InProgress] = "阻止操作正在进行中。";
            errorMsg[SocketError.AlreadyInProgress] = "非阻止性 Socket 已有一个操作正在进行中。";
            errorMsg[SocketError.NotSocket] = "对非套接字尝试 Socket 操作。";
            errorMsg[SocketError.DestinationAddressRequired] = "在对 Socket 的操作中省略了必需的地址。";
            errorMsg[SocketError.MessageSize] = "数据报太长。";
            errorMsg[SocketError.ProtocolType] = "此 Socket 的协议类型不正确。";
            errorMsg[SocketError.ProtocolOption] = "对 Socket 使用了未知、无效或不受支持的选项或级别。";
            errorMsg[SocketError.ProtocolNotSupported] = "未实现或未配置协议。";
            errorMsg[SocketError.SocketNotSupported] = "在此地址族中不存在对指定的套接字类型的支持。";
            errorMsg[SocketError.OperationNotSupported] = "协议族不支持地址族。";
            errorMsg[SocketError.ProtocolFamilyNotSupported] = "未实现或未配置协议族。";
            errorMsg[SocketError.AddressFamilyNotSupported] = "不支持指定的地址族。如果指定了 IPv6 地址族而未在本地计算机上安装 IPv6 堆栈，则会返回此错误。如果指定了 IPv4 地址族而未在本地计算机上安装 IPv4 堆栈，则会返回此错误。";
            errorMsg[SocketError.AddressAlreadyInUse] = "通常，只允许使用地址一次。";
            errorMsg[SocketError.AddressNotAvailable] = "选定的 IP 地址在此上下文中无效。";
            errorMsg[SocketError.NetworkDown] = "网络不可用。";
            errorMsg[SocketError.NetworkUnreachable] = "不存在到远程主机的路由。";
            errorMsg[SocketError.NetworkReset] = "应用程序试图在已超时的连接上设置选项。";
            errorMsg[SocketError.ConnectionAborted] = "此连接由 .NET Framework 或基础套接字提供程序中止。";
            errorMsg[SocketError.ConnectionReset] = "此连接由远程对等计算机重置。";
            errorMsg[SocketError.NoBufferSpaceAvailable] = "没有可用于 Socket 操作的可用缓冲区空间。";
            errorMsg[SocketError.IsConnected] = "Socket 已连接。";
            errorMsg[SocketError.NotConnected] = "应用程序试图发送或接收数据，但是 Socket 未连接。";
            errorMsg[SocketError.Shutdown] = "发送或接收数据的请求未得到允许，因为 Socket 已被关闭。";
            errorMsg[SocketError.TimedOut] = "连接尝试超时，或者连接的主机没有响应。";
            errorMsg[SocketError.ConnectionRefused] = "远程主机正在主动拒绝连接。";
            errorMsg[SocketError.HostDown] = "由于远程主机被关闭，操作失败。";
            errorMsg[SocketError.HostUnreachable] = "没有到指定主机的网络路由。";
            errorMsg[SocketError.ProcessLimit] = "正在使用基础套接字提供程序的进程过多。";
            errorMsg[SocketError.SystemNotReady] = "网络子系统不可用。";
            errorMsg[SocketError.VersionNotSupported] = "基础套接字提供程序的版本超出范围。";
            errorMsg[SocketError.NotInitialized] = "尚未初始化基础套接字提供程序。";
            errorMsg[SocketError.Disconnecting] = "正常关机正在进行中。";
            errorMsg[SocketError.TypeNotFound] = "未找到指定的类。";
            errorMsg[SocketError.HostNotFound] = "无法识别这种主机。该名称不是正式的主机名或别名。";
            errorMsg[SocketError.TryAgain] = "无法解析主机名。请稍后重试。";
            errorMsg[SocketError.NoRecovery] = "错误不可恢复或找不到请求的数据库。";
            errorMsg[SocketError.NoData] = "在名称服务器上找不到请求的名称或 IP 地址。";
            errorMsg[SocketError.IOPending] = "应用程序已启动一个无法立即完成的重叠操作。";
            errorMsg[SocketError.OperationAborted] = "由于 Socket 已关闭，重叠的操作被中止。";
            return errorMsg;
        }

        public static string GetMessage(SocketError pError)
        {
            if (!MSocketErrorList.ContainsKey(pError))
                return "未知错误！";
            return MSocketErrorList[pError];
        }
    }
}
