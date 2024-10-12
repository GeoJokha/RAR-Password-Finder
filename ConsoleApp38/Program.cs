using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace G19_20241009
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FileInfo file = new FileInfo("C:\\Users\\Joha\\Desktop\\test.txt");
            BruteForceGenerator generator = new BruteForceGenerator();
            using (var stream = file.Open(FileMode.Create, FileAccess.Write, FileShare.None))
            {
                generator.Generate(
                    1,
                    4,
                    BruteForceType.Digits | BruteForceType.LowercaseLetters,
                    stream);
            }

            string rarFilePath = "C:\\Users\\Joha\\Desktop\\test.rar";
            TestRarFile(rarFilePath, file.FullName);
        }

        static void TestRarFile(string rarFilePath, string combinationsFilePath)
        {
            foreach (var password in File.ReadLines(combinationsFilePath))
            {
                if (TryExtractRar(rarFilePath, password))
                {
                    Console.WriteLine($"Password found: {password}");
                    break;
                }
            }
        }

        static bool TryExtractRar(string rarFilePath, string password)
        {
            try
            {
                using (var archive = RarArchive.Open(rarFilePath, new ReaderOptions { Password = password }))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (!entry.IsDirectory)
                        {
                            entry.WriteToDirectory("C:\\Users\\Joha\\Desktop\\extracted_files\\", new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
                        }
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public sealed class BruteForceGenerator
    {
        public void Generate(int minLength, int maxLength, BruteForceType type, Stream stream)
        {
            using (StreamWriter writer = new StreamWriter(stream))
            {
                char[] charset = GetCharset(type);
                for (int length = minLength; length <= maxLength; length++)
                {
                    GenerateCombinations(writer, charset, new char[length], 0);
                }
            }
        }

        private char[] GetCharset(BruteForceType type)
        {
            List<char> charset = new List<char>();
            if (type.HasFlag(BruteForceType.Digits))
            {
                charset.AddRange("0123456789");
            }
            if (type.HasFlag(BruteForceType.LowercaseLetters))
            {
                charset.AddRange("abcdefghijklmnopqrstuvwxyz");
            }
            if (type.HasFlag(BruteForceType.UppercaseLetters))
            {
                charset.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            }
            return charset.ToArray();
        }

        private void GenerateCombinations(StreamWriter writer, char[] charset, char[] combination, int position)
        {
            if (position == combination.Length)
            {
                writer.WriteLine(new string(combination));
                return;
            }

            foreach (char c in charset)
            {
                combination[position] = c;
                GenerateCombinations(writer, charset, combination, position + 1);
            }
        }
    }

    [Flags]
    public enum BruteForceType : byte
    {
        Digits = 1,
        LowercaseLetters = 2,
        UppercaseLetters = 4,
    }
}
