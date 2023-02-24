using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace MultipleCopyUtil.Controls
{
    /// <summary>
    /// Interaction logic for CopyTab.xaml
    /// </summary>
    public partial class CopyTab : UserControl
    {
        private const string DefaultLabelChooseDirectoryPathContent = "Выберите папки...";
        private const string DefaultLabelChooseFilePathContent = "Выберите файлы...";

        private IEnumerable<string> _directoryPathes;
        private IEnumerable<string> DirectoryPathes
        {
            get
            {
                return _directoryPathes;
            }

            set
            {
                _directoryPathes = value;
                CheckEnablingOfCopyFileToDirectoriesButton();
            }
        }

        private IEnumerable<string> _filePathes;
        private IEnumerable<string> FilePathes
        {
            get
            {
                return _filePathes;
            }

            set
            {
                _filePathes = value;
                CheckEnablingOfCopyFileToDirectoriesButton();
            }
        }

        private void CheckEnablingOfCopyFileToDirectoriesButton()
        {
            if (IsNotNullOrEmpty(_directoryPathes) && IsNotNullOrEmpty(_filePathes))
            {
                Btn_CopyFileToDirectories.IsEnabled = true;
            }
            else
            {
                Btn_CopyFileToDirectories.IsEnabled = false;
            }
        }

        private bool IsNotNullOrEmpty<T>(IEnumerable<T> @enum)
        {
            return @enum != null && @enum.Any();
        }

        public CopyTab()
        {
            InitializeComponent();

            Lbl_ChooseDirectoryPath.Content = DefaultLabelChooseDirectoryPathContent;
            Lbl_ChooseFilePath.Content = DefaultLabelChooseFilePathContent;
        }

        private void Btn_ChooseDirectoryPath_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var directoryDialog = new CommonOpenFileDialog(DefaultLabelChooseDirectoryPathContent)
            {
                IsFolderPicker = true,
                Multiselect = true
            };

            if (directoryDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Lbl_ChooseDirectoryPath.Content = $"{directoryDialog.FileNames.Count()} папок выбрано";
                DirectoryPathes = directoryDialog.FileNames;
            }
        }

        private void Btn_ChooseFilePath_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var fileDialog = new CommonOpenFileDialog(DefaultLabelChooseFilePathContent)
            {
                Multiselect = true
            };

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Lbl_ChooseFilePath.Content = $"{fileDialog.FileNames.Count()} файлов выбрано";
                FilePathes = fileDialog.FileNames;
            }
        }

        private void Btn_CopyFileToDirectories_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                foreach (var folder in DirectoryPathes)
                {
                    foreach (var file in FilePathes)
                    {
                        File.Copy(file, Path.Combine(folder, Path.GetFileName(file)), Chk_OverwriteFile.IsChecked.Value);
                    }
                }

                MessageBox.Show("Копирование завершено", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Btn_ClearChoosedDirectoryPath_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Lbl_ChooseDirectoryPath.Content = DefaultLabelChooseDirectoryPathContent;
            DirectoryPathes = null;
        }

        private void Btn_ClearChooseFilePath_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Lbl_ChooseFilePath.Content = DefaultLabelChooseFilePathContent;
            FilePathes = null;
        }
    }
}
