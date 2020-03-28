using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Xml.Linq;

namespace Puzzle_XML
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //token per lo stop
        CancellationTokenSource ct = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();


        }

        private void Btn_visualize_Click(object sender, RoutedEventArgs e)
        {
            if (ct.Token.IsCancellationRequested)
                ct = new CancellationTokenSource();
            lst_collection.Items.Clear();

            //caricamento
            Task.Factory.StartNew(()=>Load());

            //disattivamento del buttone
            btn_visualize.IsEnabled=false;
        }

        /// <summary>
        /// cerica la lista
        /// </summary>
        private void Load()
        {
            //cerca la lista di puzzle
            string path = @"Collezione.xml";
            XDocument xmlDoc = XDocument.Load(path);
            XElement xmlpuzzles = xmlDoc.Element("puzzles");
            var xmlpuzzlelist = xmlpuzzles.Elements("puzzle");

            //scorre la lista
            foreach (var xmlpuzzle in xmlpuzzlelist)
            {
                //se c'è stata richiesta di stop si esce dal ciclo
                if (ct.Token.IsCancellationRequested)
                    break;

                //creazione nuovo puzzle
                Puzzle p = NuovoPuzzle(xmlpuzzle);

                //aggiunta alla listbox
                Dispatcher.Invoke(() => Update(p));

                //attesa che simula calcoli pesanti
                Thread.Sleep(1500);
            }

            //riattivamento del bottone di visualizzazione
            Dispatcher.Invoke(() => btn_visualize.IsEnabled = true);
        }

        /// <summary>
        /// aggiorna interafaccia
        /// </summary>
        private void Update(Puzzle p)
        {
            lst_collection.Items.Add(p);
        }

        /// <summary>
        /// aggiunta puzzle alla lista
        /// </summary>
        /// <param name="item"></param>
        private Puzzle NuovoPuzzle(XElement item)
        {
            //ricavo informazioni da xml
            XElement xmlName = item.Element("name");
            XElement xmlstates = item.Element("nStates");
            XElement xmlSolved = item.Element("solved");
            XElement xmlfaces = item.Element("nFaces");

            //creazione puzzle
            Puzzle p = new Puzzle();
            p.Name = xmlName.Value;
            p.NStates = xmlstates.Value;
            p.NFaces = Convert.ToInt32(xmlfaces.Value);
            if (xmlSolved.Value == "yes")
                p.Solved = true;
            else
                p.Solved = false;

            return p;
        }

        /// <summary>
        /// ferma il caricamento dei dati
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_stop_Click(object sender, RoutedEventArgs e)
        {
            ct.Cancel();

            //riattivamento del bottone di visualizzazione
            btn_visualize.IsEnabled = true;
        }

        private void Lst_collection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Puzzle p = lst_collection.SelectedItem as Puzzle;

            lst_selectedPuzzle.Items.Clear();

            if (p != null)
            {
                lst_selectedPuzzle.Items.Add(p.Name);
                lst_selectedPuzzle.Items.Add($"it has {p.NFaces} faces");
                lst_selectedPuzzle.Items.Add($"it has {p.NStates} possible states");
                if (p.Solved)
                    lst_selectedPuzzle.Items.Add($"is solved");
                else
                    lst_selectedPuzzle.Items.Add($"is not solved");
            }
        }
    }
}
