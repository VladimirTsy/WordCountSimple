using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordCountSimple
{
    public class Programm
    {


        [STAThread]
        public static void Main(string[] args)
        {

            Console.WriteLine("*** Counting words ***");

            int given_word_length = 0;
            do
            {
                Console.Write("Show words where length more then: ");


                int.TryParse(Console.ReadLine(), out given_word_length);
            }
            while (given_word_length == 0);


            Console.WriteLine("Select a folder with text files.");

            FolderBrowserDialog FolderBrowserDialog = new FolderBrowserDialog();
            FolderBrowserDialog.ShowDialog();
            if (FolderBrowserDialog.SelectedPath == "")
                return;


            DateTime start_at = DateTime.Now;

         

            Thread.CurrentThread.Name = "main";

            Dictionary<DataReader, Thread> readers = new Dictionary<DataReader, Thread>();


            DirectoryInfo dinfo = new DirectoryInfo(FolderBrowserDialog.SelectedPath);
            FileInfo[] Files = dinfo.GetFiles("*.txt",SearchOption.AllDirectories);
            args = new string[Files.Length];

            for (int i = 0; Files.Length > i; i++)
            {
                args[i] = Files[i].FullName;
            }




            List<Dictionary<string, int>> dictionaries = new List<Dictionary<string, int>>();

         
            var maxThreads = 4;

            var semaphore = new Semaphore(maxThreads, maxThreads);
            var threads = new List<Thread>(args.Length);

            foreach (var file in Files)
            {
                var thread = new Thread(() => {
                    semaphore.WaitOne();

                    try
                    {
                        DataReader newReader = new DataReader(file.FullName);
                        dictionaries.Add(newReader.ThreadRun(given_word_length));
                      


                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                threads.Add(thread);

                thread.Start();
            }

            foreach (var thread in threads)
                thread.Join();




            // if (args.Length > 0)
            // {
            //for (int i = 0; args.Length > i; i++)
            //{
            //    DataReader new_reader = new DataReader(args[i]);

            //    Thread new_thread = new Thread(() => dictionaries.Add( new_reader.ThreadRun(given_word_length)));
            //    new_thread.Name = "Thread № " + i;
            //    readers.Add(new_reader, new_thread);
            //    new_thread.Start();
            //}
            //}


            foreach (Thread t in readers.Values) t.Join();

            DateTime stop_at = DateTime.Now;

            Console.WriteLine("Input data processed in {0} secs", new TimeSpan(stop_at.Ticks - start_at.Ticks).TotalSeconds);

            Console.WriteLine();

            Console.WriteLine("Most commonly found words:");




            var result = dictionaries.SelectMany(dict => dict)
                .ToLookup(pair => pair.Key, pair => pair.Value)
                .ToDictionary(group => group.Key, group => group.Sum()).OrderByDescending(x=>x.Value).Take(10);



            foreach (var node in result)
            {
                Console.WriteLine("{0} - {1} times", node.Value, node.Key);
            }


            Console.WriteLine();
            Console.WriteLine("done.");
            Console.ReadKey();


        }
    }
}
