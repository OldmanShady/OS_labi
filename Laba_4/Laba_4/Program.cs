using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

// Мисенев Александр БББО-08-20

namespace Laba_4
{
    class Program
    {
        // Квант времени (мс) 
        static readonly int timeSlotDefault = 800;

        // Список процессов
        static List<ProcessPlan> processes = new List<ProcessPlan>();

        // Поток управления процессами
        static Thread threadControl;

        // Поток изменения кванта времени процесса
        static Thread threadTimeSlot;

        static void GoTimer(int ms)
        {
            var timer = Stopwatch.StartNew();
            do
            {
            } while (timer.ElapsedMilliseconds != ms);
            timer.Stop();
        }

        // Удаление завершенного процесса из списка

        static void WaitUntilProcessEnd(object obj)
        {
            ProcessPlan process = (ProcessPlan)obj;
            process.MyProcess.WaitForExit();
            processes.Remove(process);
            Console.WriteLine("Процесс " + Thread.CurrentThread.Name + " завершил работу");
        }

        static void PrintTimeSlots()
        {
            Console.WriteLine("Кванты для потоков");
            foreach (var item in processes)
                Console.WriteLine("Поток " + item.Thread.Name + ": " + item.TimeSlot);
        }

        static void ChangeTimeSlot()
        {
            // Событие
            while (processes.Count > 0)
            {
                Console.Write("Введите № потока, которому хотите увеличить квант времени на 500мс: ");
                int iProccess = int.Parse(Console.ReadLine());
                bool isChange = false;
                try
                {
                    foreach (var item in processes)
                    {
                        if (item.Thread.Name == iProccess.ToString())
                        {
                            item.TimeSlot += 500;
                            isChange = true;
                            break;
                        }
                    }
                }
                catch
                {
                    isChange = false;
                }

                if (isChange == false)
                {
                    Console.WriteLine("Поток не существует/завершил работу");
                }
                PrintTimeSlots();
            }
        }

        static void ControlProcess()
        {
            for (int i = 0; processes.Count > 0;)
            {
                int nP = processes.Count;
                if (i > processes.Count - 1)
                    i = 0;
                if (processes[i].IsStarted == false)
                {
                    processes[i].MyProcess.Start();
                    processes[i].Thread.Start(processes[i]);
                    processes[i].IsStarted = true;
                }
                else
                {
                    processes[i].Resume();
                }
                Console.WriteLine("Процесс №" + (i + 1) + " запущен");
                GoTimer(processes[i].TimeSlot);
                if (nP == processes.Count)
                {
                    processes[i].Suspend();
                    Console.WriteLine("Процесс №" + (i + 1) + " приостановлен");
                    if (processes.Count != 0 && i == processes.Count - 1)
                        i = 0;
                    else
                        i++;
                }
            }
            threadTimeSlot.Abort();
            Console.WriteLine("Все процессы заверешены");
        }

        static void Main(string[] args)
        {
            for (int i = 0; i < 3; i++)
            {
                string line = "ConsoleApp" + (i + 1);
                line = "..\\..\\..\\" + line + "\\bin\\Debug\\" + line;
                ProcessPlan processPlan = new ProcessPlan(line);
                processPlan.TimeSlot = timeSlotDefault;
                processPlan.IsStarted = false;
                processPlan.Thread = new Thread(new ParameterizedThreadStart(WaitUntilProcessEnd));
                processPlan.Thread.Name = (i + 1).ToString();
                processes.Add(processPlan);
            }
            PrintTimeSlots();

            threadControl = new Thread(new ThreadStart(ControlProcess));
            threadControl.Start();

            threadTimeSlot = new Thread(new ThreadStart(ChangeTimeSlot));
            threadTimeSlot.Start();
        }
    }
}