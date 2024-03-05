using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lr2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CommandBinding saveCommand = new CommandBinding(ApplicationCommands.Save, execute_Save, canExecute_Save);
            CommandBindings.Add(saveCommand);

            CommandBinding cutCommand = new CommandBinding(ApplicationCommands.Cut, execute_Cut, canExecute_Cut);
            CommandBindings.Add(cutCommand);

            CommandBinding openCommand = new CommandBinding(ApplicationCommands.Open, execute_Open, canExecute_Open);
            CommandBindings.Add(openCommand);
        }

        void canExecute_Save(object sender, CanExecuteRoutedEventArgs e)
        {
            if (inputTextBox.Text.Trim().Length > 0)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }

        }
        void execute_Save(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, inputTextBox.Text);
            }
        }

        void canExecute_Cut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

        }
        void execute_Cut(object sender, ExecutedRoutedEventArgs e)
        {
            inputTextBox.Clear();
        }

        void canExecute_Open(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        void execute_Open(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == true)
            {
                inputTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

    }
}