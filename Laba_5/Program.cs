using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

// Мисенев Александр БББО-08-20

namespace Laba_5
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 3;
            bool[] gFlag = { false, false, false };
            bool[] gIsAlive = { true, true, true };
            bool keyFlag = true;
            int threadCtr = 0;
            bool flag = true;

            void Start()
            {
                Queue<Thread> queue = new Queue<Thread>();

                Console.WriteLine("Создание 1 потока");

                Thread mod_1 = new Thread(m1);
                mod_1.Name = $"Поток 1";
                queue.Enqueue(mod_1);

                Thread.Sleep(1000);

                Console.WriteLine("Создание 2 потока");

                Thread mod_2 = new Thread(m2);
                mod_2.Name = $"Поток 2";
                queue.Enqueue(mod_2);

                Thread.Sleep(1000);

                Console.WriteLine("Создание 3 потока");

                Thread mod_3 = new Thread(m3);
                mod_3.Name = $"Поток 3";
                queue.Enqueue(mod_3);

                Console.WriteLine("\n<><><><><><><><><><>");
                Thread.Sleep(1000);

                while (true)
                {
                    gFlag[threadCtr] = false;

                    if (count == 0) break;

                    if (gIsAlive[threadCtr] == true)
                    {
                        Thread firstThread = queue.Dequeue();
                        DateTime Start = DateTime.Now;

                        try
                        {
                            firstThread.Start();
                        }
                        catch
                        {
                            gFlag[threadCtr] = false;
                        }

                        keyFlag = true;
                        Thread wait = new Thread(waitFunc);
                        flag = true;

                        wait.Start();

                        while (true)
                        {
                            if (flag == false)
                            {
                                queue.Enqueue(firstThread);
                                break;
                            }

                            if (firstThread.IsAlive == false)
                            {
                                flag = false;
                                gFlag[threadCtr] = false;
                                keyFlag = false;
                                break;
                            }

                            if ((float)DateTime.Now.Subtract(Start).TotalSeconds >= 6 && (gIsAlive[threadCtr] != false))
                            {
                                flag = false;
                                gFlag[threadCtr] = true;
                                queue.Enqueue(firstThread);
                                keyFlag = false;

                                Console.WriteLine("\nПоток был приостановлен тк работает дольше чем должен");
                                break;
                            }
                        }
                    }

                    if (threadCtr == 2)
                        threadCtr = 0;
                    else
                        threadCtr++;
                }

                Console.WriteLine("\nПотоки завершили работу\n");
                Thread.Sleep(1000);
            }

            void waitFunc()
            {
                while (true)
                {
                    Thread.Sleep(100);

                    if (keyFlag != true)
                        break;

                    if (Console.ReadKey(true).Key == ConsoleKey.Q && keyFlag)
                    {
                        flag = false;
                        gFlag[threadCtr] = true;

                        Console.WriteLine("\nПоток был остановлен\n");
                        break;
                    }
                }
            }

            void m1()
            {
                Console.WriteLine($"\n*{Thread.CurrentThread.Name} запущен\n");

                for (int item = 0; item < 5; item++)
                {
                    while (gFlag[0] == true)
                        Thread.Sleep(100);    

                    Console.WriteLine("1 поток работает...");
                    Thread.Sleep(1000);

                }

                Console.WriteLine($"\n{Thread.CurrentThread.Name} завершил работу");
                gIsAlive[0] = false;
                count--;

                Thread.Sleep(1000);
            }

            void m2()
            {
                Console.WriteLine($"\n*{Thread.CurrentThread.Name} запущен\n");

                for (int item = 0; item < 14; item++)
                {
                    while (gFlag[1] == true)
                        Thread.Sleep(100);

                    Console.WriteLine("2 поток работает...");
                    Thread.Sleep(1000);
                }

                Console.WriteLine($"\n{Thread.CurrentThread.Name} завершил работу");
                gIsAlive[1] = false;
                count--;

                Thread.Sleep(1000);
            }

            void m3()
            {
                Console.WriteLine($"\n*{Thread.CurrentThread.Name} запущен\n");

                for (int item = 0; item < 9; item++)
                {
                    while (gFlag[2] == true)
                        Thread.Sleep(100);

                    Console.WriteLine("3 поток работает...");
                    Thread.Sleep(1000);
                }

                Console.WriteLine($"\n{Thread.CurrentThread.Name} завершил работу");
                gIsAlive[2] = false;
                count--;

                Thread.Sleep(1000);
            }

            Start();

            Console.WriteLine("Программа завершила работу");

            Console.ReadKey();
        }
    }
}
