using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ModelCreator
{
    public class Mp3ModelCreator
    {
        public Mp3ModelCreator()
        {

        }

        public async Task<VisionModel.Mp3Model> CreateMp3Model(StorageFile file)
        {
            VisionModel.Mp3Model model = new VisionModel.Mp3Model();

            IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            MusicProperties musicProp = await file.Properties.GetMusicPropertiesAsync();
            StorageItemThumbnail thumb = await file.GetThumbnailAsync(ThumbnailMode.SingleItem);

            Windows.Storage.Streams.Buffer Mybuffer = new Windows.Storage.Streams.Buffer(Convert.ToUInt32(thumb.Size));
            model.Album = musicProp.Album;
            model.Title = musicProp.Title;
            model.Length = musicProp.Duration.ToString();
            model.Image2 = new BitmapImage();
            model.ImageUri = "ms-appx:///Assets/play2.jpg";
            
            await model.Image2.SetSourceAsync(thumb);
            model.MyFile = file;
         

            return model;
        }

        public async Task<VisionModel.ImageModel> CreateImageModel(StorageFile file )
        {
            VisionModel.ImageModel model = new VisionModel.ImageModel();

            IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            model.ImageSource = new BitmapImage();
            
            await model.ImageSource.SetSourceAsync(stream);

           int width = model.ImageSource.PixelWidth;
           int height = model.ImageSource.PixelHeight;

           model.DesignerX = width;
           model.DesignerY = height;

            return model;

        }
    }
}
