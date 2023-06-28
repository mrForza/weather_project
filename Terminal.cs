using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WeatherProject
{
    internal static class Terminal
    {
        private static List<WeatherData>? _listOfWeatherEvent;

        /// <summary>
        /// Метод, отображающий информацию о доступных командах
        /// </summary>
        public static void DisplayCommandDescription()
        {
            Console.WriteLine("\nВам доступен следующий набор комманд:\n");
            Console.WriteLine("1 - Нужно ввести имя файла, из которого будут загружаться данные о погоде в Австралии\n");
            Console.WriteLine("2 - На экран выведится информация о погоде, собранной в Сиднее за 2009 и 2010 год и сохраняется в файл Sydney_2009_2010_weatherAUS.csv\n");
            Console.WriteLine("3 - На экран выводится переупорядоченный набор исходных данных о записях, в котором выделены группы по месту расположения станции сбора метеоданных, и записывается в файл average_rain_weatherAUS.csv\n");
            Console.WriteLine("4 - На экран выводится выборка записей дней, когда солнечная погода держалась хотя бы 4 часа за день\n");
            Console.WriteLine("5 - На экран выводится сводная статистика по данным загруженного файла\n");
        }

        /// <summary>
        /// Метод, обрабатывающий введённые команды
        /// </summary>
        public static void HandleCommand()
        {
            try
            {
                int command = int.Parse(Console.ReadLine());

                switch (command)
                {
                    case 1:
                        GetFileNameAndSetData();                       
                        break;
                    case 2:
                        SetSydneyInfo();
                        break;
                    case 3:
                        SetLocationAndRainfallInfo();
                        break;
                    case 4:
                        DisplaySunshineDaysInfo();
                        break;
                    case 5:
                        GetStatistics();
                        break;
                    default:
                        throw new Exception();
                }
            }
            catch
            {
                Console.Clear();
                DisplayMessage("Вы ввели некорректну команду!\nДоступные комманды: 1 2 3 4 5\n", true);
            }
        }

        /// <summary>
        /// Метод, предназначенный для исполнения команды 5 (Подробнее о ней вы можете узнать, запустив программу)
        /// </summary>
        private static void GetStatistics()
        {
            if (_listOfWeatherEvent is not null)
            {
                try
                {
                    double a = 0;
                    IEnumerable<WeatherData> goodFishing = _listOfWeatherEvent.Where(x => (double.TryParse(x.WindSpeed3pm, out a) == true && a < 13) || x.WindDir3pm == "NA");
                    Console.WriteLine($"Количество дней, пригодных для рыбалки: {goodFishing.Count()}\n");

                    IEnumerable<IGrouping<string?, WeatherData>> info = _listOfWeatherEvent.GroupBy(x => x.Location);
                    Console.WriteLine($"Количество групп, объединённых по локации: {info.Count()}");
                    foreach (IGrouping<string?, WeatherData> group in info)
                    {
                        Console.WriteLine($"Группа: {group.Key}   Количество записей: {group.Count()}");
                    }

                    a = 0;
                    IEnumerable<WeatherData> warmRainy = _listOfWeatherEvent.Where(x => (double.TryParse(x.MaxTemp, out a) == true && a >= 20 && x.RainToday == "Yes"));
                    Console.WriteLine($"\nКоличество тёплыйх и дождливых дней: {warmRainy.Count()}\n");

                    a = 0;
                    IEnumerable<WeatherData> goodPressure = _listOfWeatherEvent.Where(x => double.TryParse(x.Pressure9am, out a) == true && a >= 1000 && a <= 1007);
                    Console.WriteLine($"Количество дней с нормальным атмосферным давлением: {goodPressure.Count()}\n");
                }
                catch
                {
                    DisplayMessage("Произошла ошибка при чтении файла, убедитесь, что он корректный", true);
                }

            }
            else
            {
                Console.Clear();
                DisplayMessage("Вы не выбрали файл, из которого будет считываться информация!", true);
                ContinueCommmand(() => GetStatistics());
            }
        }

        /// <summary>
        /// Метод, предназначенный для исполнения команды 4 (Подробнее о ней вы можете узнать, запустив программу)
        /// </summary>
        private static void DisplaySunshineDaysInfo()
        {
            if (_listOfWeatherEvent is not null)
            {
                try
                {
                    double a = 0;
                    IEnumerable<WeatherData> info = _listOfWeatherEvent.Where(x => double.TryParse(x.Sunshine, out a) == true && a >= 4);
                    List<string> datasetArray = new List<string>();
                    double maxSunshine = -1;
                    DateTime date = DateTime.Now;
                    double maxTemp = 0;

                    foreach (WeatherData weather in info)
                    {
                        double temp = double.Parse(weather.Sunshine);

                        if(maxSunshine == -1 || maxSunshine < temp)
                        {
                            maxSunshine = temp;
                            date = weather.Date;
                            maxTemp = double.Parse(weather.MaxTemp);
                        }
                    }

                    DirectoryInfo

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"\nДата: {date.ToShortDateString()} и макс. температура макс. солнечного дня: {maxTemp}");
                    Console.ForegroundColor = ConsoleColor.White;
                    int i = 0;

                    foreach (WeatherData weather in info)
                    {
                        datasetArray.Add(weather.ComposeInfo());
                        DisplayMessage($"Датасет #{++i}", false);
                        Console.WriteLine(weather.ToString());
                    }

                    Console.WriteLine("Введите имя файла, в который нужно сохранить выборку");
                    string? str = Console.ReadLine();

                    SaveInfo(datasetArray, str);
                }
                catch
                {
                    DisplayMessage("Произошла ошибка при чтении файла, убедитесь, что он корректный", true);
                }
                
            }
            else
            {
                Console.Clear();
                DisplayMessage("Вы не выбрали файл, из которого будет считываться информация!", true);
                ContinueCommmand(() => DisplaySunshineDaysInfo());
            }
        }

        /// <summary>
        /// Метод, предназначенный для исполнения команды 3 (Подробнее о ней вы можете узнать, запустив программу)
        /// </summary>
        private static void SetLocationAndRainfallInfo()
        {
            if (_listOfWeatherEvent is not null)
            {
                try
                {
                    IEnumerable<IGrouping<string?, WeatherData>> info = _listOfWeatherEvent.OrderByDescending(x => x.Rainfall).GroupBy(x => x.Location);      
                    List<string> datasetArray = new List<string>();          
                    int i = 0;

                    foreach (IGrouping<string?, WeatherData> LocGroup in info)
                    {
                        double average = 0;
                        int index = 0;
               

                        foreach (WeatherData weather in LocGroup)
                        {
                            double currentRainFall = 0;
                            
                            weather.Rainfall = weather.Rainfall.Replace('.', ',');
                            bool isANumber = double.TryParse(weather.Rainfall, out currentRainFall);

                            if (isANumber == true)
                            {
                                average += double.Parse(weather.Rainfall);
                            }
                            

                            index++;
                        }

                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"\nСредний арифметический показатель осадков группы: {average /= index}\n");
                        Console.ForegroundColor = ConsoleColor.White;

                        foreach (WeatherData weather in LocGroup)
                        {
                            DisplayMessage($"Группа: {weather.Location}   Датасет #{++i}", false);
                            datasetArray.Add(weather.ComposeInfo());
                            //weather.Rainfall = weather.Rainfall.Replace(',', '.');
                            Console.WriteLine(weather.ToString());
                        }
                    }

                    SaveInfo(datasetArray, "average_rain_weatherAUS.csv");
                }
                catch
                {
                    DisplayMessage("Файл не содержит нужный заголовок", true);
                    ContinueCommmand(() => SetLocationAndRainfallInfo());
                }
            }
            else
            {
                Console.Clear();
                DisplayMessage("Вы не выбрали файл, из которого будет считываться информация!", true);
                ContinueCommmand(() => SetLocationAndRainfallInfo());
            }
        }

        /// <summary>
        /// Метод, предназначенный для исполнения команды 2 (Подробнее о ней вы можете узнать, запустив программу)
        /// </summary>
        private static void SetSydneyInfo()
        {
            if (_listOfWeatherEvent is not null)
            {
                try
                {
                    IEnumerable<WeatherData> info = _listOfWeatherEvent.Where(x => x.Location == "Sydney" && (x.Date.Year == 2009 || x.Date.Year == 2010));
                    List<string> datasetArray = new List<string>();
                    int i = 0;

                    foreach (WeatherData weather in info)
                    {
                        DisplayMessage($"Датасет #{++i}", false);
                        datasetArray.Add(weather.ComposeInfo());
                        Console.WriteLine(weather.ToString());
                    }

                    SaveInfo(datasetArray, "Sydney_2009_2010_weatherAUS.csv");
                }
                catch
                {
                    DisplayMessage("Файл не содержит нужный заголовок", true);
                    ContinueCommmand(() => SetSydneyInfo());
                }
                
            }
            else
            {
                Console.Clear();
                DisplayMessage("Вы не выбрали файл, из которого будет считываться информация!", true);
                ContinueCommmand(() => SetSydneyInfo());
            }
            
        }

        /// <summary>
        /// Метод, записывающий выборку в csv файл
        /// </summary>
        /// <param name="dataset">Список строк выборки, который нужно сохранить и записать</param>
        /// <param name="fileName">Имя фалйа, в который нужно записать выборку</param>
        private static void SaveInfo(List<string> dataset, string? fileName)
        {
            try
            {
                if (File.Exists(fileName) == false)
                {
                    File.Create(fileName).Close();
                }                

                using (StreamWriter stream = new StreamWriter(fileName, false))
                {
                    using (CsvWriter csv = new CsvWriter(stream, CultureInfo.InvariantCulture))
                    {
                        csv.WriteField("Date,Location,MinTemp,MaxTemp,Rainfall,Evaporation,Sunshine,WindGustDir,WindGustSpeed,WindDir9am,WindDir3pm,WindSpeed9am,WindSpeed3pm,Humidity9am,Humidity3pm,Pressure9am,Pressure3pm,Cloud9am,Cloud3pm,Temp9am,Temp3pm,RainToday,RainTomorrow");
                        csv.NextRecord();
                        foreach (string item in dataset)
                        {
                            csv.WriteField(item);
                            csv.NextRecord();
                        }
                    }
                }
            }
            catch
            {
                DisplayMessage($"Файл с названием {fileName} невозможно создать. Возможно он уже был создан, либо он занят другим процессоом", true);
                ContinueCommmand(() => SetSydneyInfo());
            }
            
        }


        private static void GetFileNameAndSetData()
        {
            Console.Clear();
            Console.WriteLine("Введите название файла, из которого будут загружаться данные о погоде в Австралии");

            string? fileName = Console.ReadLine();

            bool correctness = CheckFileCorretness(fileName, out _listOfWeatherEvent);

            if (_listOfWeatherEvent is not null && correctness == true)
            { 
                DisplayMessage("Имя файла было введено корректно и все данные были прочитаны программой", false);
            }
        }

        /// <summary>
        /// Метод, проверяющий корректность имени файла и его наличие в директории с исполняющим фалйом.
        /// </summary>
        /// <param name="fileName">передаваемый параметр имени csv файла</param>
        /// <param name="list">Список объектов типа WeatherData, который нужно будет заполнить в методе CheckFileStructureCorrectness</param>
        /// <returns>возвращает true, если нет ошибок ни в данном методе, ни в методу CheckFileStructureCorrectness, иначе false</returns>
        private static bool CheckFileCorretness(string? fileName, out List<WeatherData>? list)
        {
            list = null;
            bool correctness = true;

            if (fileName != "" && fileName is not null)
            {
                try
                {
                    using (StreamReader reader = new StreamReader($"{fileName}"))
                    {
                        correctness = CheckFileStructureCorrectness(reader, out list);
                    }
                }
                catch (FileNotFoundException)
                {
                    DisplayMessage("Файл не найден!", true);
                    ContinueCommmand(() => GetFileNameAndSetData());
                    correctness = false;
                }
                catch (IOException)
                {
                    DisplayMessage("Файл занят другим процессом!", true);
                    ContinueCommmand(() => GetFileNameAndSetData());
                    correctness = false;
                }                
            }
            else
            {
                DisplayMessage($"Вы ввели 'пустое' имя", true);
                ContinueCommmand(() => GetFileNameAndSetData());
                correctness = false;
            }

            return correctness;
        }

        /// <summary>
        /// Метод, заполняющий список объектов типа WeatherData и проверящий корректность структуры csv файла
        /// </summary>
        /// <param name="reader">Передаваемый объект StreamReader'а, который нужен для работы с рекордами</param>
        /// <param name="list">Список объектов типа WeatherData, который заполняется информацией из csv файла</param>
        /// <returns>Возвращает true, если отсутствуют ошибки, иначе false</returns>
        private static bool CheckFileStructureCorrectness(StreamReader reader, out List<WeatherData>? list)
        {
            list = null;
            bool correctness = true;

            try
            {
                using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    IEnumerable<WeatherData> records = csv.GetRecords<WeatherData>();
                    list = records.ToList();
                }

                foreach (WeatherData weather in list)
                {
                    weather.Sunshine = weather.Sunshine.Replace('.', ',');
                    weather.MaxTemp = weather.MaxTemp.Replace('.', ',');
                    weather.Pressure9am = weather.Pressure9am.Replace('.', ',');
                    weather.Rainfall = weather.Rainfall.Replace('.', ',');

                    if (weather.Sunshine == "NA")
                    {
                        weather.Sunshine = "0";
                    }
                }
            }
            catch
            {
                DisplayMessage("Структура файла некорректна", true);
                ContinueCommmand(() => GetFileNameAndSetData());
                correctness = false;
            }
            
            return correctness;
        }

        /// <summary>
        /// Метод, который красит цвет текста, делая его либо красным, либо зелёным
        /// </summary>
        /// <param name="text">Вводимый текст, цвет которого нужно поменять</param>
        /// <param name="error">true - красный цвет (сигнал об ошибке), false - зелёный цвет (сигнал об успешном завершении команды)</param>
        private static void DisplayMessage(string text, bool error)
        {
            if (error == true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.WriteLine($"\n{text}\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Метод, который спрашивает у пользователя, продолжить ли работу с командой, или произвести выход из неё, чтобы ввести другую команду
        /// </summary>
        /// <param name="a">Делегат для вызова функции, в которой произошла ошибка</param>
        /// <returns></returns>
        private static int ContinueCommmand(Action a)
        {
            Console.WriteLine("\nВы хотите продолжить работу с данной команды?\n");
            Console.WriteLine("YES - продолжаем работу");
            Console.WriteLine("NO - вводим другую команду\n");

            while (true)
            {
                string? str = Console.ReadLine();

                switch (str)
                {
                    case "YES":
                        a.Invoke();
                        return 0;
                        break;
                    case "NO":
                        return 0;
                        break;
                    default:
                        Console.WriteLine("Вы ввели неправильное слово. Либо YES, либо NO\n");
                        break;
                }
            }           
        }
    }
}
