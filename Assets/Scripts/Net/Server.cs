using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;


public class Server : MonoBehaviour
{
    private Player player;
    private IPAddress ipAdr;
    private Socket clientSocket;

    private EndPoint serverEnd; // 服务端
    private byte[] recvData = new byte[64];
    private string recvStr;
    private int recvLen;
    private Thread receiveMessage;

    
    public List<Message> messageList = new List<Message>(); // 接受的消息
    public struct Message {
        public float x;
        public float z;
        public int unit;
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct OtherPlayer {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =4)]
        public float x;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =4)]
        public float z;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =4)]
        public int root;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =4)]
        public int unit;
    }

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        ConnectServer();
    }

    private void Update() {
        
    }

    // 连接服务器
    private void ConnectServer() {
        ipAdr = IPAddress.Loopback;

        IPEndPoint ipEP = new IPEndPoint(ipAdr, 7777);

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        clientSocket.Connect(ipEP);

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        serverEnd = sender;

        receiveMessage = new Thread(ReceiveMessage);
        receiveMessage.Start();
    }

    private void OnDestroy() {
        CloseClient();
    }

    //关闭连接
    private void CloseClient() {
        if(receiveMessage != null) {
            receiveMessage.Interrupt();
            receiveMessage.Abort();
        }
        if(clientSocket != null) {
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }

    // 接受服务器信息并保存在messageList中
    private void ReceiveMessage() {
        while (true) {
            recvData = new byte[64];
            recvLen = clientSocket.ReceiveFrom(recvData, ref serverEnd);

            Debug.Log(recvLen);

            // 处理接收到的数据
            if(recvLen == 64) {
                Debug.Log("message from:" + serverEnd.ToString());
                OtherPlayer op = new OtherPlayer();
                op = BytesToStruct(recvData, op.GetType());
                Debug.Log(op.x + "__" + op.z + "__" + op.root + "__" + op.unit);
                if(op.x == 0f && op.z == 0f && op.root == 0 && op.unit == 0) {

                }
                else {
                    messageList.Add(new Message() {x = op.x, z = op.z, unit = op.unit});
                }
            }
            else if(recvLen == 30) {

            }

        }
    }

    public void SendTest() {
        SendPos(1, 1, 1);
    }

    public void SendPos(float x, float z, int unit) {
        
        OtherPlayer op = new OtherPlayer();

        op.x = x;
        op.z = z;
        op.root = 0;
        op.unit = unit;

        clientSocket.Send(StructToByte(op));
    }

    private OtherPlayer BytesToStruct(byte[] bytes, Type structType) {
        int size = Marshal.SizeOf(structType);
        IntPtr buffer = Marshal.AllocHGlobal(size);
        try {
            Marshal.Copy(bytes, 0, buffer, size);
            return (OtherPlayer)Marshal.PtrToStructure(buffer, structType);
        }
        finally {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private byte[] StructToByte(OtherPlayer op) {
        int size = Marshal.SizeOf(op);
        IntPtr buffer = Marshal.AllocHGlobal(size);

        try {
            Marshal.StructureToPtr(op, buffer, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(buffer, bytes, 0, size);
            return bytes;
        }
        finally {
            Marshal.FreeHGlobal(buffer);
        }
    }

}
