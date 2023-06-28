namespace WeatherProject
{
    internal class Program
    {
        /// <summary>
        /// Метод для входа в программу
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    Terminal.DisplayCommandDescription();
                    Terminal.HandleCommand();
                }
            }
            catch
            {
                Console.WriteLine("Произошла ошибка, возможно вы ввели неверное имя файла, или файл занят другим процессом!");
                Console.WriteLine("Проверьте правильность вводимых данных и корректность csv файлов");
            }
        }
    }
}