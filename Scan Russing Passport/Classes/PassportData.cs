using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scan_Russing_Passport.Classes
{
    public class PassportData
    {
        public List<string> Issueds = new List<string>();
        public List<string> FirstNames = new List<string>();
        public List<string> Names = new List<string>();
        public List<string> PlacesPlaceOfBirth = new List<string>();
        public PassportData() {
            StreamReader sr = new StreamReader("russian_surnames.csv");
            while (!sr.EndOfStream)
            {
                string[] Data = sr.ReadLine().Split(';');

                string Surname = "";
                for (int i = 0; i < Data[1].Length; i++)
                {
                    Surname += Char.ToUpper(Data[1][i]);
                }
                FirstNames.Add(Surname);
            }
            sr.Close();

            sr = new StreamReader("russian_names.csv");
            while (!sr.EndOfStream)
            {
                string[] Data = sr.ReadLine().Split(';');

                string Name = "";
                for (int i = 0; i < Data[1].Length; i++)
                {
                    Name += Char.ToUpper(Data[1][i]);
                }
                Names.Add(Name);
            }
            sr.Close();

            #region База УФМС
            Issueds.Add("ГУ МВД РОССИИ ПО ПЕРМСКОМУ КРАЮ");
            Issueds.Add("УМВД РОССИИ ПО КИРОВСКОЙ ОБЛАСТИ");
            Issueds.Add("ОТДЕЛОМ УФМС РОССИИ ПО ПЕРМСКОМУ КРАЮ в СВЕРДЛОВСКРМ РАЙОНЕ ГОРОДА ПЕРМИ");
            Issueds.Add("ГУ МВД ПО ПЕРМСКОМУ КРАЮ");
            Issueds.Add("МВД ПО РЕСПУБЛИКЕ АДЫГЕЯ");
            Issueds.Add("ГУ МВД РОССИИ ПО ЧЕЛЯБИНСКОЙ ОБЛАСТИ");
            Issueds.Add("УМВД РОССИИ ПО БЕЛГОРОДСКОЙ ОБЛАСТИ");
            Issueds.Add("МВД ПО УДМУРСКОЙ РЕСПУБЛИКЕ");
            Issueds.Add("УМВД РОССИИ ПО ХАНТЫ-МАНСИЙСКОМУ АВТОНОМНОМУ ОКРУГУ - ЮРГЕ");
            Issueds.Add("ГУ МВД РОССИИ ПО СВЕРДЛОВСКОЙ ОБЛАСТИ");
            Issueds.Add("ГУ МВД ПО РЕСПУБЛИКЕ ТАТАРСТАН");
            Issueds.Add("МВД ПО РЕСПУБЛИКЕ БАШКОРТОСТАН");
            Issueds.Add("ГУ МВД РОССИИ ПО НОВОСИБИРСКОЙ ОБЛАСТИ");

            #endregion

            #region Места Рождения
            PlacesPlaceOfBirth.Add("БАРДЫМСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("БЕРЕЗОВСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("БОЛЬШЕСОСНОВСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ВЕРЕЩАГИНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ГАЙНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ГОРНОЗАВОДСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ЕЛОВСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ИЛЬИНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("КАРАГАЙСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("КИШЕРТСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("КОСИНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("КОЧЕВСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("КРАСНОВИШЕРСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("КРАСНОКАМСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("КУДЫМКАРСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("КУЕДИНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("КУНГУРСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("НЫТВЕНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ОКТЯБРЬСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ОРДИНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ОСИНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ОХАНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ОЧЕРСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ПЕРМСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("СИВИНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("СОЛИКАМСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("СУКСУНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("УИНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("УСОЛЬСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ЧАСТИНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ЧЕРДЫНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ЧЕРНУШИНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ЮРЛИНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ЮСЬВИНСКИЙ Р-Н");
            PlacesPlaceOfBirth.Add("ЧУСОВСКОЙ Р-Н");
            PlacesPlaceOfBirth.Add("Г. АЛЕКСАНДРОВСК");
            PlacesPlaceOfBirth.Add("Г. БЕРЕЗНИКИ");
            PlacesPlaceOfBirth.Add("Г. ГРЕМЯЧИНСК");
            PlacesPlaceOfBirth.Add("Г. ГУБАХА");
            PlacesPlaceOfBirth.Add("Г. ДОБРЯНКА");
            PlacesPlaceOfBirth.Add("Г. КИЗЕЛ");
            PlacesPlaceOfBirth.Add("Г. КРАСНОКАМСК");
            PlacesPlaceOfBirth.Add("Г. КУДЫМКАР");
            PlacesPlaceOfBirth.Add("Г. КУНГУР");
            PlacesPlaceOfBirth.Add("Г. ЛЫСЬВА");
            PlacesPlaceOfBirth.Add("Г. ПЕРМЬ");
            PlacesPlaceOfBirth.Add("Г. СОЛИКАМСК");
            PlacesPlaceOfBirth.Add("Г. ЧАЙКОВСКИЙ");
            PlacesPlaceOfBirth.Add("Г. ЧУСОВОЙ");
            PlacesPlaceOfBirth.Add("Г. НЫТВА");
            #endregion
        }
    }
}
