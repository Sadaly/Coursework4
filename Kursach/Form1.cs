using BumpKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Kursach
{
    public partial class Form1 : Form
    {
        public static List<string> imagesLinks = new List<string>();
        ImageList imgList = new ImageList();

        public Form1()
        {
            //Выбор размера отображаемых в списке изображений, только визуал
            imgList.ImageSize = new Size(128, 128); 
            InitializeComponent();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            //освобождаем память и удаляем дамп-файл
            if (File.Exists("dump.gif"))
            {
                if (GIFPictureBox.Image != null)
                    GIFPictureBox.Image.Dispose();
                File.Delete("dump.gif");
            }
        }

        private void OpenImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.BMP;*.PNG;*.JPG)|*.BMP;*.PNG;*.JPG|All files (*.*)|*.*";
            if (open.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }

            //imagesLinks - ссылка на картинки img - лист изображений
            imagesLinks.Add(open.FileName);
            imgList.Images.Add(Image.FromFile(imagesLinks[imagesLinks.Count-1]));

            listView1.Items.Add((imagesLinks.Count).ToString(), imagesLinks.Count - 1);
            listView1.LargeImageList = imgList;
        }
        
        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (imagesLinks.Count == 0)
            {
                MessageBox.Show("Сначала введите первый элемент");
            }
            
            else if (Convert.ToInt32(textBox1.Text) > 0)
            {
                //imagesLinks - ссылка на картинки img - лист изображений
                imgList.Images.Clear();
                imagesLinks.RemoveAt(Convert.ToInt32(textBox1.Text)-1);

                foreach (var item in imagesLinks)
                    imgList.Images.Add(Image.FromFile(item));
                
                listView1.Items.Clear();

                for (int i = 0; i < imagesLinks.Count; i++)
                {
                    listView1.Items.Add((i + 1).ToString(), i);
                }

                listView1.LargeImageList = imgList;
            }

            else
            {
                MessageBox.Show("Введите значение в пределах диапазона от 1 до " + imagesLinks.Count);
            }
        }

        private void insertButton_Click(object sender, EventArgs e)
        {
            if (imagesLinks.Count == 0)
            {
                MessageBox.Show("Сначала введите первый элемент");
            }
            
            else if (Convert.ToInt32(textBox2.Text) < imagesLinks.Count && Convert.ToInt32(textBox2.Text) > 0)
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Filter = "Image Files(*.BMP;*.PNG;*.JPG)|*.BMP;*.PNG;*.JPG|All files (*.*)|*.*";
                if (open.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }

                //imagesLinks - ссылка на картинки img - лист изображений
                imgList.Images.Clear();
                imagesLinks.Insert(Convert.ToInt32(textBox2.Text) - 1, open.FileName);

                foreach (var item in imagesLinks)
                    imgList.Images.Add(Image.FromFile(item));

                listView1.Items.Clear();

                for (int i = 0; i < imagesLinks.Count; i++)
                {
                    listView1.Items.Add((i + 1).ToString(), i);
                }

                listView1.LargeImageList = imgList;
            }

            else
            {
                MessageBox.Show("Введите значение в пределах диапазона от 1 до " + imagesLinks.Count);
            }

        }
        private void CreateGIF_Click(object sender, EventArgs e)
        {
            if (GIFPictureBox.Image != null)
            {
                GIFPictureBox.Image.Dispose();
            }
            
            if (imagesLinks.Count > 0)
            {
                double delay;
                if (delayNum.Text != "")
                {
                    delay = Convert.ToDouble(delayNum.Text);
                }
                else
                {
                    delay = 33;
                }


                using (var gif = File.OpenWrite("dump.gif"))
                {
                    using (var encoder = new GifEncoder(gif))
                        for (var i = 0; i < imagesLinks.Count; i += 1)
                        {
                            using (var frame = Image.FromFile(imagesLinks[i]).Rotate(0, false))
                            {
                                encoder.FrameDelay = TimeSpan.FromMilliseconds(delay);
                                encoder.AddFrame(frame);
                            }
                        }
                }

                GIFPictureBox.Image = Image.FromFile("dump.gif");
            }
            else
            {
                MessageBox.Show("Выберите изображения!");
            }
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            listView1.Clear();
            imgList.Dispose();
            imagesLinks.Clear();
            
            if (GIFPictureBox.Image != null)
            {
                GIFPictureBox.Image.Dispose();
                GIFPictureBox.Image = null;
            }

            delayNum.Text = "33";
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Image Files(*.GIF)|*.GIF|All files (*.*)|*.*";
            if (save.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }

            var img = Image.FromFile("dump.gif");
            img.Save(save.FileName);
        }
    }
}
