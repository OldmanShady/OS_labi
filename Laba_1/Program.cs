using System;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

//Мисенев Александр БББО-08-20

namespace first_program
{
    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            char cn2;
            
            do
            {
                Console.Clear();
                Console.WriteLine("Информация о дисках -> 1");
                Console.WriteLine("Работа с файлами -> 2");
                Console.WriteLine("Работа с JSON -> 3");
                Console.WriteLine("Работа с XML -> 4");
                Console.WriteLine("Работа со сжатием ZIP -> 5");
                Console.WriteLine("Выход -> 0");
                Console.Write("\nВведите команду:: ");

                int cn = Convert.ToInt32(Console.ReadLine());
                
                switch (cn)
                {
                    case 1:
                        {
                            Console.WriteLine("\n1) информация о дисках\n");
                            DriveInfo[] drivers = DriveInfo.GetDrives();
                            
                            foreach (DriveInfo drive in drivers)
                            {
                                Console.WriteLine($"Название: {drive.Name}");
                                Console.WriteLine($"Тип: {drive.DriveType}");
                                
                                if (drive.IsReady)
                                {
                                    Console.WriteLine($"Объем диска: {drive.TotalSize}");
                                    Console.WriteLine($"Свободное пространство: {drive.TotalFreeSpace}");
                                    Console.WriteLine($"Метка: {drive.VolumeLabel}");
                                }
                                
                                Console.WriteLine();
                            }
                            break;
                        }
                    case 2:
                        {
                            Console.WriteLine("\n2) работа с файлами\n");

                            string path = @"D:\C#\Laba_1";

                            Console.Write("Введите строку для записи в файл:: ");

                            string text = Console.ReadLine();

                            using (FileStream fstream = new FileStream($"{path}\\file.txt", FileMode.OpenOrCreate))
                            {
                                byte[] array = System.Text.Encoding.Default.GetBytes(text);
                                fstream.Write(array, 0, array.Length);

                                Console.WriteLine("\nСтрока записана в файл");
                            }
                            using (FileStream fstream = File.OpenRead($"{path}\\file.txt"))
                            {
                                byte[] array = new byte[fstream.Length];
                                fstream.Read(array, 0, array.Length);

                                string fileTxt = System.Text.Encoding.Default.GetString(array);

                                Console.WriteLine($"\nТекст из файла: {fileTxt}");
                            }
                            Console.Write("\nУдалить файл? y/n:: ");
                            
                            switch (Console.ReadLine())
                            {
                                case "y":
                                    if (File.Exists($"{path}\\file.txt"))
                                    {
                                        File.Delete($"{path}\\file.txt");
                                        
                                        Console.WriteLine("\nФайл удален");
                                    }
                                    else 
                                        Console.WriteLine("\nФайл не существует");
                                    break;
                                case "n":
                                    break;
                                default:
                                    Console.WriteLine("\nВведено неверное значение");
                                    break;
                            }
                            Console.WriteLine();
                            break;
                        }
                    case 3:
                        {
                            Console.WriteLine("\n3) работа с JSON\n");

                            using (FileStream fs = new FileStream(@"D:\C#\Laba_1\user.json", FileMode.OpenOrCreate))
                            {
                                Person pers = new Person() { Name = "Alex", Age = 20 };
                                await JsonSerializer.SerializeAsync<Person>(fs, pers);

                                Console.WriteLine("Данные были введены автоматически и сохранены");
                            }

                            using (FileStream fs = new FileStream(@"D:\C#\Laba_1\user.json", FileMode.OpenOrCreate))
                            {
                                Person restPerson = await JsonSerializer.DeserializeAsync<Person>(fs);
                                Console.WriteLine($"Name: {restPerson.Name}  Age: {restPerson.Age}");
                            }
                            Console.Write("Удалить файл? (y/n):: ");

                            switch (Console.ReadLine())
                            {
                                case "y":
                                    File.Delete(@"D:\C#\Laba_1\user.json");
                                    Console.WriteLine("\nФайл удален");

                                    break;
                                case "n":
                                    break;
                            }
                            break;
                        }
                    case 4:
                        {
                            Console.WriteLine("\n4) работа с XML\n");

                            XmlDocument xmlDoc = new XmlDocument();
                            XDocument xDoc = new XDocument();

                            Console.Write("Введите кол-во пользователей:: ");
                            
                            int cnt = Convert.ToInt32(Console.ReadLine());
                            XElement list = new XElement("users");
                            Console.WriteLine();

                            for (int i = 1; i <= cnt; i++)
                            {
                                XElement User = new XElement("user");
                                Console.Write("Введите имя пользователя:: "); 
                                XAttribute UserName = new XAttribute("name", Console.ReadLine());

                                Console.Write("Введите возраст пользователя:: ");
                                XElement UserAge = new XElement("age", Convert.ToInt32(Console.ReadLine()));

                                Console.Write("Введите название компании:: ");
                                XElement UserCompany = new XElement("company", Console.ReadLine());

                                Console.WriteLine();

                                User.Add(UserName);
                                User.Add(UserAge);
                                User.Add(UserCompany);
                                list.Add(User);
                            }
                            xDoc.Add(list);
                            xDoc.Save(@"D:\C#\Laba_1\users.xml");

                            Console.Write("Прочитать XML файл? (y/n):: ");
                            
                            switch (Console.ReadLine())
                            {
                                case "y":
                                    Console.WriteLine();
                                    xmlDoc.Load(@"D:\C#\Laba_1\users.xml");
                                    XmlElement xRoot = xmlDoc.DocumentElement;

                                    foreach (XmlNode xnode in xRoot)
                                    {
                                        if (xnode.Attributes.Count > 0)
                                        {
                                            XmlNode attr = xnode.Attributes.GetNamedItem("name");
                                            
                                            if (attr != null)
                                                Console.WriteLine($"Имя: {attr.Value}");
                                        }

                                        foreach (XmlNode childnode in xnode.ChildNodes)
                                        {
                                            if (childnode.Name == "age")
                                                Console.WriteLine($"Возраст: {childnode.InnerText}");

                                            if (childnode.Name == "company")
                                                Console.WriteLine($"Компания: {childnode.InnerText}");
                                        }
                                    }
                                    Console.WriteLine();
                                    break;
                                case "n":
                                    break;
                                default:
                                    Console.WriteLine("Введены неверные данные");
                                    break;
                            }
                            Console.Write("Удалить xml файл? (y/n):: ");

                            switch (Console.ReadLine())
                            {
                                case "y":
                                    FileInfo xmlfilecheck = new FileInfo(@"D:\C#\Laba_1\users.xml");
                                    if (xmlfilecheck.Exists)
                                    {
                                        xmlfilecheck.Delete();
                                        Console.WriteLine("Файл удален");
                                    }
                                    else 
                                        Console.WriteLine("Файл не существует");
                                    break;
                                case "n":
                                    break;
                                default:
                                    Console.WriteLine("Введено неверное зачение");
                                    break;
                            }
                            Console.WriteLine();
                            break;
                        }
                    case 5:
                        {
                            Console.WriteLine("\n5) работа с ZIP\n");

                            string srcFile = @"D:\C#\Laba_1\zip.txt";
                            string comprFile = @"D:\C#\Laba_1\bin.gz";
                            string trgtFile = @"D:\C#\Laba_1\zip_1.txt";

                            Compress(srcFile, comprFile);
                            Decompress(comprFile, trgtFile);
                            Console.Write("Удалить файлы? (y/n):: ");
                            
                            switch (Console.ReadLine())
                            {
                                case "y":
                                    if ((File.Exists(srcFile) &&
                                         File.Exists(comprFile) && 
                                         File.Exists(trgtFile)) == true)
                                    {
                                        File.Delete(srcFile);
                                        File.Delete(comprFile);
                                        File.Delete(trgtFile);
                                        Console.WriteLine("Файлы удалены");
                                    }
                                    else 
                                        Console.WriteLine("Ошибка удаления");
                                    break;
                                case "n":
                                    break;
                                default:
                                    Console.WriteLine("Введены неверные данные");
                                    break;
                            }
                            Console.WriteLine();
                            break;
                        }
                    case 0:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\nВведены неверные данные");
                        break;
                }
                Console.WriteLine("------------------");
                Console.Write("\nПродолжить? (y/n):: ");

                cn2 = Convert.ToChar(Console.ReadLine());
                Console.Write('\n');
            } while (cn2 != 'n');
        }
        public static void Compress(string srcFile, string comprFile)
        {
            using (FileStream srcStream = new FileStream(srcFile, FileMode.OpenOrCreate))
            {
                using (FileStream trgtStream = File.Create(comprFile))
                {
                    using (GZipStream comprStream = new GZipStream(trgtStream, CompressionMode.Compress))
                    {
                        srcStream.CopyTo(comprStream);
                        Console.WriteLine("Сжатие файла {0} завершено\nИсходный размер: {1}\nCжатый размер: {2}",
                            srcFile, srcStream.Length.ToString(), trgtStream.Length.ToString());
                    }
                }
            }
        }
        public static void Decompress(string comprFile, string trgtFile)
        {
            using (FileStream srcStream = new FileStream(comprFile, FileMode.OpenOrCreate))
            {
                using (FileStream trgtStream = File.Create(trgtFile))
                {
                    using (GZipStream decomprStream = new GZipStream(srcStream, CompressionMode.Decompress))
                    {
                        decomprStream.CopyTo(trgtStream);
                        Console.WriteLine("Восстановлен файл: {0}", trgtFile);
                    }
                }
            }
        }
    }
}
