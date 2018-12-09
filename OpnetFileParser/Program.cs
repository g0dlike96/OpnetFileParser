using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using OpnetFileParser.Parser;

namespace OpnetFileParser
{
    internal class Program
    {
        private static string fileToParsePath;
        private static string parsedFilePath;

        private static FileParser fileParser;

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void Main(string[] args)
        {
            fileParser = Initialize();

            try
            {
                fileParser.Parse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally 
            {
                fileParser.Cleanup();
            }

            Console.WriteLine($"{Environment.NewLine}Program zakonczyl dzialanie. Nacisnij dowolny przycisk by zamknac okno...");
            Console.ReadKey();
        }

        private static FileParser Initialize()
        {           
            FileParser parser = null;

            while (parser == null)
            {
                ShowStartupMenu();

                var choice = Console.ReadKey().KeyChar.ToString();
                Console.WriteLine(Environment.NewLine);
                try
                {
                    switch (choice)
                    {
                        case "1":
                            SetPathsMenu();
                            SetFileExtensionForTr1();
                            parser = BuildTr1FileParser();
                            break;

                        case "2":
                            SetPathsMenu();
                            SetFileExtensionForTr2();
                            parser = BuildTr2FileParser();
                            break;

                        case "3":
                            Environment.Exit(0);
                            break;

                        default:
                            Console.WriteLine($"Prosze wybrac Poprawna opcje! {Environment.NewLine}");
                            continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(Environment.NewLine);
                }               
            }

            return parser;
        }

        private static void ShowStartupMenu()
        {
            Console.WriteLine
            (
                "Program sluzy do parsowania plikow wygenerowanych przez pakiet nfdump do formatu," +
                $"ktory umozliwi ich zaimportowanie do Opnet Modeller 14.5 {Environment.NewLine}" +
                $"{Environment.NewLine}" +
                "Wybierz format do ktorego chcesz sparsowac plik: " +
                $"{Environment.NewLine}" +
                "Wybierz 1 aby uzyc formatu .tr1" +
                $"{Environment.NewLine}" +
                "Wybierz 2 aby uzyc formatu .tr2" +
                $"{Environment.NewLine}" +
                "Wybierz 3 aby zakonczyc dzialanie programu..." +
                Environment.NewLine
            );
        }

        private static void SetPathsMenu()
        {
            SetInputFilePath();
            SetOutputFilePath(fileToParsePath);
        }

        private static void SetInputFilePath()
        {
            Console.WriteLine("Wskaz sciezke do pliku, ktory ma byc parsowany: ");

            var inputFilePath = Console.ReadLine();

            ValidateInputFilePath(inputFilePath);

            fileToParsePath = inputFilePath;
        }

        private static void SetOutputFilePath(string inputFilePath)
        {
            Console.WriteLine($"{Environment.NewLine}Wskaz pelna sciezke do folderu w ktorym ma byc zapisany sparsowany plik. " +
                              "W przypadku nacisniecie ENTER plik zostanie utworzony w folderze roboczym aplikacji:");

            var outputFileLocalization = Console.ReadLine();

            ValidateOutputFilePath(ref outputFileLocalization);

            parsedFilePath = string.Concat
            (
                outputFileLocalization,
                @"\",
                Path.GetFileNameWithoutExtension(inputFilePath),
                @"_parsed"
            );
        }

        private static void ValidateInputFilePath(string inputFilePath)
        {
            if (!File.Exists(inputFilePath))
            {
                throw new ArgumentException($"{Environment.NewLine}Wprowadzona sciezka do pliku: {inputFilePath} jest " +
                                            "nieprawidlowa badz plik nie istnieje!");
            }
        }

        private static void ValidateOutputFilePath(ref string outputFileLocalization)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(outputFileLocalization))
                {
                    outputFileLocalization = Environment.CurrentDirectory;
                }
                else if (!Directory.Exists(outputFileLocalization))
                {
                    Directory.CreateDirectory(outputFileLocalization);
                }
            }
            catch (Exception)
            {
                throw new DirectoryNotFoundException("Tworzenie nowego katalogu zakonczone niepowodzeniem! " +
                                                     $"Prosze sprawdzic poprawnosc sciezki.{Environment.NewLine}");
            }           
        }

        private static void SetFileExtensionForTr1()
        {
            parsedFilePath += @".tr1";
        }

        private static void SetFileExtensionForTr2()
        {
            parsedFilePath += @".tr2";
        }

        private static Tr1FileParser BuildTr1FileParser()
        {
            return new Tr1FileParser
                (
                    new StreamWriter(parsedFilePath), 
                    new StreamReader(fileToParsePath)
                );
        }

        private static Tr2FileParser BuildTr2FileParser()
        {
            return new Tr2FileParser
            (
                new StreamWriter(parsedFilePath),
                new StreamReader(fileToParsePath)
            );
        }
    }
}
