using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
namespace VisionModel
{
    public class Mp3Model : FileBase,INotifyPropertyChanged
    {
        public bool IsPlay { get; set; }
        public String Title { get;  set; }
        public String Album { get;  set; }
        public String Length { get;  set; }
        public String MusicPath { get; set; }


        public string _ImageUri;
        public String ImageUri
        {
            get
            {
                return _ImageUri;
            }
            set
            {
                _ImageUri = value;
                OnPropertyChanged("ImageUri");
                
            }
        }

        public void OnPropertyChanged(String propertyName)
        {

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

     
        public BitmapImage Image2{ get; set; }
        public MediaElement Media { get; set; }

        public override string ToString()
        {
            return "Mp3Model";
        }
  
        public  Mp3Model()
        {
            IsPlay = false;
            ClientFile = false;
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}
