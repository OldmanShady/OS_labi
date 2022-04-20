using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Мисенев Александр БББО-08-20

// Напечатайте каждый соответствующий пароль вместе с его хешем SHA-256. Количество потоков от 2 до 7
namespace Laba_2
{
    class Program
    {
        // Кол-во хешей и паролей, которые надо найти
        static readonly int hashCount = 3;

        // Кол-во найденных паролей
        static int foundPass;

        // Словарь (ключ: хеш; значение: пароль)
        static Dictionary<string, string> dictPass;

        // Алфавит содержащий все символы возможные в пароле
        static char[] alph;

        // Время запуска потоков
        static DateTime dateTimeBegin;

        // Генерация хеша

        static string GetHash(string str)
        {
            StringBuilder hash = new StringBuilder();

            using (SHA256 sha256 = SHA256Managed.Create())
            {
                // ASCII кодировка
                Encoding enc = Encoding.ASCII;

                // Преобразовываем строку с паролем в массив байтов для хеширования
                byte[] byteRes = sha256.ComputeHash(enc.GetBytes(str));

                // Преобрзовываем хеш в строку
                foreach (byte b in byteRes)
                {
                    // Конвертируем 1 байт в число 16-ой системы счисления
                    hash.Append(b.ToString("x2"));
                }
            }

            return hash.ToString();
        }

        // Вызов метода построения хеша на основании пароля и сравнение полученного хеша с заданными

        static bool CompareHash(string password)
        {
            string hash = GetHash(password);

            if (dictPass.ContainsKey(hash))
            {
                dictPass[hash] = password.ToString();
                foundPass++;

                Console.WriteLine("Пароль: " + dictPass[hash] + "\t|\tХэш: " + hash);

                return true;
            }
            else
            {
                return false;
            }
        }

        // Выполняем брутфорс (полный перебор)
        // Вызываем метод сравнения хеша полученной последовательности с эталонными хешами

        static void NextSequence(object beginEnd)
        {
            var temp = (char[])beginEnd;
            string sequence;

            for (char i1 = temp[0]; i1 <= temp[1]; i1++)
            {
                foreach (var i2 in alph)
                {
                    foreach (var i3 in alph)
                    {
                        foreach (var i4 in alph)
                        {
                            foreach (var i5 in alph)
                            {

                                sequence = string.Concat(i1, i2, i3, i4, i5);

                                if (CompareHash(sequence))
                                {
                                    Console.WriteLine("С запуска потоков и до нахождения пароля прошло " + Math.Round((DateTime.Now - dateTimeBegin).TotalSeconds) + " с.");
                                }

                                if (foundPass == hashCount)
                                {
                                    Console.WriteLine("Поток {" + temp[2] + "} завершил свою работу");

                                    return;
                                }
                            }
                        }
                    }
                }
            }
         
            Console.WriteLine("Поток {" + temp[2] + "} завершил свою работу");
        }

        static void Main(string[] args)
        {
            foundPass = 0;

            // Заполняем массив символов возможных в пароле
            alph = new char[26];

            for (char i = 'a'; i <= 'z'; i++)
            {
                alph[i - 'a'] = i;
            }

            // Задаем эталонные хеши
            dictPass = new Dictionary<string, string>();
            dictPass.Add("1115dd800feaacefdf481f1f9070374a2a81e27880f187396db67958b207cbad", "");
            dictPass.Add("3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b", "");
            dictPass.Add("74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f", "");

            dateTimeBegin = DateTime.Now;

            // Объявление потоков
            Thread threadBruteForce1 = new Thread(new ParameterizedThreadStart(NextSequence));
            Thread threadBruteForce2 = new Thread(new ParameterizedThreadStart(NextSequence));
            Thread threadBruteForce3 = new Thread(new ParameterizedThreadStart(NextSequence));
            Thread threadBruteForce4 = new Thread(new ParameterizedThreadStart(NextSequence));
            Thread threadBruteForce5 = new Thread(new ParameterizedThreadStart(NextSequence));

            // Запуск потоков
            threadBruteForce1.Start(new char[] { 'a', 'e', '1' });
            threadBruteForce2.Start(new char[] { 'f', 'j', '2' });
            threadBruteForce3.Start(new char[] { 'k', 'o', '3' });
            threadBruteForce4.Start(new char[] { 'p', 'u', '4' });
            threadBruteForce5.Start(new char[] { 'v', 'z', '5' });

            Console.WriteLine("Запущено 5 потоков. Ожидайте окончание их выполнения.");

            Console.ReadKey();
        }
    }
}