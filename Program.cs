using System;
using System.ComponentModel;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using static GeniyIdiotConsoleApp.Program;

namespace GeniyIdiotConsoleApp
{
    internal class Program
    {
        static void Main()
        {
            var newGame = new GameManager();            
            
            Console.WriteLine(@"Добро пожаловать в игру ""Гений-Идиот""");
            var user = new User("", "", "");
            user.FullName();
            do
            {                
                var newListQuestionsAnswers = new QuestionRepository();
                int countRightAnswers = 0;
                var listWithQuestionNumbers = newGame.RandomNumberQuestions(newListQuestionsAnswers.NumberOfQuestions);

                for (int i = 0; i < newListQuestionsAnswers.NumberOfQuestions; i++)
                {
                    Console.WriteLine("Вопрос №" + (i + 1));
                    newListQuestionsAnswers.ShowQuestion(listWithQuestionNumbers[i]);
                    bool IsCorrectAnswer = newListQuestionsAnswers.CheckAnswer(listWithQuestionNumbers[i]);

                    if (IsCorrectAnswer)
                    {
                        countRightAnswers++;
                    }
                }

                newGame.AddResult(user.Lastname, user.Firstname, user.Patronomic, countRightAnswers, newGame.Diagnose(countRightAnswers, newListQuestionsAnswers.NumberOfQuestions));
                               
                Console.WriteLine($"Хотите попробовать еще раз? (Введите ДА или НЕТ для завершения игры)");

            } while (RepeatTest(Console.ReadLine()));

            Console.WriteLine($"Хотите посмотреть таблицу с результатами? (Введите ДА или НЕТ)");
            if (RepeatTest(Console.ReadLine()))
            {
                newGame.HistoryOfResults();
            }
        }

        public class FileManager
        {            
            public void WriteToFile(string path, string formatSave)
            {
                using (var writer = new StreamWriter(path, true, Encoding.Default))
                {
                    writer.WriteLine(formatSave);
                }
            }

            public List<string> ReadFromFile(string path)
            {
                var result = new List<string>();
                using (var read = new StreamReader(path, Encoding.Default))
                {
                    string line;
                    while ((line = read.ReadLine()) != null)
                    {
                        result.Add(line);
                    }
                }
                return result;
            }

            public void CreateFile(string path) //очистить файл
            {
                File.Create(path).Close();
            }
        }
        public class GameManager
        {
            public string FileWithLogs { get; } = @"..\..\..\log.txt"; //!!!!!!вытаскивает содержимое файла если использовать Properties.Resources.log;
            FileManager workWithFileSistem = new FileManager();        //надо переписывать код под неё

            public void AddResult(string lastname, string firstname, string patronomic, int countRightAnswers, string diagnose)
            {                
                var formatSave = $"|{lastname,15}|{firstname,15}|{patronomic,15}|{countRightAnswers,21}|{diagnose,10}|";
                workWithFileSistem.WriteToFile(FileWithLogs, formatSave);

                Console.WriteLine("Количество правильных ответов: " + countRightAnswers);
                Console.WriteLine($"{firstname}, ваш диагноз: " + diagnose);
            }

            public void HistoryOfResults()
            {                
                Console.WriteLine(new string('-', 82));
                Console.WriteLine($"|{"Фамилия",15}|{"Имя",15}|{"Отчество",15}|{"Кол-во верных ответов",21}|{"Диагноз",10}|");
                Console.WriteLine(new string('-', 82));
                List<string> forOutput = new List<string>();
                forOutput = workWithFileSistem.ReadFromFile(FileWithLogs);
                foreach (var str in forOutput)
                {
                    Console.WriteLine(str);
                }
            }

            public void ClearTheGameHistory()//очистить историю логов
            {
                workWithFileSistem.CreateFile(FileWithLogs);
            }

            public string Diagnose(int countRightAnswers, int numberOfQuestions)
            {
                string diagnose;
                var percentageOfCorrectAnswers = Math.Round((double)countRightAnswers / numberOfQuestions * 100);

                if (percentageOfCorrectAnswers == 0)
                { diagnose = "кретин"; }
                else if (percentageOfCorrectAnswers > 0 && percentageOfCorrectAnswers <= 20)
                { diagnose = "идиот"; }
                else if (percentageOfCorrectAnswers > 20 && percentageOfCorrectAnswers <= 40)
                { diagnose = "дурак"; }
                else if (percentageOfCorrectAnswers > 40 && percentageOfCorrectAnswers <= 60)
                { diagnose = "нормальный"; }
                else if (percentageOfCorrectAnswers > 60 && percentageOfCorrectAnswers <= 80)
                { diagnose = "талант"; }
                else
                { diagnose = "гений"; }

                return diagnose;
            }

            public List<int> RandomNumberQuestions(int numberQuestion)
            {
                var result = new List<int>();
                var random = new Random();
                int nextRandom;
                for (int i = 1; i <= numberQuestion;)
                {
                    nextRandom = random.Next(numberQuestion);
                    if (!result.Contains(nextRandom))
                    {
                        result.Add(nextRandom);
                        i++;
                    }
                }
                return result;
            }

        }
        public class QuestionAnswer
        {
            public int Index { get; }
            public string Question { get; }
            public int Answer { get; }
            
            public QuestionAnswer(int index, string question, int answer)
            {
                Index = index;
                Question = question;
                Answer = answer;
            }
        }
        public class QuestionRepository
        {
            public string FileWithQuestionsAnswers { get; } = @"..\..\..\QuestionsAnswers.txt";//!!!!!!вытаскивает содержимое файла если использовать Properties.Resources.QuestionsAnswers;
                                                                                               //надо переписывать код под неё
            FileManager workWithFileSistem = new FileManager();                                              

            public QuestionAnswer[] listQuestionsAnswers;
            public int NumberOfQuestions { get; }
            
            public QuestionRepository()
            {
                var arrayOfLinesFromTheFile = new List<string[]>();
                List<string> linesFromTheFile = workWithFileSistem.ReadFromFile(FileWithQuestionsAnswers);
                foreach (var str in linesFromTheFile)
                {
                    string[] elements = str.Split('=');
                    arrayOfLinesFromTheFile.Add(elements);
                }

                listQuestionsAnswers = new QuestionAnswer[arrayOfLinesFromTheFile.Count];
                for (int i = 0; i < arrayOfLinesFromTheFile.Count; i++)
                {
                    int index = Convert.ToInt32(arrayOfLinesFromTheFile[i][0]);
                    string question = arrayOfLinesFromTheFile[i][1];
                    int answer = Convert.ToInt32(arrayOfLinesFromTheFile[i][2]);
                    listQuestionsAnswers[i] = new QuestionAnswer (index, question, answer);
                }
                NumberOfQuestions = listQuestionsAnswers.Length;
            }
            public void ShowQuestion(int numberQuestion)
            {
                foreach (var questionsAnswers in listQuestionsAnswers)
                {
                    if (numberQuestion == questionsAnswers.Index)
                    {
                        Console.WriteLine($"{questionsAnswers.Question}");
                    }
                }
            }
            public bool CheckAnswer(int numberQuestion)
            {
                int answer;
                bool isNumeric;

                do
                {
                    isNumeric = int.TryParse(Console.ReadLine(), out answer);
                    if (!isNumeric)
                    {
                        Console.WriteLine("Пожалуйста, введите число!");
                    }

                } while (!isNumeric);

                bool result = false;
                foreach (var questionsAnswers in listQuestionsAnswers)
                {
                    if (numberQuestion == questionsAnswers.Index)
                    {
                        if (questionsAnswers.Answer == answer)
                        { result = true; }
                    }
                }
                return result;
            }            
            public void AddQuestionsAnswers() //пока просто для тестов
            {
                int index = NumberOfQuestions + 1;
                Console.Write("Введите новый вопрос для игры: ");
                string question = Console.ReadLine();
                Console.Write("Введите ответ для нового вопроса: ");
                string answer = Console.ReadLine();                

                workWithFileSistem.WriteToFile(FileWithQuestionsAnswers, $"{index}={question}={answer}");
            }
            
        }
        public class User
        {
            public string Lastname { get; set; }
            public string Firstname { get; set; }
            public string Patronomic { get; set; }
            public User(string lastname, string firstname, string patronomic)
            {
                Lastname = lastname;
                Firstname = firstname;
                Patronomic = patronomic;
            }
            public void FullName()
            {
                Console.WriteLine();
                Console.Write("Введите фамилию: ");
                Lastname = Console.ReadLine(); 
                Console.Write("Введите имя: ");
                Firstname = Console.ReadLine();
                Console.Write("Введите отчество: ");
                Patronomic = Console.ReadLine();
            }
        }
        static bool RepeatTest(string answer)
        {
            return answer.ToLower() == "да";
        }                        
    }
}

