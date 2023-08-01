using Scan_Russing_Passport.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Scan_Russing_Passport
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static PassportData DictionariesPasport = new PassportData();

        public MainWindow()
        {
            InitializeComponent();

            // Инициализируем класс паспорта
            Passport ScanPassport = new Passport();
            // Вызываем сканирование паспорта
            ScanPassport.ParsePDF(@"F:\Doki\doc00362720230518111102.pdf");
        }
    }
}
