using System;
using System.Security.Permissions;
using LegionKun.Module;

/*461284473799966730 - шароновский легион
 *423154703354822668 - [Legion Sharon'a]
 *435485527156981770 - Disboard*/

/*1) Переработать вывод бота
 *2) идея о общем классе наследнике для всех Command*/

namespace LegionKun
{
    public class Program : Module.MainClass
    {
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main(string[] args)
        {
            ConstVariables.SetDelegate(new Program().Messege);

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
#if !DEBUG
            if (args.Length != 0)
                ConstVariables.Patch = args[0];
#endif

            if (ConstVariables.InstallationLists())
            {
                ConstVariables.Logger.Info("Запуск программы");
#if !DEBUG
                new Program().Youtube();

                new Program().Twitch();
#endif

                try
                {
                    ConstVariables.LegionDiscordThread.Start();
                }
                catch (Exception e)
                {
                    ConstVariables.Logger.Error(e.Message);
                }
            }

            Console.ReadKey();
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            ConstVariables.Logger.Error("MyHandler caught : " + e.Message);
            ConstVariables.Logger.Error("Runtime terminating: {0}", args.IsTerminating);
        }
    }
}