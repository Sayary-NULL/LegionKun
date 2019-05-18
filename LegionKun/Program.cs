using System;
using LegionKun.Module;

/*461284473799966730 - шароновский легион
 *423154703354822668 - [Legion Sharon'a]
 *435485527156981770 - Disboard*/

/*?) Доделать игру крестики-нолики
 *1) Переработать вывод бота
 *2) идея о общем классе наследнике для всех Command*/

namespace LegionKun
{
    public class Program : Module.MainClass
    {
        static void Main(string[] args)
        {
            ConstVariables.SetDelegate(new Program().Messege);

            ConstVariables.InstallationLists();

            new Program().MainTime();

            new Program().Youtube();
            do
            {
                try
                {
                    ConstVariables.LegionDiscordThread.Start();
                }
                catch (Exception e)
                {
                    ConstVariables.logger.Error(e.Message);
                    break;
                }
               
            } while (true);

            Console.ReadKey();
        } 
    }
}