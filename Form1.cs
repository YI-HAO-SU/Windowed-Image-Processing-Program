using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Bitmap openImg;                         // 開啟的照片
        Bitmap state_img;                        // 暫存的照片
        Bitmap undo_image;                   // 還原後該出現的照片
        Bitmap process_image;               // 正要進行處理的照片

        public int Threshold_value = 0;
        public int Threshold_value_overlap = 0;
        public int index_saver = 0;
        public int index_saver_compare = 1;
        public int flag = 0;
        Stack<Bitmap> processing_stack = new Stack<Bitmap>();

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void Process_img_saver(Bitmap Source_image)
        {

            //最後一次產生的圖暫存 但先不push進stack
            //第一次僅暫存處理後的照片
            if (index_saver == 0)
            {
                state_img = Source_image;
                process_image = state_img;
                index_saver++;
            }
            //第二次開始push進stack
            else
            {
                // Press Undo will make flag up
                if (flag == 1)
                {
                    undo_image = process_image;
                    state_img = Source_image;
                    process_image = state_img;
                    processing_stack.Push(undo_image);
                    flag = 0;
                }
                else
                {
                    processing_stack.Push(state_img);
                    state_img = Source_image;
                    process_image = state_img;
                }
            }
            
            
        }
        private void Load_Image(object sender, EventArgs e)
        {
            // Load Image Button
            openFileDialog1.Filter = "All Files|*.*|Bitmap Files (.bmp)|*.bmp|Jpeg File(.jpg)|*.jpg";
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                openImg = new Bitmap(openFileDialog1.FileName);

                pictureBox1.Size = new Size(openImg.Width, openImg.Height);
                pictureBox2.Size = new Size(openImg.Width, openImg.Height);

                pictureBox1.Image = openImg;
                pictureBox2.Image = openImg;
                processing_stack.Clear();
                processing_stack.Push(openImg);
                process_image = openImg;
                index_saver = 0;
                pictureBox3.Image = null;
                pictureBox4.Image = null;
                label3.Text = "Num of Connected region : ";
                label4.Text = "Scaling factor : ";
                label5.Text = "The rotation angle 𝜃 : ";
                label6.Text = "Intensity difference : ";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void Save_Image(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "All Files|*.*|Bitmap Files (.bmp)|*.bmp|Jpeg File(.jpg)|*.jpg";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.Image.Save(sfd.FileName);
            }
        }

        private void Undo(object sender, EventArgs e)
        {
            //案UNDO要把當前state存的圖片清空
            try
            {
                undo_image = processing_stack.Pop();
                pictureBox2.Image = undo_image;
                process_image = undo_image;
                // Press Undo will make flag up
                flag = 1;
                // Make pictureBox1 can show the image before last processing
                
                Bitmap previous_image = processing_stack.Pop();
                processing_stack.Push(previous_image);
                pictureBox1.Image = previous_image;
                
            }
            catch (Exception)
            {
                MessageBox.Show("Already Back To First Image");
            }
        }

        private Bitmap RGB_Extracter(int mode, Bitmap curBitmap)
        {
                Bitmap d = new Bitmap(curBitmap.Width, curBitmap.Height);
                if (mode == 1)
                { 
                    for (int i = 0; i < curBitmap.Width; i++)
                    {
                        for (int x = 0; x < curBitmap.Height; x++)
                        {
                            Color oc = curBitmap.GetPixel(i, x);
                            // 取單一通道資料
                            int grayScale = (int)((oc.R * 1) + (oc.G * 0) + (oc.B * 0));
                            Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                            d.SetPixel(i, x, nc);
                        }
                    }
                }
                else if(mode == 2)
                {
                    for (int i = 0; i < curBitmap.Width; i++)
                    {
                        for (int x = 0; x < curBitmap.Height; x++)
                        {
                            Color oc = curBitmap.GetPixel(i, x);
                            int grayScale = (int)((oc.R * 0) + (oc.G * 1) + (oc.B * 0));
                            Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                            d.SetPixel(i, x, nc);
                        }
                    }
                }
                else if (mode == 3)
                {
                    for (int i = 0; i < curBitmap.Width; i++)
                    {
                        for (int x = 0; x < curBitmap.Height; x++)
                        {
                            Color oc = curBitmap.GetPixel(i, x);
                            int grayScale = (int)((oc.R * 0) + (oc.G * 0) + (oc.B * 1));
                            Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                            d.SetPixel(i, x, nc);
                        }
                    }
                }
                else if (mode == 4)
                {
                    for (int i = 0; i < curBitmap.Width; i++)
                    {
                        for (int x = 0; x < curBitmap.Height; x++)
                        {
                            Color oc = curBitmap.GetPixel(i, x);
                            int grayScale = (int)((oc.R * 0.299) + (oc.G * 0.587) + (oc.B * 0.114));
                            Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                            d.SetPixel(i, x, nc);
                        }
                    }
                }
                return (d);
        }

        private Bitmap Meanfilter(Bitmap Source)
        {
            int[] mask = new int[9];
            int k = 3;
            int width = Source.Width;
            int height = Source.Height;

            BitmapData SourceData = Source.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr intPtr = SourceData.Scan0;
            int size = SourceData.Stride * height;
            byte[] source_byte = new byte[size];

            Bitmap Mean_Image = new Bitmap(Source.Width, Source.Height);
            BitmapData Mean_Image_Data = Mean_Image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ResultPtr = Mean_Image_Data.Scan0;
            byte[] Mean_byte = new byte[size];

            System.Runtime.InteropServices.Marshal.Copy(intPtr, source_byte, 0, size);
            System.Runtime.InteropServices.Marshal.Copy(ResultPtr, Mean_byte, 0, size);

            for (int y = 0; y < height - 2; y++)
            {
                for (int x = 0; x < width - 2; x++)
                {
                    mask[0] = source_byte[y * SourceData.Stride + x * k];
                    mask[1] = source_byte[y * SourceData.Stride + x * k + 3];
                    mask[2] = source_byte[y * SourceData.Stride + x * k + 6];

                    mask[3] = source_byte[(y + 1) * SourceData.Stride + x * k];
                    mask[4] = source_byte[(y + 1) * SourceData.Stride + x * k + 3];
                    mask[5] = source_byte[(y + 1) * SourceData.Stride + x * k + 6];

                    mask[6] = source_byte[(y + 2) * SourceData.Stride + x * k];
                    mask[7] = source_byte[(y + 2) * SourceData.Stride + x * k + 3];
                    mask[8] = source_byte[(y + 2) * SourceData.Stride + x * k + 6];

                    int mean = (mask[0] + mask[1] + mask[2] + mask[3] + mask[4] + mask[5] + mask[6] + mask[7] + mask[8]) / 9;

                    Mean_byte[(y + 1) * SourceData.Stride + x * k + 3] = (byte)mean;
                    Mean_byte[(y + 1) * SourceData.Stride + x * k + 4] = (byte)mean;
                    Mean_byte[(y + 1) * SourceData.Stride + x * k + 5] = (byte)mean;

                }
            }
            System.Runtime.InteropServices.Marshal.Copy(source_byte, 0, intPtr, size);
            System.Runtime.InteropServices.Marshal.Copy(Mean_byte, 0, ResultPtr, size);
            Source.UnlockBits(SourceData);
            Mean_Image.UnlockBits(Mean_Image_Data);

            return Mean_Image;
        }

        private Bitmap Medianfilter(Bitmap Source)
        {
            int[] mask = new int[9];
            int k = 3;
            int width = Source.Width;
            int height = Source.Height;

            BitmapData SourceData = Source.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr intPtr = SourceData.Scan0;
            int size = SourceData.Stride * height;
            byte[] source_byte = new byte[size];

            Bitmap Mean_Image = new Bitmap(Source.Width, Source.Height);
            BitmapData Mean_Image_Data = Mean_Image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr ResultPtr = Mean_Image_Data.Scan0;
            byte[] Mean_byte = new byte[size];

            System.Runtime.InteropServices.Marshal.Copy(intPtr, source_byte, 0, size);
            System.Runtime.InteropServices.Marshal.Copy(ResultPtr, Mean_byte, 0, size);

            for (int y = 0; y < height - 2; y++)
            {
                for (int x = 0; x < width - 2; x++)
                {
                    mask[0] = source_byte[y * SourceData.Stride + x * k];
                    mask[1] = source_byte[y * SourceData.Stride + x * k + 3];
                    mask[2] = source_byte[y * SourceData.Stride + x * k + 6];

                    mask[3] = source_byte[(y + 1) * SourceData.Stride + x * k];
                    mask[4] = source_byte[(y + 1) * SourceData.Stride + x * k + 3];
                    mask[5] = source_byte[(y + 1) * SourceData.Stride + x * k + 6];

                    mask[6] = source_byte[(y + 2) * SourceData.Stride + x * k];
                    mask[7] = source_byte[(y + 2) * SourceData.Stride + x * k + 3];
                    mask[8] = source_byte[(y + 2) * SourceData.Stride + x * k + 6];

                    Array.Sort(mask);
                    int median = mask[4];

                    Mean_byte[(y + 1) * SourceData.Stride + x * k + 3] = (byte)median;
                    Mean_byte[(y + 1) * SourceData.Stride + x * k + 4] = (byte)median;
                    Mean_byte[(y + 1) * SourceData.Stride + x * k + 5] = (byte)median;

                }
            }
            System.Runtime.InteropServices.Marshal.Copy(source_byte, 0, intPtr, size);
            System.Runtime.InteropServices.Marshal.Copy(Mean_byte, 0, ResultPtr, size);
            Source.UnlockBits(SourceData);
            Mean_Image.UnlockBits(Mean_Image_Data);

            return Mean_Image;
        }

        private void R_Extract(object sender, EventArgs e)
        {
            pictureBox1.Image = process_image;
            Bitmap R_img = RGB_Extracter(1, process_image);
            pictureBox2.Image = R_img;
            Process_img_saver(R_img);
        }

        private void G_Extract(object sender, EventArgs e)
        {
            pictureBox1.Image = process_image;
            Bitmap G_img = RGB_Extracter(2, process_image);
            pictureBox2.Image = G_img;
            Process_img_saver(G_img);
        }

        private void B_Extract(object sender, EventArgs e)
        {
            pictureBox1.Image = process_image;
            Bitmap B_img = RGB_Extracter(3, process_image);
            pictureBox2.Image = B_img;
            Process_img_saver(B_img);
        }

        private void Gray_Extract(object sender, EventArgs e)
        {
            pictureBox1.Image = process_image;
            Bitmap Grayscale =  RGB_Extracter(4, process_image);
            pictureBox2.Image = Grayscale;
            Process_img_saver(Grayscale);
        }

        private void Mean_Filter(object sender, EventArgs e)
        {
            pictureBox1.Image = process_image;
            Bitmap Mean = Meanfilter(process_image);
            pictureBox2.Image = Mean;
            Process_img_saver(Mean);
        }

        private void Median_Filter(object sender, EventArgs e)
        {
            pictureBox1.Image = process_image;
            Bitmap Median = Medianfilter(process_image);
            pictureBox2.Image = Median;
            Process_img_saver(Median);
        }

        private Bitmap DrawTest(Bitmap bmp)
        {
            int[] histogram_r = new int[256];
            float max = 0;

            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    //Source image will be gray level so not mind
                    int redValue = bmp.GetPixel(i, j).R;
                    //MessageBox.Show(redValue.ToString());
                    histogram_r[redValue]++;
                    //MessageBox.Show(histogram_r[redValue].ToString());
                    if (max < histogram_r[redValue])
                        max = histogram_r[redValue];
                }
            }

            int histHeight = 128;
            Bitmap img = new Bitmap(356, histHeight + 60);
            using (Graphics g = Graphics.FromImage(img))
            {
                for (int i = 0; i < histogram_r.Length; i++)
                {
                    float pct = histogram_r[i] / max;   // What percentage of the max is this value?
                    g.DrawLine(Pens.Blue,
                        new Point(50+i, img.Height - 30),
                        new Point(50+i, img.Height - 30 - (int)(pct * histHeight))  // Use that percentage of the height
                        );
                }
                Pen curPen = new Pen(Brushes.Black, 1);
                Pen curPen_2 = new Pen(Brushes.Black, 1);
                curPen_2.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawLine(curPen, 50, 158, 306, 158);    // X axis
                g.DrawLine(curPen, 50, 20, 50, 158);        // Y axis
                g.DrawLine(curPen, 50, 20, 306, 20);        // Right axis
                g.DrawLine(curPen, 306, 20, 306, 158);    // Top axis 
                g.DrawString("Histogram of Gray Level", new Font("Arial", 10), Brushes.Black, new PointF(105, 5));
                float[] Y = new float[6];
                int j = 0;

                for (int i = 0; i < 6; i++)
                {
                    Y[i] = (float)((Math.Floor(max / 1000.0) - i) * 1000);
                    if(Y[i] <= 0)  Y[i] = 0;
                    else  j++;
                }
                int Y_axis_dis = 128 / j;
                for (int i = 0; i < 6; i++)
                {
                    int X_axis = (i * 50);
                    g.DrawString(X_axis.ToString(), new Font("Arial", 8f), Brushes.Black, new PointF(40 + i *50, 158));
                    if (i != 0)
                    g.DrawLine(curPen_2, new PointF(50 + i * 50, 158), new PointF(50 + i * 50, 20));
                }
                for(int i = 0; i < j; i++)
                {
                    float Y_axis = Y[j - i - 1];
                    if (Y_axis != 0)
                    {
                        g.DrawString(Y_axis.ToString(), new Font("Arial", 8f), Brushes.Black, new PointF(20, 158 - (i + 1) * Y_axis_dis));
                        g.DrawLine(curPen_2, new PointF(50, 158 - (i + 1) * Y_axis_dis), new PointF(306, 158 - (i + 1) * Y_axis_dis));
                    }
                }

            }

            return (img);
        }

        private Bitmap EQ(Bitmap bitmap)
        {   
                Bitmap newbitmap = bitmap.Clone() as Bitmap;        
                int width = newbitmap.Width;
                int height = newbitmap.Height;
                int size = width * height;
                //總像素個數
                int[] gray = new int[256];
                //存放各像元值的個數
                double[] graydense = new double[256];
                //存放每個灰階像素個數占比
                for (int i = 0; i < width; ++i)
                    for (int j = 0; j < height; ++j)
                    {
                        Color pixel = newbitmap.GetPixel(i, j);
                        //計算各像元值的個數
                        gray[Convert.ToInt16(pixel.R)] += 1;
                        //因為灰階讀R值即可
                    }
                for (int i = 0; i < 256; i++)
                {
                    graydense[i] = (gray[i] * 1.0) / size;
                    //每個灰階像素個數占比
                }

                for (int i = 1; i < 256; i++)
                {
                    graydense[i] = graydense[i] + graydense[i - 1];
                    //累計百分比
                }

                for (int i = 0; i < width; ++i)
                    for (int j = 0; j < height; ++j)
                    {
                        Color pixel = newbitmap.GetPixel(i, j);
                        int oldpixel = Convert.ToInt16(pixel.R);//原始灰度
                        int newpixel = 0;
                        if (oldpixel == 0)
                            newpixel = 0;
                        //如果原始灰階值=0 則變換後也=0
                        else
                            newpixel = Convert.ToInt16(graydense[Convert.ToInt16(pixel.R)] * 255);
                        //如果原始灰階質不=0，則公式為   <新pixel灰階 = 原始灰階 * 累計百分比>
                        pixel = Color.FromArgb(newpixel, newpixel, newpixel);
                        newbitmap.SetPixel(i, j, pixel);
                    }
                return (newbitmap);
        }

        private void Histogram_Equalization(object sender, EventArgs e)
        {
            pictureBox1.Image = process_image;
            Bitmap testimg = DrawTest(process_image);
            pictureBox3.Image = testimg;

            Bitmap Eq_img = EQ(process_image);
            pictureBox2.Image = Eq_img;
            Process_img_saver(Eq_img);
           
            Bitmap testimg_2 = DrawTest(process_image);
            pictureBox4.Image = testimg_2;
        }
        //
        public Bitmap ConvertTo1Bpp(Bitmap bmp, int threshold)
        {
            Bitmap newbitmap = bmp.Clone() as Bitmap;//clone一个副本
            for (int i = 0; i < newbitmap.Width; i++)
            {
                for (int j = 0; j < newbitmap.Height; j++)
                {
                    //获取该点的像素的RGB的颜色
                    Color color = newbitmap.GetPixel(i, j);
                    int value = 255 - color.B;
                    Color newColor = value > threshold ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
                    newbitmap.SetPixel(i, j, newColor);
                }
            }
            return(newbitmap);
        }

        private void Thresholding(object sender, EventArgs e)
        {
            pictureBox1.Image = process_image;
            Bitmap Thresholding_img = ConvertTo1Bpp(process_image, Threshold_value);
            pictureBox2.Image = Thresholding_img;
            Process_img_saver(Thresholding_img);
        }

        private void Thresholding_Scroll(object sender, EventArgs e)
        {
            trackBar1.Maximum = 255;
            trackBar1.Minimum = 0;
            Threshold_value = trackBar1.Value;
        }
        //

        private static Bitmap ConvolutionFilter(Bitmap sourceImage, double[,] xkernel, double[,] ykernel, int mode)
        {

            //Image dimensions stored in variables for convenience
            int width = sourceImage.Width;
            int height = sourceImage.Height;

            //Lock source image bits into system memory
            BitmapData srcData = sourceImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            //Get the total number of bytes in your image - 32 bytes per pixel x image width x image height -> for 32bpp images
            int bytes = srcData.Stride * srcData.Height;

            //Create byte arrays to hold pixel information of your image
            byte[] pixelBuffer = new byte[bytes];
            byte[] resultBuffer = new byte[bytes];

            //Get the address of the first pixel data
            IntPtr srcScan0 = srcData.Scan0;

            //Copy image data to one of the byte arrays
            System.Runtime.InteropServices.Marshal.Copy(srcScan0, pixelBuffer, 0, bytes);

            //Unlock bits from system memory -> we have all our needed info in the array
            sourceImage.UnlockBits(srcData);

            //Create variable for pixel data for each kernel
            double xr = 0.0;
            double xg = 0.0;
            double xb = 0.0;
            double yr = 0.0;
            double yg = 0.0;
            double yb = 0.0;
            double rt = 0.0;
            double gt = 0.0;
            double bt = 0.0;

            //This is how much your center pixel is offset from the border of your kernel
            //Sobel is 3x3, so center is 1 pixel from the kernel border
            int filterOffset = 1;
            int calcOffset = 0;
            int byteOffset = 0;

            //Start with the pixel that is offset 1 from top and 1 from the left side
            //this is so entire kernel is on your image
            for (int OffsetY = filterOffset; OffsetY < height - filterOffset; OffsetY++)
            {
                for (int OffsetX = filterOffset; OffsetX < width - filterOffset; OffsetX++)
                {
                    //reset rgb values to 0
                    xr = xg = xb = yr = yg = yb = 0;
                    rt = gt = bt = 0.0;

                    //position of the kernel center pixel
                    byteOffset = OffsetY * srcData.Stride + OffsetX * 4;
                    //kernel calculations
                    for (int filterY = -filterOffset; filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset; filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset + filterX * 4 + filterY * srcData.Stride;
                            xb += (double)(pixelBuffer[calcOffset]) * xkernel[filterY + filterOffset, filterX + filterOffset];
                            xg += (double)(pixelBuffer[calcOffset + 1]) * xkernel[filterY + filterOffset, filterX + filterOffset];
                            xr += (double)(pixelBuffer[calcOffset + 2]) * xkernel[filterY + filterOffset, filterX + filterOffset];
                            yb += (double)(pixelBuffer[calcOffset]) * ykernel[filterY + filterOffset, filterX + filterOffset];
                            yg += (double)(pixelBuffer[calcOffset + 1]) * ykernel[filterY + filterOffset, filterX + filterOffset];
                            yr += (double)(pixelBuffer[calcOffset + 2]) * ykernel[filterY + filterOffset, filterX + filterOffset];
                        }
                    }
                    if (mode == 0)
                    {
                        //set limits, bytes can hold values from 0 up to 255;
                        if (xb > 255) xb = 255;
                        else if (xb < 0) xb = 0;
                        if (xg > 255) xg = 255;
                        else if (xg < 0) xg = 0;
                        if (xr > 255) xr = 255;
                        else if (xr < 0) xr = 0;

                        //set new data in the other byte array for your image data
                        resultBuffer[byteOffset] = (byte)(xb);
                        resultBuffer[byteOffset + 1] = (byte)(xg);
                        resultBuffer[byteOffset + 2] = (byte)(xr);
                        resultBuffer[byteOffset + 3] = 255;
                    }
                    else if (mode == 1)
                    {
                        //set limits, bytes can hold values from 0 up to 255;
                        if (yb > 255) yb = 255;
                        else if (yb < 0) yb = 0;
                        if (yg > 255) yg = 255;
                        else if (yg < 0) yg = 0;
                        if (yr > 255) yr = 255;
                        else if (yr < 0) yr = 0;

                        //set new data in the other byte array for your image data
                        resultBuffer[byteOffset] = (byte)(yb);
                        resultBuffer[byteOffset + 1] = (byte)(yg);
                        resultBuffer[byteOffset + 2] = (byte)(yr);
                        resultBuffer[byteOffset + 3] = 255;
                    }
                    else if (mode == 2)
                    {
                        //total rgb values for this pixel
                        bt = Math.Sqrt((xb * xb) + (yb * yb));
                        gt = Math.Sqrt((xg * xg) + (yg * yg));
                        rt = Math.Sqrt((xr * xr) + (yr * yr));

                        //set limits, bytes can hold values from 0 up to 255;
                        if (bt > 255) bt = 255;
                        else if (bt < 0) bt = 0;
                        if (gt > 255) gt = 255;
                        else if (gt < 0) gt = 0;
                        if (rt > 255) rt = 255;
                        else if (rt < 0) rt = 0;

                        //set new data in the other byte array for your image data
                        resultBuffer[byteOffset] = (byte)(bt);
                        resultBuffer[byteOffset + 1] = (byte)(gt);
                        resultBuffer[byteOffset + 2] = (byte)(rt);
                        resultBuffer[byteOffset + 3] = 255;
                    }
                }
            }
            //Create new bitmap which will hold the processed data
            Bitmap resultImage = new Bitmap(width, height);
            //Lock bits into system memory
            BitmapData resultData = resultImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            //Copy from byte array that holds processed data to bitmap
            System.Runtime.InteropServices.Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            //Unlock bits from system memory
            resultImage.UnlockBits(resultData);
            //Return processed image
            return resultImage;
        }
        //Sobel operator kernel for horizontal pixel changes
        private static double[,] xSobel
        {
            get
            {
                return new double[,]
                {
            { -1, 0, 1 },
            { -2, 0, 2 },
            { -1, 0, 1 }
                };
            }
        }
        //Sobel operator kernel for vertical pixel changes
        private static double[,] ySobel
        {
            get
            {
                return new double[,]
                {
            {  1,  2,  1 },
            {  0,  0,  0 },
            { -1, -2, -1 }
                };
            }
        }
        private void Vertical(object sender, EventArgs e)
        {
            pictureBox1.Image = process_image;
            Bitmap Combine_img = ConvolutionFilter(process_image, xSobel, ySobel, 0);
            pictureBox2.Image = Combine_img;
            Process_img_saver(Combine_img);
        }

        private void Horizontal(object sender, EventArgs e)
        {
            pictureBox1.Image = process_image;
            Bitmap Combine_img = ConvolutionFilter(process_image, xSobel, ySobel, 1);
            pictureBox2.Image = Combine_img;
            Process_img_saver(Combine_img);
        }

        private void Combine(object sender, EventArgs e)
        {
            pictureBox1.Image = process_image;
            Bitmap Combine_img = ConvolutionFilter(process_image, xSobel, ySobel, 2);
            pictureBox2.Image = Combine_img;
            Process_img_saver(Combine_img);
        }

        private Bitmap Contour_finder(Bitmap source)
        {
            Bitmap contour = source.Clone() as Bitmap;
            contour.MakeTransparent(Color.Black);//当图片的背景为black时
            Color newColor = Color.Lime;
            for (int x = 0; x < contour.Width; x++)
            {
                for (int y = 0; y < contour.Height; y++)
                {
                    Color pixelColor = contour.GetPixel(x, y);
                    if (pixelColor.ToArgb() == -1)
                    {
                        contour.SetPixel(x, y, newColor);
                    }
                }
            }
            return contour;
        }

        private Bitmap Overlap_image(Bitmap contour)
        {
            Image imgBackground = openImg.Clone() as Image; //Background
            Image imgForeground = contour;   // Foreground
            Graphics g = Graphics.FromImage(imgBackground);
            g.DrawImage(imgForeground,
                new Rectangle(0, 0, imgForeground.Width, imgForeground.Height),
                new Rectangle(0, 0, imgForeground.Width, imgForeground.Height),
                GraphicsUnit.Pixel);
            g.Dispose();
            pictureBox2.Image = imgBackground;
            Bitmap bitmap = new Bitmap(imgBackground);
            return bitmap;
        }
        private void Overlap(object sender, EventArgs e)
        {
            pictureBox1.Image = openImg;
            Bitmap Thresholding_img = ConvertTo1Bpp(process_image, Threshold_value_overlap);
            Bitmap contour = Contour_finder(Thresholding_img);
            Bitmap concate = Overlap_image(contour);
            Process_img_saver(concate);
        }

        private void Overlap_Scroll(object sender, EventArgs e)
        {
            trackBar2.Maximum = 255;
            trackBar2.Minimum = 0;
            Threshold_value_overlap = trackBar2.Value;
        }

        private Bitmap Connected_Comnponent(Bitmap Source)
        {
            int height = Source.Height;
            int width = Source.Width;
            Bitmap Background = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            
            int[,] label = new int[width, height];
            int LabelCount = 1;
            int TotalLabelConut = 0;
            var set = new HashSet<int>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int PIC = Source.GetPixel(x, y).R;
                    if (PIC == 0)//if black
                    {
                        if (x == 0 && y == 0) //  (0, 0)
                        {
                            label[x, y] = LabelCount;
                            set.Add(LabelCount);
                            LabelCount++;
                            TotalLabelConut++;
                        }
                        else if (y == 0 && x != 0) //top line
                        {
                            if (label[x - 1, y] != 0)
                                label[x, y] = label[x - 1, y];
                            else
                            {
                                label[x, y] = LabelCount;
                                set.Add(LabelCount);
                                LabelCount++;
                                TotalLabelConut++;
                            }
                        }
                        else if (x == 0 && y != 0) //left line
                        {
                            if (label[x, y - 1] != 0)
                                label[x, y] = label[x, y - 1];
                            else
                            {
                                label[x, y] = LabelCount;
                                set.Add(LabelCount);
                                LabelCount++;
                                TotalLabelConut++;
                            }
                        }
                        else
                        {
                            if (label[x - 1, y - 1] != 0 && label[x, y - 1] == 0 && label[x - 1, y] == 0)               //  left-top = 1
                                label[x, y] = label[x - 1, y - 1];
                            else if (label[x - 1, y - 1] == 0 && label[x, y - 1] != 0 && label[x - 1, y] == 0)        //  top = 1
                                label[x, y] = label[x, y - 1];
                            else if (label[x - 1, y - 1] == 0 && label[x, y - 1] == 0 && label[x - 1, y] != 0)        //  left = 1
                                label[x, y] = label[x - 1, y];
                            else if (label[x - 1, y - 1] != 0 && label[x, y - 1] != 0 && label[x - 1, y] == 0)         //  left-top = top = 1
                            {
                                label[x, y] = label[x - 1, y - 1];
                                int TOPLabel = label[x, y - 1];
                                int Left_Top_Label = label[x - 1, y - 1];
                                if (TOPLabel != Left_Top_Label) //left!=top
                                {
                                    for (int i = 0; i < width; i++)
                                    {
                                        for (int j = 0; j < height; j++)
                                        {
                                            if (label[i, j] == TOPLabel)
                                            {
                                                label[i, j] = Left_Top_Label;
                                            }
                                        }
                                    }
                                    TotalLabelConut--;
                                }
                            }
                            else if (label[x - 1, y - 1] != 0 && label[x, y - 1] == 0 && label[x - 1, y] != 0)         //  left-top = left = 1
                            {
                                label[x, y] = label[x - 1, y - 1];
                                int Left_TOPLabel = label[x - 1, y - 1];
                                int Left_Label = label[x - 1, y];
                                if (Left_Label != Left_TOPLabel) //left!=top
                                {
                                    for (int i = 0; i < width; i++)
                                    {
                                        for (int j = 0; j < height; j++)
                                        {
                                            if (label[i, j] == Left_Label)
                                            {
                                                label[i, j] = Left_TOPLabel;
                                            }
                                        }
                                    }
                                    TotalLabelConut--;
                                }
                            }
                            else if (label[x - 1, y - 1] == 0 && label[x, y - 1] != 0 && label[x - 1, y] != 0)         //  top = left = 1
                            {
                                label[x, y] = label[x, y - 1];
                                int TOPLabel = label[x, y - 1];
                                int LeftLabel = label[x - 1, y];
                                if (TOPLabel != LeftLabel) //left!=top
                                {
                                    for (int i = 0; i < width; i++)
                                    {
                                        for (int j = 0; j < height; j++)  
                                        {
                                            if (label[i, j] == LeftLabel)
                                            {
                                                label[i, j] = TOPLabel;
                                            }
                                        }
                                    }
                                    TotalLabelConut--;
                                }
                            }
                            else if (label[x - 1, y - 1] != 0 && label[x, y - 1] != 0 && label[x - 1, y] != 0)         //  left-top = top = left = 1
                            {
                                label[x, y] = label[x - 1, y - 1];
                                int Top_Label = label[x, y - 1];
                                int Left_Label = label[x - 1, y];
                                int Top_Left_Label = label[x - 1, y - 1];
                                if (Top_Left_Label != Top_Label || Top_Left_Label != Left_Label || Top_Label != Left_Label)
                                {
                                    for (int i = 0; i < width; i++)
                                    {
                                        for (int j = 0; j < height; j++) 
                                        {
                                            if (label[i, j] == Left_Label)
                                            {
                                                label[i, j] = Top_Left_Label;
                                            }
                                            else if (label[i, j] == Top_Label)
                                            {
                                                label[i, j] = Top_Left_Label;
                                            }
                                        }
                                    }
                                    TotalLabelConut--;
                                }
                            }
                            else
                            {
                                label[x, y] = LabelCount;
                                set.Add(LabelCount);
                                LabelCount++;
                                TotalLabelConut++;
                            }
                        }
                    }
                    else//if white
                        label[x, y] = 0;
                }
            }           
            // Label Normalization
            for (int X_nor = 0; X_nor < Background.Width; X_nor++)
            {
                for (int Y_nor = 0; Y_nor < Background.Height; Y_nor++)
                {
                    if (label[X_nor, Y_nor] != 0)
                    {
                        label[X_nor, Y_nor] = (label[X_nor, Y_nor] % set.Count) + 1;
                    }
                }
            }

            // Random color generator for Label
            Color[] label_Color = new Color[set.Count];
            Random random = new Random();
            for (int Color_num = 0; Color_num < set.Count; Color_num++)
            {
                Color random_color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
                label_Color[Color_num] = random_color;
            }

            // Color on the Background
            Graphics g = Graphics.FromImage(Background);
            for (int Xcount = 0; Xcount < Background.Width; Xcount++)
            {
                for (int Ycount = 0; Ycount < Background.Height; Ycount++)
                {
                    for (int C_num = 1; C_num < set.Count + 1; C_num++)
                    {
                        if (label[Xcount, Ycount] == C_num)
                        {
                            Background.SetPixel(Xcount, Ycount, label_Color[C_num-1]);
                            break;
                        }
                        else if (label[Xcount, Ycount] == 0)
                        {
                            Background.SetPixel(Xcount, Ycount, Color.White);
                            break;
                        }
                    }
                   
                }
            }            
            g.DrawImage(Background, Background.Width, 0, Background.Width, Background.Height);
            g.Dispose();
            label3.Text = "Num of Connected region : " + (TotalLabelConut - 1);
            return Background;
        }
        private void Connected_Component(object sender, EventArgs e)
        {
            pictureBox1.Image = process_image;
            Bitmap Connected_Component_Image = Connected_Comnponent(process_image);
            pictureBox2.Image = Connected_Component_Image;       //Concate picture
            Process_img_saver(Connected_Component_Image);
        }

        
        private void Image_Registration(object sender, EventArgs e)
        {
            //label4.Text = Position_2[3,1].ToString();
            double[,] Cal_Matrix_X = Make_Matrix(0, Position_1, Position_2);
            double[,] Cal_Matrix_Y = Make_Matrix(1, Position_1, Position_2);
            Solve_Matrix(Cal_Matrix_X);
            Solve_Matrix(Cal_Matrix_Y);
            // T12 / T11
            double Arc = Math.Atan(Cal_Matrix_Y[0, 3] / Cal_Matrix_X[0, 3]);
            double Theta = Arc / Math.PI * 180;
            double Scale_factor_X_1 = Cal_Matrix_Y[0, 3] / Cal_Matrix_Y[0, 0]/ Math.Sin(Arc);
            double Scale_factor_Y_1 = Cal_Matrix_Y[1, 3] / Cal_Matrix_Y[1, 1]/Math.Cos(Arc);
            float angle = (float)Theta;
            
            Bitmap Rotate_image = Rotate(process_image, angle);
            Bitmap Scale_image = ScaleImage(Rotate_image, 1/ Scale_factor_X_1 , 1/ Scale_factor_Y_1);
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap Origin = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Size = new Size(Origin.Width, Origin.Height);
                pictureBox1.Image = Origin;
            }
            pictureBox2.Size = new Size(pictureBox1.Width, pictureBox1.Height);

            int[,] A = ToInteger(pictureBox1.Image as Bitmap);
            int[,] B = ToInteger(Scale_image);
            double Intensity = Intensity_sum(A, B);

            label4.Text = "Scaling factor : [X: " + (1 / Scale_factor_X_1).ToString("0.0000") + ",Y: " + (1/ Scale_factor_Y_1).ToString("0.0000") + "]";
            label5.Text = "The rotation angle 𝜃 : " + angle.ToString("0.0000");
            label6.Text = "Intensity difference : " + Intensity.ToString("0.0000");
            pictureBox2.Image = Scale_image;
            Process_img_saver(Scale_image);
        }

        private void pictureBox1_MouseMove_1(object sender, MouseEventArgs e)
        {
                //label4.Text = e.X.ToString() + ":" + e.Y.ToString();
        }
        
        public int[,] Position_1 = new int[4, 2];
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //string Posi = e.X.ToString() + ":" + e.Y.ToString();
            //label1.Text = Posi;
            /*
            if (e.X<= 73 && e.X>=53 && e.Y<=68 && e.Y >= 48)        // Left Top
            {
                Position_1[0, 0] = e.X;
                Position_1[0, 1] = e.Y;
            }
            else if (e.X <= 465 && e.X >= 445 && e.Y <= 68 && e.Y >= 48)        // Right Top
            {
                Position_1[1, 0] = e.X;
                Position_1[1, 1] = e.Y;
            }
            else if (e.X <= 471 && e.X >= 451 && e.Y <= 327 && e.Y >= 307)      //  Right Bottom
            {
                Position_1[2, 0] = e.X;
                Position_1[2, 1] = e.Y;
            }
            else if (e.X <= 73 && e.X >= 53 && e.Y <= 324 && e.Y >= 304)        // Left Bottom
            {
                Position_1[3, 0] = e.X;
                Position_1[3, 1] = e.Y;
            }
            */
            if (e.X <= 36 && e.X >= 16 && e.Y <= 39 && e.Y >= 19)        // Left Top
            {
                Position_1[0, 0] = e.X;
                Position_1[0, 1] = e.Y;
            }
            else if (e.X <= 427 && e.X >= 407 && e.Y <= 41 && e.Y >= 21)        // Right Top
            {
                Position_1[1, 0] = e.X;
                Position_1[1, 1] = e.Y;
            }
            else if (e.X <= 434 && e.X >= 414 && e.Y <= 300 && e.Y >= 280)      //  Right Bottom
            {
                Position_1[2, 0] = e.X;
                Position_1[2, 1] = e.Y;
            }
            else if (e.X <= 36 && e.X >= 16 && e.Y <= 295 && e.Y >= 275)        // Left Bottom
            {
                Position_1[3, 0] = e.X;
                Position_1[3, 1] = e.Y;
            }

        }
        public int[,] Position_2 = new int[4, 2];
        private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            //string Posi = e.X.ToString() + ":" + e.Y.ToString();
            //label2.Text = Posi;
            /*
            if (e.X <= 190 && e.X >= 174 && e.Y <= 38 && e.Y >= 22)             // Left Top
            {
                Position_2[0, 0] = e.X;
                Position_2[0, 1] = e.Y;
            }
            else if (e.X <= 443 && e.X >= 427 && e.Y <= 185 && e.Y >= 169)   // Right Top
            {
                Position_2[1, 0] = e.X;
                Position_2[1, 1] = e.Y;
            }
            else if (e.X <= 350 && e.X >= 334 && e.Y <= 355 && e.Y >= 339)      // Right Bottom
            {
                Position_2[2, 0] = e.X;
                Position_2[2, 1] = e.Y;
            }
            else if (e.X <= 95 && e.X >= 79 && e.Y <= 223 && e.Y >= 187)        // Left Bottom
            {
                Position_2[3, 0] = e.X;
                Position_2[3, 1] = e.Y;
            }
            */
            if (e.X <= 138 && e.X >= 122 && e.Y <= 37 && e.Y >= 21)             // Left Top
            {
                Position_2[0, 0] = e.X;
                Position_2[0, 1] = e.Y;
            }
            else if (e.X <= 390 && e.X >= 374 && e.Y <= 183 && e.Y >= 167)   // Right Top
            {
                Position_2[1, 0] = e.X;
                Position_2[1, 1] = e.Y;
            }
            else if (e.X <= 297 && e.X >= 281 && e.Y <= 353 && e.Y >= 337)      // Right Bottom
            {
                Position_2[2, 0] = e.X;
                Position_2[2, 1] = e.Y;
            }
            else if (e.X <= 42 && e.X >= 26 && e.Y <= 201 && e.Y >= 185)        // Left Bottom
            {
                Position_2[3, 0] = e.X;
                Position_2[3, 1] = e.Y;
            }
        }
        private double[,] Make_Matrix(int mode, int[,] src, int[,] des)
        {
            double[,] Process = new double[4, 4];
            if (mode == 0)
            {
                for(int i=0; i < 4; i++)
                {
                    Process[i, 0] = src[i, 0];
                    Process[i, 1] = src[i, 1];
                    Process[i, 2] = 1;
                    Process[i, 3] = des[i, 0];
                }
            }
            else if (mode == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    Process[i, 0] = src[i, 0];
                    Process[i, 1] = src[i, 1];
                    Process[i, 2] = 1;
                    Process[i, 3] = des[i, 1];
                }
            }
            return Process;
        }
        private void Solve_Matrix(double[,] array)
        {
            /*
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    string title = j.ToString() + ":" + i.ToString();
                    MessageBox.Show(array[j, i].ToString(), title);
                }
            }
            */
            // Reduce First column
            for (int i = 0; i < 4; ++i)
            {
                if (i == 0) continue;
                double sc = array[i, 0] / array[0, 0];
                array[i, 0] = 0;
                array[i, 1] -= array[0, 1] * sc;
                array[i, 2] -= array[0, 2] * sc;
                array[i, 3] -= array[0, 3] * sc;

            }
            // Reduce Second column
            for (int i = 0; i < 4; ++i)
            {
                if (i == 1) continue;
                double sc = array[i, 1] / array[1, 1];
                array[i, 1] = 0;
                array[i, 2] -= array[1, 2] * sc;
                array[i, 3] -= array[1, 3] * sc;
            }
            // Reduce Third column
            for (int i = 0; i < 4; ++i)
            {
                if (i == 2) continue;
                double sc = array[i, 2] / array[2, 2];
                array[i, 2] = 0;
                array[i, 3] -= array[2, 3] * sc;
            }
            /*
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    string title = j.ToString() + ":" + i.ToString();
                    MessageBox.Show(array[j, i].ToString(), title);
                }
            }
            */
        }
        
        public static Bitmap Rotate(Bitmap b, float angle)
        {
            // Parameters
            angle %=  360;
            double radian = angle * Math.PI / 180.0;
            double cos = Math.Cos(radian);
            double sin = Math.Sin(radian);
            int w = b.Width;
            int h = b.Height;
            int W = (int)(Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin)));
            int H = (int)(Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos)));
            Bitmap dsImage = new Bitmap(W, H);
            Graphics g = Graphics.FromImage(dsImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            // Cal offset
            Point Offset = new Point((W - w) / 2, (H - h) / 2);
            // Make image center corresond to pictureBox center
            Rectangle rect = new Rectangle(Offset.X, Offset.Y, w, h);
            Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(360 - angle);
            g.TranslateTransform(-center.X, -center.Y);
            g.DrawImage(b, rect);
            g.ResetTransform();
            g.Save();
            g.Dispose();
            return dsImage;
        }

        static Bitmap ScaleImage(Bitmap bmp, double ratioX, double ratioY)
        {
            int newWidth = (int)(bmp.Width * ratioX);
            int newHeight = (int)(bmp.Height * ratioY);
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(newImage);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            g.DrawImage(bmp, 0, 0, newWidth, newHeight);
            g.Dispose();
            return newImage;
        }

        public static int[,] ToInteger(Bitmap input)
        {
            int Width = input.Width;
            int Height = input.Height;
            int[,] array2d = new int[Width, Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Color cl = input.GetPixel(x, y);
                    //Grayscale
                    int gray = (int)Convert.ChangeType(cl.R * 0.3 + cl.G * 0.59 + cl.B * 0.11, typeof(int));
                    array2d[x, y] = gray;
                }
            }
            return array2d;
        }

        public double Intensity_sum(int[,] A, int[,] B)
        {
            double sum = 0;

            for (int i = 1; i < A.GetLength(0); i++)
            {
                for (int j = 1; j < A.GetLength(1); j++)
                {
                    sum += Math.Abs(A[i, j] - B[i, j]);
                }
            }
            sum /= (A.GetLength(0) * A.GetLength(1)); 
            return sum;
        }
    }
}
