using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VisionTest.SocketServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 http://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace UnitTestProject
{
    /// <summary>
    /// 자체에서 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SocketService service = new SocketService("192.168.0.2", 30000);
        public MainPage()
        {
            this.InitializeComponent();
            service.LoginDataReceived += service_LoginDataReceived;
            service.FileReceived += service_FileReceived;
        }

        async void service_FileReceived(FileBuffer _filebuffer)
        {
            
        }

        async void service_LoginDataReceived(LoginDataBuffer _loginDataBuffer)
        {
            await service.SendAsync(new LoginDataBuffer("", true));
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await service.StartListener();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            service.ServerClose();
        }
        
        private async  void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await service.ConnectAsync();
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var data = new LoginDataBuffer("",true);
            await service.SendAsync(data);
            
        }
       

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".mp3");
            StorageFile file = await picker.PickSingleFileAsync();
            var properties = await file.GetBasicPropertiesAsync();
            if (properties == null)
                return;
            byte[] data = new byte[properties.Size];
            using( var stream = await file.OpenStreamForReadAsync())
            {
                using( BinaryReader bw = new BinaryReader(stream))
                {
                    data = bw.ReadBytes(data.Length);
                }
            }

            FileBuffer sendData = new FileBuffer(data, file.Name);
            await service.SendAsync(sendData);
            
        }
    }
}
