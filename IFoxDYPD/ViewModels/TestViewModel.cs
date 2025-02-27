using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IFoxDYPD.ViewModels
{
    public class TestViewModel: ObservableObject
    {
        private string _title;

        // ViewModel的属性
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _teststring;

        public string Tsetstring
        {
            get => _teststring;
            set => SetProperty(ref _teststring, value);
        }


        // 一个示例命令
        public ICommand DoSomethingCommand { get; }

        public TestViewModel()
        {
            Tsetstring = "MVVM测试";
            Title = "Window3 - MVVM Example";  // 初始化窗口的标题
            DoSomethingCommand = new RelayCommand(DoSomething);  // 初始化命令

            //原有的.xaml.cs
            // 初始化 MemoDtos 集合
            MemoDtos = new ObservableCollection<MemoDto>
            {
                new MemoDto { ID = "柜号:LA5", Property1 = "馈电柜", Property2 = "MNS" },
                new MemoDto { ID = "柜号:LA6", Property1 = "馈电柜", Property2 = "MNS" },
                new MemoDto { ID = "柜号:LA7", Property1 = "馈电柜", Property2 = "MNS" },
                new MemoDto { ID = "柜号:LA8", Property1 = "馈电柜", Property2 = "MNS" }
            };

            // 将 DataContext 设为当前窗口，以便进行数据绑定
            //this.DataContext = this;

        }

        // 命令逻辑的实现
        private void DoSomething()
        {
            Title = "Button clicked!";
            Tsetstring= "Button clicked!";
            // 你可以在这里加入AutoCAD的相关逻辑或其他逻辑
        }
        public ObservableCollection<MemoDto> MemoDtos { get; set; }
        public class MemoDto
        {
            public string ID { get; set; }
            public string Property1 { get; set; }
            public string Property2 { get; set; }
        }

        //private void CheckOrder_Click(object sender, RoutedEventArgs e)
        //{
        //    // 遍历 MemoItemsControl 的数据源，显示当前顺序
        //    var items = MemoItemsControl.ItemsSource as ObservableCollection<MemoDto>;

        //    if (items != null)
        //    {
        //        // 构建序号显示的字符串
        //        string message = "当前序号：\n";
        //        message += string.Join("\n", items.Select((item, index) => $"序号 {index + 1}: ID = {item.ID}"));

        //        // 显示当前顺序
        //        MessageBox.Show(message, "序号变化");
        //    }
        //}
    }
}
