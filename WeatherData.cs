using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherProject
{
    internal class WeatherData
    {
        public DateTime Date { get; set; }

        public string? Location { get; set; }

        public string? MinTemp { get; set; }

        public string? MaxTemp { get; set; }

        public string? Rainfall { get; set; }

        public string? Evaporation { get; set; }

        public string? Sunshine { get; set; }

        public string? WindGustDir { get; set; }

        public string? WindGustSpeed { get; set; }

        public string? WindDir9am { get; set; }

        public string? WindDir3pm { get; set; }

        public string? WindSpeed9am { get; set; }

        public string? WindSpeed3pm { get; set; }

        public string? Humidity9am { get; set; }

        public string? Humidity3pm { get; set; }

        public string? Pressure9am { get; set; }

        public string? Pressure3pm { get; set; }

        public string? Cloud9am { get; set; }

        public string? Cloud3pm { get; set; }

        public string? Temp9am { get; set; }

        public string? Temp3pm { get; set; }

        public string? RainToday { get; set; }

        public string? RainTomorrow { get; set; }

        /// <summary>
        /// Метод, который нужен для корректного отображения выборки в csv файле
        /// </summary>
        /// <returns>Возвращает отформатированную строку</returns>
        public string ComposeInfo()
        {
            string remadeDateTime = $"{Date.Year}-{(Date.Month < 10 ? $"0{Date.Month}" : Date.Month)}-{(Date.Day < 10 ? $"0{Date.Day}" : Date.Day)}";
            string str = $"{remadeDateTime},{Location},{MinTemp},{MaxTemp},{Rainfall},{Evaporation},{Sunshine},{WindGustDir},{WindGustSpeed},{WindDir9am},{WindSpeed9am},{WindDir3pm},{WindSpeed3pm},{Humidity9am},{Humidity3pm},{Pressure9am},{Pressure3pm},{Cloud9am},{Cloud3pm},{Temp9am},{Temp3pm},{RainToday},{RainTomorrow}";
            return str;
        }

        /// <summary>
        /// Переопределённый метод ToString(), который красиво передаёт информацию об объекте типа WeatherData
        /// </summary>
        /// <returns>Возвращает красивую отформатированную строку</returns>
        public override string ToString()
        {
            string str = $"Дата: {Date}\n" +
                         $"Город: {Location}\n" +
                         $"Минимальная температура: {MinTemp}\n" +
                         $"Максимальная температура: {MaxTemp} \n" +
                         $"Коэффицент осадков: {Rainfall} \n" +
                         $"Коэффицент испарения: {Evaporation} \n" +
                         $"Солнечность: {Sunshine} \n" +
                         $"Направление ветра: {WindGustDir} \n" +
                         $"Скорость ветра: {WindGustSpeed} \n" +
                         $"Направление ветра в 9:00: {WindDir9am}\n" +
                         $"Скорость ветра в 9:00: {WindSpeed9am}\n" +
                         $"Направление ветра в 15:00: {WindDir3pm}\n" +
                         $"Скорость ветра в 15:00: {WindSpeed3pm}\n" +
                         $"Влажность воздуха в 9:00: {Humidity9am}\n" +
                         $"Влажность воздуха в 15:00: {Humidity3pm}\n" +
                         $"Давление воздуха в 9:00: {Pressure9am}\n" +
                         $"Давление воздуха в 15:00: {Pressure3pm}\n" +
                         $"Облачность в 9:00: {Cloud9am}\n" +
                         $"Облачность в 15:00: {Cloud3pm}\n" +
                         $"Температура в 9:00: {Temp9am}\n" +
                         $"Температура в 15:00: {Temp3pm}\n" +
                         $"Был ли дождь сегодня: {RainToday}\n" +
                         $"Будет ли дождь завтра: {RainTomorrow}\n\n";

            return str;
        }
    }
}
