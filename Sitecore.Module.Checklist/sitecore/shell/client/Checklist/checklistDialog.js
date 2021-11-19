(function (Speak) {
    Speak.pageCode(["sitecore", "jquery"],
        function (sitecore, $) {
            return {
                initialized: function () {
                    var app = this;

                    app.Frame.set("sourceUrl", "");
                    app.on("show", this.loadChecklist, this);
                    app.ExecuteButton.on("click", this.executeChecklist, this);
                    app.EditBtn.on("click", this.editChecklist, this);
                    app.DeleteBtn.on("click", this.deleteChecklist, this);
                },
                loadChecklist: function () {

                    var app = this;
                    var checklistModel = Sitecore.Speak.app.AllChecklist.ClickedItem;

                    app.ChecklistHeaderText.Text = checklistModel.ItemName;
                    app.ChecklistIcon.ImageUrl = checklistModel.ItemImageUrl;

                    app.AllReportList.DynamicData = [];
                    app.LastExecutionText.set("value", "");
                    app.OverallStatusText.set("value", "");
                    $(".my-custom-rulepanel").empty();

                    $.ajax({
                        type: "GET",
                        dataType: "json",
                        data: { checklistId: checklistModel.ItemId },
                        url: "/api/checklist/getchecklistdetail",
                        cache: false,
                        success: function (data) {

                            $(".my-custom-rulepanel").html(data.RulePanel);

                            app.OverallStatusText.set("value", data.ChecklistDetail.OverallStatus);

                            if (data.ChecklistDetail.OverallStatus === "FAILED") {
                                app.OverallStatusText.set("valueColor", "Red");
                            } else if (data.ChecklistDetail.OverallStatus === "SUCCESS") {
                                app.OverallStatusText.set("valueColor", "Green");
                            } else {
                                app.OverallStatusText.set("valueColor", "Grey");
                            }

                            app.LastExecutionText.set("value", data.ChecklistDetail.LastExecution);

                            app.AllReportList.reset(data.ChecklistReports);

                            app.DialogWindow.HeaderSubtext = data.ChecklistDetail.ItemPath;
                        },
                        error: function () {
                            console.log("There was an error. Try again please!");
                        }
                    });
                },
                editChecklist: function () {

                    var app = this;
                    var checklistModel = Sitecore.Speak.app.AllChecklist.ClickedItem;

                    app.Frame.set("sourceUrl", "");

                    $.ajax({
                        type: "GET",
                        dataType: "json",
                        data: { checklistId: checklistModel.ItemId },
                        url: "/api/checklist/editchecklist",
                        cache: false,
                        success: function (data) {
                            app.Frame.set("sourceUrl", data);
                        },
                        error: function () {
                            console.log("There was an error. Try again please!");
                        }
                    });
                },
                deleteChecklist: function () {

                    var app = this;
                    var checklistModel = Sitecore.Speak.app.AllChecklist.ClickedItem;

                    app.Frame.set("sourceUrl", "");

                    $.ajax({
                        type: "GET",
                        dataType: "json",
                        data: { checklistId: checklistModel.ItemId },
                        url: "/api/checklist/deletechecklist",
                        cache: false,
                        success: function (data) {
                            app.Frame.set("sourceUrl", data);
                        },
                        error: function () {
                            console.log("There was an error. Try again please!");
                        }
                    });
                },
                executeChecklist: function () {

                    var app = this;
                    var checklistModel = Sitecore.Speak.app.AllChecklist.ClickedItem;

                    app.Frame.set("sourceUrl", "");

                    $.ajax({
                        type: "POST",
                        dataType: "json",
                        data: { checklistId: checklistModel.ItemId },
                        url: "/api/checklist/triggerchecklist",
                        cache: false,
                        success: function (data) {
                            alert("Triggered " + data);
                        },
                        error: function () {
                            console.log("There was an error. Try again please!");
                        }
                    });
                }
            };
        },
        "SubAppRendererChecklistDetail");
})(Sitecore.Speak);