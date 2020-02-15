using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WordCountSimple
{
    public class DataReader
    {
        public Dictionary<string, int> dict { get; set; }

        private string m_path;

        public DataReader(string path)
        {
            dict = new Dictionary<string, int>();

            m_path = path;

        }


        public Dictionary<string, int> ThreadRun(int given_word_length)
        {

            Regex reg_exp = new Regex("[^a-zA-Z0-9а-яА-Я]");



            using (FileStream fstream = new FileStream(m_path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sreader = new StreamReader(fstream))
                {
                    string line;
                    while ((line = sreader.ReadLine()) != null)
                    {
                        string[] chunks = reg_exp.Split(line);

                        foreach (string chunk in chunks)
                        {
                            AddWord(chunk, given_word_length);
                        }
                    }
                }
            }
            return dict;
        }


        public void AddWord(string word, int given_word_length)
        {
            if (word.Length > given_word_length)
            {
                word = word.Trim();

                if (dict.ContainsKey(word))
                {

                    dict[word]++;
                }
                else
                {
                    dict.Add(word, 1);
                }
            }
        }
    }
}
