(function (Speak) {
    Speak.pageCode(["sitecore", "jquery"], function (sitecore, $) {
        var selectedChecklist = function (model) {
            var app = this;

            if (typeof model !== "undefined" && model !== null && model !== "") {
                //console.log(model);
                app.SubAppRendererChecklistDetail.DialogWindow.show();
            }
        };

        var selectedReport = function (model) {
            var app = this;

            if (typeof model !== "undefined" && model !== null && model !== "") {
                //console.log(model);
                app.SubAppRendererReportDetail.DialogWindow.show();
            }
        };
		
		var checklistCreator = function (model) {
            var app = this;

			app.DashboardFrame.set("sourceUrl", "");

			$.ajax({
				type: "GET",
				dataType: "json",
				url: "/api/checklist/createchecklist",
				cache: false,
				success: function (data) {
					app.DashboardFrame.set("sourceUrl", data);
				},
				error: function () {
					console.log("There was an error. Try again please!");
				}
			});
        };

        return {
            initialized: function () {
                this.getItemList();
                this.CreateBtn.on("click", checklistCreator, this);
                this.AllChecklist.on("change:ClickedItem", selectedChecklist, this);
                this.RecentList.on("change:ClickedItem", selectedReport, this);
            },
            getItemList: function () {
                var app = this;

                $.ajax({
                    type: "GET",
                    dataType: "json",
                    url: "/api/checklist/getallchecklist",
                    cache: false,
                    success: function (data) {
                        // either use below or //app.AllChecklist.reset(data.ScriptModel)
                        app.GenericDataSource.DynamicData = data.ScriptModel;
                        app.RecentReportDataSource.DynamicData = data.ChecklistReports;
                        $("[data-sc-id='AllChecklist']").find(".sc-listcontrol-item").attr("style", "cursor: pointer");

                        app.SubAppRendererChecklistDetail.DialogWindow.on("hide", function (job) {
                            app.SubAppRendererChecklistDetail.Frame.set("sourceUrl", "");
                            app.AllChecklist.SelectedItem = null;
                        });
                    },
                    error: function (data) {
                        console.log("There was an error. Try again please!");
                    }
                });
            }

        }
    });
})(Sitecore.Speak);