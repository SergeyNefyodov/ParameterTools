using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using SWD = System.Windows.Data;

namespace ParameterTools.CopyParameters
{
    /// <summary>
    /// Логика взаимодействия для CopyForm.xaml
    /// </summary>
    /// 

    public partial class CopyParametersView : Window
    {
        public CopyParametersView(ExternalCommandData commandData)
        {           
            InitializeComponent();
            CopyParametersViewModel vm = new CopyParametersViewModel(commandData);
            spcw = vm;
            vm.createDictCategories();
            vm.CloseRequest += (s, e) => this.Close();
            DataContext = vm;
            fillCategories(vm);
        }

        private void fillCategories(CopyParametersViewModel vm)
        {
            for (int i = 0; i < vm.dictCategories.Count; i++)
            {
                Category c = vm.dictCategories.Keys.ElementAt(i);
                ListBoxItem lbi = new ListBoxItem();
                CheckBox cb = new CheckBox()
                {
                    Name = "cb" + i.ToString(),
                    Content = c.Name,
                    MinHeight = 20,
                    IsChecked = false,
                    Margin = new Thickness(4, 3,0,0),
                };

                SWD.Binding binding = new SWD.Binding
                {
                    Path = new PropertyPath("Value"),
                    Source = vm.dictCategories.Values.ElementAt(i),
                    Mode = SWD.BindingMode.OneWayToSource
                };

                SWD.BindingMode m = binding.Mode;
                cb.SetBinding(CheckBox.IsCheckedProperty, binding);
                
                this.categoriesStack.Children.Add(cb);
            }
        }

        internal CopyParametersViewModel spcw { get; set; }        

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {            
            Close();
        }

        private void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
           Close();
        }
        private void ButtonHide_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < categoriesStack.Children.Count; i++)            
            {
                object obj = categoriesStack.Children[i];
                CheckBox cb = null;
                if (obj is CheckBox)
                    cb = (CheckBox)categoriesStack.Children[i];
                else
                    continue;
                if (cb.IsChecked == false)
                {
                    cb.Visibility = System.Windows.Visibility.Collapsed;
                    cb.Height = 0;
                }               
            }
        }
        private void ButtonShow_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox cb in this.categoriesStack.Children)
            {
                cb.Visibility = System.Windows.Visibility.Visible;
                cb.Height = 20;
            }            
        }
        private void ButtonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox cb in this.categoriesStack.Children)
            {
                cb.IsChecked = true;
            }
        }

        private void ButtonAddParam_Click(object sender, RoutedEventArgs e)
        {
            int q = Parameters.Children.Count;
            if (q > 7)
            {
                return;
            }
            SWD.Binding bindingSource = new SWD.Binding();
            bindingSource.Path = new PropertyPath("allParametersList");
            bindingSource.Source = spcw;
            SWD.Binding bindingItem = new SWD.Binding();
            bindingItem.Path = new PropertyPath("selectedParametersList["+q.ToString()+"]");
            bindingItem.Source = spcw;
            bindingItem.Mode = SWD.BindingMode.OneWayToSource;
            System.Windows.Controls.ComboBox cb = new System.Windows.Controls.ComboBox()
            {
                DisplayMemberPath = "Name",
                Margin = new System.Windows.Thickness(20, 5, 20, 5),
                Name = "comboBox" + q.ToString(), 
            };
            cb.SetBinding(System.Windows.Controls.ComboBox.ItemsSourceProperty, bindingSource);
            cb.SetBinding(System.Windows.Controls.ComboBox.SelectedItemProperty, bindingItem);
            int a = Parameters.Children.Add(cb);            
        }
        private void ButtonDeleteParam_Click(object sender, RoutedEventArgs e)
        {
            int q = Parameters.Children.Count;
            if (q>1)
            {
                Parameters.Children.RemoveAt(q-1);
            }
        }
    }
}
