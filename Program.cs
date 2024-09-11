using System;
using System.IO;
using System.Reflection.Metadata;
using System.Text;

namespace GeniyIdiotConsoleApp
{
    internal class Program
    {
        public class QuestionsAnswers
        {
            public int Index { get; }
            public string Question { get; }
            public int Answer { get; }

            public QuestionsAnswers(int index, string question, int answer)
            {
                Index = index;
                Question = question;
                Answer = answer;
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
            public void СreatingFirstNameLastNamePatronomic()
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
        static bool AnswerToTheQuestionIsYesOrNo(string answer)
        {
            return answer.ToLower() == "да";
        }
        public class Game
        {
            public QuestionsAnswers[] listQuestionsAnswers;
            public int NumberOfQuestions { get; }

            public Game()
            {
                listQuestionsAnswers = new QuestionsAnswers[5]
                {
                    new QuestionsAnswers(0, "Сколько будет два плюс два умноженное на два?", 6),
                    new QuestionsAnswers(1, "Бревно нужно распилить на 10 частей. Сколько распилов нужно сделать?", 9),
                    new QuestionsAnswers(2, "На двух руках 10 пальцев. Сколько пальцев на 5 руках?", 25),
                    new QuestionsAnswers(3, "Укол делают каждые полчаса. Сколько нужно минут, чтобы сделать три укола?", 60),
                    new QuestionsAnswers(4, "Пять свечей горело, две потухли. Сколько свечей осталось?", 2)
                };
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
            public string Diagnose(int countRightAnswers) 
            {
                string diagnose;                
                double percentageOfCorrectAnswers = Math.Round((double)countRightAnswers / NumberOfQuestions*100);

                if (percentageOfCorrectAnswers == 0)
                { diagnose = "кретин"; }
                else if (percentageOfCorrectAnswers >0&& percentageOfCorrectAnswers<=20)
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


        static void Main()
        {
            string path = @"log.txt";
            Console.WriteLine(@"Добро пожаловать в игру ""Гений-Идиот""");
            var user = new User("", "", "");
            user.СreatingFirstNameLastNamePatronomic();
            do
            {
                var newGame = new Game();
                int countRightAnswers = 0;
                List <int> listWithQuestionNumbers = newGame.RandomNumberQuestions(newGame.NumberOfQuestions);

                for (int i = 0; i < newGame.NumberOfQuestions; i++)
                {
                    Console.WriteLine("Вопрос №" + (i + 1));
                    newGame.ShowQuestion(listWithQuestionNumbers[i]);
                    bool IsCorrectAnswer = newGame.CheckAnswer(listWithQuestionNumbers[i]);
                                                                                           
                    if (IsCorrectAnswer)
                    {
                        countRightAnswers++;
                    }
                }
                
                string formatSave = $"|{user.Lastname,15}|{user.Firstname, 15}|{user.Patronomic, 15}|{countRightAnswers, 21}|{newGame.Diagnose(countRightAnswers),10}|";
                using (var writer = new StreamWriter(path, true, Encoding.Default))
                {
                    writer.WriteLine(formatSave);
                }
                
                Console.WriteLine("Количество правильных ответов: " + countRightAnswers);
                Console.WriteLine($"{user.Firstname}, ваш диагноз: " + newGame.Diagnose(countRightAnswers));

                Console.WriteLine($"Хотите попробовать еще раз? (Введите ДА или НЕТ для завершения игры)");

            } while (AnswerToTheQuestionIsYesOrNo(Console.ReadLine()));

            Console.WriteLine($"Хотите посмотреть таблицу с результатами? (Введите ДА или НЕТ)");
            if (AnswerToTheQuestionIsYesOrNo(Console.ReadLine()))
            {
                Console.WriteLine(new string('-', 82));
                Console.WriteLine($"|{"Фамилия",15}|{"Имя",15}|{"Отчество",15}|{"Кол-во верных ответов",21}|{"Диагноз",10}|");
                Console.WriteLine(new string('-', 82));
                using (var read = new StreamReader(path, Encoding.Default))
                {
                    string line;
                    while ((line = read.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }

            }
        }

    }
}

