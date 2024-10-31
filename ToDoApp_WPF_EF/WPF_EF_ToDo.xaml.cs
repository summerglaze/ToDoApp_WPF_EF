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
using System.Windows.Shapes;

namespace ToDoApp_WPF_EF
{
    /// <summary>
    /// Logika interakcji dla klasy WPF_EF_ToDo.xaml
    /// </summary>
    public partial class WPF_EF_ToDo : Window
    {
        public WPF_EF_ToDo()
        {
            InitializeComponent();
            Refresh(DateTime.Today);
        }
        
        private void Refresh(DateTime dt )
        {
            datePick.SelectedDate = dt;

            ToDoDBEntities db = new ToDoDBEntities();
            var ts = from t in db.Tasks
                     where t.Data == datePick.SelectedDate
                     select t;

            this.gridTasks.ItemsSource = ts.ToList();

            btnAdd.IsEnabled = true;
            txtZadanie.Text = String.Empty;
            dpData.SelectedDate = null;
            btnDelete.IsEnabled = false;
            btnUpdate.IsEnabled = false;
        }

        private void datePick_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ToDoDBEntities db = new ToDoDBEntities();
            var ts = from t in db.Tasks
                     where t.Data == datePick.SelectedDate
                     select t;

            this.gridTasks.ItemsSource = ts.ToList();
        }

        private void btnEnableAdd_Click(object sender, RoutedEventArgs e)
        {
            btnAdd.IsEnabled = true;
            dpData.SelectedDate = null;
            txtZadanie.Text = String.Empty;
            btnDelete.IsEnabled = false;
            btnUpdate.IsEnabled = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ToDoDBEntities db = new ToDoDBEntities();

            DateTime parsedDate;
            bool isValid = DateTime.TryParse(dpData.Text, out parsedDate);

            if (isValid)
            {
                Task taskObj = new Task()
                {
                    Zadanie = txtZadanie.Text,
                    Data = parsedDate
                };

                db.Tasks.Add(taskObj);

                db.SaveChanges();
                lblKomunikat.Visibility = Visibility.Collapsed;
                lblKomunikat.Content = String.Empty;
                Refresh(parsedDate);
            }
            else
            {
                lblKomunikat.Visibility = Visibility.Visible;
                lblKomunikat.Content = "Nieprawidłowy format daty";
            }
        }

        private int taskId = 0;
        private void gridTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.gridTasks.SelectedIndex >= 0)
            {
                if (this.gridTasks.SelectedItems.Count > 0)
                {
                    if (this.gridTasks.SelectedItems[0].GetType() == typeof(Task))
                    {
                        Task t = (Task)this.gridTasks.SelectedItems[0];
                        this.txtZadanie.Text = t.Zadanie;
                        this.dpData.Text = t.Data.ToString();
                        this.taskId = t.Id;
                        btnAdd.IsEnabled = false;
                        btnDelete.IsEnabled = true;
                        btnUpdate.IsEnabled = true;
                    }
                }
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ToDoDBEntities db = new ToDoDBEntities();

            var q = from d in db.Tasks
                    where d.Id == this.taskId
                    select d;
            Task obj = q.SingleOrDefault();
            DateTime parsedDate;
            bool isValid = DateTime.TryParse(dpData.Text, out parsedDate);

            if (isValid)
            {
                if (obj != null)
                {
                    obj.Zadanie = this.txtZadanie.Text;
                    obj.Data = parsedDate;
                    db.SaveChanges();
                    Refresh(parsedDate);
                }
            }
        }
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("Czy na pewno usunąć wpis?","Usuń",MessageBoxButton.YesNo,MessageBoxImage.Warning,MessageBoxResult.No);

            if (mbr == MessageBoxResult.Yes)
            {
                ToDoDBEntities db = new ToDoDBEntities();
                var q = from d in db.Tasks
                        where d.Id == this.taskId
                        select d;
                Task obj = q.SingleOrDefault();

                if (obj != null)
                {
                    db.Tasks.Remove(obj);
                    db.SaveChanges();
                    Refresh(obj.Data);
                }
            }
        }
    }
}
