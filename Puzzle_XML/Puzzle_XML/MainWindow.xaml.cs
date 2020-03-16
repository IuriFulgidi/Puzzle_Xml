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
        //creazione lista
        List<Puzzle> puzzles = new List<Puzzle>();

        //token per lo stop
        CancellationTokenSource ct = new CancellationTokenSource();

        //token per il lock dell'azione principale
        object padlock = new object();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btn_visualize_Click(object sender, RoutedEventArgs e)
        {
            if (ct.Token.IsCancellationRequested)
                ct = new CancellationTokenSource();
            lst_collection.Items.Clear();
            puzzles.Clear();
            //Task.Factory.StartNew(()=>Load());
            Task load = new Task(() =>
            {
                lock (padlock)
                    Load();
            });
            load.Start();
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
            var xmlpuzzle = xmlpuzzles.Elements("puzzle");

            //scorre la lista
            foreach (var item in xmlpuzzle)
            {
                //se c'è stata richiesta di stop si esce dal ciclo
                if (ct.Token.IsCancellationRequested)
                    break;

                //aggiunta elemento
                Add(item);

                //aggiunta alla listbox
                Dispatcher.Invoke(() => Update(puzzles));

                //attesa che simula calcoli pesanti
                Thread.Sleep(1500);
            }
        }

        /// <summary>
        /// aggiorna interafaccia
        /// </summary>
        private void Update(List<Puzzle> list)
        {
            lst_collection.Items.Clear();
            foreach (var item in list)
                lst_collection.Items.Add(item);
        }

        /// <summary>
        /// aggiunta puzzle alla lista
        /// </summary>
        /// <param name="item"></param>
        private void Add(XElement item)
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

            //aggiunta puzzle alla lista
            puzzles.Add(p);
        }

        /// <summary>
        /// ferma il caricamento dei dati
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_stop_Click(object sender, RoutedEventArgs e)
        {
            ct.Cancel();
        }
    }
}
