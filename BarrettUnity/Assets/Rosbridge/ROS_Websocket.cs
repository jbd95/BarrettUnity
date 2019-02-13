using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WebSocketSharp;
using SimpleJSON;
using System.IO;
using System.Text.RegularExpressions;

/*
 * This script uses two third party plugins: WebSocketSharp and SimpleJSON
 */

public class ROS_Websocket : MonoBehaviour
{

    public GameManager manager;
    public string Message_OP;
    public string Message_Topic;
    public string Message_Type;
    public Forward_Kinematics fk;

    ROS_Message send_message;
    AdvertiseMessage advertise_message;
    ROS_Out_Message message_back;
    ROS_Message shieldvr_subscribe;
    ROS_Out_Message bucket_coordinates;
    AdvertiseMessage bucket_advertise;

    public RotateBody body;
    public RotateJoint2 joint2;
    public RotateJoint3 joint3;

    public string IP;
    public string PORT;

    float RotationA = 0, RotationB = 0, RotationC = 0, RotationD = 0;
    float resizePending = -1;

    float j1 = 0, j2 = 0, j3 = 0, j4 = 0;

    WebSocket ws;
    Vector3 new_pos = new Vector3();
    Quaternion new_rotation = new Quaternion();

    bool SendCommand = false;
    bool InitializeCommand = false;
    float Speed = -1;
    string table_data = "";
    int turn_count = -1;
    int show_grid = -1;
    bool has_update;
    string has_simulation = "";

    List<string> RequestedDestinations = new List<string>();

    // Use this for initialization
    void Start()
    {

        advertise_message = new AdvertiseMessage();
        advertise_message.op = "advertise";
        //advertise_message.id = "s1h2ie3l5d";
        advertise_message.topic = "/ShieldVRComm";
        advertise_message.type = "std_msgs/String";

        shieldvr_subscribe = new ROS_Message();
        shieldvr_subscribe.op = "subscribe";
        shieldvr_subscribe.topic = "/ShieldVRComm";
        shieldvr_subscribe.type = "std_msgs/String";

        bucket_advertise = new AdvertiseMessage();
        bucket_advertise.op = "advertise";
        //bucket_advertise.id = "shBucketvr1";
        bucket_advertise.topic = "/ShieldVRCoordinates";
        bucket_advertise.type = "std_msgs/String";

        bucket_coordinates = new ROS_Out_Message();
        bucket_coordinates.op = "publish";
        bucket_coordinates.topic = "/ShieldVRCoordinates";

        message_back = new ROS_Out_Message();
        message_back.op = "publish";
        message_back.topic = "/ShieldVRComm";

        //creates a new message to be sent to ROS
        send_message = new ROS_Message();

        //set the op, topic, and type parameters (as detailed in the ROS message protocol)
        send_message.op = Message_OP;
        send_message.topic = Message_Topic;
        send_message.type = Message_Type;


        //use WebSocketSharp to connect to the rosbridge
        ws = new WebSocket("ws://" + IP + ":" + PORT);
        ws.Connect();

        if (ws.IsAlive)
        {
            SendConnectionMessages();
        }
        else
        {
            StartCoroutine(Connect());
        }
        //call a method whenever a message comes from ROS
        ws.OnMessage += (sender, e) => DecodeMessage(JSON.Parse(e.Data));
    }

    IEnumerator Connect()
    {
        while(!ws.IsAlive)
        {
            ws.ConnectAsync();
            yield return new WaitForSeconds(5f);
        }
        SendConnectionMessages();
    }

    private void SendConnectionMessages()
    {
        //send the message in JSON format through the websocket
        ws.Send(JsonUtility.ToJson(send_message));
        ws.Send(JsonUtility.ToJson(advertise_message));
        ws.Send(JsonUtility.ToJson(shieldvr_subscribe));
        ws.Send(JsonUtility.ToJson(bucket_advertise));
    }

    void Update()
    {
        /* body.RotationA = RotationA;
         body.CallRotateA();

         joint2.RotationB = RotationB;
         joint2.RotationD = RotationD;
         joint2.CallRotateB();

         joint3.RotationC = RotationC;
         joint3.CallRotateC();*/

        //if (!manager.controller.AI.IsActive)
        //{
            fk.Move(j1, j2, j3, j4);
        //}
        if (has_update)
        {

            if (SendCommand)
            {
                manager.BeginEpisode(RequestedDestinations);
                SendCommand = false;
                RequestedDestinations = new List<string>();
            }

            if (InitializeCommand)
            {
                manager.controller.Initialize();
                InitializeCommand = false;
            }

            if (resizePending != -1)
            {
                manager.ResizeShield(resizePending);
                resizePending = -1;
            }

            if (Speed != -1)
            {
                manager.controller.ReceivedSpeedChange(Speed);
                Speed = -1;
            }

            if (table_data != "")
            {
                ActionTable<int> table = new ActionTable<int>(manager.controller.XValues.Count - 1, manager.controller.YValues.Count - 1, _zero:0, _one:1, _initializer: table_data);
                manager.BeginEpisode(table);
                table_data = "";
            }

            if (turn_count != -1)
            {
                manager.TurnCount = turn_count;
                turn_count = -1;
            }
            if(show_grid != -1)
            {
                if(show_grid == 1)
                {
                    manager.controller.SetGridVisibility(true);
                }
                else
                {
                    manager.controller.SetGridVisibility(false);
                }
                show_grid = -1;
            }

            if(has_simulation != "")
            {
                string temp = has_simulation.Replace('(', ' ');
                temp = temp.Replace(')', ' ');
                temp = temp.Trim();
                SendScore("resultsimulation:" + has_simulation + ":" + manager.RL.RequestSimulationData(manager, int.Parse(temp.Split(',')[0]), int.Parse(temp.Split(',')[1])));
                //Debug.Log(manager.RL.RequestSimulationData(manager, int.Parse(temp.Split(',')[0]), int.Parse(temp.Split(',')[1])));
                has_simulation = "";
            }

            has_update = false;
        }
    }

    public void SendRLValue(string value)
    {
        if (ws.IsAlive)
        {
            bucket_coordinates.msg = new Message(value);
            ws.Send(JsonUtility.ToJson(bucket_coordinates));
        }
    }

    public void SendScore(string score)
    {
        message_back.msg = new Message(score);
        if (ws.IsAlive)
        {
            ws.Send(JsonUtility.ToJson(message_back));
        }
    }

    public void SendGridInformation(int x, int y)
    {
        message_back = new ROS_Out_Message();
        message_back.op = "publish";
        message_back.topic = "/ShieldVRComm";
        message_back.msg = new Message("<" + x + "," + y + ">");

        if (ws.IsAlive)
        {
            ws.Send(JsonUtility.ToJson(message_back));
        }
    }

    public void SendClearGraph(int xlim, int ylim)
    {
        if (ws.IsAlive)
        {
            bucket_coordinates.msg = new Message("start:" + xlim + ":" + ylim);
            ws.Send(JsonUtility.ToJson(bucket_coordinates));
        }
    }

    public void SendBucketCoordinates(GameManager.ProjectileInfo projectile, double elapsed_time, Pair user_pos)
    {
        if(projectile == null)
        {
            return;
        }
        if(!projectile.Position.IsValid())
        {
            return;
        }
        bucket_coordinates.msg = new Message("|" + projectile.ID + "|" + ((int)(user_pos.x)) + "|" + ((int)user_pos.y) + "|" + projectile.IsCaught.ToString() + "|" + ((int)projectile.Position.x) + "|" + ((int)projectile.Position.y) + "|" + elapsed_time.ToString() + "|");
        if (ws.IsAlive && (("|" + projectile.ID + "|" + user_pos.x + "|" + user_pos.y + "|" + projectile.IsCaught.ToString() + "|" + projectile.Position.x + "|" + projectile.Position.y + "|") != projectile.LastSubmitted))
        {
            ws.Send(JsonUtility.ToJson(bucket_coordinates));
            projectile.LastSubmitted = "|" + projectile.ID + "|" + user_pos.x + "|" + user_pos.y + "|" + projectile.IsCaught.ToString() + "|" + projectile.Position.x + "|" + projectile.Position.y + "|";
        }
    }

    public IEnumerator Restart()
    {
        //unsubscribe from the ROS topic
        send_message.op = "unsubscribe";
        shieldvr_subscribe.op = "unsubscribe";
        //message_back.msg.data = "quit";
        message_back.msg = new Message("game closed");
        advertise_message.op = "unadvertise";
        bucket_advertise.op = "unadvertise";
        bucket_coordinates.op = "unsubscribe";
        bucket_coordinates.msg = new Message("game closed");

        if (ws.IsAlive)
        {
            ws.Send(JsonUtility.ToJson(send_message));
            ws.Send(JsonUtility.ToJson(message_back));
            ws.Send(JsonUtility.ToJson(bucket_coordinates));
            ws.Send(JsonUtility.ToJson(shieldvr_subscribe));
            ws.Send(JsonUtility.ToJson(advertise_message));
            ws.Send(JsonUtility.ToJson(bucket_advertise));
        }
        //close the websocket whenever the application closes
        ws.Close();

        yield return new WaitForSeconds(3f);

        Start();
    }

    void OnApplicationQuit()
    {

        //unsubscribe from the ROS topic
        send_message.op = "unsubscribe";
        shieldvr_subscribe.op = "unsubscribe";
        //message_back.msg.data = "quit";
        message_back.msg = new Message("game closed");
        advertise_message.op = "unadvertise";
        bucket_advertise.op = "unadvertise";
        //bucket_coordinates.op = "unsubscribe";
        bucket_coordinates.msg = new Message("game closed");

        if (ws.IsAlive)
        {
            ws.Send(JsonUtility.ToJson(send_message));
            ws.Send(JsonUtility.ToJson(message_back));
            ws.Send(JsonUtility.ToJson(bucket_coordinates));
            ws.Send(JsonUtility.ToJson(shieldvr_subscribe));
            ws.Send(JsonUtility.ToJson(advertise_message));
            ws.Send(JsonUtility.ToJson(bucket_advertise));
        }
        //close the websocket whenever the application closes
        ws.Close();
    }

    /// <summary>
    /// Turns the JSON message recieved from ROS into usable data.
    /// </summary>
    /// <param name="message">Message.</param>
    void DecodeMessage(JSONNode message)
    {
        if (message["topic"] == "/wam/joint_states")
        {
            j1 = message["msg"]["position"][0];
            j2 = message["msg"]["position"][1];
            j3 = message["msg"]["position"][2];
            j4 = message["msg"]["position"][3];


            //negative one inverts the motion
            RotationA = (-1) * ((float)((message["msg"]["position"][0]) / 0.018));

            RotationB = /* (-1) * */((float)((message["msg"]["position"][1]) / 0.018181818));

            RotationC = /* (-1) * */((float)((message["msg"]["position"][3]) / 0.0175));

            RotationD = (-1) * ((float)((message["msg"]["position"][2]) / 0.01866667));

        }
        else if (message["topic"] == "/ShieldVRComm")
        {
            string data = message["msg"]["data"];

            if (data == "Start Episode")
            {
                SendCommand = true;
                has_update = true;
                return;
            }

            if(data == "Initialize")
            {
                InitializeCommand = true;
                has_update = true;
                return;
            }

            if(data.Contains("speed:"))
            {
                Speed = float.Parse(data.Split(':')[1]);
                has_update = true;
                return;
            }

            if(data.Contains("actiontable"))
            {
                table_data = data.Split('{')[1].Split('}')[0];
                has_update = true;
                return;
            }

            if(data.Contains("turns:"))
            {
                turn_count = int.Parse(data.Split(':')[1]);
                has_update = true;
                return;
            }

            if(data.Contains("showgrid:"))
            {
                show_grid = int.Parse(data.Split(':')[1]);
                has_update = true;
                return;
            }

            if (data.Contains("simulation:") && !data.Contains("result"))
            {
                has_update = true;
                has_simulation = data.Split(':')[1];
                return;
            }

            MatchCollection coordinates_regex = Regex.Matches(data, @"\([0-9]+,[0-9]+\)");

            foreach (Match match in coordinates_regex)
            {
                if (match.Value == data)
                {
                    BoxCoordinatesRecieved(match.Value);
                    return;
                }
            }

            MatchCollection resizeshield_regex = Regex.Matches(data, @"\#[0-9]+\.[0-9]+\#");

            foreach (Match match in resizeshield_regex)
            {
                if (match.Value == data)
                {
                    string final = match.Value;
                    final = final.Replace("#", "");
                    resizePending = float.Parse(final);
                    has_update = true;
                    return;
                }
            }

        }

        //Optionally output the whole message to make sure everything is working
        //Debug.Log(message.ToString());
    }

    private void BoxCoordinatesRecieved(string msg)
    {
        RequestedDestinations.Add(msg);
       /* SendCommand = true;
        manager.Begin();*/
    }

    /// <summary>
    /// Class that stores a message sent to the rosbridge
    /// </summary>
    [Serializable]
    public class ROS_Message
    {
        public string op;
        public string topic;
        public string type;
    }

    [Serializable]
    public class ROS_Out_Message
    {
                public string op;
        public string topic;
        // public string type;
        public Message msg;
    }
    [Serializable]
    public class Message
    {
        public string data;
        public Message(string _data)
        {
            data = _data;
        }
    }

    [Serializable]
    public class AdvertiseMessage
    {
        public string op;
        //public string id;
        public string topic;
        public string type;
    }
}
