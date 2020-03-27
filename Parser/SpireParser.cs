//using iTextSharp.text;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Parser
{
    public static class SpireParser
    {
        private static PdfDocument Load(string filePath)
        {
            //实例化一个PdfDocument对象
            PdfDocument doc = new PdfDocument();
            //doc.
            //加载PDF文档
            doc.LoadFromFile(filePath);

            return doc;
        }
        #region
        private static string GetPageText(this PdfDocument doc, int page)
        {
            return doc.Pages[page]?.ExtractText();
        }
        private static PdfPageBase GetPage(this PdfDocument doc, int page)
        {
            return doc.Pages[page];
        }
        private static string ReadAllText(this PdfDocument doc)
        {
            //实例化一个StringBuilder 对象
            StringBuilder content = new StringBuilder();

            //提取PDF所有页面的文本
            foreach (PdfPageBase page in doc.Pages)
            {
                content.Append(page.ExtractText());
            }

            return content.ToString();
        }
        private static void SaveImage(this PdfPageBase page, string saveImagePath)
        {
            foreach (var item in page.ExtractImages())
            {
                if (!Directory.Exists(saveImagePath))
                {
                    Directory.CreateDirectory(saveImagePath);
                }
                String imageFileName = Path.Combine(saveImagePath, Guid.NewGuid().ToString() + ".png");
                item.Save(imageFileName, ImageFormat.Png);
            }
        }
        #endregion


        public static string GetPageText(string filePath, int page)
        {
            return Load(filePath).GetPageText(page);
        }
        public static string GetAllText(string filePath)
        {
            return Load(filePath).ReadAllText();
        }

        public static void SaveAllImage(string filePath, string saveImagePath)
        {
            int index = 1;
            var pdf = Load(filePath);

            foreach (PdfPageBase documentPage in pdf.Pages)
            {
                documentPage.SaveImage(saveImagePath + $"\\{index++}");
            }
        }
        public static void SavePageImage(string filePath, int page, string saveImagePath)
        {
            var pdf = Load(filePath);
            var documentPage = pdf.GetPage(page);

            documentPage.SaveImage(saveImagePath);
        }

        

        public static int GetPage(string filePath)
        {
            return Load(filePath).Pages.Count;
        }
    }
}
