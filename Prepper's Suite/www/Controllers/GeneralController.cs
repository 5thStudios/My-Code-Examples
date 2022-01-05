using ScrumTests.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using bl.Models;
using bl.EF;

namespace ScrumTests.Controllers
{
    public class GeneralController : SurfaceController
    {
        #region "Actions"
        public ActionResult RenderForm_22()
        {
            AllScrumData allScrumData = new AllScrumData();

            if (TempData[Common.TempData.Model] != null)
            {
                allScrumData = (AllScrumData)TempData[Common.TempData.Model];

                //temp update to card data
                foreach (ScrumColumn CardGroup in allScrumData.LstScrumColumns)
                {
                    foreach (ScrumCard tempCard in CardGroup.LstScrumCards)
                    {
                        if (tempCard.CompletionDate != null)
                        {
                            tempCard.CompletionDateString = Convert.ToDateTime(tempCard.CompletionDate).ToString("MMM d");
                            tempCard.ChecklistStatus = "";
                            if (tempCard.IsComplete)
                            {
                                tempCard.ChecklistStatus = "complete";
                            }
                            else if (tempCard.CompletionDate <= DateTime.Today) { 
                                tempCard.ChecklistStatus = "due"; 
                            }
                        }
                    }
                }
            }
            else
            {
                //
                ScrumColumn CardGroup = new ScrumColumn();
                CardGroup.ColumnId = 0;
                CardGroup.ColumnName = "TO DO";
                CardGroup.ColumnColor = "lightseagreen";
                CardGroup.LstScrumCards.Add(new ScrumCard(1, 0, 0, "Card 01"));
                ScrumCard tempCard = new ScrumCard(2, 1, 0, "Card 02");

                //other vars.
                tempCard.AccountId = 1;
                tempCard.ToolId = 4;
                tempCard.IsArchived = false;
                tempCard.CreatedTimestamp = DateTime.Now;
                tempCard.LastUpdatedTimestamp = DateTime.Now;

                tempCard.CompletionDate = DateTime.Today;
                tempCard.CompletionDateString = Convert.ToDateTime(tempCard.CompletionDate).ToString("MMM d");
                tempCard.IsComplete = false;
                if (tempCard.IsComplete) { tempCard.ChecklistStatus = "complete"; }
                else if (tempCard.CompletionDate <= DateTime.Today) { tempCard.ChecklistStatus = "due"; }


                //Checklists
                tempCard.LstScrumChecklist = new List<ScrumChecklist>();
                for (var i = 0; i < 4; i++)
                {
                    ScrumChecklist _sclItem = new ScrumChecklist();
                    _sclItem.CardId = 1;
                    _sclItem.EntryId = i;
                    if (i % 2 == 0)
                    {
                        _sclItem.IsComplete = true;
                    }
                    _sclItem.Title = "Item 0" + i;
                    tempCard.LstScrumChecklist.Add(_sclItem);
                }
                tempCard.TotalChecklistItems = tempCard.LstScrumChecklist.Count();
                tempCard.CompletedChecklistItems = tempCard.LstScrumChecklist.Where(z => z.IsComplete == true).Count();
                if (tempCard.TotalChecklistItems > 0 && tempCard.TotalChecklistItems == tempCard.CompletedChecklistItems) { tempCard.ActivityStatus = "complete"; }


                //Activities
                tempCard.LstScrumActivity = new List<ScrumActivity>();
                for (var i = 0; i < 2; i++)
                {
                    ScrumActivity _activity = new ScrumActivity();
                    _activity.ActivityId = i;
                    _activity.CardId = 1;
                    _activity.Text = "Text 0" + i;
                    _activity.CreatedTimestamp = DateTime.Now;
                    _activity.LastUpdatedTimestamp = DateTime.Now;
                    tempCard.LstScrumActivity.Add(_activity);
                }


                CardGroup.LstScrumCards.Add(tempCard);
                allScrumData.LstScrumColumns.Add(CardGroup);


                CardGroup = new ScrumColumn();
                CardGroup.ColumnId = 1;
                CardGroup.ColumnName = "RESEARCHING";
                CardGroup.ColumnColor = "yellow";
                allScrumData.LstScrumColumns.Add(CardGroup);

                CardGroup = new ScrumColumn();
                CardGroup.ColumnId = 2;
                CardGroup.ColumnName = "IN PROGRESS";
                CardGroup.ColumnColor = "orange";
                allScrumData.LstScrumColumns.Add(CardGroup);

                CardGroup = new ScrumColumn();
                CardGroup.ColumnId = 3;
                CardGroup.ColumnName = "COMPLETE";
                CardGroup.ColumnColor = "green";
                allScrumData.LstScrumColumns.Add(CardGroup);

                CardGroup = new ScrumColumn();
                CardGroup.ColumnId = 4;
                CardGroup.ColumnName = "URGENT";
                CardGroup.ColumnColor = "red";
                allScrumData.LstScrumColumns.Add(CardGroup);

                CardGroup = new ScrumColumn();
                CardGroup.ColumnId = 5;
                CardGroup.ColumnName = "FUTURE PLANS";
                CardGroup.ColumnColor = "blue";
                allScrumData.LstScrumColumns.Add(CardGroup);
            }

            //Create predefined cards
            allScrumData.LstPredefinedScrumCards.Add(new ScrumCard(-1, -1, -1, "Predefined 01"));
            allScrumData.LstPredefinedScrumCards.Add(new ScrumCard(-1, -1, -1, "Predefined 02"));
            allScrumData.LstPredefinedScrumCards.Add(new ScrumCard(-1, -1, -1, "Predefined 03"));

            //Return data with partial view
            
            return PartialView("~/Views/Partials/_Test22.cshtml", allScrumData);
        }
        #endregion



        #region "Submit ActionResults"

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FormUpdate_22(AllScrumData Model)
        {
            if (!ModelState.IsValid)
            {
                //
                List<bl.Models.Error> result = new List<bl.Models.Error>();
                var erroneousFields = ModelState.Where(ms => ms.Value.Errors.Any()).Select(x => new { x.Key, x.Value.Errors });

                foreach (var erroneousField in erroneousFields)
                {
                    var fieldKey = erroneousField.Key;
                    var fieldErrors = erroneousField.Errors.Select(error => new Error(fieldKey, error.ErrorMessage));
                    result.AddRange(fieldErrors);
                }



                foreach (Error error in result)
                {
                    ModelState.AddModelError(string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(error));
                }
                ModelState.AddModelError(string.Empty, "===========result============");
                ModelState.AddModelError(string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(result));
                ModelState.AddModelError(string.Empty, "=============erroneousFields==========");
                ModelState.AddModelError(string.Empty, Newtonsoft.Json.JsonConvert.SerializeObject(erroneousFields));

                return CurrentUmbracoPage();
            }
            else if (ModelState.IsValid)
            {
                //Instantiate variables
                List<ScrumCard> LstCards = new List<ScrumCard>();
                AllScrumData UpdatedAllCards = new AllScrumData();


                //Create a temp card id.
                if (Model.AddCard == true)
                {
                    int _cardId = -1;
                    foreach (var _cardGroup in Model.LstScrumColumns)
                    {
                        foreach (var _cardList in _cardGroup.LstScrumCards)
                        {
                            if (_cardList.CardId > _cardId)
                                _cardId = _cardList.CardId;
                        }
                    }

                    Model.VirtualScrumCard.CardId = _cardId + 1;
                    Model.VirtualScrumCard.SortId = Int32.MaxValue;
                    LstCards.Add(Model.VirtualScrumCard);
                }


                //Consolidate all cards into single list
                foreach (var _cardGroup in Model.LstScrumColumns)
                {
                    foreach (var _cardList in _cardGroup.LstScrumCards)
                    {
                        if (_cardList.CardId != Model.DeleteCardId)
                        {
                            LstCards.Add(_cardList);
                        }
                    }
                }


                //Copy column lists into new list of groups
                foreach (ScrumColumn _modelCardGroup in Model.LstScrumColumns)
                {
                    ScrumColumn CardList = new ScrumColumn();
                    CardList.ColumnId = _modelCardGroup.ColumnId;
                    CardList.ColumnName = _modelCardGroup.ColumnName;
                    CardList.ColumnColor = _modelCardGroup.ColumnColor;
                    UpdatedAllCards.LstScrumColumns.Add(CardList);
                }


                //Reorganize cards into new list of card groups
                foreach (ScrumCard _card in LstCards.OrderBy(x => x.StatusId).ThenBy(y => y.SortId))
                {
                    ScrumColumn _cardList = UpdatedAllCards.LstScrumColumns.Where(z => z.ColumnId == _card.StatusId).FirstOrDefault();
                    _cardList.LstScrumCards.Add(_card);
                }


                //Reorganize sort IDs
                foreach (ScrumColumn _cardList in UpdatedAllCards.LstScrumColumns)
                {
                    int sortId = 0;

                    foreach (ScrumCard _card in _cardList.LstScrumCards)
                    {
                        _card.SortId = sortId;
                        sortId++;
                    }
                }


                TempData[Common.TempData.Model] = UpdatedAllCards;

            }

            //return CurrentUmbracoPage();
            return RedirectToCurrentUmbracoPage();
        }
        #endregion

    }
}



















//public ActionResult RenderForm_19()
//{
//    AllCards allCards = new AllCards();

//    if (TempData["Model"] != null)
//    {
//        allCards = (AllCards)TempData["Model"];
//    }
//    else
//    {
//        CardList CardGroup = new CardList();
//        CardGroup.ColumnId = 0;
//        CardGroup.ColumnName = "To Do";
//        CardGroup.ColumnColor = "pink";
//        CardGroup.LstCards.Add(new Card(1, 0, 0, "Card 01"));
//        CardGroup.LstCards.Add(new Card(2, 1, 0, "Card 02"));
//        //CardGroup.LstCards.Add(new Card(3, 2, 0, "Card 03"));
//        allCards.LstCardGroups.Add(CardGroup);

//        CardGroup = new CardList();
//        CardGroup.ColumnId = 1;
//        CardGroup.ColumnName = "In Progress";
//        CardGroup.ColumnColor = "orange";
//        //CardGroup.LstCards.Add(new Card(4, 0, 1, "Card 04"));
//        //CardGroup.LstCards.Add(new Card(5, 1, 1, "Card 05"));
//        allCards.LstCardGroups.Add(CardGroup);

//        CardGroup = new CardList();
//        CardGroup.ColumnId = 2;
//        CardGroup.ColumnName = "Verifying";
//        CardGroup.ColumnColor = "yellow";
//        //CardGroup.LstCards.Add(new Card(6, 0, 2, "Card 06"));
//        allCards.LstCardGroups.Add(CardGroup);

//        CardGroup = new CardList();
//        CardGroup.ColumnId = 3;
//        CardGroup.ColumnName = "Done";
//        CardGroup.ColumnColor = "green";
//        allCards.LstCardGroups.Add(CardGroup);
//    }

//    //Create predefined cards
//    allCards.LstPredefinedCards.Add(new Card(-1, -1, -1, "Predefined 01"));
//    allCards.LstPredefinedCards.Add(new Card(-1, -1, -1, "Predefined 02"));
//    allCards.LstPredefinedCards.Add(new Card(-1, -1, -1, "Predefined 03"));

//    //Return data with partial view
//    return PartialView("~/Views/Partials/_Test19.cshtml", allCards);
//}
//public ActionResult RenderForm_20()
//{
//    AllCards allCards = new AllCards();

//    if (TempData["Model"] != null)
//    {
//        allCards = (AllCards)TempData["Model"];
//    }
//    else
//    {
//        CardList CardGroup = new CardList();
//        CardGroup.ColumnId = 0;
//        CardGroup.ColumnName = "TO DO";
//        CardGroup.ColumnColor = "lightseagreen";
//        CardGroup.LstCards.Add(new Card(1, 0, 0, "Card 01"));
//        CardGroup.LstCards.Add(new Card(2, 1, 0, "Card 02"));
//        allCards.LstCardGroups.Add(CardGroup);

//        CardGroup = new CardList();
//        CardGroup.ColumnId = 1;
//        CardGroup.ColumnName = "RESEARCHING";
//        CardGroup.ColumnColor = "yellow";
//        allCards.LstCardGroups.Add(CardGroup);

//        CardGroup = new CardList();
//        CardGroup.ColumnId = 2;
//        CardGroup.ColumnName = "IN PROGRESS";
//        CardGroup.ColumnColor = "orange";
//        allCards.LstCardGroups.Add(CardGroup);

//        CardGroup = new CardList();
//        CardGroup.ColumnId = 3;
//        CardGroup.ColumnName = "COMPLETE";
//        CardGroup.ColumnColor = "green";
//        allCards.LstCardGroups.Add(CardGroup);

//        CardGroup = new CardList();
//        CardGroup.ColumnId = 4;
//        CardGroup.ColumnName = "URGENT";
//        CardGroup.ColumnColor = "red";
//        allCards.LstCardGroups.Add(CardGroup);

//        CardGroup = new CardList();
//        CardGroup.ColumnId = 5;
//        CardGroup.ColumnName = "FUTURE PLANS";
//        CardGroup.ColumnColor = "blue";
//        allCards.LstCardGroups.Add(CardGroup);
//    }

//    //Create predefined cards
//    allCards.LstPredefinedCards.Add(new Card(-1, -1, -1, "Predefined 01"));
//    allCards.LstPredefinedCards.Add(new Card(-1, -1, -1, "Predefined 02"));
//    allCards.LstPredefinedCards.Add(new Card(-1, -1, -1, "Predefined 03"));

//    //Return data with partial view
//    return PartialView("~/Views/Partials/_Test20.cshtml", allCards);
//}
//public ActionResult RenderForm_21()
//{
//    AllCards allCards = new AllCards();

//    if (TempData["Model"] != null)
//    {
//        allCards = (AllCards)TempData["Model"];
//    }
//    else
//    {
//        CardList CardGroup = new CardList();
//        CardGroup.ColumnId = 0;
//        CardGroup.ColumnName = "TO DO";
//        CardGroup.ColumnColor = "lightseagreen";
//        CardGroup.LstCards.Add(new Card(1, 0, 0, "Card 01"));
//        CardGroup.LstCards.Add(new Card(2, 1, 0, "Card 02"));
//        allCards.LstCardGroups.Add(CardGroup);

//        CardGroup = new CardList();
//        CardGroup.ColumnId = 1;
//        CardGroup.ColumnName = "RESEARCHING";
//        CardGroup.ColumnColor = "yellow";
//        allCards.LstCardGroups.Add(CardGroup);

//        CardGroup = new CardList();
//        CardGroup.ColumnId = 2;
//        CardGroup.ColumnName = "IN PROGRESS";
//        CardGroup.ColumnColor = "orange";
//        allCards.LstCardGroups.Add(CardGroup);

//        CardGroup = new CardList();
//        CardGroup.ColumnId = 3;
//        CardGroup.ColumnName = "COMPLETE";
//        CardGroup.ColumnColor = "green";
//        allCards.LstCardGroups.Add(CardGroup);

//        CardGroup = new CardList();
//        CardGroup.ColumnId = 4;
//        CardGroup.ColumnName = "URGENT";
//        CardGroup.ColumnColor = "red";
//        allCards.LstCardGroups.Add(CardGroup);

//        CardGroup = new CardList();
//        CardGroup.ColumnId = 5;
//        CardGroup.ColumnName = "FUTURE PLANS";
//        CardGroup.ColumnColor = "blue";
//        allCards.LstCardGroups.Add(CardGroup);
//    }

//    //Create predefined cards
//    allCards.LstPredefinedCards.Add(new Card(-1, -1, -1, "Predefined 01"));
//    allCards.LstPredefinedCards.Add(new Card(-1, -1, -1, "Predefined 02"));
//    allCards.LstPredefinedCards.Add(new Card(-1, -1, -1, "Predefined 03"));

//    //Return data with partial view
//    return PartialView("~/Views/Partials/_Test21.cshtml", allCards);
//}




//[HttpPost]
//[ValidateAntiForgeryToken]
//public ActionResult FormUpdate_19(AllCards Model)
//{
//    if (!ModelState.IsValid)
//    {
//        return CurrentUmbracoPage();
//    }
//    else if (ModelState.IsValid)
//    {
//        //Instantiate variables
//        List<Card> LstCards = new List<Card>();
//        AllCards UpdatedAllCards = new AllCards();


//        if (Model.AddCard == true)
//        {
//            //Create a temp card id.
//            int cardCount = 1;
//            foreach (var _cardGroup in Model.LstCardGroups)
//            {
//                foreach (var _cardList in _cardGroup.LstCards)
//                {
//                    cardCount++;
//                }
//            }

//            Model.VirtualCard.CardId = cardCount;
//            LstCards.Add(Model.VirtualCard);
//        }



//        //Consolidate all cards into single list
//        foreach (var _cardGroup in Model.LstCardGroups)
//        {
//            foreach (var _cardList in _cardGroup.LstCards)
//            {
//                if (_cardList.CardId != Model.DeleteCardId)
//                {
//                    LstCards.Add(_cardList);
//                }
//            }
//        }

//        //Copy column lists into new list of groups
//        foreach (CardList _modelCardGroup in Model.LstCardGroups)
//        {
//            CardList CardList = new CardList();
//            CardList.ColumnId = _modelCardGroup.ColumnId;
//            CardList.ColumnName = _modelCardGroup.ColumnName;
//            CardList.ColumnColor = _modelCardGroup.ColumnColor;
//            UpdatedAllCards.LstCardGroups.Add(CardList);
//        }

//        //Reorganize cards into new list of card groups
//        foreach (Card _card in LstCards.OrderBy(x => x.ColumnId).ThenBy(y => y.SortId))
//        {
//            CardList _cardList = UpdatedAllCards.LstCardGroups.Where(z => z.ColumnId == _card.ColumnId).FirstOrDefault();
//            _cardList.LstCards.Add(_card);
//        }

//        TempData["Model"] = UpdatedAllCards;


//    }

//    //return CurrentUmbracoPage();
//    return RedirectToCurrentUmbracoPage();
//}


//[HttpPost]
//[ValidateAntiForgeryToken]
//public ActionResult FormUpdate_20(AllCards Model)
//{
//    if (!ModelState.IsValid)
//    {
//        return CurrentUmbracoPage();
//    }
//    else if (ModelState.IsValid)
//    {
//        //Instantiate variables
//        List<Card> LstCards = new List<Card>();
//        AllCards UpdatedAllCards = new AllCards();


//        if (Model.AddCard == true)
//        {
//            //Create a temp card id.
//            int cardCount = 1;
//            foreach (var _cardGroup in Model.LstCardGroups)
//            {
//                foreach (var _cardList in _cardGroup.LstCards)
//                {
//                    cardCount++;
//                }
//            }

//            Model.VirtualCard.CardId = cardCount;
//            LstCards.Add(Model.VirtualCard);
//        }



//        //Consolidate all cards into single list
//        foreach (var _cardGroup in Model.LstCardGroups)
//        {
//            foreach (var _cardList in _cardGroup.LstCards)
//            {
//                if (_cardList.CardId != Model.DeleteCardId)
//                {
//                    LstCards.Add(_cardList);
//                }
//            }
//        }

//        //Copy column lists into new list of groups
//        foreach (CardList _modelCardGroup in Model.LstCardGroups)
//        {
//            CardList CardList = new CardList();
//            CardList.ColumnId = _modelCardGroup.ColumnId;
//            CardList.ColumnName = _modelCardGroup.ColumnName;
//            CardList.ColumnColor = _modelCardGroup.ColumnColor;
//            UpdatedAllCards.LstCardGroups.Add(CardList);
//        }

//        //Reorganize cards into new list of card groups
//        foreach (Card _card in LstCards.OrderBy(x => x.ColumnId).ThenBy(y => y.SortId))
//        {
//            CardList _cardList = UpdatedAllCards.LstCardGroups.Where(z => z.ColumnId == _card.ColumnId).FirstOrDefault();
//            _cardList.LstCards.Add(_card);
//        }

//        TempData["Model"] = UpdatedAllCards;


//    }

//    //return CurrentUmbracoPage();
//    return RedirectToCurrentUmbracoPage();
//}


//[HttpPost]
//[ValidateAntiForgeryToken]
//public ActionResult FormUpdate_21(AllCards Model)
//{
//    if (!ModelState.IsValid)
//    {
//        return CurrentUmbracoPage();
//    }
//    else if (ModelState.IsValid)
//    {
//        //Instantiate variables
//        List<Card> LstCards = new List<Card>();
//        AllCards UpdatedAllCards = new AllCards();


//        if (Model.AddCard == true)
//        {
//            //Create a temp card id.
//            int _cardId = -1;
//            foreach (var _cardGroup in Model.LstCardGroups)
//            {
//                foreach (var _cardList in _cardGroup.LstCards)
//                {
//                    if (_cardList.CardId > _cardId)
//                        _cardId = _cardList.CardId;
//                }
//            }

//            Model.VirtualCard.CardId = _cardId + 1;
//            Model.VirtualCard.SortId = Int32.MaxValue;
//            LstCards.Add(Model.VirtualCard);
//        }



//        //Consolidate all cards into single list
//        foreach (var _cardGroup in Model.LstCardGroups)
//        {
//            foreach (var _cardList in _cardGroup.LstCards)
//            {
//                if (_cardList.CardId != Model.DeleteCardId)
//                {
//                    LstCards.Add(_cardList);
//                }
//            }
//        }

//        //Copy column lists into new list of groups
//        foreach (CardList _modelCardGroup in Model.LstCardGroups)
//        {
//            CardList CardList = new CardList();
//            CardList.ColumnId = _modelCardGroup.ColumnId;
//            CardList.ColumnName = _modelCardGroup.ColumnName;
//            CardList.ColumnColor = _modelCardGroup.ColumnColor;
//            UpdatedAllCards.LstCardGroups.Add(CardList);
//        }

//        //Reorganize cards into new list of card groups
//        foreach (Card _card in LstCards.OrderBy(x => x.ColumnId).ThenBy(y => y.SortId))
//        {
//            CardList _cardList = UpdatedAllCards.LstCardGroups.Where(z => z.ColumnId == _card.ColumnId).FirstOrDefault();
//            _cardList.LstCards.Add(_card);
//        }

//        //Reorganize sort IDs
//        foreach (CardList _cardList in UpdatedAllCards.LstCardGroups)
//        {
//            int sortId = 0;

//            foreach (Card _card in _cardList.LstCards)
//            {
//                _card.SortId = sortId;
//                sortId++;
//            }
//        }

//        TempData["Model"] = UpdatedAllCards;


//    }

//    //return CurrentUmbracoPage();
//    return RedirectToCurrentUmbracoPage();
//}
