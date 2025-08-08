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
using static SampleMaskedTextBox.UI.Control.MaskedTextBoxControl;

namespace SampleMaskedTextBox.UI.View
{
    /// <summary>
    /// Interação lógica para Home.xam
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();

            txtBoxMain.Focus();

            cmbMask.ItemsSource = Enum.GetValues(typeof(MaskTypes));
            cmbMask.SelectedItem = txtBoxMain.Mask;
        }

        private void cmbMask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbMask.SelectedItem is MaskTypes mask)
            {
                txtBoxMain.Mask = mask;
                txtBoxRegex.IsEnabled = mask == MaskTypes.None;
                txtBoxRegex.Clear();
            }
        }

        private void txtBoxRegex_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtBoxMain.Clear();
            txtBoxMain.RegexPattern = txtBoxRegex.Text;
        }
    }
}
