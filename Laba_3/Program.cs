using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Мисенев Александр БББО-08-20

namespace Laba_3
{
    class Program
    {
        // Размер очереди
        static readonly int nQueue = 200;

        // Генерация случайных чисел
        static Random random;

        // Очередь
        static Queue<int> queue;

        // Массив потоков производителей
        static Thread[] generators;

        // Массив потоков потребителей
        static Thread[] consumers;

        static Mutex mtx;

        // Запуск/остановка потоков
        static bool isOver;

        // Метод для потоков производителей
        static void Generator()
        {
            int value;
            while (!isOver)
            {
                Thread.Sleep(500);
                mtx.WaitOne();
                if (queue.Count < nQueue)
                {
                    value = random.Next(1, 101);
                    queue.Enqueue(value);
                    Console.WriteLine(Thread.CurrentThread.Name + ". В конец очереди добавленно число " + value + ". Длина очереди: " + queue.Count + ".");
                }

                mtx.ReleaseMutex();
            }
            Console.WriteLine(Thread.CurrentThread.Name + " остановлен.");
        }

        static void Dequeue()
        {
            mtx.WaitOne();
            if (queue.Count > 0)
            {
                int value = queue.Dequeue();
                Console.WriteLine(Thread.CurrentThread.Name + ". Из начала очереди удаленно число " + value + ". Длина очереди: " + queue.Count + ".");
            }

            mtx.ReleaseMutex();
        }

        // Метод для потоков потребителей
        static void Consumer()
        {
            while (true)
            {
                if (!isOver)
                {
                    Thread.Sleep(500);
                    if (queue.Count >= 100 || queue.Count == 0)
                    {
                        Thread.Sleep(1000);
                    }
                    else if (queue.Count <= 80)
                    {
                        Dequeue();
                    }
                }
                else if (queue.Count == 0)
                {
                    break;
                }
                else
                {
                    Dequeue();
                }
            }
            Console.WriteLine(Thread.CurrentThread.Name + " остановлен.");
        }

        static void Main(string[] args)
        {
            int nGenerator = 3, nConsumer = 2;
            isOver = false;
            queue = new Queue<int>();
            mtx = new Mutex();
            random = new Random();
            generators = new Thread[nGenerator];
            consumers = new Thread[nConsumer];

            Console.WriteLine("* Нажмите q для остановки потоков");
            Thread.Sleep(1000);
            for (int i = 0; i < nGenerator; i++)
            {
                generators[i] = new Thread(new ThreadStart(Generator));
                generators[i].Name = "Поток производителя #" + (i + 1);
                generators[i].Start();
            }
            for (int i = 0; i < nConsumer; i++)
            {
                consumers[i] = new Thread(new ThreadStart(Consumer));
                consumers[i].Name = "Поток потребителя #" + (i + 1);
                consumers[i].Start();
            }

            char key = '0';

            while (key != 'q')
            {
                key = (char)Console.Read();
            }
            isOver = true;

            Console.ReadKey();
        }
    }
}
