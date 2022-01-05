using formulate.app.Types;
using formulate.core.Models;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;
using ContentModels = Umbraco.Web.PublishedModels;


namespace Controllers
{
    public class ScriptureController : SurfaceController
    {
        public static ScriptureContent ObtainScriptureData(ContentModels.Scripture cmModel, string chapter)
        {
            //Instantiate variables
            Models.ScriptureContent PgContent = new ScriptureContent();

            //Obtain current chapter
            PgContent.chapterCount = cmModel.Chapters;
            int currentChapter = int.TryParse(chapter, out currentChapter) ? currentChapter : 1;
            PgContent.currentChapter = currentChapter;
            if ((PgContent.currentChapter < 1) | (PgContent.currentChapter > PgContent.chapterCount)) { PgContent.currentChapter = 1; }

            //Obtain chapter content to display on page.
            IPublishedContent ipChapter = cmModel.Children.Skip(PgContent.currentChapter - 1).FirstOrDefault();
            foreach (IPublishedElement ipeVerse in ipChapter.Value<IEnumerable<IPublishedElement>>(Common.NodeProperties.Verses))
            {
                Models.ScriptureVerse scriptureVerse = new Models.ScriptureVerse();
                scriptureVerse.Verse = ipeVerse.Value<int>(Common.NodeProperties.Verse);
                scriptureVerse.Content = ipeVerse.Value<string>(Common.NodeProperties.content);
                PgContent.LstScriptureVerses.Add(scriptureVerse);
            }


            return PgContent;
        }
        public static TOCContent ObtainTocContent(UmbracoHelper Umbraco, ContentModels.TableOfContent cmTOC)
        {
            TOCContent tOCContent = new TOCContent();
            tOCContent.prefaceUrl = Umbraco.Content((int)Common.siteNode.Preface).Url();

            //Loop thru all book and build ToC structure
            foreach (var ipBook in cmTOC.Children<Scripture>().Where(x => x.ContentType.Alias == Common.docType.Scripture))
            {
                if (ipBook.Testament != tOCContent.currentTestament)
                {
                    //Compare current testament with previous testament
                    tOCContent.currentTestament = ipBook.Testament;

                    //Add testament set
                    tOCContent.testament = new tocTestament();
                    tOCContent.testament.testament = tOCContent.currentTestament;
                    tOCContent.toc.lstTestaments.Add(tOCContent.testament);
                }

                if (ipBook.BookSet != tOCContent.currentBookset)
                {
                    //Compare current book set with previous book set
                    tOCContent.currentBookset = ipBook.BookSet;

                    //Add book set
                    tOCContent.bookSet = new tocBookSet();
                    tOCContent.bookSet.bookSet = tOCContent.currentBookset;
                    tOCContent.testament.lstBookSets.Add(tOCContent.bookSet);
                }

                //Add book to proper set
                tOCContent.book = new tocBook();
                tOCContent.book.name = ipBook.FullName;
                tOCContent.book.url = ipBook.Url();
                tOCContent.bookSet.lstBooks.Add(tOCContent.book);
            }

            return tOCContent;
        }






        public static void ImportScriptures(IContentService icService)
        {
            //======================================================================
            //  ONLY USED TO IMPORT BIBLE XML INTO UMBRACO
            //  ....................................................................
            //  HOW TO USE:  INSERT THE FOLLOWING LINE INTO A VIEW TEMOPORARILY
            //  Controllers.ScriptureController.ImportScriptures(Services.ContentService);
            //======================================================================

            //Instantiate list of scripture books
            List<Book> lstBooks = new List<Book>();

            //Obtain list of all files within folder
            List<string> lstFiles = Directory.GetFiles(@"C:\Inetpub\vhosts\afterthewarning.com\dev.AtWv7\AtW_v7\www\tempData\DouayRheimsBibleXML\").ToList();

            //Loop thru each item in list and convert to class
            foreach (string filePath in lstFiles)
            {
                Book book = null;
                XmlSerializer serializer = new XmlSerializer(typeof(Book));
                StreamReader reader = new StreamReader(filePath);
                book = (Book)serializer.Deserialize(reader);
                reader.Close();
                lstBooks.Add(book);
            }

            //Get root folder of bible
            IContent icBible = icService.GetById((int)Common.siteNode.TheDouayRheimsBible);

            foreach (Book book in lstBooks.OrderBy(x => int.Parse(x.Order)))
            {
                //======================================================================
                //Add new book
                IContent icScripture = icService.Create(
                                          name: book.Name,
                                          parentId: icBible.Key,
                                          documentTypeAlias: Common.docType.Scripture);

                //Set the name and alt name
                string[] bookName = book.FullName.Split('[');
                icScripture.SetValue(Common.NodeProperties.fullName, bookName.FirstOrDefault());
                if (bookName.Count() > 1) { icScripture.SetValue(Common.NodeProperties.alternateName, bookName.LastOrDefault().Replace("]", "")); }

                //Select what testament to add book to
                if (int.Parse(book.Order) < 47)
                {
                    icScripture.SetValue(Common.NodeProperties.testament, Common.SerializeValue(Common.miscellaneous.OldTestament));
                }
                else
                {
                    icScripture.SetValue(Common.NodeProperties.testament, Common.SerializeValue(Common.miscellaneous.NewTestament));
                }

                //Add total chapters in this book
                icScripture.SetValue(Common.NodeProperties.chapters, book.TotalChapters);

                //Select what book set this book belongs to
                int bookId = int.Parse(book.Order);
                string bookSet = string.Empty;
                if (bookId >= 1 && bookId <= 8) { bookSet = Common.SerializeValue(Common.miscellaneous.ThePentateuchBooks); }
                else if (bookId >= 9 && bookId <= 21) { bookSet = Common.SerializeValue(Common.miscellaneous.TheHistoricalBooks); }
                else if (bookId >= 22 && bookId <= 28) { bookSet = Common.SerializeValue(Common.miscellaneous.TheWisdomBooks); }
                else if (bookId >= 29 && bookId <= 46) { bookSet = Common.SerializeValue(Common.miscellaneous.ThePropheticBooks); }
                else if (bookId >= 47 && bookId <= 50) { bookSet = Common.SerializeValue(Common.miscellaneous.TheGospels); }
                else if (bookId >= 51 && bookId <= 73) { bookSet = Common.SerializeValue(Common.miscellaneous.TheEpistles); }
                icScripture.SetValue(Common.NodeProperties.bookSet, bookSet);

                //Save node
                icService.SaveAndPublish(icScripture);
                //======================================================================



                //======================================================================
                //Add chapters for book
                foreach (Models.Chapter chapter in book.Chapter)
                {
                    //Add new chapter
                    IContent icChapter = icService.Create(
                                              name: chapter.Number,
                                              parentId: icScripture.Key,
                                              documentTypeAlias: Common.docType.Chapter);

                    //For each chapter, create list of verses
                    List<Dictionary<string, string>> lstVerses = new List<Dictionary<string, string>>();
                    foreach (Versicle versicle in chapter.Versicle)
                    {
                        lstVerses.Add(new Dictionary<string, string>()
                        {
                            {"key",Guid.NewGuid().ToString()},
                            {"name",versicle.Number},
                            {"ncContentTypeAlias",Common.dataType.ScriptureVerse},
                            {"verse",versicle.Number},
                            {"content",versicle.Text},
                        });
                    }
                    icChapter.SetValue(Common.NodeProperties.Verses, JsonConvert.SerializeObject(lstVerses));

                    //Save verses
                    icService.SaveAndPublish(icChapter);
                }
                //======================================================================
            }
        }
    }
}
