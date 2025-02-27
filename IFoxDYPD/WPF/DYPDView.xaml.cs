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

namespace IFoxDYPD.WPF
{
    /// <summary>
    /// _01DYPDView.xaml 的交互逻辑
    /// </summary>
    public partial class DYPDView : Window
    {
        public DYPDView()
        {
            InitializeComponent();
            Loaded += DYPDView_Loaded;
        }

        private void DYPDView_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (DYPDViewModel)DataContext;
            viewModel.RequestClose += OnRequestClose; // 订阅关闭请求
        }

        private void OnRequestClose()
        {
            Close(); // 关闭窗口
        }
    }
}
