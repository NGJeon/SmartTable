using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace VisionModel
{
    public class ModelBase : INotifyPropertyChanged
    {
        public int DesignerX { get; set; }
        public int DesignerY { get; set; }


        public int _DesignerLocationX;
      
        public int _DesignerLocationY;
        public int DesignerLocationX
        {
              
            get
            {
                return _DesignerLocationX;
            }
            set
            {
                _DesignerLocationX = value;
                OnPropertyChanged("DesignerLocationX");

                
            }
        }
        
        public int DesignerLocationY { get; set; }

         public   void OnPropertyChanged(String propertyName)
        {

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
           
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class FileBase : ModelBase
    {
        
        public bool ClientFile { get; set; }
       public  Windows.Storage.StorageFile MyFile { get; set; }
    }
}
