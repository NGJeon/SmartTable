using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace VisionModel
{
    public class ImageModel : FileBase, INotifyPropertyChanged
    {
        public BitmapImage ImageSource { get; set; }
       


        public void OnPropertyChanged(String propertyName)
        {

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }

        public ImageModel()
        {
            
        }

        public override string ToString()
        {
            return "ImageModel";
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
