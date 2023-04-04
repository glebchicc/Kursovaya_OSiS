using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;

namespace OSiS
{
    internal class Program
    {
        public static void ConvertJpegToGif(string jpegFilePath, string gifFilePath)
        {
            // Загрузка JPEG изображения
            Image jpegImage = Image.FromFile(jpegFilePath);

            // Создание нового изображения с подходящим пиксельным форматом
            Bitmap bitmap = new Bitmap(jpegImage.Width, jpegImage.Height, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(jpegImage.HorizontalResolution, jpegImage.VerticalResolution);

            // Создание объекта Graphics для нового изображения
            Graphics graphics = Graphics.FromImage(bitmap);

            // Копирование пикселей из JPEG изображения в новое изображение
            graphics.DrawImage(jpegImage, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

            // Сохранение нового изображения в формате GIF
            bitmap.Save(gifFilePath, ImageFormat.Gif);

            // Освобождение ресурсов
            jpegImage.Dispose();
            graphics.Dispose();
            bitmap.Dispose();

            Console.WriteLine("Конвертация завершена.");
        }

        public static void CompressJpeg(string inputFilePath, string outputFilePath, int qualityLevel)
        {
            using (var image = new Bitmap(inputFilePath))
            {
                var encoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var parameters = new EncoderParameters(1);
                parameters.Param[0] = new EncoderParameter(Encoder.Quality, qualityLevel);

                image.Save(outputFilePath, encoder, parameters);
            }

            Console.WriteLine("Сжатие изображения завершено.");
        }
        public static void CompressAudio(string inputFilePath = "input.mp3", string outputFilePath = "output.wav", 
            int bitRate = 128, int sampleRate = 44100, int channels = 2, string preset = "medium", int quality = 1)
        {
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }

            // Путь к утилите FFmpeg
            string ffmpegPath = "ffmpeg.exe";

            // Выполняем команду FFmpeg для конвертации аудио файла в формат MP3
            var process = new Process();
            process.StartInfo.FileName = ffmpegPath;
            process.StartInfo.Arguments = $"-i \"{inputFilePath}\" -b:a {bitRate}k -ar {sampleRate} -ac {channels} -q:a {quality} -preset {preset} \"{outputFilePath}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            // Ожидаем завершения процесса FFmpeg
            process.WaitForExit();

            Console.WriteLine("Сжатие аудио завершено.");
        }

        public static void CompressVideo(string inputFile = "input.mp4", string outputFile = "output.mp4", 
            int width = 640, int height = 360, int bitrate = 1000, int frameRate = 30, string codec = "libx264", 
            string format = "mp4", string pixelFormat = "yuv420p", string profile = "main")
        {
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }
            // параметры сжатия видео:
            // ширина видео
            // высота видео
            // битрейт в kbps
            // кадровая частота
            // кодек для сжатия видео
            // формат выходного видеофайла
            // цветовое пространство
            // профиль для кодека

            // создаем процесс FFMpeg для сжатия видео
            var ffmpegProcess = new Process
            {
                StartInfo =
                {
                    FileName = "ffmpeg.exe",
                    Arguments = $"-i {inputFile} -vf scale={width}:{height} -b:v {bitrate}k -r {frameRate} -c:v {codec} -pix_fmt {pixelFormat} -profile:v {profile} -f {format} {outputFile}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            // запускаем процесс FFMpeg
            ffmpegProcess.Start();
            ffmpegProcess.WaitForExit();

            Console.WriteLine("Сжатие видео завершено.");
        }

        static void Main()
        {
            Console.WriteLine("Конвертация изображения из JPG в GIF...");
            ConvertJpegToGif("input.jpg", "output.gif");
            Console.WriteLine("Сжатие изображения с заданным качеством...");
            CompressJpeg("input.jpg", "output90.jpg", 90);
            Console.WriteLine("Сжатие аудио с заданными параметрами...");
            CompressAudio();
            Console.WriteLine("Сжатие видео с заданными параметрами...");
            CompressVideo();
        }
    }
}