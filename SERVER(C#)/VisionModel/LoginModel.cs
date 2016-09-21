using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace VisionModel
{
    public class LoginModel : ModelBase, INotifyPropertyChanged
    {

        private string _ImageUri;
        public string ImageUri
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
        public int MyProperty { get; set; }

        public Thickness ControlMargin { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public   void OnPropertyChanged(String propertyName)
        {

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
           
        }

        public override string ToString()
        {
            return "LoginControl";
        }

        
       
    }
}
