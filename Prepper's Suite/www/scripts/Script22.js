//
//  https://api.jqueryui.com/sortable/
//
$(function () {
    //===============================================
    //VARIABLES
    //===============================================
    //  Add Card
    var SidePnl_AddCard = $('.side-pnl.add-card')
    var PnlAddCard = $('#PnlAddCard');
    var pnlAddCard_Custom = $("#PnlAddCard #pnl-custom");
    var pnlAddCard_Predefined = $("#PnlAddCard #pnl-predefined");

    var AddBtns = $('.add-new-card');
    var AddPnl_CancelBtn = $('#PnlAddCard .button.cancel');
    var AddPnl_SubmitBtn = $('#PnlAddCard .button.submit');
    var AddPnl_Custom_Textbox = $('#PnlAddCard #pnl-custom input[type=text]');
    var AddPnl_Predefined_Dropdown = $('#PnlAddCard #pnl-predefined select');
    var btnAddCard_Custom = $('#PnlAddCard .btns-new-card-type .button.custom');
    var btnAddCard_Predefined = $('#PnlAddCard .btns-new-card-type .button.predefined');

    var hfldAddCard = $('.hfldAddCard');
    var hfldAddCard_CustomType = $('#PnlAddCard .btns-new-card-type #hfldCustomType');

    //Update Card
    var SidePnl_UpdateCard = $('.side-pnl.update-card')
    var PnlUpdateCard = $('#PnlUpdateCard');

    var IsSorting = false;
    var LstCards = $('.scrum-columns .card');
    var HfldTemp_UniqueCardName = $('#HfldTemp_UniqueCardName');
    var TxbUpdateCardName = $('#txbUpdateCardName');
    var TxbUpdateDescription = $('#txbUpdateDescription');
    var TxbUpdateDueDate = $('#txbUpdateDueDate');
    var CbUpdateDueDateComplete = $('#cbUpdateDueDateComplete');

    var UpdatePnl_SubmitBtn = $('#PnlUpdateCard .button.submit');
    var UpdatePnl_CancelBtn = $('#PnlUpdateCard .button.cancel');
    var UpdatePnl_DeleteBtn = $('#PnlUpdateCard .button.delete');

    //Delete Card
    var SidePnl_DeleteCard = $('.side-pnl.delete-card');
    var PnlConfirmDeletion = $('#PnlConfirmDeletion');
    var ConfirmDeletion_CancelBtn = $('#PnlConfirmDeletion .button.cancel');
    var ConfirmDeletion_DeleteBtn = $('#PnlConfirmDeletion .button.delete');

    var hfldDeleteCardId = $('.hfldDeleteCardId');
    var HfldTemp_CardId = $('#HfldTemp_CardId');




    //===============================================
    //METHODS
    //===============================================
    var reOrganizeCards = function ReOrganizeCards() {
        //Instantiate variables
        var LstColumns = $('.sortable');
        var columnId = "";

        //Update cards
        $(LstColumns).each(function (i, column) {
            //Variables
            columnId = $(column).data('column-id');

            //Update data attributes
            var cards = $(column).find('.card');
            $(cards).each(function (index, card) {
                $(card).data('sort-id', index);
                $(card).data('column-id', columnId);
            })
        });


        //Update hidden fields
        $(LstColumns).each(function (i, column) {
            //Variables
            columnId = $(column).data('column-id');

            //Update fields
            var cards = $(column).find('.card');
            $(cards).each(function (index, card) {
                var cardId = $(card).attr("id");
                var hfldSortId = $('.hfldSortId.' + cardId);
                var hfldCardColumnId = $('.hfldCardColumnId.' + cardId);

                hfldSortId.val($(card).data('sort-id'));
                hfldCardColumnId.val($(card).data('column-id'));
            })
        });

        return "";
    };
    var saveUpdates = function SaveUpdates() {
        var $form = $('#FormUpdate');
        $form.submit();
    };
    function InitializePg() {
        //clear values
        hfldAddCard.val(false);
        $(PnlAddCard).find('.hfldAddCardName').first().val('');
        $(PnlAddCard).find('.hfldAddColumnId').first().val(-1);
        $(PnlAddCard).find('.hfldAddSortId').first().val(-1);
    }
    function stringToBoolean(val) {
        switch (val.toLowerCase().trim()) {
            case "true":
            case "yes":
            case "1":
                return true;

            default:
                return false;
        }
    }


    //===============================================
    //HANDLES
    //===============================================
    $(".sortable").sortable({
        connectWith: ".connectedSortable",
        start: function (event, ui) {
            IsSorting = true;
        },
        stop: function (event, ui) {
            $.when(1 == 1).then(reOrganizeCards).then(saveUpdates);
            IsSorting = false;
        }
    });

    //Add
    $(AddBtns).click(function () {
        //Populate hidden fields
        var _columnId = $(this).data('column-id');
        var _sortId = $(this).data('next-sort-index');
        $(PnlAddCard).find('.hfldAddCard').first().val(true);
        $(PnlAddCard).find('.hfldAddColumnId').first().val(_columnId);
        $(PnlAddCard).find('.hfldAddSortId').first().val(_sortId);
        SidePnl_AddCard.removeClass('hide-pnl');
    })
    $(btnAddCard_Custom).click(function (e) {
        pnlAddCard_Custom.show();
        pnlAddCard_Predefined.hide();
        hfldAddCard_CustomType.val(true);
    });
    $(btnAddCard_Predefined).click(function (e) {
        pnlAddCard_Custom.hide();
        pnlAddCard_Predefined.show();
        hfldAddCard_CustomType.val(false);
    });
    $(AddPnl_CancelBtn).click(function () {
        //clear values and close panel
        $(PnlAddCard).find('.hfldAddCard').first().val(false);
        $(PnlAddCard).find('.hfldAddCardName').first().val('');
        $(PnlAddCard).find('.hfldAddColumnId').first().val(-1);
        $(PnlAddCard).find('.hfldAddSortId').first().val(-1);
        //$(PnlAddCard).css('display', 'none');
        SidePnl_AddCard.addClass('hide-pnl');
    })
    $(AddPnl_SubmitBtn).click(function () {
        //If false, add predefined value instead
        var blAddCard_CustomType = (hfldAddCard_CustomType.val().toLowerCase() == 'true' ? true : false);
        if (is.falsy(blAddCard_CustomType)) {
            AddPnl_Custom_Textbox.val(AddPnl_Predefined_Dropdown.val());
        }

        saveUpdates();
    })

    //Update
    $(LstCards).click(function () {
        if (IsSorting == false) {
            //Initiate edit card mode
            var UniqueCardName = $(this).attr('id');
            HfldTemp_UniqueCardName.val(UniqueCardName);
            HfldTemp_CardId.val($(this).data('card-id'));

            //Pass values to update panel
            TxbUpdateCardName.val($('.hfldCardName.' + UniqueCardName).val());
            TxbUpdateDescription.val($('.hfldCardDescription.' + UniqueCardName).val());


            TxbUpdateDueDate.val('');
            var DueDate = $('.hfldCompletionDate.' + UniqueCardName).val();
            console.log(DueDate);
            if (DueDate.length > 0) {
                TxbUpdateDueDate.val(new Date(DueDate).toString("yyyy-MM-dd"));
            }



            var isComplete = $('.hfldIsComplete.' + UniqueCardName).val();
            $(CbUpdateDueDateComplete).prop("checked", stringToBoolean(isComplete));


            //Show pnl
            SidePnl_UpdateCard.removeClass('hide-pnl');
        }
    })
    $(UpdatePnl_SubmitBtn).click(function () {
        if ($.trim(TxbUpdateCardName.val()).length > 0) {
            var UniqueCardName = HfldTemp_UniqueCardName.val();
            var CardName = $.trim(TxbUpdateCardName.val());
            var Description = $.trim(TxbUpdateDescription.val());
            var DueDate = $.trim(TxbUpdateDueDate.val());
            //
            $('.hfldCardName.' + UniqueCardName).val(CardName);
            $('.hfldCardDescription.' + UniqueCardName).val(Description);
            $('.hfldCompletionDate.' + UniqueCardName).val(new Date(Date.parse(DueDate)).toString("M/d/yyyy"));
            $('.hfldIsComplete.' + UniqueCardName).val($(CbUpdateDueDateComplete).prop("checked"));




            saveUpdates();
        }
        else {
            //Invalid entry, close panel without saving.
            HfldTemp_CardId.val('');
            HfldTemp_UniqueCardName.val('');
            TxbUpdateCardName.val('');
            TxbUpdateDescription.val('');
            TxbUpdateDueDate.val('');
        }
        $(PnlUpdateCard).css('display', 'none');
    })
    $(UpdatePnl_CancelBtn).click(function () {
        //clear values and close panel
        HfldTemp_CardId.val('');
        HfldTemp_UniqueCardName.val('');
        TxbUpdateCardName.val('');
        TxbUpdateDescription.val('');
        SidePnl_UpdateCard.addClass('hide-pnl');
    })
    $(UpdatePnl_DeleteBtn).click(function () {
        SidePnl_DeleteCard.removeClass('hide-pnl');
    })

    //Delete
    $(ConfirmDeletion_CancelBtn).click(function () {
        SidePnl_DeleteCard.addClass('hide-pnl');
    })
    $(ConfirmDeletion_DeleteBtn).click(function () {
        hfldDeleteCardId.val(HfldTemp_CardId.val());
        saveUpdates();
    })



    InitializePg();
});



//
//  Jquery Progressbar      https://jqueryui.com/progressbar/#default
//
$(function () {
    $("#clProgressbar").progressbar({ value: 37 });
});