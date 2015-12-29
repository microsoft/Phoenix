/// <reference path="CmpWapExtensionadmin.controller.js" />
/*globals,jQuery,trace,cdm,CmpWapExtensionAdminExtension,waz,Exp*/
(function ($, global, fx, Exp, undefined) {
    "use strict";

    var handleTextboxTab = function () {
        // Bind our buttons to the textbox
        $("#sample-textbox-enable").click(function () {
            $("#sample-textbox").fxTextBox("enable");
        });

        $("#sample-textbox-disable").click(function () {
            $("#sample-textbox").fxTextBox("disable");
        });

        // Bind validation to the textboxes
        $("#sample-textbox").fxTextBox({ value: "", autoRevert: true, validateOnKeyPress: true });
        $("#required-textbox").fxTextBox({ validateOnKeyPress: false, autoRevert: false });
        $("#data-range-min-only").fxTextBox({ validateOnKeyPress: false, autoRevert: false });
        $("#data-range-textbox").fxTextBox({ validateOnKeyPress: false, autoRevert: false });
        $("#data-number-textbox").fxTextBox({ validateOnKeyPress: false, autoRevert: false });
        $("#data-digit-textbox").fxTextBox({ validateOnKeyPress: false, autoRevert: false });
    }

    var handleTablistTab = function () {
        // Bind our buttons to the tablist
        $("#tablist-enable").click(function () {
            $("#sample-tablist").fxTabList("enable");
        });

        $("#tablist-disable").click(function () {
            $("#sample-tablist").fxTabList("disable");
        });

        // Initialize our tablist
        $("#sample-tablist").fxTabList({
            // Tabs to show
            values: [
                { text: "Production", value: "Production" },
                { text: "Staging", value: "Staging" }
            ],
            // The corresponding "panels" that the tab values  map to.  This is matched based on ordering between values and panels
            panels: [$("#tablist-tab1"), $("#tablist-tab2")]
        });
    }

    var handleSliderTab = function () {
        var getSelectedValue = function () {
            return $("#slider").fxSlider("value");
        }

        $("#sliderEnableButton").click(function () {
            $("#slider").fxSlider("enable");
        });

        $("#sliderDisableButton").click(function () {
            $("#slider").fxSlider("disable");
        });

        $("#sliderCurrentValueButton").click(function () {
            $("#sliderCurrentValueOnDemandSpan").text(getSelectedValue());
        });

        $("#sliderSelectValueButton").click(function () {
            $("#slider").fxSlider("option", "value", $("#sliderSelectValueTextBox").val());
        });

        $("#sliderRevertButton").click(function () {
            $("#slider").fxSlider("revert");
        });

        $("#sliderUpdateInitialValueButton").click(function () {
            $("#slider").fxSlider("setOriginalValue");
        });

        $("#slider").bind("change.fxcontrol", function () {
            $("#sliderMessageSpan").html($(this).fxEditableControl("hasEditedControls") ? "<span id='MessageSpan' style='color:Red;font-weight:bold'>Edited</span>" : "<span id='MessageSpan' style='color:Green;font-weight:normal'>Not Edited</span>");
        });

        $("#sliderTrackEditCheckbox").change(function () {
            $("#slider").fxSlider("option", "trackedit", $("#sliderTrackEditCheckbox").is(":checked") ? true : false);
        });

        $("#sliderMaxValueButton").click(function () {
            $("#slider").fxSlider("option", "max", $("#sliderMaxValueTextBox").val());
        });

        $("#sliderMinValueButton").click(function () {
            $("#slider").fxSlider("option", "min", $("#sliderMinValueTextBox").val());
        });

        $("#sliderMaxSlidableValueButton").click(function () {
            $("#slider").fxSlider("option", "slidableMax", $("#sliderMaxSlidableValueTextBox").val());
        });

        $("#sliderMinSlidableValueButton").click(function () {
            $("#slider").fxSlider("option", "slidableMin", $("#sliderMinSlidableValueTextBox").val());
        });

        $("#sliderStepButton").click(function () {
            $("#slider").fxSlider("option", "step", $("#sliderStepTextBox").val());
        });

        $("#slider").fxSlider({
            value: 5,
            min: 0,
            max: 10,
            slidableMin: 1,
            slidableMax: 9,
            change: function (event, args) {
                $("#sliderCurrentValueOnChangedSpan").text(args.value);
            }
        });
    }

    var handleScrollbarTab = function () {
        // Helper for enabling our scrollbar
        var enableScrollbar = function () {
            // If we don't know when the content size will change, we can set automatic polling
            // This comes with a small performance hit
            //$(".scroll").fxScrollbar({ autoRefreshContent: true });
            $(".scroll").fxScrollbar();
        };

        // Helper for disabling our scrollbar
        var disableScrollbar = function () {
            $(".scroll").fxScrollbar("destroy");
        };

        var scrollTopScrollbar = function () {
            $(".scroll").fxScrollbar("scrollTop", 0);
        };

        $("#scrollTurnOnButton").click(function () {
            if ("hidden" == $("#scrollVerticalDiv").css("overflow-y")) {
                $("#scrollVerticalDiv").css("overflow-y", "scroll");
            }
            enableScrollbar();
        });

        $("#scrollTurnOffButton").click(function () {
            disableScrollbar();
            $("#scrollVerticalDiv").css("overflow-y", "hidden");
        });

        $("#scrollTopButton").click(function () {
            scrollTopScrollbar();
        });
    }

    var handleRadioTab = function () {
        var getSelectedValue = function () {
            return $("#sample-radio").fxRadio("value");
        };

        var sizes = [
                { text: "1GB", value: "1GB" },
                { text: "5GB", value: "5GB" },
                { text: "10GB", value: "10GB" },
                { text: "25GB", value: "25GB" },
                { text: "50GB", value: "50GB" },
                { text: "100GB", value: "100GB" },
                { text: "150GB", value: "150GB" },
                { text: "200GB", value: "200GB" }
        ];

        $("#EnableButton").click(function () {
            $("#sample-radio").fxRadio("enable");
        });

        $("#DisableButton").click(function () {
            $("#sample-radio").fxRadio("disable");
        });

        $("#CurrentValueButton").click(function () {
            $("#CurrentValueOnDemandSpan").text(getSelectedValue().text);
        });

        $("#SelectValueButton").click(function () {
            var updatedValue = $.grep(sizes, function (n, i) {
                return n.value === $("#SelectValueTextBox").val();
            })[0];
            $("#sample-radio").fxRadio("option", "value", updatedValue);
        });

        $("#RevertButton").click(function () {
            $("#sample-radio").fxRadio("revert");
        });

        $("#UpdateInitialValueButton").click(function () {
            $("#sample-radio").fxRadio("setOriginalValue");
        });

        $("#sample-radio").bind("change.fxcontrol", function () {
            $("#MessageSpan").html($(this).fxEditableControl("hasEditedControls") ? "<span id='MessageSpan' style='color:Red;font-weight:bold'>Edited</span>" : "<span id='MessageSpan' style='color:Green;font-weight:normal'>Not Edited</span>");
        });

        $("#TrackEditCheckbox").change(function () {
            $("#sample-radio").fxRadio("option", "trackedit", $("#TrackEditCheckbox").is(":checked") ? true : false);
        });

        $("#sample-radio").fxRadio({
            value: sizes[3],
            values: sizes,
            change: function (event, args) {
                $("#CurrentValueOnChangedSpan").text(args.value.text);
            }
        });
        $("#CurrentValueOnDemandSpan").text(getSelectedValue().text);

    }

    var handleGridTab = function () {
        // Initialize our grid
        var grid_numbers = 10;
        var grid_rows = [];
        for (var i = 1; i <= grid_numbers; i++) {
            grid_rows.push({ type: "Type " + i, status: "Status " + i, subscription: "Subscription " + i, location: "Field " + i });
        }
        grid_rows[3].selected = true; // Default with row selected
        grid_rows[4].disabled = true; // Disable this row

        // Add a new row
        $("#grid-add").click(function () {
            var grid_rows2 = $("#grid").fxGrid("option", "data");
            $.observable(grid_rows2).insert(grid_rows2.length, { type: "New Row Added", status: "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum rhoncus accumsan eros, volutpat laoreet ligula porta", subscription: "Subscription Just Added", location: "Location Just Added" });
        });

        //// Remove a row
        $("#grid-remove").click(function () {
            var grid_rows2 = $("#grid").fxGrid("option", "data");
            $.observable(grid_rows2).remove(grid_rows2.length - 1, 1);
        });

        // Make the grid selectable
        $("#grid-selectable").click(function () {
            $("#grid").fxGrid("option", "selectable", $(this).prop("checked"));
        });

        // Enable multi-selection in the grid
        $("#grid-multiselect").click(function () {
            $("#grid").fxGrid("option", "multiselect", $(this).prop("checked"));
        });

        // Initialize our grid
        $("#grid")
            .fxGrid({
                columns: [ // List of columns to display
                    { name: "Type", field: "type" },
                    { name: "Status", field: "status" },
                    { name: "Subscription", field: "subscription" },
                    { name: "Location", field: "location" }
                ],
                rowSelect: function () {
                    // Bind an event when a row is selected
                },
                data: grid_rows, // Data for the grid
                selectable: false, // Default value
                multiselect: false // Default value
            });


        var numbers = 5;
        var rows = [];

        //Mapping of icon values to their images and text to display
        var icons = {
            "er-Error": { url: "/Content/Images/icon-validation-invalid.gif", text: "Error" },
            "ch-Starting Up": { url: "/Content/Images/icon-help.png", text: "Starting up" },
            "ch-Deploying": { url: "/Content/Images/icon-validation-pending.gif", text: "Deploying" },
            "ru-Running": { url: "/Content/Images/icon-validation-valid.gif", text: "Running" },
            "su-Suspended": { url: "/Content/Images/icon-help.png", text: "Suspended" }
        };

        //List of icon values to show for example
        var iconArr = ["er-Error", "ch-Starting Up", "ch-Deploying", "ru-Running", "su-Suspended"];

        //Generate our data
        for (var i = 1; i <= numbers; i++) {
            //Make a long tooltip string
            var tooltipString = "Hover over me for tooltips! ";
            for (var j = 1; j < i; j++) {
                tooltipString += tooltipString;
            }

            rows.push({
                tooltip: tooltipString,
                bool: (i % 2 == 0), //alternate true and false
                date: new Date(Math.random() * 101 + 1900, Math.random() * 12 + 1, Math.random() * 30 + 1), //year (at least 1900), month (at least 1), day (at least 1)
                text: "<div> escaped text here</div> ",
                url: { url: "http://www.microsoft.com", text: "Link to MS page" },
                status: iconArr[i % 5], //See mapping of icon values above
                custom: "Row " + i //Any data can be here
            });
        }

        //Create our grid
        $("#formated-grid")
            .fxGrid({
                columns: [ //Define our columns and how to format them
                    { name: "Tooltips", field: "tooltip", formatter: $.fxGridFormatter.tooltip }, //Simple tooltip that will show on hover. Overflow doesn't matter
                    { name: "Checkbox", field: "bool", formatter: $.fxGridFormatter.checkbox, cssClass: "sampleGridCheckBoxCell" }, //Simple checkbox, does not update data model
                    { name: "Date Time", field: "date", formatter: $.fxGridFormatter.date("G") }, //See /Scripts/auxfx/polyfills/date.js for supported format types
                    { name: "Text", field: "text", formatter: $.fxGridFormatter.text }, //Text, useful to escape html
                    { name: "URL", field: "url", formatter: $.fxGridFormatter.url }, //Url link
                    { name: "icon", field: "status", formatter: $.fxGridFormatter.iconLookup(icons) }, //Icons for various status
                    {
                        name: "Custom",
                        field: "custom",
                        formatter: function (value) { //Make your own custom formatter to do anything you want
                            var changedText = "<div style='color:red'>Boogy Woogy! " + value + "</div>";
                            return changedText;
                        }
                    }
                ],
                data: rows, //Our data
                selectable: true,
                multiselect: false
            });

    }

    // Public
    var loadTab = function(renderData, container) {
        // Initialize our tablist
        $("#controls-tablist").fxTabList({
            // Tabs to show
            values: [
                { text: "Testbox", value: "tab1" },
                { text: "Tablist", value: "tab2" },
                { text: "Slider", value: "tab3" },
                { text: "Scrollbar", value: "tab4" },
                { text: "Radio", value: "tab5" },
                { text: "Grid", value: "tab6" }
            ],
            // The corresponding "panels" that the tab values  map to.  This is matched based on ordering between values and panels
            panels: [$("#tab1"), $("#tab2"), $("#tab3"), $("#tab4"), $("#tab5"), $("#tab6")]//,
            //click: function (event, data) {
            //    do something here
            //    return true;
            //}
        });

        handleTextboxTab();
        handleTablistTab();
        handleSliderTab();
        handleScrollbarTab();
        handleRadioTab();
        handleGridTab();
    }

    var cleanUp = function() {
    }

    global.CmpWapExtensionAdminExtension = global.CmpWapExtensionAdminExtension || {};
    global.CmpWapExtensionAdminExtension.ControlsTab = {
        loadTab: loadTab,
        cleanUp: cleanUp
    };

})(jQuery, this, this.fx, this.Exp);

