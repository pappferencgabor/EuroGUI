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
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;

namespace EuroGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=eurovizio;";
        private readonly MySqlConnection connection;
        private readonly List<Dal> dalLista;

        public MainWindow()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
            connection.Open();
            dalLista = new List<Dal>();
            string queryText = "select ev, eloado, cim, helyezes, pontszam, orszag from dal";
            MySqlCommand command = new MySqlCommand(queryText, connection);
            MySqlDataReader reader = command.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    dalLista.Add(new Dal(reader));
                }
                reader.Close();
                dgResult.ItemsSource = dalLista;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            connection.Close();
        }

        private void btnFeladat4_Click(object sender, RoutedEventArgs e)
        {
            int versenyzok = 0;
            int helyezes = 999999999;

            foreach (Dal dal in dalLista)
            {
                if (dal.Orszag == "Magyarország")
                {
                    versenyzok++;

                    if (dal.Helyezes < helyezes)
                    {
                        helyezes = dal.Helyezes;
                    }
                }
            }
            MessageBox.Show($"A magyar versenyzők száma: {versenyzok}\nA legjobb helyetés: {helyezes}.");
        }

        private void btnFeladat5_Click(object sender, RoutedEventArgs e)
        {
            int ossz = 0;
            int versenyzok = 0;

            foreach (Dal dal in dalLista)
            {
                if (dal.Orszag == "Németország")
                {
                    versenyzok++;
                    ossz += dal.Pontszam;
                }
            }
            double avg = ossz / versenyzok;
            MessageBox.Show($"A német versenyzők átlagos pontszáma: {Math.Round(avg, 2)}");
        }

        private void btnFeladat6_Click(object sender, RoutedEventArgs e)
        {
            string fuzott = "";

            foreach (Dal dal in dalLista)
            {
                if (dal.Cim.Contains("Luck") || dal.Eloado.Contains("Luck"))
                {
                    fuzott += $"{dal.Eloado} - {dal.Cim}, ";
                }
            }
            MessageBox.Show(fuzott.Substring(0, fuzott.Length-2));
        }

        private void btnFeladat7_Click(object sender, RoutedEventArgs e)
        {
            foreach (Dal dal in dalLista)
            {
                if (dal.Eloado.Contains(txtKeresett.Text))
                {
                    lbEredmeny.Items.Add(dal.Cim);
                }
            }
        }

        private void btnFeladat8_Click(object sender, RoutedEventArgs e)
        {
            if (dgResult.SelectedIndex != -1)
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"select datum from verseny where ev={dgResult.SelectedIndex}", connection);
                MySqlDataReader data = command.ExecuteReader();

                while (data.Read())
                {
                    lblDatum.Content = $"Verseny dátuma: {data.GetDateTime(0)}";
                }

            }
        }
    }
}
