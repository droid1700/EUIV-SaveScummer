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

namespace EUIV_SaveScummer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private FileOps fileops;

		public MainWindow()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Load button click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LoadButton_Click(object sender, RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.DefaultExt = ".eu4";
			dlg.Filter = "EU4 Game Files (.eu4)|*.eu4";

			//Build the default path
			string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			path += "\\Paradox Interactive\\Europa Universalis IV\\save games";

			dlg.InitialDirectory = path;

			if(dlg.ShowDialog().Value)
			{
				//Create the task that will continue to run
				Task.Factory.StartNew(() =>
				{
					fileops = new FileOps(dlg.FileName);

					while(true)
					{
						if(fileops.FileChanged())
						{
							//Save a copy of the file
							fileops.SaveScum();
						}

						//Check again in 5 minutes
						Thread.Sleep(300000);
					}
				}, CancellationToken.None,
				TaskCreationOptions.LongRunning,
				TaskScheduler.Default
				);
			}
		}

		/// <summary>
		/// Set where to save copies of the save game
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DirSetButton_Click(object sender, RoutedEventArgs e)
		{
			//https://stackoverflow.com/questions/11624298/how-do-i-use-openfiledialog-to-select-a-folder

			if(fileops == null)
			{
				return;
			}

			System.Windows.Forms.FolderBrowserDialog fdb = new System.Windows.Forms.FolderBrowserDialog();
			System.Windows.Forms.DialogResult result = fdb.ShowDialog();

			if(result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrEmpty(fdb.SelectedPath))
			{
				fileops.SetSaveDir(fdb.SelectedPath);
			}
		}
	}
}
