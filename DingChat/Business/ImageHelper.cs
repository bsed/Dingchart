using cn.lds.chatcore.pcw.Common;
using cn.lds.chatcore.pcw.Common.Enums;
using cn.lds.chatcore.pcw.Common.Utils;
using cn.lds.chatcore.pcw.DataSqlite;
using cn.lds.chatcore.pcw.Models.Tables;
using cn.lds.chatcore.pcw.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.Concurrent;
using System.Drawing;
using cn.lds.chatcore.pcw.Beans;
using cn.lds.chatcore.pcw.imtp.message;
using cn.lds.chatcore.pcw.Services.core;
using Kaliko.ImageLibrary;
using Kaliko.ImageLibrary.Scaling;
using PictureViewer;
using Image = System.Windows.Controls.Image;

namespace cn.lds.chatcore.pcw.Business {
public class ImageHelper {

    // 头像路径缓存
    public static ConcurrentDictionary<String, String> loadAvatarPool = new ConcurrentDictionary<string, string>();

    /// <summary>
    /// 显示图片查看器
    /// </summary>
    /// <param Name="messagesList"></param>
    public static void ShowPictuerViewer(MessageItem currentMessageItem) {
        try {
            List<MessageItem> messagesList = MessageService.getInstance().findAllImageMessagesByUser(currentMessageItem.user);
            //当前显示的图片
            ImageBean currentImageBean = CoverImageBeanFromMessageItem(currentMessageItem);
            List<ImageBean> list = new List<ImageBean>();
            foreach (MessageItem messageItem in messagesList) {
                if(MsgType.Image.ToStr().Equals(messageItem.type)) {
                    ImageBean imageBean = CoverImageBeanFromMessageItem(messageItem);
                    if (imageBean!=null) {
                        list.Add(imageBean);
                    }
                }
            }
            PictureViewHelper.ShowPictureView(list, currentImageBean);
        } catch (Exception e) {
            Log.Error(typeof(ImageHelper), e);
        }
    }
    /// <summary>
    /// 将消息转换成ImageBean
    /// </summary>
    /// <param Name="messageItem"></param>
    /// <returns></returns>
    private static ImageBean CoverImageBeanFromMessageItem(MessageItem messageItem) {
        ImageBean currentImageBean = new ImageBean();
        try {
            FilesTable filesTable = null;
            PictureMessage messageBean = new PictureMessage().toModel(messageItem.content);
            //当前显示的图片
            if (messageBean.imageStorageId==null || "".Equals(messageBean.imageStorageId)) {
                // 如果无法通过存储ID查找到文件，则尝试查看发送的文件
                filesTable = FilesService.getInstance().getFileByOwner(messageItem.messageId);
            } else {
                filesTable = FilesService.getInstance().getFile(messageBean.imageStorageId);
            }

            if (filesTable == null) {
                // 如果收发都没有查到的话，可能是一个发送失败的图片消息
                currentImageBean.fileName = "";
                currentImageBean.localPath = "";
                currentImageBean.imageStorageId = messageBean.imageStorageId;
                currentImageBean.thumbnail = messageBean.thumbnail;
            } else {
                currentImageBean.fileName = DateTimeHelper.getFormateDateString(DateTimeType.yyyyMMddHHmmss_1) + filesTable.localpath.Substring(filesTable.localpath.LastIndexOf(".") - 1);
                currentImageBean.localPath = filesTable.localpath;
                currentImageBean.imageStorageId = messageBean.imageStorageId;
                currentImageBean.thumbnail = messageBean.thumbnail;
                currentImageBean.size = filesTable.size;
            }
        } catch (Exception e) {
            Log.Error(typeof(ImageHelper), e);
        }
        return currentImageBean;
    }

    /// <summary>
    /// 显示头像
    /// </summary>
    /// <param Name="fileStorageId"></param>
    /// <returns></returns>
    public static void loadAvatar(Object fileStorageId, Image image) {
        try {
            if (string.IsNullOrEmpty(fileStorageId.ToStr())) {
                image.Source = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
                return;
            };
            image.UseLayoutRounding = true;
            String localpath = null;
            if (loadAvatarPool.ContainsKey(fileStorageId.ToStr())) {
                localpath = loadAvatarPool[fileStorageId.ToStr()];
            } else {
                FilesTable table = FilesDao.getInstance().findByFileStorageId(fileStorageId.ToStr());
                if (table != null) {
                    localpath = table.localpath;
                    // 缓存下头像路径
                    loadAvatarPool.TryAdd(fileStorageId.ToStr(), localpath);
                }
            }

            if (localpath != null && File.Exists(localpath)) {
                // 显示头像
                image.Source = new BitmapImage(new Uri(localpath, UriKind.RelativeOrAbsolute));


            } else {
                // 头像尚未下载，先显示默认头像
                image.Source = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
                // 执行下载动作
                DownloadServices.getInstance().DownloadMethod(fileStorageId.ToStr(), DownloadType.SYSTEM_APP_IMAGE, null);
            }
        } catch (Exception e) {
            Log.Error(typeof(ImageHelper), e);
            image.Source = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
        }
    }



    /// <summary>
    /// 显示头像
    /// </summary>
    /// <param Name="fileStorageId"></param>
    /// <returns></returns>
    public static void loadAvatarImageBrush(Object fileStorageId, ImageBrush image) {
        try {
            if (string.IsNullOrEmpty(fileStorageId.ToStr())) {
                image.ImageSource = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
                return;
            };

            String localpath=null;
            if (loadAvatarPool.ContainsKey(fileStorageId.ToStr())) {
                localpath = loadAvatarPool[fileStorageId.ToStr()];
            } else {
                FilesTable table = FilesDao.getInstance().findByFileStorageId(fileStorageId.ToStr());
                if (table != null) {
                    localpath = table.localpath;
                    // 缓存下头像路径
                    loadAvatarPool.TryAdd(fileStorageId.ToStr(), localpath);
                }
            }

            if (localpath != null && File.Exists(localpath)) {
                // 显示头像
                image.ImageSource = new BitmapImage(new Uri(localpath, UriKind.RelativeOrAbsolute));

            } else {
                // 头像尚未下载，先显示默认头像
                image.ImageSource = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
                // 执行下载动作
                DownloadServices.getInstance().DownloadMethod(fileStorageId.ToStr(),DownloadType.SYSTEM_IMAGE, null);
            }
        } catch (Exception e) {
            Log.Error(typeof(ImageHelper), e);

            image.ImageSource = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
        }
    }


    /// <summary>
    /// 显示头像地址
    /// </summary>
    /// <param Name="fileStorageId"></param>
    /// <returns></returns>
    public static BitmapImage loadAvatarPath(Object fileStorageId) {
        try {
            if (string.IsNullOrEmpty(fileStorageId.ToStr()))
                return new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
            FilesTable table = FilesDao.getInstance().findByFileStorageId(fileStorageId.ToStr());
            if (table != null && File.Exists(table.localpath)) {
                // 显示头像
                return new BitmapImage(new Uri(table.localpath, UriKind.RelativeOrAbsolute));
            } else {
                // 执行下载动作
                DownloadServices.getInstance().DownloadMethod(fileStorageId.ToStr(), DownloadType.SYSTEM_IMAGE, null);
                // 头像尚未下载，先显示默认头像
                return new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
            }
        } catch (Exception e) {
            Log.Error(typeof(ImageHelper), e);
            return new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
        }
    }

    public static BitmapImage loadSysImageBrush(String fileName) {
        try {
            if (string.IsNullOrEmpty(fileName)) return null;
            String strFilePaht = @"pack://application:,,,/ResourceDictionary;Component/images/" + fileName;
            //if (File.Exists(strFilePaht)) {
            return new BitmapImage(new Uri(strFilePaht,
                                           UriKind.RelativeOrAbsolute));
            //} else {
            //    image.ImageSource = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
            //}
        } catch (Exception e) {
            Log.Error(typeof(ImageHelper), e);
            return new BitmapImage(new Uri(App.ImagePathDefault,
                                           UriKind.RelativeOrAbsolute));

        }
    }
    /// <summary>
    /// 加载系统图标
    /// </summary>
    /// <param Name="fileName"></param>
    /// <param Name="image"></param>
    public static void loadSysImageBrush(String fileName, ImageBrush image) {
        try {
            if (string.IsNullOrEmpty(fileName)) return;
            String strFilePaht =  @"pack://application:,,,/ResourceDictionary;Component/images/" + fileName;
            //if (File.Exists(strFilePaht)) {
            image.ImageSource = new BitmapImage(new Uri(strFilePaht,
                                                UriKind.RelativeOrAbsolute));
            //} else {
            //    image.ImageSource = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
            //}
        } catch (Exception e) {
            image.ImageSource = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
            Log.Error(typeof(ImageHelper), e);
        }
    }

    /// <summary>
    /// 加载系统图标
    /// </summary>
    /// <param Name="fileName"></param>
    /// <param Name="image"></param>
    public static void loadSysImage(String fileName, Image image) {
        try {
            if (string.IsNullOrEmpty(fileName)) return;
            image.UseLayoutRounding = true;
            String strFilePaht =  @"pack://application:,,,/ResourceDictionary;Component/images/" + fileName;

            //if (File.Exists(strFilePaht)) {
            image.Source = new BitmapImage(new Uri(strFilePaht,
                                                   UriKind.RelativeOrAbsolute));
            //} else {
            //    image.Source = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
            //}
        } catch (Exception e) {
            image.Source = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
            Log.Error(typeof(ImageHelper), e);
        }
    }

    /// <summary>
    /// 加载系统图标
    /// </summary>
    /// <param Name="fileName"></param>
    /// <param Name="image"></param>
    public static void loadSysImage(String fileName,String defaultImage, Image image) {
        try {
            if (string.IsNullOrEmpty(fileName)) return;
            image.UseLayoutRounding = true;
            String strFilePaht = @"pack://application:,,,/ResourceDictionary;Component/images/" + fileName;
            //if (File.Exists(strFilePaht)) {
            image.Source = new BitmapImage(new Uri(strFilePaht,
                                                   UriKind.RelativeOrAbsolute));
            //} else {
            //    if (defaultImage == null) {
            //        image.Source = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
            //    } else {
            //        image.Source = new BitmapImage(new Uri(defaultImage, UriKind.RelativeOrAbsolute));
            //    }

            //}
        } catch (Exception e) {
            if (defaultImage == null) {
                image.Source = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
            } else {
                image.Source = new BitmapImage(new Uri(defaultImage, UriKind.RelativeOrAbsolute));
            }
            Log.Error(typeof(ImageHelper), e);
        }
    }

    /// <summary>
    /// 显示头像
    /// </summary>
    /// <param Name="fileStorageId"></param>
    /// <returns></returns>
    public static void loadLoginUserAvatar(Object fileStorageId, Image image) {
        try {
            if (string.IsNullOrEmpty(fileStorageId.ToStr())) {
                image.Source = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
                return;
            };
            image.UseLayoutRounding = true;
            String localpath = App.DefaultCacheRootPath +
                               ToolsHelper.getForderNameByDownloadType(DownloadType.SYSTEM_IMAGE) + "/" +
                               FileType.IMAGES.ToStr()+ "/" + fileStorageId + ".jpg"; ;
            if (localpath != null && File.Exists(localpath)) {
                // 显示头像
                image.Source = new BitmapImage(new Uri(localpath, UriKind.RelativeOrAbsolute));
            } else {
                // 头像尚未下载，先显示默认头像
                image.Source = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
            }
        } catch (Exception e) {
            image.Source = new BitmapImage(new Uri(App.ImagePathDefault, UriKind.RelativeOrAbsolute));
            Log.Error(typeof(ImageHelper), e);
        }
    }

    /// <summary>
    /// 获取系统图标路径
    /// </summary>
    /// <param Name="fileName"></param>
    /// <param Name="image"></param>
    public static String getSysImagePath(String fileName) {
        if (string.IsNullOrEmpty(fileName)) return string.Empty;
        try {

            return  @"pack://application:,,,/ResourceDictionary;Component/images/" + fileName;
        } catch (Exception e) {
            Log.Error(typeof(ImageHelper), e);
            return App.ImagePathDefault;
        }
    }
    /// <summary>
    /// 显示头像地址
    /// </summary>
    /// <param Name="fileStorageId"></param>
    /// <returns></returns>
    public static String getAvatarPath(Object fileStorageId) {
        try {
            if (string.IsNullOrEmpty(fileStorageId.ToStr()))  {
                return App.ImagePathDefault;
            }
            ;
            FilesTable table = FilesDao.getInstance().findByFileStorageId(fileStorageId.ToStr());
            if (table != null && File.Exists(table.localpath)) {
                // 显示头像
                return table.localpath;
            } else {
                // 头像尚未下载，先显示默认头像
                return App.ImagePathDefault;
            }
        } catch (Exception e) {
            Log.Error(typeof(ImageHelper), e);
            return App.ImagePathDefault;
        }
    }
    public static String getOrgAvatarPath(Object fileStorageId) {
        string ImagePathDefault = @"pack://application:,,,/ResourceDictionary;Component/images/Org.png";
        try {
            if (string.IsNullOrEmpty(fileStorageId.ToStr())) {
                return ImagePathDefault;
            }

            FilesTable table = FilesDao.getInstance().findByFileStorageId(fileStorageId.ToStr());
            if (table != null && File.Exists(table.localpath)) {
                // 显示头像
                return table.localpath;
            } else {
                // 头像尚未下载，先显示默认头像
                return ImagePathDefault;
            }
        } catch (Exception e) {
            Log.Error(typeof(ImageHelper), e);
            return ImagePathDefault;
        }
    }
    public static void CreateThumbnail(System.Drawing.Image image,int width,int height,String thumbFilePath) {
        KalikoImage kalikoImage = null;
        try {
            if (image == null) {
                return;
            }
            kalikoImage = new KalikoImage(image);
            KalikoImage thumb = kalikoImage.Scale(new FitScaling(width, height));
            thumb.SaveJpg(@thumbFilePath, 99);

        } finally {
            if (kalikoImage!=null) {
                kalikoImage.Dispose();
                kalikoImage.Destroy();
            }

        }

    }


    public static BitmapImage Base64ToImage(string base64String) {
        // Convert base 64 string to byte[]
        try {

            byte[] imageBytes =
                Convert.FromBase64String(base64String);

            MemoryStream strmImg = new MemoryStream(imageBytes);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = strmImg;
            image.EndInit();
            return image;

        } catch (Exception e) {
            Log.Error(typeof (ImageHelper), e);
        }

        return null;
    }


    public static string ImageToBase64(BitmapImage image) {

        Stream stream = image.StreamSource;
        Byte[] buffer = null;
        if (stream != null && stream.Length > 0) {
            using (BinaryReader br = new BinaryReader(stream)) {
                buffer = br.ReadBytes((Int32)stream.Length);
            }
        }
        if (buffer != null) {
            // Convert byte[] to base 64 string
            string base64String = Convert.ToBase64String(buffer);
            return base64String;
        } else {
            return null;
        }


    }

    public static string ImageToBase64(Bitmap image, System.Drawing.Imaging.ImageFormat format) {
        using (MemoryStream ms = new MemoryStream()) {
            // Convert Image to byte[]
            image.Save(ms, format);
            byte[] imageBytes = ms.ToArray();
            // Convert byte[] to base 64 string
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
    }
    public static BitmapImage BitmapToBitmapImage(Bitmap bitmap) {
        Bitmap bitmapSource = new Bitmap(bitmap.Width, bitmap.Height);
        int i, j;
        for (i = 0; i < bitmap.Width; i++)
            for (j = 0; j < bitmap.Height; j++) {
                System.Drawing.Color pixelColor = bitmap.GetPixel(i, j);
                System.Drawing.Color newColor = System.Drawing.Color.FromArgb(pixelColor.R, pixelColor.G, pixelColor.B);
                bitmapSource.SetPixel(i, j, newColor);
            }
        MemoryStream ms = new MemoryStream();
        bitmapSource.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = new MemoryStream(ms.ToArray());
        bitmapImage.EndInit();

        return bitmapImage;
    }

    /// <summary>
    /// 显示二维码
    /// </summary>
    /// <param Name="fileStorageId"></param>
    /// <returns></returns>
    public static String load2DBarcode(Object fileStorageId, Image image) {
        try {
            if (string.IsNullOrEmpty(fileStorageId.ToStr())) {
                return "";
            };
            FileType fileType = ToolsHelper.getFileTypeBySuffix(".jpg");
            String localpath = App.CacheRootPath + "SYSTEM" + "/" + fileType.ToStr()+"/" + fileStorageId.ToStr()+".jpg";
            // 显示头像
            image.Source = new BitmapImage(new Uri(localpath, UriKind.RelativeOrAbsolute));
            return localpath;
        } catch (Exception e) {
            Log.Error(typeof(ImageHelper), e);
        }
        return "";
    }
}
}
