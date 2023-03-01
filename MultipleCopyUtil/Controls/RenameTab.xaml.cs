using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MultipleCopyUtil.Controls
{
    /// <summary>
    /// Interaction logic for RenameTab.xaml
    /// </summary>
    public partial class RenameTab : UserControl
    {
        private const string DefaultLabelChooseDirectoryPathContent = "Выберите папки...";
        private const string DefaultLabelChooseFilePathContent = "Выберите файлы...";
        
        private bool _isFolderPicker;

        private List<string> _directoryPathes;
        private List<string> DirectoryPathes
        {
            get
            {
                return _directoryPathes;
            }

            set
            {
                _directoryPathes = value;
                CheckEnablingOfRenameDirectoriesButton();
            }
        }

        private TypeOfRenamingEnum _renamingType;
        private TypeOfRenamingEnum RenamingType
        {
            get
            {
                return _renamingType;
            }

            set
            {
                _renamingType = value;
                CheckEnablingOfRenameDirectoriesButton();
            }
        }

        public RenameTab()
        {
            _isFolderPicker = true;
            _renamingType = TypeOfRenamingEnum.None;

            InitializeComponent();

            Lbl_ChooseDirectoryPath.Content = DefaultLabelChooseDirectoryPathContent;
        }

        private void Btn_ChooseDirectoryPath_Click(object sender, RoutedEventArgs e)
        {
            var dialogName = _isFolderPicker ? DefaultLabelChooseDirectoryPathContent : DefaultLabelChooseFilePathContent;
            var directoryDialog = new CommonOpenFileDialog(dialogName)
            {
                IsFolderPicker = _isFolderPicker,
                Multiselect = true
            };

            var typeOfItems = _isFolderPicker ? "папок" : "файлов";
            if (directoryDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Lbl_ChooseDirectoryPath.Content = $"{directoryDialog.FileNames.Count()} {typeOfItems} выбрано";
                DirectoryPathes = directoryDialog.FileNames.ToList();
            }
        }

        private void CheckEnablingOfRenameDirectoriesButton()
        {
            if (IsNotNullOrEmpty(DirectoryPathes) && RenamingType != TypeOfRenamingEnum.None)
            {
                Btn_Rename.IsEnabled = true;
            }
            else
            {
                Btn_Rename.IsEnabled = false;
            }
        }

        private bool IsNotNullOrEmpty<T>(IEnumerable<T> @enum)
        {
            return @enum != null && @enum.Any();
        }

        private bool IsNullOrEmpty<T>(IEnumerable<T> @enum)
        {
            return @enum == null || !@enum.Any();
        }

        private void DisableAllTextBoxesForRadioButtons()
        {
            Txt_Prefix.IsEnabled = false;
            Txt_Postfix.IsEnabled = false;
            Txt_Find.IsEnabled = false;
            Txt_Replace.IsEnabled = false;
        }

        private void Rbtn_Prefix_Checked(object sender, RoutedEventArgs e)
        {
            RenamingType = TypeOfRenamingEnum.Prefix;
            DisableAllTextBoxesForRadioButtons();

            Txt_Prefix.IsEnabled = true;
        }

        private void Rbtn_Postfix_Checked(object sender, RoutedEventArgs e)
        {
            RenamingType = TypeOfRenamingEnum.Postfix;
            DisableAllTextBoxesForRadioButtons();

            Txt_Postfix.IsEnabled = true;
        }

        private void Rbtn_Replace_Checked(object sender, RoutedEventArgs e)
        {
            RenamingType = TypeOfRenamingEnum.Replace;
            DisableAllTextBoxesForRadioButtons();

            Txt_Find.IsEnabled = true;
            Txt_Replace.IsEnabled = true;
        }

        private void Rbtn_Directories_Checked(object sender, RoutedEventArgs e)
        {
            _isFolderPicker = true;
            Btn_ChooseDirectoryPath.Content = "Выбрать папки";

            if (IsNullOrEmpty(_directoryPathes))
            {
                Lbl_ChooseDirectoryPath.Content = DefaultLabelChooseDirectoryPathContent;
            }
        }

        private void Rbtn_Files_Checked(object sender, RoutedEventArgs e)
        {
            _isFolderPicker = false;
            Btn_ChooseDirectoryPath.Content = "Выбрать файлы";

            if (IsNullOrEmpty(_directoryPathes))
            {
                Lbl_ChooseDirectoryPath.Content = DefaultLabelChooseFilePathContent;
            }
        }

        private void Btn_RemoveDirectoryPath_Click(object sender, RoutedEventArgs e)
        {
            Lbl_ChooseDirectoryPath.Content = _isFolderPicker ? DefaultLabelChooseDirectoryPathContent : DefaultLabelChooseFilePathContent;
            DirectoryPathes = null;
        }

        private void Btn_Rename_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < DirectoryPathes.Count; i++)
                {
                    var fileAttributes = File.GetAttributes(DirectoryPathes[i]);
                    var newPath = GetNewNameForPath(DirectoryPathes[i]);

                    if (DirectoryPathes[i] == newPath)
                    {
                        continue;
                    }

                    if (fileAttributes.HasFlag(FileAttributes.Directory))
                    {
                        Directory.Move(DirectoryPathes[i], newPath);
                    }
                    else
                    {
                        File.Move(DirectoryPathes[i], newPath);
                    }

                    DirectoryPathes[i] = newPath;
                }

                MessageBox.Show("Переименование завершено", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetNewNameForPath(string path)
        {
            switch (RenamingType)
            {
                case TypeOfRenamingEnum.Prefix:
                    return GetPathWithPrefix(path);
                case TypeOfRenamingEnum.Postfix:
                    return GetPathWithPostfix(path);
                case TypeOfRenamingEnum.Replace:
                    return GetPathWithReplace(path);
                default:
                    return path;
            }
        }

        private string GetPathWithPrefix(string path)
        {
            var (folderOrFileName, parentFolder) = (Path.GetFileName(path), Path.GetDirectoryName(path));
            folderOrFileName = Txt_Prefix.Text + folderOrFileName;

            return Path.Combine(parentFolder, folderOrFileName);
        }

        private string GetPathWithPostfix(string path)
        {
            var (folderOrFileName, parentFolder) = (Path.GetFileName(path), Path.GetDirectoryName(path));
            folderOrFileName = Path.GetFileNameWithoutExtension(folderOrFileName) + Txt_Postfix.Text + Path.GetExtension(folderOrFileName);

            return Path.Combine(parentFolder, folderOrFileName);
        }

        private string GetPathWithReplace(string path)
        {
            var (folderOrFileName, parentFolder) = (Path.GetFileName(path), Path.GetDirectoryName(path));
            folderOrFileName = Path.GetFileNameWithoutExtension(folderOrFileName).Replace(Txt_Find.Text, Txt_Replace.Text) + Path.GetExtension(folderOrFileName);

            return Path.Combine(parentFolder, folderOrFileName);
        }

        private enum TypeOfRenamingEnum
        {
            /// <summary>
            /// Не выбрано.
            /// </summary>
            None = 0,

            /// <summary>
            /// Префикс.
            /// </summary>
            Prefix = 1,

            /// <summary>
            /// Постфикс.
            /// </summary>
            Postfix = 2,

            /// <summary>
            /// Замена.
            /// </summary>
            Replace = 3,
        }
    }
}
