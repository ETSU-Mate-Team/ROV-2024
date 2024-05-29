using Gamepad;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Text;
using CalypsoGUI.Shared;

namespace CalypsoGUI.Data
{
    public class Controller
    {
        public static int[] buttons = new int[12];
        public static int[] axes = new int[6];
        public static double Limiter = 1.5;
        public static string outputData = "";
        public static string thrusterData = "";
        public static string buttonData = "";
        public static bool sendSignal = false;
        public static bool HoverMode = false;
        public static bool PrecisionMode = false;
        public static bool TurboMode = false;
        public static bool ClawState = false;
        public static bool WarningState = false;
        public static int MainCamera = 0;
        public static void ControllerStart()
        {
            getButtonData();
            Thread socketStuff = new Thread(startServer);
            //socketStuff.Start();
            while (true)
            {
                //getData();
                if (Console.ReadLine() == "0") StartProcedure();
            }
        }
        public static void StartProcedure()
        {
            Thread.Sleep(3000);
            TurboMode = false;
            PrecisionMode = false;
            HoverMode = false;
            ClawState = false;
            WarningState = false;
            ClawState = true;
            Thread.Sleep(100);
            TurboMode = true;
            Thread.Sleep(100);
            PrecisionMode = true;
            Thread.Sleep(100);
            HoverMode = true;
            Thread.Sleep(100);
            WarningState = true;
            Thread.Sleep(5000);
            ClawState = false;
            Thread.Sleep(100);
            TurboMode = false;
            Thread.Sleep(100);
            PrecisionMode = false;
            Thread.Sleep(100);
            HoverMode = false;
            Thread.Sleep(100);
            WarningState = false;
            Sensors.Pressure = 20;
            Sensors.Temperature = 20;
            Sensors.Depth = 20;
        }
        public static void getData()
        {
            using (var gamepad = new GamepadController("/dev/input/js0"))
            {
                // Configure this if you want to get events when the state of a button changes
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
                gamepad.ButtonChanged += (object sender, ButtonEventArgs e) =>
                {
                    //Console.WriteLine($"Button {e.Button} Pressed: {e.Pressed}");
                    if (e.Pressed) buttons[e.Button] = 1;
                    if (!e.Pressed) buttons[e.Button] = 0;
                    getButtonData();

                };
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

                // Configure this if you want to get events when the state of an axis changes
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
                gamepad.AxisChanged += (object sender, AxisEventArgs e) =>
                {
                    //Console.WriteLine($"Axis {e.Axis} Pressed: {e.Value}");
                    axes[e.Axis] = e.Value;
                    testSamuelTakeTwo();
                    //vectorData();
                };
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

                Console.ReadLine();
            }
        }

        public static void testSamuelTakeTwo()
        {
            int[] ForwardFull = { 400, 400, -400, -400, 400, 400, -400, -400 };
            int[] BackwardFull = { 400, 400, -400, -400, 400, 400, -400, -400 };
            int[] LeftFull = { -400, 400, -400, 400, -400, 400, -400, 400 };
            int[] RightFull = { -400, 400, -400, 400, -400, 400, -400, 400 };
            int[] UpFull = { -400, -400, -400, -400, 400, 400, 400, 400 };
            int[] DownFull = { -400, -400, -400, -400, 400, 400, 400, 400 };
            int[] TurnLeftFull = { 400, 0, 0, 400, 400, 0, 0, 400 };
            int[] TurnRightFull = { 0, 400, 400, 0, 0, 400, 400, 0 };
            double[] ThrusterVectors = { 0, 0, 0, 0, 0, 0, 0, 0, };
            int[] ThrusterFinalAmount = { 1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500 };

            double JoystickAmount = 0;
            if (axes[0] > 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    ThrusterVectors[i] += RightFull[i] * Math.Round((axes[0] / 32767.5), 4);
                }
                JoystickAmount++;
                Console.WriteLine("right");
            }
            if (axes[0] < 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    ThrusterVectors[i] += LeftFull[i] * Math.Round((axes[0] / 32767.5), 4) * -1;
                }
                JoystickAmount++;
                Console.WriteLine("left");
            }
            if (axes[1] > 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    ThrusterVectors[i] += BackwardFull[i] * Math.Round((axes[1] / 32767.5), 4);
                }
                JoystickAmount++;
                Console.WriteLine("backward");
            }
            if (axes[1] < 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    ThrusterVectors[i] += ForwardFull[i] * Math.Round((axes[1] / 32767.5), 4);
                }
                JoystickAmount++;
                Console.WriteLine("forward");
            }

            if (axes[2] > 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    ThrusterVectors[i] += TurnRightFull[i] * Math.Round((axes[2] / 32767.5), 4);
                }
                JoystickAmount++;
                Console.WriteLine("turnright");
            }
            if (axes[2] < 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    ThrusterVectors[i] += TurnLeftFull[i] * Math.Round((axes[2] / 32767.5), 4) * -1;
                }
                JoystickAmount++;
                Console.WriteLine("turnleft");
            }
            if (axes[3] > 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    ThrusterVectors[i] += DownFull[i] * Math.Round((axes[3] / 32767.5), 4);
                }
                JoystickAmount++;
                Console.WriteLine("down");
            }
            if (axes[3] < 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    ThrusterVectors[i] += UpFull[i] * Math.Round((axes[3] / 32767.5), 4);
                }
                JoystickAmount++;
                Console.WriteLine("up");
            }
            for (int i = 0; i < 8; i++)
            {
                if (JoystickAmount != 0) ThrusterFinalAmount[i] += Convert.ToInt32((ThrusterVectors[i] / JoystickAmount) / Limiter);
            }
            thrusterData = "start," + ThrusterFinalAmount[0] + "," +
                ThrusterFinalAmount[1] + "," + ThrusterFinalAmount[2] + "," +
                ThrusterFinalAmount[3] + "," + ThrusterFinalAmount[4] + "," +
                ThrusterFinalAmount[5] + "," + ThrusterFinalAmount[6] + "," +
                ThrusterFinalAmount[7];
            //Console.WriteLine(thrusterData);
            combineStrings();
        }

        /// <summary>
        /// front top right 1
        /// front top left 2
        /// front bottom right 3
        /// front bottom left 4
        /// back top right 5
        /// back top left 6
        /// back bottom right 7
        /// back bottom left 8
        /// </summary>

        public static void getButtonData()
        {
            if (buttons[2] == 1)        {
                if (Limiter == 2.5) Limiter = 1.5;
                else if (Limiter == 1.5) Limiter = 2.5;
            }
            if (buttons[3] == 1)        {
                Limiter = 1.5;
            }
            buttonData = "," + buttons[0] + "," +
                buttons[1] + "," + buttons[2] + "," +
                buttons[3] + "," + buttons[4] + "," +
                buttons[5] + "," + buttons[6] + "," +
                buttons[7] + "," + buttons[8] + "," +
                buttons[9] + "," + buttons[10] + "," +
                buttons[11] + ",";
            combineStrings();
        }
        public static void combineStrings()
        {
            outputData = thrusterData + buttonData;
            Console.WriteLine(outputData);
            sendSignal = true;
        }
        public static void startServer()
        {
            IPAddress ipAddr = IPAddress.Parse("169.254.1.6");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 10000);
            Socket sender = new Socket(ipAddr.AddressFamily,
                       SocketType.Stream, ProtocolType.Tcp);

            sender.Connect(localEndPoint);
            while (true)
            {
                if (sendSignal)
                {
                    byte[] msg = Encoding.ASCII.GetBytes(outputData);
                    int byteSent = sender.Send(msg);
                    sendSignal = false;
                }
            }

        }
        /*
        public static void rumbleController()
        {
            FileStream rumbleMotor = new FileStream(@"/dev/input/event0",
                     FileMode.Open, FileAccess.ReadWrite, FileShare.None);



            rumbleMotor.Close();
        }
        */
    }

}
