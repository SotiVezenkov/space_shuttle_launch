using System;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Collections.Generic;
using System.Net.Mail;
using space_shuttle_launch.Models;
using space_shuttle_launch;

namespace SpaceMissionAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            string language = "";
            while (language != "en" && language != "de")
            {
                Console.WriteLine("Please choose a language: en/de");
                language = Console.ReadLine();
                if (language != "en" && language != "de")
                {
                    Console.WriteLine("Invalid input. Please choose 'en' for English or 'de' for German.");
                }
            }

            if (language == "en")
            {
                Console.WriteLine("Welcome to Hitachi Space Mission!");
                Console.Write("Please enter the folder path, where the .csv files with the data are located:");
            }
            else // German
            {
                Console.WriteLine("Willkommen bei der Hitachi-Weltraummission!");
                Console.Write("Bitte geben Sie den Ordnerpfad ein, in dem sich die .csv-Dateien mit den Daten befinden:");
            }
            string folderPath = Console.ReadLine();

            string senderEmail = "";
            while (!senderEmail.Contains("@"))
            {
                Console.Write(language == "en" ? "Please enter your email: " : "Bitte geben Sie ihre E-Mail-Adresse ein: ");
                senderEmail = Console.ReadLine();
                if (!senderEmail.Contains("@"))
                {
                    Console.WriteLine(language == "en" ? "Invalid email. Please enter a valid email address:" : "Ungültige E-Mail. Bitte geben Sie eine gültige E-Mail-Adresse ein:");
                }
            }

            Console.Write(language == "en" ? "Please enter the password: " : "Bitte geben Sie das Passwort ein: ");
            string password = Console.ReadLine();

            string receiverEmail = "";
            while (!receiverEmail.Contains("@"))
            {
                Console.Write(language == "en" ? "Please enter the receiver's email: " : "Bitte geben Sie die E-Mail des Empfängers ein: ");
                receiverEmail = Console.ReadLine();
                if (!receiverEmail.Contains("@"))
                {
                    Console.WriteLine(language == "en" ? "Invalid email. Please enter a valid email address:" : "Ungültige E-Mail. Bitte geben Sie eine gültige E-Mail-Adresse ein:");
                }
            }

            double minTemperature = 0;
            bool validInput = false;
            while (!validInput)
            {
                Console.Write(language == "en" ? "Please enter the minimum temperature: " : "Bitte geben Sie die Mindesttemperatur ein: ");
                try
                {
                    minTemperature = double.Parse(Console.ReadLine());
                    validInput = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine(language == "en" ? "Invalid input. Please enter a number:" : "Ungültige Eingabe. Bitte geben Sie eine Zahl ein:");
                }
            }

            double maxTemperature = 0;
            validInput = false;
            while (!validInput)
            {
                Console.Write(language == "en" ? "Please enter the maximum temperature: " : "Bitte geben Sie die Höchsttemperatur ein: ");
                try
                {
                    maxTemperature = double.Parse(Console.ReadLine());
                    validInput = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine(language == "en" ? "Invalid input. Please enter a number:" : "Ungültige Eingabe. Bitte geben Sie eine Zahl ein:");
                }
            }

            double maxWindSpeed = 0;
            validInput = false;
            while (!validInput)
            {
                Console.Write(language == "en" ? "Please enter the maximum wind speed: " : "Bitte geben Sie die maximale Windgeschwindigkeit ein: ");
                try
                {
                    maxWindSpeed = double.Parse(Console.ReadLine());
                    validInput = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine(language == "en" ? "Invalid input. Please enter a number:" : "Ungültige Eingabe. Bitte geben Sie eine Zahl ein:");
                }
            }

            double maxHumidity = 0;
            validInput = false;
            while (!validInput)
            {
                Console.Write(language == "en" ? "Please enter the maximum humidity: " : "Bitte geben Sie die maximale Luftfeuchtigkeit ein: ");
                try
                {
                    maxHumidity = double.Parse(Console.ReadLine());
                    validInput = true;
                }
                catch (FormatException)
                {
                    Console.WriteLine(language == "en" ? "Invalid input. Please enter a number:" : "Ungültige Eingabe. Bitte geben Sie eine Zahl ein:");
                }
            }

            //WeatherCriteria object with the input
            var criteria = new WeatherCriteria
            {
                MinTemperature = minTemperature,
                MaxTemperature = maxTemperature,
                MaxWindSpeed = maxWindSpeed,
                MaxHumidity = maxHumidity,
                AllowPrecipitation = false,
                AllowLightning = false,
                DisallowedCloudTypes = new List<string> { "cumulus", "nimbus" }
            };

            //Latitudes
            Dictionary<string, double> spaceportLatitudes = new Dictionary<string, double>
            {
                { "Cape Canaveral USA", 28.3922 },
                { "Kodiak USA", 57.7900 },
                { "Kourou French Guyana", 5.2136 },
                { "Mahia New Zealand", -39.1106 },
                { "Tanegashima Japan", 30.3850 }
            };

            //read data
            var spaceports = ReadWeatherData(folderPath, spaceportLatitudes);
            ReportData bestOverallReportData;

            ReportData bestOverall = null; //holding the best overall report data
            List<ReportData> reportDataList = new List<ReportData>(); //here we hold the data for each spaceport
            //the 'big' foreach simply iterates over the each spaceport like so:
            //It analyzes the data for the current spaceport based on the given criteria, then determines the best launch day, then it creates a new report data object for the current spaceport
            //Checks if everything is fine(whether there is or there's not a suitable launch day) if so it sets the bestLaunchDay property and prints a message
            //if there is not it prints 'No suitable day for launch'
            //then it adds reportData object to the reportDataList and the final if checks for which is closer to the equator and the current best spaceport, updates bestOverall with the current reportData
            foreach (var spaceport in spaceports)
            {
                string bestDay = AnalyzeWeatherData(spaceport, criteria);
                var reportData = new ReportData { SpaceportName = spaceport.Name, Latitude = spaceport.Latitude };
                if (bestDay != "No suitable day for launch")
                {
                    reportData.BestLaunchDay = bestDay;
                    if (bestOverall == null || Math.Abs(spaceport.Latitude) < Math.Abs(bestOverall.Latitude))
                    {
                        bestOverall = reportData;
                    }
                }
                else
                {
                    reportData.BestLaunchDay = "No suitable day for launch";
                }
                reportDataList.Add(reportData);
            }

            //here we add the best launch day in general after all the math stuff, below the best launch for each spaceport
            //looks like the following: {spaceportName} followed by the best day.


            if (bestOverall != null && bestOverall.BestLaunchDay != "No suitable day for launch")
            {
                bestOverallReportData = new ReportData
                {
                    SpaceportName = "Overall",
                    BestLaunchDay = $"The best launch day is {bestOverall.BestLaunchDay} at {bestOverall.SpaceportName}"
                };
            }
            else
            {
                Console.WriteLine(language == "en" ? "There is no suitable day for launch." : "Es gibt keinen geeigneten Starttag.");
                bestOverallReportData = new ReportData
                {
                    SpaceportName = "Overall",
                    BestLaunchDay = "No suitable day for launch"
                };
            }

            reportDataList.Add(bestOverallReportData);

            //pritns stuff
            //then sets a path for the new generated csv file
            //writes the data
            //the reportDataMap thingy basically navigates the mapping of the properties in the file
            //then it sends the email
            if (bestOverall != null && bestOverall.BestLaunchDay != null && bestOverall.SpaceportName != null)
            {
                Console.WriteLine(language == "en" ? $"The best overall launch day is {bestOverall.BestLaunchDay} at {bestOverall.SpaceportName}." : $"Der insgesamt beste Starttag ist {bestOverall.BestLaunchDay} bei {bestOverall.SpaceportName}.");
            }
            else
            {
                Console.WriteLine(language == "en" ? "There is no suitable day for launch." : "Es gibt keinen geeigneten Starttag.");
            }
            string reportFilePath = "./HitachiSpaceReport.csv";
            using (var writer = new StreamWriter(reportFilePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<ReportDataMap>();
                csv.WriteRecords(reportDataList);
            }

            SendEmail(senderEmail, password, receiverEmail, reportFilePath);
        }

        //makes a list that will hold the spaceport objects
        //then it reads the csv files in this directory
        //the foreach iterates over the each csv file like so:
        //streamReader that reads the file
        //WeatherDataMap tells the csvReader how to map the colums to the WeatherData properties
        //new spaceport object woo
        //adds the spaceport object to the spaceports list
        static List<Spaceport> ReadWeatherData(string folderPath, Dictionary<string, double> spaceportLatitudes)
        {
            var spaceports = new List<Spaceport>();
            var csvFiles = Directory.GetFiles(folderPath, "*.csv");

            foreach (var file in csvFiles)
            {
                using (var reader = new StreamReader(file))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<WeatherDataMap>();
                    var weatherData = csv.GetRecords<WeatherData>();
                    var spaceport = new Spaceport
                    {
                        Name = Path.GetFileNameWithoutExtension(file),
                        Latitude = spaceportLatitudes[Path.GetFileNameWithoutExtension(file)],
                        WeatherForecast = weatherData.ToList()
                    };
                    spaceports.Add(spaceport);
                }
            }

            return spaceports;
        }

        public static string AnalyzeWeatherData(Spaceport spaceport, WeatherCriteria criteria)
        {
            int bestDay = -1;
            int bestScore = -1;

            for (int i = 0; i < spaceport.WeatherForecast.Count; i++)
            {
                var weatherData = spaceport.WeatherForecast[i];

                if (weatherData.Temperature >= criteria.MinTemperature && weatherData.Temperature <= criteria.MaxTemperature &&
                    weatherData.Wind <= criteria.MaxWindSpeed &&
                    weatherData.Humidity < criteria.MaxHumidity &&
                    !weatherData.Precipitation &&
                    !weatherData.Lightning &&
                    !criteria.DisallowedCloudTypes.Contains(weatherData.Clouds, StringComparer.OrdinalIgnoreCase))
                {
                    int score = (int)(criteria.MaxWindSpeed - weatherData.Wind + (criteria.MaxHumidity - weatherData.Humidity));

                    if (score > bestScore)
                    {
                        bestDay = i + 1;
                        bestScore = score;
                    }
                }
            }

            return bestDay != -1 ? bestDay.ToString() : "No suitable day for launch";
        }

        // IMPORTANT! *In here i have made it so the data we have will be available here - ./WeatherData, if you have a different path you can type it aswell.*
        // IMPORTANT! *We need to be sending the emails from outlook. Example input: email: email@outlook.com, password: your password, where do we send it: receiver@email.com. *
        // After this we input the weather criteria. 
        // I have tried using resource files, but the Visual Studio decided not to cooperate on this, so i had to think of another way of making the multilanguage stuff work.
        static void SendEmail(string senderEmail, string password, string receiverEmail, string attachmentPath)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp-mail.outlook.com");

                mail.From = new MailAddress(senderEmail);
                mail.To.Add(receiverEmail);
                mail.Subject = "Space Mission Report";
                mail.Body = "Houston, here is your report.  ";
                Attachment attachment = new Attachment(attachmentPath);
                mail.Attachments.Add(attachment);

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(senderEmail, password);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email could not be sent: {ex.Message}");
            }
        }
    }
}
