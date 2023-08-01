using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using IronOcr;
using Newtonsoft.Json;

namespace Scan_Russing_Passport.Classes
{
    public class Passport
    {
        /// <summary>
        /// Кем выдан паспорт
        /// </summary>
        public string Issued { get; set; }
        /// <summary>
        /// Дата выдачи
        /// </summary>
        public string DateIssued { get; set; }
        /// <summary>
        /// Код выдачи
        /// </summary>
        public string SubdivisionCode { get; set; }
        /// <summary>
        /// Серия и номер паспорта
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// Фамилия
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Имя
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Отчество
        /// </summary>
        public string SurName { get; set; }
        /// <summary>
        /// Пол
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// Дата рождения
        /// </summary>
        public string DateOfBirth { get; set; }
        /// <summary>
        /// Место рождения
        /// </summary>
        public string PlaceOfBirth { get; set; }
        /// <summary>
        /// Путь до файла
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Тип документа
        /// </summary>
        public string Type = "passport";
        /// <summary>
        /// Правильность проверки даты выдачи
        /// </summary>
        public bool IsCheckDateIssued = false;
        /// <summary>
        /// Правильность проверки даты рождения
        /// </summary>
        public bool IsCheckDateOfBirth = false;
        /// <summary>
        /// Правильность проверки кода подразделения
        /// </summary>
        public bool IsCheckSubdivisionCode = false;
        /// <summary>
        /// Правильность проверки серии и номера паспорта
        /// </summary>
        public bool IsCheckSerialNumber = false;

        public void ParsePDF(string Path)
        {
            // Сохраняем путь до файла
            this.Path = Path;
            // Создаём переменную, которая будет хранить результат
            string sResultOce = "";
            // Инициализируем библиотеку
            var ocr = new IronTesseract();
            // Устанавливаем язык
            ocr.Language = OcrLanguage.Russian;
            // Добавляем в исключение набор символов, которые нам не нужны
            ocr.Configuration.BlackListCharacters = "—~`$#^*_}{]=[|\\@¢©«»°±·×‘’“”•…′″€™←↑→↓↔⇄⇒∅∼≅≈≠≤≥≪≫⌁⌘○◔◑◕●☐☑☒☕☮☯☺♡⚓✓✰";
            // Разбиваем файл на байты
            byte[] bytes = File.ReadAllBytes(Path);
            // Создаём Input для чтения файла
            using (var input = new OcrInput())
            {
                // Добавляем PDF, указывая набор байт
                input.AddPdf(bytes);
                // Удаляем цифровой шум
                input.DeNoise();
                // Поворачиваем изображение так, чтобы оно было правильно направлено вверх и ортогонально
                input.Deskew();
                // Начинам читать PDF
                OcrResult result = ocr.Read(input);
                // Весь резльтат записываем в переменную
                sResultOce = result.Text;
            }
            // Выводим текст
            Console.WriteLine(sResultOce);
            // Разделяем весь результат чтобы работать со строками
            string[] SplitResult = sResultOce.Split(new string[1] { "\r\n" }, StringSplitOptions.None);

            #region Паспорт выдан
            // Задаём индекс который будет отвечать за наименьшее совпадение со строкой
            int IndexMin = SplitResult.Length;
            // Задаём индекс совпадения в справочнике
            int IndexLevenshtein = 0;
            // Указываем совпадение
            int Min = 1000000000;
            // Перебираем строки
            for (int iRow = 0; iRow < SplitResult.Length; iRow++)
            {
                // Запоминаем результат
                string Row = SplitResult[iRow];
                // Если результат не равен пустоте
                if (Row != "")
                {
                    // Удаляем лишние слова которые могут спарсится
                    Row = SplitResult[iRow].Replace("Паспорт вндан", "");
                    Row = Row.Replace("Паспорт задан", "");
                    Row = Row.Replace(".", "");
                    Row = Row.Replace("-", "");
                    // Удаляем двойные пробелы
                    Row = DoubleSpace(Row);
                    // Перебираем справочник
                    for (int iIssued = 0; iIssued < MainWindow.DictionariesPasport.Issueds.Count; iIssued++)
                    {
                        // Определеяем совпадение
                        int iLevenshtein = Levenshtein(Row, MainWindow.DictionariesPasport.Issueds[iIssued]);
                        // Если совпадение с записью в справочнике меньше чем мы запомнили
                        if (iLevenshtein < Min)
                        {
                            // Запоминаем совпадение в справочнике
                            IndexLevenshtein = iIssued;
                            // Запоминаем наименьший результат
                            Min = iLevenshtein;
                            // Запоминаем индекс строки на которой был результат
                            IndexMin = iRow;
                        }
                    }
                }
            }
            // Выводим надписи
            Debug.WriteLine("Строка с наименьшим совпадением: " + SplitResult[IndexMin]);
            Debug.WriteLine("!Определено. Паспорт выдан: " + MainWindow.DictionariesPasport.Issueds[IndexLevenshtein]);
            // Запоминаем кем был выдан паспорт по наименьшому совпадению
            this.Issued = MainWindow.DictionariesPasport.Issueds[IndexLevenshtein];
            #endregion


            #region Немного чистим список и переводим его в лист, для более удобной работы
            // Создаём список, который будет содержать не проверенные строки
            List<string> ListSplitResult = new List<string>();
            // Перебираем массив
            for (int iRow = IndexMin + 1; iRow < SplitResult.Length; iRow++)
                // Если строка не пустая, заносим строку в массив
                if (SplitResult[iRow] != "")
                    ListSplitResult.Add(SplitResult[iRow]);
            #endregion

            #region Определение кода подразделения
            // Создаём маску для определения кода подразделения
            Regex regexSubdivisionCode = new Regex(@"([0-9]{3}-[0-9]{3})|([0-9]{3} [0-9]{3})");
            // Перебираем оставшиеся строки
            foreach (string Row in ListSplitResult)
            {
                // Ищем совпадение по маске
                MatchCollection matches = regexSubdivisionCode.Matches(Row);
                // Если совпадение есть
                if (matches.Count > 0)
                {
                    // Перебираем совпадения и запоминаем
                    foreach (Match match in matches)
                    {
                        Console.WriteLine("!Определено. код подразделения: " + match.Value);
                        this.SubdivisionCode = match.Value;
                    }
                }
            }
            #endregion

            #region Определение даты выдачи паспорта и рождения
            // Создаём список дат
            List<string> Dates = new List<string>();
            // Указываем маску для даты
            Regex regexDate = new Regex(@"([0-9]{1,2}(\.|:)[0-9]{1,2}(\.|:)[0-9]{4})|([0-9]{1,2}(\.|:)[0-9]{4})");
            // Перебираем строки и ищем совпадения, если совпадения есть, заносим их в массив
            foreach (string Row in ListSplitResult)
            {
                MatchCollection matches = regexDate.Matches(Row);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        Dates.Add(match.Value);
                        Console.WriteLine("Дата: " + match.Value);
                    }
                }
            }
            // Если в массиве два значения
            if (Dates.Count == 2)
            {
                // Присваиваем дате выдачи первое значение
                this.DateIssued = Dates[0];
                // Пристваиваем дате рождения второе значение
                this.DateOfBirth = Dates[1];
                // Корректируем даты, чтобы они выглядели по стандарту YYYY-MM-DD
                this.DateIssued = CorrectDate(this.DateIssued);
                this.DateOfBirth = CorrectDate(this.DateOfBirth);
                Console.WriteLine("!Определено. Дата выдачи: " + Dates[0]);
                Console.WriteLine("!Определено. Дата рождения: " + Dates[1]);
            }
            // Если в массиве одно значение
            else if (Dates.Count == 1)
            {
                // Присваиваем дате выдачи и дате рождения первое значение
                this.DateIssued = Dates[0];
                this.DateOfBirth = Dates[0];
                // Корректируем даты, чтобы они выглядели по стандарту YYYY-MM-DD
                this.DateIssued = CorrectDate(this.DateIssued);
                this.DateOfBirth = CorrectDate(this.DateOfBirth);
            }
            #endregion

            #region Определение фамилии
            List<string> DataFirstName = new List<string>();
            Regex regexFirstName = new Regex(@"[аА-яЯ]{2,20}(([оО][вВ])|([оО][вВ][аА])|([иИ][нН])|([иИ][нН][аА])|([еЕ][вВ])|([еЕ][вВ][аА]))( |$)");
            foreach (string Row in ListSplitResult)
            {
                string newRow = Row;
                newRow = newRow.Replace(",", "");
                newRow = newRow.Replace(";", "");
                newRow = newRow.Replace(".", "");
                newRow = newRow.Replace(".", ")");

                string newRow2 = "";
                for (int iChar = 0; iChar < newRow.Length; iChar++)
                    newRow2 += Char.ToUpper(newRow[iChar]);

                MatchCollection matches = regexFirstName.Matches(newRow2);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        DataFirstName.Add(match.Value);
                    }
                }
            }
            IndexMin = DataFirstName.Count;
            IndexLevenshtein = 0;
            Min = 1000000000;
            int globalIndex = 0;

            for (int iFirstName = 0; iFirstName < MainWindow.DictionariesPasport.FirstNames.Count; iFirstName++)
            {
                for (int iWord = 0; iWord < DataFirstName.Count; iWord++)
                {
                    int iLevenshtein = Levenshtein(DataFirstName[iWord], MainWindow.DictionariesPasport.FirstNames[iFirstName]);

                    if (iLevenshtein < Min)
                    {
                        IndexLevenshtein = iFirstName;
                        Min = iLevenshtein;
                        IndexMin = iWord;
                    }
                }
            }
            try
            {
                globalIndex = ListSplitResult.FindIndex(x => x.Contains(DataFirstName[IndexMin]));
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
            }

            if (DataFirstName.Count > 0)
            {
                Console.WriteLine("Строка с наибольшим совпадением: " + DataFirstName[IndexMin]);
                Console.WriteLine("!Определено. Фамилия: " + MainWindow.DictionariesPasport.FirstNames[IndexLevenshtein]);

                this.LastName = MainWindow.DictionariesPasport.FirstNames[IndexLevenshtein];
                this.LastName = CorrectName(this.LastName);
            }
            #endregion

            #region Немного чистим список и переводим его в лист, для более удобной работы
            for (int iRow = 0; iRow < globalIndex + 1; iRow++)
                ListSplitResult.Remove(ListSplitResult[0]);

            #endregion

            #region Определение имени
            // Для начала получаем все слова набранные большими буквами
            List<string> DataName = new List<string>();
            Regex regexName = new Regex(@"[аА-яЯ]{2,20}");
            foreach (string Row in ListSplitResult)
            {
                MatchCollection matches = regexName.Matches(Row);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        DataName.Add(match.Value);
                    }
                }
            }
            IndexMin = DataName.Count;
            IndexLevenshtein = 0;
            Min = 1000000000;

            for (int iName = 0; iName < MainWindow.DictionariesPasport.Names.Count; iName++)
            {
                for (int iWord = 0; iWord < DataName.Count; iWord++)
                {
                    int iLevenshtein = Levenshtein(DataName[iWord], MainWindow.DictionariesPasport.Names[iName]);

                    if (iLevenshtein < Min)
                    {
                        IndexLevenshtein = iName;
                        Min = iLevenshtein;
                        IndexMin = iWord;
                    }
                }
            }

            if (DataName.Count > 0)
            {
                Console.WriteLine("Строка с наибольшим совпадением: " + DataName[IndexMin]);
                Console.WriteLine("!Определено. Имя: " + MainWindow.DictionariesPasport.Names[IndexLevenshtein]);
                this.FirstName = MainWindow.DictionariesPasport.Names[IndexLevenshtein];
                this.FirstName = CorrectName(this.FirstName);
            }
            #endregion

            #region Определение отчества

            string sSurname = "";
            Regex regexSurname = new Regex(@"([аА-яЯ]{2,20})((ИЧ)|(ВНА))");
            foreach (string Row in ListSplitResult)
            {
                MatchCollection matches = regexSurname.Matches(Row);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        sSurname = match.Value;
                        this.SurName = match.Value;
                        this.SurName = CorrectName(this.SurName);
                        Console.WriteLine("!Определено. Отчетсво: " + match.Value);
                    }
                }
            }

            globalIndex = 0;
            try
            {
                globalIndex = ListSplitResult.FindIndex(x => x.Contains(sSurname));
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp.Message);
            }
            #endregion

            #region Немного чистим список и переводим его в лист, для более удобной работы
            for (int iRow = 0; iRow < globalIndex + 1; iRow++)
                ListSplitResult.Remove(ListSplitResult[0]);

            #endregion

            #region Определение пола
            if (sResultOce.Contains("МУЖ"))
                this.Sex = "МУЖ";
            else
                this.Sex = "ЖЕН";

            Console.WriteLine("!Определено. Пол: " + this.Sex);

            #endregion

            #region Определение места рождения
            // Для начала получаем все слова набранные большими буквами
            List<string> PlacesName = new List<string>();
            Regex regexPlaces = new Regex(@"([гГ][оО][рР] [аА-яЯ]{2,20})|([гГ] [аА-яЯ]{2,20})|([аА-яЯ]{2,20} [рР][нН])");
            foreach (string Row in ListSplitResult)
            {
                MatchCollection matches = regexPlaces.Matches(Row.Replace(".", "").Replace("-", ""));
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        PlacesName.Add(match.Value);
                    }
                }
            }
            // Поднимем все буквы в верхний регистр
            for (int i = 0; i < PlacesName.Count; i++)
            {
                string NewString = "";
                for (int iChar = 0; iChar < PlacesName[i].Length; iChar++)
                {
                    NewString += Char.ToUpper(PlacesName[i][iChar]);
                }
                PlacesName[i] = NewString;
                PlacesName[i] = PlacesName[i].Replace(".", "");
                PlacesName[i] = PlacesName[i].Replace("ГОР", "");
                PlacesName[i] = PlacesName[i].Replace("г", "");
                PlacesName[i] = PlacesName[i].Replace("РН", "");
                PlacesName[i] = PlacesName[i].Replace(" ", "");

                Console.WriteLine(PlacesName[i]);
            }


            IndexMin = ListSplitResult.Count;
            IndexLevenshtein = 0;
            Min = 1000000000;
            for (int iPlaceName = 0; iPlaceName < MainWindow.DictionariesPasport.PlacesPlaceOfBirth.Count; iPlaceName++)
            {
                for (int iRow = 0; iRow < PlacesName.Count; iRow++)
                {
                    int iLevenshtein = Levenshtein(PlacesName[iRow], MainWindow.DictionariesPasport.PlacesPlaceOfBirth[iPlaceName]);

                    if (iLevenshtein < Min)
                    {
                        IndexLevenshtein = iPlaceName;
                        Min = iLevenshtein;
                        IndexMin = iRow;
                    }
                }
            }
            if (PlacesName.Count > 0)
            {
                Console.WriteLine("Строка с наибольшим совпадением: " + PlacesName[IndexMin]);
                for (int iPlaceName = 0; iPlaceName < MainWindow.DictionariesPasport.PlacesPlaceOfBirth.Count; iPlaceName++)
                {
                    Console.WriteLine(MainWindow.DictionariesPasport.PlacesPlaceOfBirth[iPlaceName]);
                    Console.WriteLine(PlacesName[IndexMin]);
                    int iLevenshtein = Levenshtein(PlacesName[IndexMin], MainWindow.DictionariesPasport.PlacesPlaceOfBirth[iPlaceName]);
                    Console.WriteLine(iLevenshtein);
                }
                Console.WriteLine("!Определено. Место рождения: " + MainWindow.DictionariesPasport.PlacesPlaceOfBirth[IndexLevenshtein]);
                this.PlaceOfBirth = MainWindow.DictionariesPasport.PlacesPlaceOfBirth[IndexLevenshtein];
            }
            #endregion

            #region Определеение серии и нормера
            string Number = "";
            for (int i = 0; i < ListSplitResult[ListSplitResult.Count - 1].Length; i++)
            {
                if (Char.IsNumber(ListSplitResult[ListSplitResult.Count - 1][i]))
                {
                    Number += ListSplitResult[ListSplitResult.Count - 1][i];
                }

                if (Number.Length == 9)
                    break;
            }
            // пытаемся вытянуть ещё один символ
            string sLastNumber = "";
            Regex regexLastNumber = new Regex(@"<<<<<<<[0-9]");
            MatchCollection matchesNumber = regexLastNumber.Matches(ListSplitResult[ListSplitResult.Count - 1]);
            if (matchesNumber.Count > 0)
                foreach (Match match in matchesNumber)
                    sLastNumber = match.Value;

            if (sLastNumber != "")
            {
                sLastNumber = sLastNumber.Replace("<<<<<<<", "");
            }

            int Serial = 0;
            string newNumber = "";
            for (int iWord = 0; iWord < Number.Length; iWord++)
            {
                newNumber += Number[iWord];
                Serial++;

                if (Serial == 3)
                    newNumber += sLastNumber;
            }

            Console.WriteLine("!Определено. Серия и номер паспорта: " + newNumber);
            this.SerialNumber = newNumber;
            this.SerialNumber = CorrectSerialNumber(this.SerialNumber);
            Console.WriteLine("!Скорректировано. Серия и номер паспорта: " + this.SerialNumber);

            #endregion

            // Дополнительная проверка
            ParsePDFEng(this.Path);
        }
        public void ParsePDFEng(string Path)
        {
            this.Path = Path;

            string sResultOce = "";

            var ocr = new IronTesseract();
            ocr.Language = OcrLanguage.English;
            ocr.Configuration.BlackListCharacters = "—~`$#^*_}{]=[|\\@¢©«»°±·×‘’“”•…′″€™←↑→↓↔⇄⇒∅∼≅≈≠≤≥≪≫⌁⌘○◔◑◕●☐☑☒☕☮☯☺♡⚓✓✰";

            byte[] bytes = File.ReadAllBytes(Path);

            using (var input = new OcrInput())
            {
                input.AddPdf(bytes);
                input.DeNoise();
                input.Deskew();

                OcrResult result = ocr.Read(input);
                sResultOce = result.Text;
            }
            //Console.WriteLine(sResultOce);
            string[] SplitResult = sResultOce.Split(new string[1] { "\r\n" }, StringSplitOptions.None);

            // Берём две последние строки
            string FIO = SplitResult[SplitResult.Length - 2];
            string Data = SplitResult[SplitResult.Length - 1];

            // Убираем лишние пробелы из строки
            FIO = FIO.Replace(" ", "");
            Data = Data.Replace(" ", "");
            Console.WriteLine(FIO);
            Console.WriteLine(Data);

            #region Получаем дату выдачи
            try
            {
                string[] DateSplit = Data.Split(new string[1] { "<<<<<<<" }, StringSplitOptions.None);

                string Year = DateSplit[1][1].ToString() + DateSplit[1][2].ToString() + "";
                string Month = DateSplit[1][3].ToString() + DateSplit[1][4].ToString() + "";
                string Day = DateSplit[1][5].ToString() + DateSplit[1][6].ToString() + "";

                if (char.IsNumber(Year[0]) && char.IsNumber(Year[1]))
                    if (char.IsNumber(Month[0]) && char.IsNumber(Month[1]))
                        if (char.IsNumber(Day[0]) && char.IsNumber(Day[1]))
                        {
                            if (2000 + int.Parse(Year) <= DateTime.Now.Year)
                                Year = "20" + Year;
                            else
                                Year = "19" + Year;


                            string DateCorrect = CorrectDate(Year + "-" + Month + "-" + Day);

                            if (this.DateIssued != DateCorrect && DateCorrect != CorrectDate(""))
                            {
                                this.DateIssued = DateCorrect;
                                Debug.WriteLine("Даты не совпадают. Пометь поле для проверки.");
                                IsCheckDateIssued = true;
                            }
                            else
                                Debug.WriteLine("Даты совпадают. Всё хорошо. Ты лучший!");
                        }
                        else
                            IsCheckDateIssued = true;
                    else
                        IsCheckDateIssued = true;
                else
                    IsCheckDateIssued = true;

            }
            catch (Exception exp)
            {
                Debug.WriteLine("Не смог получить дату для проверки");
                IsCheckDateIssued = true;
            }
            #endregion

            #region Првоерка даты рождения
            try
            {
                string[] DateSplit = Data.Split(new string[1] { "RUS" }, StringSplitOptions.None);

                string Year = DateSplit[1][0].ToString() + DateSplit[1][1].ToString() + "";
                string Month = DateSplit[1][2].ToString() + DateSplit[1][3].ToString() + "";
                string Day = DateSplit[1][4].ToString() + DateSplit[1][5].ToString() + "";

                if (char.IsNumber(Year[0]) && char.IsNumber(Year[1]))
                    if (char.IsNumber(Month[0]) && char.IsNumber(Month[1]))
                        if (char.IsNumber(Day[0]) && char.IsNumber(Day[1]))
                        {
                            if (2000 + int.Parse(Year) <= DateTime.Now.Year)
                                Year = "20" + Year;
                            else
                                Year = "19" + Year;

                            string DateCorrect = CorrectDate(Year + "-" + Month + "-" + Day);

                            if (this.DateOfBirth != DateCorrect && DateCorrect != CorrectDate(""))
                            {
                                this.DateOfBirth = DateCorrect;
                                Debug.WriteLine("Даты не совпадают. Пометь поле для проверки.");
                                IsCheckDateOfBirth = true;
                            }
                            else
                                Debug.WriteLine("Даты совпадают. Всё хорошо. Ты лучший!");
                        }
                        else
                            IsCheckDateOfBirth = true;
                    else
                        IsCheckDateOfBirth = true;
                else
                    IsCheckDateOfBirth = true;
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Не смог получить дату для проверки");
                IsCheckDateOfBirth = true;
            }
            #endregion

            #region Проврка кода подразделения
            try
            {
                string[] DateSplit = Data.Split(new string[1] { "<<<<<<<" }, StringSplitOptions.None);

                string First = DateSplit[1][7].ToString() + DateSplit[1][8].ToString() + DateSplit[1][9].ToString();
                string Two = DateSplit[1][10].ToString() + DateSplit[1][11].ToString() + DateSplit[1][12].ToString();

                if (Char.IsNumber(DateSplit[1][7]) &&
                    Char.IsNumber(DateSplit[1][8]) &&
                    Char.IsNumber(DateSplit[1][9]) &&
                    Char.IsNumber(DateSplit[1][10]) &&
                    Char.IsNumber(DateSplit[1][11]) &&
                    Char.IsNumber(DateSplit[1][12]))
                {

                    if (this.SubdivisionCode != First + "-" + Two)
                    {
                        this.SubdivisionCode = First + "-" + Two;
                        Debug.WriteLine("Код подразделения не совпадает. Пометь поле для проверки.");
                        IsCheckSubdivisionCode = true;
                    }
                    else
                        Debug.WriteLine("Код подразделения совпадает. Всё хорошо. Ты лучший!");
                }
                else
                    IsCheckSubdivisionCode = true;
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Не смог получить код подразделения для проверки");
                IsCheckSubdivisionCode = true;
            }
            #endregion

            #region Проверка серии и номера
            try
            {
                string[] DateSplit = Data.Split(new string[1] { "<<<<<<<" }, StringSplitOptions.None);

                string Serial = Data[0].ToString() + Data[1].ToString() + " " + Data[2].ToString() + DateSplit[1][0].ToString();
                string Number = Data[3].ToString() +
                                Data[4].ToString() +
                                Data[5].ToString() +
                                Data[6].ToString() +
                                Data[7].ToString() +
                                Data[8].ToString();

                if (char.IsNumber(Data[0]) &&
                    char.IsNumber(Data[1]) &&
                    char.IsNumber(Data[2]) &&
                    char.IsNumber(Data[3]) &&
                    char.IsNumber(Data[4]) &&
                    char.IsNumber(Data[5]) &&
                    char.IsNumber(Data[6]) &&
                    char.IsNumber(Data[7]) &&
                    char.IsNumber(Data[8]) &&
                    char.IsNumber(DateSplit[1][0]))
                {

                    if (this.SerialNumber != Serial + " " + Number)
                    {
                        this.SerialNumber = Serial + " " + Number;
                        Debug.WriteLine("Серия и номер не совпадает. Пометь поле для проверки.");
                        IsCheckSerialNumber = true;
                    }
                    else
                        Debug.WriteLine("Серия и номер совпадает. Всё хорошо. Ты лучший!");
                }
                else
                    IsCheckSerialNumber = true;
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Не смог получить серию и номер для проверки");
                IsCheckSerialNumber = true;
            }
            #endregion
        }
        /// <summary>
        /// Алгоритм класного чела на совпадения
        /// </summary>
        /// <param name="a">Входная строка</param>
        /// <param name="b">Сравниваемая строка</param>
        /// <returns></returns>
        private Int32 Levenshtein(String a, String b)
        {
            if (string.IsNullOrEmpty(a))
            {
                if (!string.IsNullOrEmpty(b))
                {
                    return b.Length;
                }
                return 0;
            }

            if (string.IsNullOrEmpty(b))
            {
                if (!string.IsNullOrEmpty(a))
                {
                    return a.Length;
                }
                return 0;
            }
            Int32 cost;
            Int32[,] d = new int[a.Length + 1, b.Length + 1];
            Int32 min1;
            Int32 min2;
            Int32 min3;
            for (Int32 i = 0; i <= d.GetUpperBound(0); i += 1)
            {
                d[i, 0] = i;
            }
            for (Int32 i = 0; i <= d.GetUpperBound(1); i += 1)
            {
                d[0, i] = i;
            }
            for (Int32 i = 1; i <= d.GetUpperBound(0); i += 1)
            {
                for (Int32 j = 1; j <= d.GetUpperBound(1); j += 1)
                {
                    cost = Convert.ToInt32(!(a[i - 1] == b[j - 1]));

                    min1 = d[i - 1, j] + 1;
                    min2 = d[i, j - 1] + 1;
                    min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return d[d.GetUpperBound(0), d.GetUpperBound(1)];
        }
        private string DoubleSpace(string Value)
        {
            string newValue = "";
            string[] Data = Value.Split(' ');
            for (int i = 0; i < Data.Length; i++)
            {
                if (Data[i] != "")
                {
                    newValue += Data[i];
                    if (i < Data.Length - 1) newValue += " ";
                }
            }
            return newValue;
        }
        private string CorrectName(string Value)
        {
            string newValue = "";

            for (int iChar = 0; iChar < Value.Length; iChar++)
            {
                if (iChar == 0)
                    newValue += Char.ToUpper(Value[iChar]);
                else
                    newValue += Char.ToLower(Value[iChar]);
            }

            return newValue;
        }
        private string CorrectDate(string Value)
        {
            DateTime date = new DateTime();
            DateTime.TryParse(Value, out date);

            string sMonth = date.Month.ToString();
            if (date.Month < 10) sMonth = "0" + date.Month;

            string sDay = date.Day.ToString();
            if (date.Day < 10) sDay = "0" + date.Day;

            return date.Year + "-" + sMonth + "-" + sDay;
        }
        private string CorrectSerialNumber(string Value)
        {
            string newString = "";

            try
            {
                newString += Value[0].ToString();
            }
            catch (Exception exp)
            {
                newString += "0";
            }
            try
            {
                newString += Value[1].ToString();
            }
            catch (Exception exp)
            {
                newString += "0";
            }
            newString += " ";
            try
            {
                newString += Value[2].ToString();
            }
            catch (Exception exp)
            {
                newString += "0";
            }
            try
            {
                newString += Value[3].ToString();
            }
            catch (Exception exp)
            {
                newString += "0";
            }
            newString += " ";
            try
            {
                newString += Value[4].ToString();
            }
            catch (Exception exp)
            {
                newString += "0";
            }
            try
            {
                newString += Value[5].ToString();
            }
            catch (Exception exp)
            {
                newString += "0";
            }
            try
            {
                newString += Value[6].ToString();
            }
            catch (Exception exp)
            {
                newString += "0";
            }
            try
            {
                newString += Value[7].ToString();
            }
            catch (Exception exp)
            {
                newString += "0";
            }
            try
            {
                newString += Value[8].ToString();
            }
            catch (Exception exp)
            {
                newString += "0";

            }
            try
            {
                newString += Value[9].ToString();
            }
            catch (Exception exp)
            {
                newString += "0";
            }

            return newString;
        }
    }
}
