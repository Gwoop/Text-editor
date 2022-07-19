using Microsoft.Win32;
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
using System.IO;

namespace _02._25
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string FileName = "";

        public MainWindow()
        {
            InitializeComponent();
            btn_FontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            btn_FontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            btn_LineSpacing.ItemsSource = new List<string>() { "0,5", "0,75", "1", "1,25", "1,5", "1,75", "2", "2,5" };

        }

        private void Open(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Rich Text|*.rtf|All Files|*";
            if (dialog.ShowDialog() == true)
            {
                FileName = dialog.FileName;
                this.Title = FileName;
                FileStream fileStream = new FileStream(FileName, FileMode.Open);
                TextRange range = new TextRange(MainText.Document.ContentStart, MainText.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (FileName != "")
            {
                this.Title = FileName;
                System.IO.File.WriteAllText(FileName, string.Empty);
                FileStream fileStream = new FileStream(FileName, FileMode.OpenOrCreate);
                TextRange range = new TextRange(MainText.Document.ContentStart, MainText.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
            else SaveAs(sender, e);
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Rich Text|*.rtf|All Files|*";
            if (dialog.ShowDialog() == true)
            {
                FileName = dialog.FileName;
                Save(sender, e);
            }
        }

        private void SaveExit(object sender, RoutedEventArgs e)
        {
            Save(sender, e);
            Application.Current.Shutdown();
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you like to save file before exiting?", "Exit?", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    SaveExit(sender, e);
                    break;
                case MessageBoxResult.No:
                    Application.Current.Shutdown();
                    break;
            }
        }

        private void Print(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if ((pd.ShowDialog() == true))
            {
                //pd.PrintVisual(MainText as Visual, "printing as visual");
                pd.PrintDocument((((IDocumentPaginatorSource)MainText.Document).DocumentPaginator), "printing as paginator");
            }
        }

        private void SetColor(object sender, RoutedEventArgs e)
        {
            
        }

        private void SelectionChanged(object sender, RoutedEventArgs e)
        {
          
            object tmp = MainText.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btn_Bold.IsChecked = (tmp != DependencyProperty.UnsetValue) && (tmp.Equals(FontWeights.Bold));
            tmp = MainText.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btn_Italic.IsChecked = (tmp != DependencyProperty.UnsetValue) && (tmp.Equals(FontStyles.Italic));
            tmp = MainText.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btn_Underline.IsChecked = (tmp != DependencyProperty.UnsetValue) && (tmp.Equals(TextDecorations.Underline));

            /*tmp = MainText.Selection.GetPropertyValue(Inline.BaselineAlignmentProperty);
            btn_AlignLeft.IsChecked = (tmp != DependencyProperty.UnsetValue) && (tmp.Equals(TextAlignment.Left));*/

            tmp = MainText.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            btn_FontFamily.SelectedItem = tmp;
            tmp = MainText.Selection.GetPropertyValue(Inline.FontSizeProperty);
            if (tmp != DependencyProperty.UnsetValue && tmp != null)
                btn_FontSize.SelectedItem = tmp;
            else btn_FontSize.SelectedItem = "";



        }

        private void FontFamilySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (btn_FontFamily.SelectedItem != null)
                MainText.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, btn_FontFamily.SelectedItem);
        }

        private void FontSizeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextSelection selectedText = MainText.Selection;
            object pr_font = selectedText.GetPropertyValue(Inline.FontSizeProperty);
            if (selectedText.Start.GetOffsetToPosition(selectedText.End) == 0) return;
            if (float.TryParse(btn_FontSize.SelectedItem.ToString(), out _) &&
                pr_font.ToString() != btn_FontSize.SelectedItem.ToString())
                selectedText.ApplyPropertyValue(Inline.FontSizeProperty, btn_FontSize.SelectedItem.ToString());
        }

        private void FontColorSelectionChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            TextSelection selectedText = MainText.Selection;
            Color c = (Color)e.NewValue;
            if (c != null)
                selectedText.ApplyPropertyValue(TextElement.ForegroundProperty,
                    (SolidColorBrush)(new BrushConverter().ConvertFrom(c.ToString())));
        }

        private void BackColorSelectionChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            TextSelection selectedText = MainText.Selection;
            Color c = (Color)e.NewValue;
            if (c != null)
                selectedText.ApplyPropertyValue(TextElement.BackgroundProperty,
                    (SolidColorBrush)(new BrushConverter().ConvertFrom(c.ToString())));
        }

        private void LineSpacingSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextSelection selectedText = MainText.Selection;
            if (btn_LineSpacing.SelectedItem.ToString() != "")
                selectedText.ApplyPropertyValue(TextBlock.LineHeightProperty,
                    Convert.ToDouble(btn_LineSpacing.SelectedItem) * 12);
        }

    }
}
