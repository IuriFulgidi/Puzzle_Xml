using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml.Serialization;

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

            btn_scrambleSolve.Visibility = Visibility.Hidden;
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
        /// carica la lista
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
                Thread.Sleep(1000);
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
            XElement xmlShape = item.Element("shape");
            XElement xmlSolved = item.Element("solved");
            XElement xmlFaces = item.Element("nFaces");
            XElement xmlId = item.Element("id");

            //creazione puzzle
            Puzzle p = new Puzzle();
            p.Name = xmlName.Value;
            p.Shape = xmlShape.Value;
            p.NFaces = Convert.ToInt32(xmlFaces.Value);
            p.Solved = xmlSolved.Value;
            p.NFaces = Convert.ToInt32(xmlId.Value);

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
            //si prende il puzzle selezionato
            Puzzle p = lst_collection.SelectedItem as Puzzle;
            lst_selectedPuzzle.Items.Clear();

            if (p != null)
            {
                //aggiunta caratteristicche del puzzle alla listbox
                lst_selectedPuzzle.Items.Add(p.Name);
                lst_selectedPuzzle.Items.Add($"it has {p.NFaces} faces");
                lst_selectedPuzzle.Items.Add($"it has {p.Shape} possible states");
                if (p.Solved=="yes")
                {
                    lst_selectedPuzzle.Items.Add($"is solved");
                    btn_scrambleSolve.Content = "Scramlbe?";
                }
                else
                {
                    lst_selectedPuzzle.Items.Add($"is not solved");
                    btn_scrambleSolve.Content = "Solve?";
                }
                btn_scrambleSolve.Visibility = Visibility.Visible;
            }
            else
                btn_scrambleSolve.Visibility = Visibility.Hidden;
        }

        private void Btn_scrambleSolve_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(()=>UpdateCollection());
            Puzzle p = lst_collection.SelectedItem as Puzzle;
            if (p.Solved == "yes")
                MessageBox.Show($"{p.Name} succefully scrambled", "Update!", MessageBoxButton.OK, MessageBoxImage.Information);
            else
                MessageBox.Show($"{p.Name} succefully solved", "Update!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UpdateCollection()
        {
            //percorso del file originale
            string path = @"Collezione.xml";

            //si prende l'id del puzzle selezionato
            int id = Dispatcher.Invoke(()=>GetId());

            //si parte dalla radice
            XElement xmlCollezione = new XElement("puzzles");

            //si scorre tutto il vecchio file pre creare il nuovo
            foreach (Puzzle puzzle in lst_collection.Items)
            {
                //si controlla per id
                if (puzzle.Id == id)
                {
                    if (puzzle.Solved == "yes")
                        puzzle.Solved = "no";
                    else
                        puzzle.Solved = "yes";
                }

                //si crea il nuovo puzzle
                XElement xmlPuzzle = new XElement("puzzle");
                XElement xmlName = new XElement("name", puzzle.Name);
                XElement xmlShape = new XElement("shape", puzzle.Shape);
                XElement xmlNFaces = new XElement("nFaces", puzzle.NFaces);
                XElement xmlId = new XElement("id", puzzle.Id);
                XElement xmlSolved = new XElement("solved", puzzle.Solved);

                //si danno le proprietà al puzzle
                xmlPuzzle.Add(xmlName);
                xmlPuzzle.Add(xmlShape);
                xmlPuzzle.Add(xmlNFaces);
                xmlPuzzle.Add(xmlId);
                xmlPuzzle.Add(xmlSolved);

                //si aggiunge il puzzle alla collezione
                xmlCollezione.Add(xmlPuzzle);
            }

            //si salva il file
            xmlCollezione.Save(path);
        }

        private int GetId()
        {
            return (lst_collection.SelectedItem as Puzzle).Id;
        }
    }
}
