using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public static class ITextSharpParser
    {
        public static PdfReader Load(string filePath)
        {
            return new PdfReader(filePath);
        }
        public static string ReadPage(string filePath, int pageIndex = 1)
        {
            try
            {
                using (var pdfReader = Load(filePath))
                {
                    int numberOfPages = pdfReader.NumberOfPages;
                    if (pageIndex < 1 || pageIndex > numberOfPages)
                    {
                        throw new Exception("err page");
                    }
                    var data = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, pageIndex);
                    //pdfReader.Close();
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Extracting text from the input PDf file error. Reason：" + ex.ToString());
            }
        }

        /// <summary>
        /// 读取每一页上的文本信息
        /// </summary>
        /// <param name="pdfReader"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        private static string ReadPageText(this PdfReader pdfReader, int pageIndex = 1)
        {
            try
            {
                int numberOfPages = pdfReader.NumberOfPages;
                if (pageIndex < 1 || pageIndex > numberOfPages)
                {
                    throw new Exception("err page");
                }
                var data = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, pageIndex);
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("Extracting text from the input PDf file error. Reason：" + ex.ToString());
            }
        }

        /// <summary>
        /// 读取所有文本信息
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ReadAllText(string filePath)
        {
            StringBuilder text = new StringBuilder();
            try
            {
                using (var pdfReader = Load(filePath))
                {
                    int numberOfPages = pdfReader.NumberOfPages;

                    // Page number starts from 1.
                    for (int i = 1; i <= pdfReader.NumberOfPages; i++)
                    {
                        try
                        {
                            text.Append(pdfReader.ReadPageText(i));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Extracting text error.Error page is{i}.Reason:{e.Message}");
                        }
                    }
                    pdfReader.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Extracting text from the input PDf file error. Reason：" + ex.ToString());
            }
            return text.ToString();
        }

        public static void SaveImages(string filePath, string finalPath, int pageIndex)
        {
            RandomAccessFileOrArray raf = new iTextSharp.text.pdf.RandomAccessFileOrArray(filePath);
            var pdf = Load(filePath);

            try
            {
                PdfDictionary pg = pdf.GetPageN(pageIndex);
                PdfDictionary res = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
                PdfDictionary xobj = (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));

                foreach (PdfName name in xobj.Keys)
                {
                    PdfObject obj = xobj.Get(name);
                    if (obj.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);

                        PdfName type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));

                        if (PdfName.IMAGE.Equals(type))
                        {
                            int XrefIndex = Convert.ToInt32(((PRIndirectReference)obj).Number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                            PdfObject pdfObj = pdf.GetPdfObject(XrefIndex);
                            PdfStream pdfStrem = (PdfStream)pdfObj;
                            byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
                            if ((bytes != null))
                            {
                                File.WriteAllBytes(finalPath, bytes);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {


            }
            finally
            {
                pdf.Close();
                raf.Close();
            }

        }
        public static void SavePageAllImages(string filePath, string finalPath, int pageIndex)
        {
            RandomAccessFileOrArray raf = new iTextSharp.text.pdf.RandomAccessFileOrArray(filePath);
            var pdf = Load(filePath);

            try
            {
                PdfDictionary pg = pdf.GetPageN(pageIndex);

                // recursively search pages, forms and groups for images.
                PdfObject obj = FindImageInPDFDictionary(pg);
                if (obj != null)
                {

                    int XrefIndex = ((PRIndirectReference)obj).Number;
                    PdfObject pdfObj = pdf.GetPdfObject(XrefIndex);
                    PdfStream pdfStrem = (PdfStream)pdfObj;
                    byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
                    if ((bytes != null))
                    {
                        String imageFileName = System.IO.Path.Combine(finalPath, Guid.NewGuid().ToString() + ".png");

                        File.WriteAllBytes(imageFileName, bytes);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                pdf.Close();
                raf.Close();
            }
        }

        private static PdfObject FindImageInPDFDictionary(PdfDictionary pg)
        {
            PdfDictionary res =
                (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));


            PdfDictionary xobj =
              (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));
            if (xobj != null)
            {
                foreach (PdfName name in xobj.Keys)
                {

                    PdfObject obj = xobj.Get(name);
                    if (obj.IsIndirect())
                    {
                        PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);

                        PdfName type =
                          (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));

                        //image at the root of the pdf
                        if (PdfName.IMAGE.Equals(type))
                        {
                            return obj;
                        }// image inside a form
                        else if (PdfName.FORM.Equals(type))
                        {
                            return FindImageInPDFDictionary(tg);
                        } //image inside a group
                        else if (PdfName.GROUP.Equals(type))
                        {
                            return FindImageInPDFDictionary(tg);
                        }

                    }
                }
            }

            return null;

        }
    }
}
