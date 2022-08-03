using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Server : MonoBehaviour
{
    public static Server instance;

    private Player player;
    private IPAddress ipAdr;
    private Socket clientSocket;

    private EndPoint serverEnd; // 服务端
    private byte[] recvData = new byte[64];
    private string recvStr;
    private int recvLen;
    private Thread receiveMessage;
    private bool isStart = false;
    public InputField Room;
    public InputField Site;
    public int site;

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
        public int room;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst =4)]
        public int unit;
    }

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start() {
        ConnectServer();
    }

    private void Update() {
        if(isStart) {
            isStart = false;
            SceneManager.LoadScene("2");
        }
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
            if(recvLen >= 16) {
                Debug.Log("message from:" + serverEnd.ToString());
                OtherPlayer op = new OtherPlayer();
                op = BytesToStruct(recvData, op.GetType());
                Debug.Log(op.x + "__" + op.z + "__" + op.room + "__" + op.unit);
                
                
                if(op.x == 0f && op.z == 0f && op.room == 0 && op.unit == 0) {  // 启动游戏
                    isStart = true;
                }
                else if(op.room != 0 && op.unit != 0) { // 进入房间
                    // 想法是进入房间后根据房间号赋予一个site值
                    // 然后根据site值分配该客户端所能控制的对象


                }
                else if(op.room == -1) {   // 退出房间
                    
                }
                else if(op.x != 0 && op.z != 0 && op.room == 0 && op.unit != 0) {   // 移动单位
                    Debug.Log("receive move message");
                    messageList.Add(new Message() {x = op.x, z = op.z, unit = op.unit});
                }
            }
            else {

            }

        }
    }

    #region 发送

    public void SendGameStart() {

        OtherPlayer op = new OtherPlayer();

        op.x = 0f;
        op.z = 0f;
        op.room = 0;
        op.unit = 0;

        clientSocket.Send(StructToByte(op));
    }

    public void SendEnterRoom() {

        OtherPlayer op = new OtherPlayer();
        op.x = 0f;
        op.z = 0;
        op.room = int.Parse(Room.text);
        op.unit = int.Parse(Site.text);

        clientSocket.Send(StructToByte(op));
    }

    public void SendQuitRoom() {

        OtherPlayer op = new OtherPlayer();
        op.x = 0f;
        op.z = 0f;
        op.room = -1;
        op.unit = 0;

        clientSocket.Send(StructToByte(op));
    }

    public void SendPosTest() {
        SendPos(1.1f, 2.2f, 1);
    }

    public void SendPos(float x, float z, int unit) {
        
        OtherPlayer op = new OtherPlayer();

        op.x = x;
        op.z = z;
        op.room = 0;
        op.unit = unit;

        clientSocket.Send(StructToByte(op));
    }

    #endregion


    #region 转换

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


    #endregion
}
