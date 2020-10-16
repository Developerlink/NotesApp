using NotesApp.Model;
using NotesApp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NotesApp.View
{
    /// <summary>
    /// Interaction logic for NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
        NotesVM vm;

        public NotesWindow()
        {
            InitializeComponent();

            // Using the vm from the xaml as the same vm in codebehind.
            vm = this.Resources["vm"] as NotesVM;
            container.DataContext = vm;
            vm.SelectedNoteChanged += ViewModel_SelectedNoteChanged;

            SetKeyCombinations();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (string.IsNullOrEmpty(App.UserId))
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();
            }
        }

        private void SetKeyCombinations()
        {
            RoutedCommand saveFileCmd = new RoutedCommand();
            saveFileCmd.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(saveFileCmd, SaveFileButton_Click));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ContentRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int amountOfCharacters = (new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd)).Text.Length;

            StatusTextBox.Text = $"Document length: {amountOfCharacters} characters";
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonEnabled = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonEnabled)
            {
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Bold);
            }
            else
            {
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontWeightProperty, FontWeights.Normal);
            }
        }

        private void ContentRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedWeight = contentRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            boldButton.IsChecked = (selectedWeight.Equals(FontWeights.Bold));

            // This is not working properly
            var selectedStyle = contentRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            boldButton.IsChecked = (selectedStyle == DependencyProperty.UnsetValue) && (selectedStyle.Equals(FontStyles.Italic));

            // This is not working properly
            var selectedDecoration = contentRichTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            boldButton.IsChecked = (selectedDecoration == DependencyProperty.UnsetValue) && (selectedDecoration.Equals(TextDecorations.Underline));

            fontFamilyComboBox.SelectedValue = contentRichTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            fontSizeComboBox.Text = contentRichTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty).ToString();
        }

        private void ItalicsButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonEnabled = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonEnabled)
            {
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Italic);
            }
            else
            {
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontStyleProperty, FontStyles.Normal);
            }
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            bool isButtonEnabled = (sender as ToggleButton).IsChecked ?? false;
            if (isButtonEnabled)
            {
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, TextDecorations.Underline);
            }
            else
            {
                TextDecorationCollection textDecorations;
                (contentRichTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty) as TextDecorationCollection).TryRemove(TextDecorations.Underline, out textDecorations);
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, textDecorations);
            }
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (contentRichTextBox != null && fontFamilyComboBox.SelectedItem != null)
            {
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, fontFamilyComboBox.SelectedItem);
            }
        }

        private void FontSizeComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (fontSizeComboBox.SelectedItem != null)
            {
                // Using combobox text because the user can write in their own value
                contentRichTextBox.Selection.ApplyPropertyValue(Inline.FontSizeProperty, fontSizeComboBox.Text);
            }
        }

        private void ViewModel_SelectedNoteChanged(object sender, EventArgs e)
        {
            contentRichTextBox.Document.Blocks.Clear();
            if (vm.SelectedNote != null && !string.IsNullOrEmpty(vm.SelectedNote.FileLocation))
            {
                using (FileStream fileStream = new FileStream(vm.SelectedNote.FileLocation, FileMode.Open))
                {
                    TextRange range = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd);
                    range.Load(fileStream, DataFormats.Rtf);
                }
            }
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (contentRichTextBox.IsFocused == true || contentTitleTextBox.IsFocused == true)
            {
                if (vm.SelectedNote != null)
                {
                    string rtfFile = System.IO.Path.Combine(Environment.CurrentDirectory, $"files/{vm.SelectedNote.Id}-{vm.SelectedNote.Title}.rtf");
                    vm.SelectedNote.FileLocation = rtfFile;

                    using (FileStream fileStream = new FileStream(rtfFile, FileMode.Create))
                    {
                        TextRange range = new TextRange(contentRichTextBox.Document.ContentStart, contentRichTextBox.Document.ContentEnd);
                        range.Save(fileStream, DataFormats.Rtf);
                        MessageBox.Show("File has been saved");
                    }

                    vm.UpdateSelectedNote();
                }
            }
        }



        private void NotebookList_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.D1))
            {
                MessageBox.Show("You just made the key combination ctrl+1 from notebook list");
            }
        }

        private void NoteList_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.D1))
            {
                MessageBox.Show("You just made the key combination ctrl+1 from note list");
            }
        }

        private void Textbox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.D1))
            {
                MessageBox.Show("You just made the key combination ctrl+1 from the textbox");
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (vm.SelectedNotebook != null)
            {
                if (Keyboard.IsKeyDown(Key.Enter))
                {
                    //MessageBox.Show(vm.SelectedNotebook.Name);
                    vm.IsEdited(vm.SelectedNotebook);
                }
            }
        }

        private void NoteTitleTextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            var notebook = sender as Notebook;
            MessageBox.Show(notebook.Name);
        }

        private void ListView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {            
            Notebook notebook = (Notebook)notebookListView.SelectedItem;
            if(notebook != null)
            {
             contentRichTextBox.AppendText(notebook.Name);
            }
        }

        private void ContextMenu_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        // Key shortcuts for window and notebooklist
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (notebookListView.SelectedValue == null && noteListView.SelectedValue == null && contentRichTextBox.IsFocused == false)
            {
                // Ctrl + n + b
                if (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.N) && Keyboard.IsKeyDown(Key.B))
                {
                    //MessageBox.Show($"{ModifierKeys.Control} + {Key.N}");
                    vm.CreateNotebook();
                }

                // Tab + down
                if (Keyboard.IsKeyDown(Key.Tab) && Keyboard.IsKeyDown(Key.Down))
                {
                    //MessageBox.Show($"{Key.Tab} + {Key.Down}");
                    if (notebookListView.Items.Count > 0)
                    {
                        notebookListView.SelectedItem = notebookListView.Items[0];
                        notebookListView.Focus();
                    }
                }

                // Escape
                if (Keyboard.IsKeyDown(Key.Escape))
                {
                    if (MessageBox.Show("Do you want to close this app?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)                       
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
        }

        // Key shortcuts for notebooklist
        private void NotebookListView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (notebookListView.SelectedValue != null && noteListView.SelectedValue == null && contentRichTextBox.IsFocused == false)
            {
                // Tab + right
                if (Keyboard.Modifiers == ModifierKeys.Shift && Keyboard.IsKeyDown(Key.Right))
                {
                    //MessageBox.Show($"{ModifierKeys.Shift} + {Key.Right}");
                }

                // Ctrl + n
                if (Keyboard.Modifiers == ModifierKeys.Control && Keyboard.IsKeyDown(Key.N))
                {
                    //MessageBox.Show($"{ModifierKeys.Control} + {Key.N}");
                }
                    
                // F2
                if (Keyboard.IsKeyDown(Key.F2))
                {
                    //MessageBox.Show($"{Key.F2}");
                }

                // Delete
                if (Keyboard.IsKeyDown(Key.Delete))
                {
                    //MessageBox.Show($"{Key.Delete}");
                }
            }
        }


        // Key shortcuts for notelist

        // Key shortcuts for texteditor

    }
}
